using System;
using System.Collections.Generic;
using Animation;
using UnityEngine;

namespace Infastructure.Management
{
    public class AnimationManager : MonoBehaviour
    {
        [SerializeField] private WinAnimation winAnimation;
        [SerializeField] private ChangeBalanceAnimation changeBalanceAnimation;
        [SerializeField] private AnticipationAnimation anticipationAnimation;

        public Action ONWinAnimationComplete;
        public Action ONChangeBalanceAnimationComplete;
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

        public void ForceStopWinAnimation(RectTransform[] visibleSymbolsOnReel)
        {
            winAnimation.ForceStopWinAnim();
            ONWinAnimationComplete?.Invoke();
        }
        
        public void StartChangeBalanceAnimation()
        {
            StartCoroutine(changeBalanceAnimation.ChangeBalance());
        }

        public void StartAnticipationAnimation(float duration)
        {
            anticipationAnimation.StartAnticipationAnim(duration);
        }

        public void StopAnticipationAnimation()
        {
            ONAnticipationAnimationComplete?.Invoke();
        }
    }
}