using System.Linq;
using Animation;
using Coffee.UIExtensions;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

    public class WinChecker : MonoBehaviour
    {
        [SerializeField] private WinAnimation _winAnimation;

        [SerializeField] private ReelsLogic.Reel[] _reels;
       // private int _visibleSymbols;
        //private GameConfig _gameConfig;
        //private RectTransform[] _symbolsOnReel;
        //private UIParticle[] _particles;
        
        // public WinChecker(int visibleSymbols, GameConfig gameConfig, RectTransform[] symbolsOnReel, UIParticle[] particles)
        // {
        //     _visibleSymbols = visibleSymbols;
        //     _gameConfig = gameConfig;
        //     _symbolsOnReel = symbolsOnReel;
        //     _particles = particles;
        // }

        public void CheckResult(int visibleSymbols, GameConfig gameConfig, ReelsLogic.Reel[] reels)
        {
            int[] WinLine = new int[visibleSymbols];
            
            foreach (var winLine in gameConfig.WinLines)
            {
                int[] symbolIndex = winLine.SymbolsPosition;
        
                Sprite firstSymbol = reels[0].VisibleSymbolsOnReel[symbolIndex[0]].GetComponent<Image>().sprite;
        
                bool isWin = true;
        
                for (int i = 1; i < symbolIndex.Length; i++)
                {
                    Sprite currentSymbol = reels[i].VisibleSymbolsOnReel[symbolIndex[i]].GetComponent<Image>().sprite;
        
                    if (firstSymbol != currentSymbol)
                    {
                        isWin = false;
                        break;
                    }
        
                    // var totalWin = _gameConfig.Symbols
                    WinLine = symbolIndex;
                }
        
                if (isWin)
                {
                     _winAnimation.WinAnim(WinLine);
                     break;
                }
            }
        }
    }