using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    public class WinAnimation : MonoBehaviour
    {
        [SerializeField] private ReelsLogic.Reel[] _reels;
        private RectTransform[] _visibleSymbolsOnReel;

        private int lineCounter;

        private List<int[]> winLines;

        [SerializeField]private ChangeBalanceAnimation _changeBalanceAnimation;
        
        public void WinAnim(List<int[]> winSymbolIndex)
        {
            winLines = winSymbolIndex;
            lineCounter = 0;
            StartAnim(winSymbolIndex[lineCounter]);
        }

        private void StartAnim(int[] winLine)
        {
            for (int i = 0; i < _reels.Length; i++)
            {
                _visibleSymbolsOnReel = _reels[i].VisibleSymbolsRTOnReel;
                Animation(_visibleSymbolsOnReel, winLine, i);
            }
        }

        private void Animation(RectTransform[] visibleSymbolsOnReel, int[] winSymbolIndex, int currentReel)
        {
            for (int i = 0; i < visibleSymbolsOnReel.Length; i++)
            {
                if (i == winSymbolIndex[currentReel])
                {
                    continue;
                }

                Image symbolImage = visibleSymbolsOnReel[i].GetComponent<Image>();
                if (symbolImage != null)
                {
                    symbolImage.DOFade(0.3f, 1f);
                }
            }

            var currentSymbol = visibleSymbolsOnReel[winSymbolIndex[currentReel]];
            var currentParticle = _reels[currentReel].Particles[winSymbolIndex[currentReel]];

            DOTween.Kill(currentSymbol);
            Sequence symbolSequence = DOTween.Sequence();

            currentParticle.gameObject.SetActive(true);
            currentParticle.Play();

            symbolSequence
                .Append(currentSymbol.DOMoveZ(-10f, 1f))
                .Append(currentSymbol.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetLoops(6, LoopType.Yoyo))
                .Append(currentSymbol.DOMoveZ(0f, 1f));

            DOTween.Sequence().AppendInterval(4f).OnComplete(() =>
            {
                currentParticle.gameObject.SetActive(false);
                currentParticle.Stop();
                foreach (var symbol in visibleSymbolsOnReel)
                {
                    Image symbolImage = symbol.GetComponent<Image>();
                    if (symbolImage != null)
                    {
                        symbolImage.DOFade(1f, 0.5f);
                    }
                }

                if (currentReel == _reels.Length - 1)
                {
                    lineCounter++;
                    NextLine();
                }
            });
        }

        private void NextLine()
        {
            if (lineCounter < winLines.Count)
            {
                StartAnim(winLines[lineCounter]);
            }
            else
            {
                StartCoroutine(_changeBalanceAnimation.ChangeBalance());
            }
        }

        public void ForceStopWinAnim()
        {
            foreach (var symbol in _visibleSymbolsOnReel)
            {
                DOTween.Kill(symbol);
                symbol.GetComponent<Image>().DOFade(1f, 0.2f);
                symbol.DOScale(Vector3.one, 0.2f);
                symbol.DOMoveZ(0f, 0.2f);
            }
        }
    }
}