using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Infastructure.Management;
using Infastructure.Services;
using UnityEngine;

namespace Reels
{
    public class AntisipationScroll
    {
        private ReelsScroll _reelScroll;
        private Reel[] _reels;
        private RectTransform[] _reelsRT;
        private ScatterChecker _scatterChecker;
        private AnimationManager _animationManager;
        private Dictionary<RectTransform, Reel> _reelsDictionary;

        private int currentReelIndex = 0;

        private float _antisipationDistance;
        private float _antisipationDuration;
        private float _prepareAntisipationDistance;
        private float _prepareAntisipationDuration;

        public AntisipationScroll(ReelsScroll reelScroll, Reel[] reels, RectTransform[] reelsRT,
            ScatterChecker scatterChecker, AnimationManager animationManager,
            Dictionary<RectTransform, Reel> reelsDictionary,
            float antisipationDistance, float antisipationDuration, float prepareAntisipationDistance,
            float prepareAntisipationDuration)
        {
            _reelScroll = reelScroll;
            _reels = reels;
            _reelsRT = reelsRT;
            _scatterChecker = scatterChecker;
            _animationManager = animationManager;
            _reelsDictionary = reelsDictionary;
            _antisipationDistance = antisipationDistance;
            _antisipationDuration = antisipationDuration;
            _prepareAntisipationDistance = prepareAntisipationDistance;
            _prepareAntisipationDuration = prepareAntisipationDuration;
        }


        public void TryStartAntisipation(int AntisipationReelID, bool isForceStop)
        { 
            currentReelIndex = AntisipationReelID - 1;

            if (isForceStop == true)
                return;
            var isAntisipation = true;
            for (int i = 0; i < currentReelIndex; i++)
            {
                if (_scatterChecker.CheckAnticipation(_reels[i]) <= 0)
                {
                    isAntisipation = false;
                }
            }

            if (!isAntisipation) return;
            StartAntisipationScroll(currentReelIndex);
        }


        private void StartAntisipationScroll(int currentReelIndex)
        {
            bool lastReel = currentReelIndex + 1 == _reels.Length;

            var currentReelRT = _reelsRT[currentReelIndex];
            var currentReel = _reels[currentReelIndex];

            _animationManager.StartAnticipationAnimation(_reels, currentReel, currentReelRT, currentReelIndex, _antisipationDuration);
            AntisipationReelScroll(currentReelIndex, currentReelRT);

            if (lastReel)
                return;
            for (int i = currentReelIndex + 1; i < _reelsRT.Length; i++)
            {
                PrepareAntisipationScroll(_reelsRT[i]);
            }
        }

        private void AntisipationReelScroll(int currentReelIndex, RectTransform currentReelRT)
        {
            _reelsDictionary[currentReelRT].ReelState = ReelState.Spin;
            DOTween.Kill(currentReelRT);
            currentReelRT.DOAnchorPosY(currentReelRT.localPosition.y + _antisipationDistance, _antisipationDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _reelScroll.ReelCorrection(currentReelRT);
                    _reelScroll.ONScrollStop += FreeSpinsCheck;
                });
        }

        private void PrepareAntisipationScroll(RectTransform reelRT)
        {
            _reelsDictionary[reelRT].ReelState = ReelState.Spin;
            DOTween.Kill(reelRT);
            reelRT.DOAnchorPosY(reelRT.localPosition.y + _prepareAntisipationDistance,
                    _prepareAntisipationDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => _reelScroll.ReelCorrection(reelRT));
        }

        private void FreeSpinsCheck()
        {
            _reelScroll.ONScrollStop -= FreeSpinsCheck;
            if (_scatterChecker.FreeSpinsChecker(currentReelIndex) >= 3)
            {
                _reelScroll.isFreeSpinGame = true;
                for (int i = currentReelIndex + 1; i < _reelsRT.Length; i++)
                {
                    _reelScroll.ReelCorrection(_reelsRT[i]);
                }
            }
            else
            {
                _reelScroll.isFreeSpinGame = false;
                if (currentReelIndex + 1 == _reels.Length)
                {
                    return;
                }

                currentReelIndex++;
                StartAntisipationScroll(currentReelIndex);
            }
        }
    }
}