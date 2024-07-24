using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using DG.Tweening;
using Infastructure.Management;
using Reels;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    public class WinAnimation : MonoBehaviour
    {
        public Action ONAnimationComplete;
        [SerializeField] private Reel[] reels;
        [SerializeField] private SoundManager soundManager;
        
        private RectTransform[] _visibleSymbolsOnReel;
        private int _lineCounter;
        private List<int[]> _winLines;
        private List<Sequence> _symbolSequence;
        private List<Sequence> _sequences;

        private void Start()
        {
            _symbolSequence = new List<Sequence>();
        }

        public void WinAnim(List<int[]> winSymbolIndex)
        {
            _symbolSequence.Clear();
            _winLines = winSymbolIndex;
            _lineCounter = 0;
            if (winSymbolIndex.Count > 0)
            {
                StartAnim(winSymbolIndex[_lineCounter]);
            }
        }

        private void StartAnim(int[] winLine)
        {
            soundManager.PlaySound(SoundType.WinLineSound);
            for (int i = 0; i < reels.Length; i++)
            {
                _visibleSymbolsOnReel = reels[i].VisibleSymbolsRTOnReel;
                Animation(_visibleSymbolsOnReel, winLine, i);
            }
        }

        private void Animation(RectTransform[] visibleSymbolsOnReel, int[] winSymbolIndex, int currentReel)
        {
            //Array.Reverse(winSymbolIndex);
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
            var currentParticle = reels[currentReel].Particles[winSymbolIndex[currentReel]];

            DOTween.Kill(currentSymbol);
            Sequence symbolSequence = DOTween.Sequence();
            _symbolSequence.Add(symbolSequence);
        
            currentParticle.gameObject.SetActive(true);
            currentParticle.Play();

            symbolSequence
                //.Append(currentSymbol.DOMoveZ(-10f, 1f))
                .Append(currentSymbol.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetLoops(6, LoopType.Yoyo));
               // .Append(currentSymbol.DOMoveZ(0f, 1f));

            symbolSequence.OnComplete(() =>
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

                if (currentReel == reels.Length - 1)
                {
                    _lineCounter++;
                    NextLine();
                }
            });
        }

        private void NextLine()
        {
            if (_lineCounter < _winLines.Count)
            {
                StartAnim(_winLines[_lineCounter]);
            }
            else
            {
                ONAnimationComplete?.Invoke();
            }
        }

        public void ForceStopWinAnim()
        {
            foreach (var sequence in _symbolSequence)
            {
                sequence?.Kill();
            }
            
            foreach (var reel in reels)
            {
                foreach (var particle in reel.Particles)
                {
                    particle.Stop();
                    particle.gameObject.SetActive(false);
                }

                foreach (var symbol in reel.VisibleSymbolsRTOnReel)
                {
                    DOTween.Kill(symbol);
                    symbol.GetComponent<Image>().DOFade(1f, 0.2f);
                    symbol.DOScale(Vector3.one, 0.2f);
                    symbol.DOMoveZ(0f, 0.2f);
                }
            }
        }
    }
}