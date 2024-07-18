using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animation;
using Coffee.UIExtensions;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinChecker : MonoBehaviour
{
    [SerializeField] private PrizeCalculator _prizeCalculator;

    [SerializeField] private ReelsLogic.Reel[] _reels;
    [SerializeField] private GameConfig _gameConfig;

    private int totalWin;
    private int newBalance;
    private int startBalance;

    private List<int[]> trueWinLines;

    private Symbol[] winLineSymbols;
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

    public List<int[]> CheckResult(int visibleSymbols, GameConfig gameConfig, ReelsLogic.Reel[] reels)
    {
        winLineSymbols = new Symbol[reels.Length];
        trueWinLines = new List<int[]>();
        List<int> winSymbolCost = new List<int>();
        
        foreach (var winLine in _gameConfig.WinLines)
        {
            int[] symbolIndex = winLine.SymbolsPosition;

            for (int i = 0; i < symbolIndex.Length; i++)
            {
                winLineSymbols[i] = _reels[i].VisibleSymbols[_reels[i].VisibleSymbols.Length - 1 - symbolIndex[i]];
            }
            
            bool isWinningLine = CompareSymbols();

            if (isWinningLine)
            {
                 trueWinLines.Add(symbolIndex);
                 winSymbolCost.Add(winLineSymbols[0].SymbolInfo.Cost);
            }
        }

        _prizeCalculator.PrizeCalculate(winSymbolCost);
        return trueWinLines;
    }
    
    private bool CompareSymbols()
    {
        var firstSymbol = winLineSymbols[0].SymbolInfo;

        for (int i = 1; i < _reels.Length; i++)
        {
            var currentSymbol = winLineSymbols[i].SymbolInfo;

            if (firstSymbol != currentSymbol)
            {
                return false;
            }
        }

        return true;
    }
}