using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Data;
using DG.Tweening;
using Infastructure.Management;
using Infastructure.Services;
using UnityEngine;
using UnityEngine.UI;
using View.PopUp;

namespace Reels
{
    public class ReelsScroll : MonoBehaviour
    {
        [Header("Components")] [SerializeField]
        private RectTransform[] reelsRT;

        [SerializeField] private Reel[] reels;
        [SerializeField] private Button playButton;
        [SerializeField] private RectTransform playButtonRT;
        [SerializeField] private Button stopButton;
        [SerializeField] private RectTransform stopButtonRT;

        [SerializeField] private RectTransform freeSpinsCountFrameRT;

        [Header("Spin Parameters")] [SerializeField]
        private float delay;

        [SerializeField] private Ease startEase;
        [SerializeField] private Ease stopEase;
        [SerializeField] private float boostSpeed, linearSpeed;
        [SerializeField] private float boostDuration, linearDuration, stoppingDuration;
        private float _boostDistance;
        private float _linearDistance;

        public event Action ONScrollStop;


        [Space] [SerializeField] private int visibleSymbolsOnReel;
        [SerializeField] private float symbolHeight = 0f;

        [Header("Antisipation")] [SerializeField]
        private RectTransform antisipationReelRT;

        [SerializeField] private float prepareAntisipationSpeed;
        [SerializeField] private float prepareAntisipationDuration;

        [SerializeField] private float antisipationSpeed, antisipationDuration;
        [SerializeField] private int freeSpinsCount;
        private int _freeSpinsCounter;
        private float _prepareAntisipationDistance;
        private float _antisipationDistance;


        [Header("Infrastructure")] [SerializeField]
        private GameConfig gameConfig;

        [SerializeField] private AnimationManager animationManager;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private WinChecker winChecker;
        [SerializeField] private ScatterChecker scatterChecker;
        [SerializeField] private PopUpView popUpView;


        private Dictionary<RectTransform, Reel> _reelsDictionary;
        private float _reelStartPositionY;
        public bool isFreeSpinGame = false;
        private bool isForceStop = false;
        private List<int[]> _trueWinLines;

        private AntisipationScroll _antisipationScroll;
        private ForceStop _forceStop;
        private FreeSpinGame _freeSpinGame;
        
        private void Start()
        {
            _linearDistance = linearSpeed * linearDuration;
            _boostDistance = boostSpeed * boostDuration;
            _antisipationDistance = antisipationSpeed * antisipationDuration;
            _prepareAntisipationDistance = prepareAntisipationSpeed * prepareAntisipationDuration;
            
            stopButton.interactable = false;
            stopButtonRT.localScale = Vector3.zero;
            _reelStartPositionY = reelsRT[0].localPosition.y;

            _reelsDictionary = new Dictionary<RectTransform, Reel>();
            for (int i = 0; i < reelsRT.Length; i++)
            {
                _reelsDictionary.Add(reelsRT[i], reels[i]);
            }

            _antisipationScroll = new AntisipationScroll(this, reels, reelsRT, scatterChecker, animationManager,
                _reelsDictionary, _antisipationDistance, antisipationDuration, _prepareAntisipationDistance,
                prepareAntisipationDuration);

            _forceStop = new ForceStop(this, reelsRT, _reelsDictionary, stopButton, animationManager);

            _freeSpinGame = new FreeSpinGame(this, reelsRT, _reelsDictionary, stopButton, freeSpinsCount, popUpView,
                animationManager, soundManager, winChecker, scatterChecker, freeSpinsCountFrameRT);
        }

        public void ScrollStart()
        {
            HidePlayButton();
            stopButton.interactable = false;
            isForceStop = false;

            soundManager.PlayMusic(SoundType.ScrollingSound);
            for (int i = 0; i < reelsRT.Length; i++)
            {
                var reelRT = reelsRT[i];
                reelsRT[i].DOAnchorPosY(_boostDistance, boostDuration)
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

        public void ForceStop()
        {
            isForceStop = _forceStop.StartForceStop();
        }

        private void ScrollLinear(RectTransform reelRT)
        {
            _reelsDictionary[reelRT].ReelState = ReelState.Spin;
            DOTween.Kill(reelRT);
            reelRT.DOAnchorPosY(_linearDistance, linearDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => ReelCorrection(reelRT));
        }

        public void ReelCorrection(RectTransform reelRT)
        {
            DOTween.Kill(reelRT);
            var currentPosition = reelRT.localPosition.y;
            var extraDistance = CalculateExtraDistance(currentPosition);
            var correctionDistance = currentPosition - extraDistance;
            var correctionDuration = extraDistance / -(_linearDistance / linearDuration);
            reelRT.DOAnchorPosY(correctionDistance, correctionDuration)
                .OnComplete(() => ScrollStop(reelRT));
        }
        
        private void ScrollStop(RectTransform reelRT)
        {
            _reelsDictionary[reelRT].ReelState = ReelState.Stopping;
            DOTween.Kill(reelRT);
            var currentPosY = reelRT.localPosition.y;
            var stoppingDistance = currentPosY - symbolHeight * 4f;
            reelRT.DOAnchorPosY(stoppingDistance, stoppingDuration)
                .SetEase(stopEase)
                .OnComplete(() =>
                {
                    if (scatterChecker.CheckAnticipation(_reelsDictionary[reelRT]) > 0)
                    {
                        soundManager.PlaySound(SoundType.ScatterSound);
                    }

                    _reelsDictionary[reelRT].ReelState = ReelState.Stop;
                    
                    soundManager.PlaySound(SoundType.StopScrollingSound);
                    PrepareReel(reelRT);
                    
                    if (_reelsDictionary[reelRT].ReelID == 2 && !isFreeSpinGame)
                    {
                        _antisipationScroll.TryStartAntisipation(_reelsDictionary[reelRT].ReelID + 1, isForceStop);
                    }
                    else if (_reelsDictionary[reelRT].ReelID >= 2 && !isFreeSpinGame)
                    {
                        ONScrollStop?.Invoke();
                    }
                });
        }

        private float CalculateExtraDistance(float reelCurrentPositionY)
        {
            var traveledDistance = _reelStartPositionY - reelCurrentPositionY;
            var correctDistance = traveledDistance % (symbolHeight * 4f);
            var extraDistance = (symbolHeight * 4) - correctDistance;

            return extraDistance;
        }

        private void PrepareReel(RectTransform reelRT)
        {
            var currentReelPosY = reelRT.localPosition.y;
            var traveledDistance = -(_reelStartPositionY + currentReelPosY);
            reelRT.localPosition = new Vector3(reelRT.localPosition.x, _reelStartPositionY);

            _reelsDictionary[reelRT].ResetSymbolPosition(traveledDistance);
            
            if(_reelsDictionary[reelRT].ReelID == reels.Length) 
                FinishScroll(reelRT);
        }

        private void FinishScroll(RectTransform reelRT)
        {
            soundManager.StopMusic(SoundType.ScrollingSound);
            
            if (!_freeSpinGame.TryStartFreeSpins(reelRT))
            {
                animationManager.ONWinAnimationComplete = null;
                animationManager.ONWinAnimationComplete += animationManager.StartChangeBalanceAnimation;
                animationManager.ONWinAnimationComplete += ShowPlayButton;
                _trueWinLines = winChecker.CheckResult();
                if (_trueWinLines.Count != 0)
                {
                    HidePlayButton();
                    animationManager.StartWinAnimation(_trueWinLines);
                }
                else
                {
                    ShowPlayButton();
                }
            }
        }

        public void ShowPlayButton()
        {
            stopButton.interactable = false;
            stopButtonRT.localScale = Vector3.zero;

            playButton.interactable = true;
            playButtonRT.localScale = Vector3.one;
        }

        public void HidePlayButton()
        {
            stopButton.interactable = true;
            stopButtonRT.localScale = Vector3.one;

            playButton.interactable = false;
            playButtonRT.localScale = Vector3.zero;
        }
    }
}