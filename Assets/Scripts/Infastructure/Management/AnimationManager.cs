using System;
using System.Collections.Generic;
using Animation;
using Reels;
using UnityEngine;

namespace Infastructure.Management
{
    public class AnimationManager : MonoBehaviour
    {
        [SerializeField] private WinAnimation winAnimation;
        [SerializeField] private ChangeBalanceAnimation changeBalanceAnimation;
        [SerializeField] private AnticipationAnimation anticipationAnimation;

        public Action ONWinAnimationComplete;
        public Action ONAnticipationAnimationComplete;


        private void Start()
        {
            winAnimation.ONAnimationComplete += StopWinAnimation;
        }

        public void StartWinAnimation(List<int[]> winAnimSymbolIndexes)
        {
            if(winAnimSymbolIndexes.Count > 0)
                winAnimation.WinAnim(winAnimSymbolIndexes);
        }
        
        private void StopWinAnimation()
        {
            ONWinAnimationComplete?.Invoke();
        }

        public void ForceStopWinAnimation()
        {
            winAnimation.ForceStopWinAnim();
            winAnimation.IsAnimationComplete = true;
            ONWinAnimationComplete?.Invoke();
        }
        
        public void StartChangeBalanceAnimation()
        {
            StartCoroutine(changeBalanceAnimation.ChangeBalance());
        }

        public void ForceStopChangeBalanceAnimation()
        {
            changeBalanceAnimation.ForceChangeBalance();
        }

        public void ChangeFreeSpinsCount(int freeSpinsCount)
        {
            changeBalanceAnimation.ChangeFreeSpinsCount(freeSpinsCount);
        }

        public void StartAnticipationAnimation(Reel[] reels, Reel reel, RectTransform reelRT, int currentReelIndex, float duration)
        {
            anticipationAnimation = reel.AnticipationAnimation;
            anticipationAnimation.StartAnticipationAnim(reels, reel, reelRT, currentReelIndex, duration);
        }
    }
}