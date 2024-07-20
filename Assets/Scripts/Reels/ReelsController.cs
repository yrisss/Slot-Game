using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using Infastructure.Management;
using Infastructure.Services;
using UnityEngine;
using UnityEngine.UI;
using View.PopUp;

namespace Reels
{
    public class ReelsController : MonoBehaviour
    {
        [Header("Components")] [SerializeField]
        private RectTransform[] reelsRT;

        [SerializeField] private Reel[] reels;
        [SerializeField] private Button playButton;
        [SerializeField] private RectTransform playButtonRT;
        [SerializeField] private Button stopButton;
        [SerializeField] private RectTransform stopButtonRT;

        [Header("Spin Parameters")] [SerializeField]
        private float delay;
        [SerializeField] private Ease startEase;
        [SerializeField] private Ease stopEase;
        [SerializeField] private float boostSpeed, linearSpeed;
        [SerializeField] private float boostDuration, linearDuration, stoppingDuration;
        private float _boostDistance;
        private float _linearDistance;
        
        
        [Space]
        [SerializeField] private int visibleSymbolsOnReel;
        [SerializeField] private float symbolHeight = 0f;

        [Header("Antisipation")] [SerializeField]
        private RectTransform antisipationReelRT;

        [SerializeField] private float antisipationDistance, antisipationDuration;
        [SerializeField] private int freeSpinsCount;
        private int _freeSpinsCounter;


        [Header("Infrastructure")] [SerializeField]
        private GameConfig gameConfig;

        [SerializeField] private AnimationManager animationManager;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private WinChecker winChecker;
        [SerializeField] private ScatterChecker scatterChecker;
        [SerializeField] private PopUpView popUpView;
        private List<int[]> _trueWinLines;


        private int _startBalance = 0;
        private Dictionary<RectTransform, Reel> _reelsDictionary;
        private float _reelStartPositionY;
        public bool isFreeSpin = false;
        private bool isForceStop = false;

        private void Start()
        {
            _linearDistance = linearSpeed * linearDuration;
            _boostDistance = boostSpeed * boostDuration;
            stopButton.interactable = false;
            stopButtonRT.localScale = Vector3.zero;
            _reelStartPositionY = reelsRT[0].localPosition.y;
            _reelsDictionary = new Dictionary<RectTransform, Reel>();
            for (int i = 0; i < reelsRT.Length; i++)
            {
                _reelsDictionary.Add(reelsRT[i], reels[i]);
            }
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

        private void ScrollLinear(RectTransform reelRT)
        {
            _reelsDictionary[reelRT].ReelState = ReelState.Spin;
            DOTween.Kill(reelRT);
            reelRT.DOAnchorPosY(_linearDistance, linearDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => ReelCorrection(reelRT));
        }

        private void ReelCorrection(RectTransform reelRT)
        {
            DOTween.Kill(reelRT);
            var currentPosition = reelRT.localPosition.y;
            var extraDistance = CalculateExtraDistance(currentPosition);
            var correctionDistance = currentPosition - extraDistance;
            var correctionDuration = extraDistance / -(_linearDistance / linearDuration);
            reelRT.DOAnchorPosY(correctionDistance, correctionDuration)
                .OnComplete(() => ScrollStop(reelRT));
        }

        public void ForceStop()
        {
            foreach (var reelRT in reelsRT)
            {
                if (_reelsDictionary[reelRT].ReelState == ReelState.Stop)
                {
                    stopButton.interactable = false;
                    animationManager.ForceStopWinAnimation(_reelsDictionary[reelRT].VisibleSymbolsRTOnReel);
                }
                else
                {
                    isForceStop = true;
                    ForceScrollStop();
                }
            }
        }

        private void ForceScrollStop()
        {
            stopButton.interactable = false;
            
            foreach (var reelRT in reelsRT)
            {
                if (_reelsDictionary[reelRT].ReelState == ReelState.Spin)
                {
                    _reelsDictionary[reelRT].ReelState = ReelState.ForceStopping;
                    ReelCorrection(reelRT);
                }
            }
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
                    PrepareReel(reelRT);

                    if (_reelsDictionary[reelRT].ReelID == reelsRT.Length - 1 && !isFreeSpin)
                    {
                        TryStartAntisipation(reelRT);
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

            TryStartFreeSpins(reelRT);
        }

        private void ShowPlayButton()
        {
            stopButton.interactable = false;
            stopButtonRT.localScale = Vector3.zero;

            playButton.interactable = true;
            playButtonRT.localScale = Vector3.one;
        }

        private void HidePlayButton()
        {
            stopButton.interactable = true;
            stopButtonRT.localScale = Vector3.one;

            playButton.interactable = false;
            playButtonRT.localScale = Vector3.zero;
        }
        
        #region Antisipation

        private void TryStartAntisipation(RectTransform reelRT)
        {
            if(isForceStop == true)
                return;
            var isAntisipation = true;
            for (int i = 0; i < reels.Length - 1; i++)
            {
                if (scatterChecker.CheckAnticipation(reels[i]) <= 0)
                {
                    isAntisipation = false;
                }
            }

            if (!isAntisipation) return;
            animationManager.StartAnticipationAnimation(antisipationDuration);
            AntisipationScroll();
        }

        private void AntisipationScroll()
        {
            _reelsDictionary[antisipationReelRT].ReelState = ReelState.Spin;
            DOTween.Kill(antisipationReelRT);
            antisipationReelRT.DOAnchorPosY(antisipationReelRT.localPosition.y * 2, antisipationDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => ReelCorrection(antisipationReelRT));
        }


        #endregion
        
        #region FreeSpins

        private void TryStartFreeSpins(RectTransform reelRT)
        {
            if (_reelsDictionary[reelRT].ReelID == reelsRT.Length)
            {
                soundManager.StopMusic(SoundType.ScrollingSound);
                if (isFreeSpin && _freeSpinsCounter > 0)
                {
                    FreeSpin();
                    popUpView.ChangeFreeSpinsCount(_freeSpinsCounter);
                    _freeSpinsCounter--;
                }
                else if (isFreeSpin && _freeSpinsCounter == 0)
                {
                    soundManager.StopMusic(SoundType.FreeSpinsMusic);
                    soundManager.PlayMusic(SoundType.BackMusic);
                    isFreeSpin = false;
                    animationManager.ONWinAnimationComplete = null;
                    popUpView.ChangeFreeSpinsCount(_freeSpinsCounter);
                    animationManager.ONWinAnimationComplete += ShowTotalWinPopUp;
                    animationManager.ONWinAnimationComplete += ShowPlayButton;
                    _trueWinLines = winChecker.CheckResult();
                    if (_trueWinLines.Count != 0)
                    {
                        stopButton.interactable = true;
                        animationManager.StartWinAnimation(_trueWinLines);
                    }
                }
                else if (scatterChecker.FreeSpinsChecker() >= 3 && !isFreeSpin)
                {
                    soundManager.StopMusic(SoundType.BackMusic);
                    soundManager.PlayMusic(SoundType.FreeSpinsMusic);
                    StartCoroutine(StartFreeSpins());
                }
                else
                {
                    animationManager.ONWinAnimationComplete += animationManager.StartChangeBalanceAnimation;
                    animationManager.ONWinAnimationComplete += ShowPlayButton;
                    _trueWinLines = winChecker.CheckResult();
                    if (_trueWinLines.Count != 0)
                    {
                        HidePlayButton();
                        animationManager.StartWinAnimation(_trueWinLines);
                    }
                }
            }
        }


        private void ShowTotalWinPopUp()
        {
            popUpView.ShowWinPopUp(_startBalance, animationManager);
        }

        private IEnumerator StartFreeSpins()
        {
            stopButton.interactable = false;
            animationManager.ONWinAnimationComplete = null;
            _startBalance = PlayerPrefs.GetInt("Balance", 0);
            isFreeSpin = true;
            animationManager.ONWinAnimationComplete += ScrollStart;
            _freeSpinsCounter = freeSpinsCount;
            popUpView.ShowBonusGamePopUp(_freeSpinsCounter);
            yield return new WaitForSeconds(3f);
            ScrollStart();
        }

        private void FreeSpin()
        {
            _trueWinLines = winChecker.CheckResult();
            if (_trueWinLines.Count > 0)
            {
                stopButton.interactable = true;
                animationManager.StartWinAnimation(_trueWinLines);
            }
            else
                ScrollStart();
        }

        #endregion
    }
}