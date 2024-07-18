using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Animation;
using Data;
using DefaultNamespace;
using DG.Tweening;
using PopUp;
using UnityEngine;
using UnityEngine.UI;

namespace ReelsLogic
{
    public class ReelController : MonoBehaviour
    {
        public Action SpinStart;
        public Action SpinStop;
        public Action FreeSpinStart;
        public Action FreeSpinStop;
        
        [SerializeField] private RectTransform[] reelsRT;
        [SerializeField] private Reel[] reels;
        [SerializeField] private Button playButton;
        [SerializeField] private RectTransform playButtonRT;
        [SerializeField] private Button stopButton;
        [SerializeField] private RectTransform stopButtonRT;

        [SerializeField] private WinChecker _winChecker;
        [SerializeField] private ScatterChecker _scatterChecker;
        [SerializeField] private PopUpView _popUpView;
        
        [Header("SpinParameters")]
        [SerializeField] private float delay;
        [SerializeField] private Ease startEase;
        [SerializeField] private Ease stopEase;
        [SerializeField] private float boostDistance, linearDistance, linearSpeed; 
        [SerializeField] private float boostDuration, linearDuration, stoppingDuration;

        [SerializeField] private RectTransform antisipationReelRT;
        [SerializeField] private float antisipationDistance, antisipationDuration;
        [SerializeField] private int freeSpinsCount;
        private int freeSpinsCounter;
        
        [SerializeField] private int visibleSymbolsOnReel;
    
        private Dictionary<RectTransform, Reel> _reelsDictionary;
        private float _reelStartPositionY;
        private bool isForceStop = false;
        public bool isFreeSpin = false;
        [SerializeField]private float _symbolHeight = 0f;

        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private AnimationManager _animationManager;
        private List<int[]> trueWinLines;
        
        private int startBalance = 0;
        private void Start()
        {
           // linearDistance = linearSpeed * -linearDuration;
            StartListner();
            stopButton.interactable = false;
            stopButtonRT.localScale = Vector3.zero;
            _reelStartPositionY = reelsRT[0].localPosition.y;
            _reelsDictionary = new Dictionary<RectTransform, Reel>();
            for (int i = 0; i < reelsRT.Length; i++)
            {
                _reelsDictionary.Add(reelsRT[i], reels[i]);
            }
            
            
        }

        private void StartListner()
        {
           // FreeSpinStop += ScrollStart;
        }

        public void ScrollStart()
        {
            isForceStop = false;
            playButton.interactable = false;
            playButtonRT.localScale = Vector3.zero;
            stopButtonRT.localScale = Vector3.one;
        
            for (int i = 0; i < reelsRT.Length; i++)
            {
                var reelRT = reelsRT[i];
                reelsRT[i].DOAnchorPosY(boostDistance, boostDuration)
                    .SetDelay(i * delay)
                    .SetEase(startEase)
                    .OnComplete(() =>
                    {
                        ScrollLinear(reelRT);

                        if (_reelsDictionary[reelRT].ReelID == reelsRT.Length) 
                            stopButton.interactable = true;
                    });
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ScrollLinear(RectTransform reelRT)
        {
            _reelsDictionary[reelRT].ReelState = ReelState.Spin;
            DOTween.Kill(reelRT);
            reelRT.DOAnchorPosY(linearDistance, linearDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => ReelCorrection(reelRT));
        }

        private void ReelCorrection(RectTransform reelRT)
        {
            DOTween.Kill(reelRT);
            var currentPosition = reelRT.localPosition.y;
            var extraDistance = CalculateExtraDistance(currentPosition);
            var correctionDistance = currentPosition - extraDistance;
            var correctionDuration = extraDistance / -(linearDistance / linearDuration);
            reelRT.DOAnchorPosY(correctionDistance, correctionDuration)
                .OnComplete(() => ScrollStop(reelRT));
        }

        public void ForceStop()
        {
            // if (_reelsDictionary[reelsRT[reelsRT.Length]].ReelState == ReelState.Stop)
            // {
            //     stopButton.interactable = false;
            //     foreach (var reel in reels)
            //     {
            //         reel.ForceStopWinAnim();
            //     }
            // }
            // else
            // {
            //     ForceScrollStop();
            // }
            stopButton.interactable = false; 

            foreach (var reelRT in reelsRT)
            {
                if (_reelsDictionary[reelRT].ReelState == ReelState.Spin)
                {
                    ReelCorrection(reelRT);
                }            
            }
        }

        private void ForceScrollStop()
        {
            stopButton.interactable = false;
        
            foreach (var reelRT in reelsRT)
            {
                if(_reelsDictionary[reelRT].ReelState == ReelState.Spin)
                    ReelCorrection(reelRT);
            }
        }

        private void ScrollStop(RectTransform reelRT)
        {
            _reelsDictionary[reelRT].ReelState = ReelState.Stopping;
            DOTween.Kill(reelRT);
            var currentPosY = reelRT.localPosition.y;
            var stoppingDistance = currentPosY - _symbolHeight * visibleSymbolsOnReel;
            reelRT.DOAnchorPosY(stoppingDistance, stoppingDuration)
                .SetEase(stopEase)
                .OnComplete(() =>
                {
                    _reelsDictionary[reelRT].ReelState = ReelState.Stop;
                    PrepareReel(reelRT);

                    if (_reelsDictionary[reelRT].ReelID == reelsRT.Length - 1 && !isFreeSpin)
                    {
                        TryStartAntisipation();
                    }

                    if (_reelsDictionary[reelRT].ReelID == reelsRT.Length && isFreeSpin)
                    {
                        //FreeSpinStop?.Invoke();
                    }
                    
                    else if (_reelsDictionary[reelRT].ReelID == reelsRT.Length && !isFreeSpin)
                    {
                        stopButton.interactable = false;
                        stopButtonRT.localScale = Vector3.zero;
                    
                        playButton.interactable = true;
                        playButtonRT.localScale = Vector3.one;
                    }
                });
        }

        private void TryStartAntisipation()
        {
            if (_scatterChecker.CheckAnticipation(reels[0]) > 0 &&
                _scatterChecker.CheckAnticipation(reels[1]) > 0)
            {
                _animationManager.StartAnticipationAnimation(antisipationDuration);
                AntisipationScroll();
            }
        }

        private void AntisipationScroll()
        {
            DOTween.Kill(antisipationReelRT);
            antisipationReelRT.DOAnchorPosY(antisipationDistance, antisipationDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => ScrollStop(antisipationReelRT));
        }
        

        private float CalculateExtraDistance(float reelCurrentPositionY)
        {
            var traveledDistance = _reelStartPositionY - reelCurrentPositionY;
            var symbolUpperPart = traveledDistance % _symbolHeight;
            var extraDistance = _symbolHeight - symbolUpperPart;
        
            return extraDistance;
        }

        private void PrepareReel(RectTransform reelRT)
        {
            var currentReelPosY = reelRT.localPosition.y;
            var traveledDistance = -(_reelStartPositionY + currentReelPosY);
            reelRT.localPosition = new Vector3(reelRT.localPosition.x, _reelStartPositionY);

            _reelsDictionary[reelRT].ResetSymbolPosition(traveledDistance);
            
            if (_reelsDictionary[reelRT].ReelID == reelsRT.Length)
            {
                if (isFreeSpin && freeSpinsCounter > 0)
                {
                    FreeSpin();
                    _popUpView.ChangeFreeSpinsCount(freeSpinsCounter);
                    freeSpinsCounter--;
                }
                else if (isFreeSpin && freeSpinsCounter == 0)
                {
                    isFreeSpin = false;
                    _animationManager.ONWinAnimationComplete = null;
                    _popUpView.ChangeFreeSpinsCount(freeSpinsCounter);
                    _animationManager.ONWinAnimationComplete += ShowTotalWinPopUp;
                    trueWinLines = _winChecker.CheckResult(visibleSymbolsOnReel, gameConfig, reels);
                    if (trueWinLines.Count != 0)
                        _animationManager.StartWinAnimation(trueWinLines);
                }    
                else if (_scatterChecker.FreeSpinsChecker() >= 3 && !isFreeSpin)
                {
                    StartCoroutine(StartFreeSpins());
                }
                else
                {
                    _animationManager.ONWinAnimationComplete += _animationManager.StartChangeBalanceAnimation;
                    trueWinLines = _winChecker.CheckResult(visibleSymbolsOnReel, gameConfig, reels);
                    if(trueWinLines.Count != 0)
                        _animationManager.StartWinAnimation(trueWinLines);
                }
            }
        }

        private void ShowTotalWinPopUp()
        {
            _popUpView.ShowWinPopUp(startBalance, _animationManager);
        }

        private IEnumerator StartFreeSpins()
        {
            startBalance = PlayerPrefs.GetInt("Balance", 0);
            isFreeSpin = true;
            _animationManager.ONWinAnimationComplete += ScrollStart;
            freeSpinsCounter = freeSpinsCount;
            _popUpView.ShowBonusGamePopUp(freeSpinsCounter);
            yield return new WaitForSeconds(3f);
            ScrollStart();
        }
        
        private void FreeSpin()
        {
            trueWinLines = _winChecker.CheckResult(visibleSymbolsOnReel, gameConfig, reels);
            _animationManager.StartWinAnimation(trueWinLines);
        }
    }
}



