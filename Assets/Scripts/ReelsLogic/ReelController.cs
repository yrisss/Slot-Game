using System.Collections.Generic;
using Data;
using DG.Tweening;
using Reel;
using UnityEngine;
using UnityEngine.UI;

namespace ReelsLogic
{
    public class ReelController : MonoBehaviour
    {
        [SerializeField] private RectTransform[] reelsRT;
        [SerializeField] private Reel[] reels;
        [SerializeField] private Button playButton;
        [SerializeField] private RectTransform playButtonRT;
        [SerializeField] private Button stopButton;
        [SerializeField] private RectTransform stopButtonRT;

        [SerializeField] private WinChecker _winChecker;
        
        [Header("SpinParameters")]
        [SerializeField] private float delay;
        [SerializeField] private Ease startEase;
        [SerializeField] private Ease stopEase;
        [SerializeField] private float boostDistance, linearDistance; 
        [SerializeField] private float boostDuration, linearDuration, stoppingDuration;

        [SerializeField] private int visibleSymbolsOnReel;
    
        private Dictionary<RectTransform, Reel> _reelsDictionary;
        private float _reelStartPositionY;
        private bool isForceStop = false;
        [SerializeField]private float _symbolHeight = 144f;

        [SerializeField] private GameConfig gameConfig;
        private void Start()
        {
            stopButton.interactable = false;
            stopButtonRT.localScale = Vector3.zero;
            //_symbolHeight = reels[0].SymbolHeight;
            _reelStartPositionY = reelsRT[0].localPosition.y;
            _reelsDictionary = new Dictionary<RectTransform, Reel>();
            for (int i = 0; i < reelsRT.Length; i++)
            {
                _reelsDictionary.Add(reelsRT[i], reels[i]);
            }
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
            if (_reelsDictionary[reelsRT[reelsRT.Length]].ReelState == ReelState.Stop)
            {
                stopButton.interactable = false;
                foreach (var reel in reels)
                {
                    reel.ForceStopWinAnim();
                }
            }

            else
            {
                ForceScrollStop();
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
                    if (_reelsDictionary[reelRT].ReelID == reelsRT.Length)
                    {
                        stopButton.interactable = false;
                        stopButtonRT.localScale = Vector3.zero;
                    
                        playButton.interactable = true;
                        playButtonRT.localScale = Vector3.one;
                    }
                });
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
                _winChecker.CheckResult(visibleSymbolsOnReel, gameConfig, reels);
            }
        }
    }
}



