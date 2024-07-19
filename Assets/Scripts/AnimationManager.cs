using System;
using System.Collections.Generic;
using Animation;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class AnimationManager : MonoBehaviour
    {
        [SerializeField] private WinAnimation _winAnimation;
        [SerializeField] private ChangeBalanceAnimation _changeBalanceAnimation;
        [SerializeField] private AnticipationAnimation _anticipationAnimation;

        public Action ONWinAnimationComplete;
        public Action ONChangeBalanceAnimationComplete;
        public Action ONAnticipationAnimationComplete;


        private void Start()
        {
            _winAnimation.ONAnimationComplete += StopWinAnimation;
        }

        public void StartWinAnimation(List<int[]> winAnimSymbolIndexes)
        {
            if(winAnimSymbolIndexes.Count > 0)
                _winAnimation.WinAnim(winAnimSymbolIndexes);
        }

        private void StopWinAnimation()
        {
            ONWinAnimationComplete?.Invoke();
        }

        public void ForceStopWinAnimation(RectTransform[] visibleSymbolsOnReel)
        {
            _winAnimation.ForceStopWinAnim(visibleSymbolsOnReel);
            ONWinAnimationComplete?.Invoke();
        }
        
        public void StartChangeBalanceAnimation()
        {
            StartCoroutine(_changeBalanceAnimation.ChangeBalance());
        }

        public void StartAnticipationAnimation(float duration)
        {
            _anticipationAnimation.StartAnticipationAnim(duration);
        }

        public void StopAnticipationAnimation()
        {
            ONAnticipationAnimationComplete?.Invoke();
        }
    }
}