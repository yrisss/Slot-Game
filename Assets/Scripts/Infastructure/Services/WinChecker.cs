using System.Collections.Generic;
using Data;
using Reels;
using UnityEngine;

namespace Infastructure.Services
{
    public class WinChecker : MonoBehaviour
    {
        [SerializeField] private PrizeCalculator prizeCalculator;

        [SerializeField] private Reel[] reels;
        [SerializeField] private GameConfig gameConfig;

        private int _totalWin;
        private int _newBalance;
        private int _startBalance;

        private List<int[]> _trueWinLines;

        private Symbol[] _winLineSymbols;

        public List<int[]> CheckResult()
        {
            _winLineSymbols = new Symbol[reels.Length];
            _trueWinLines = new List<int[]>();
            List<int> winSymbolCost = new List<int>();
        
            foreach (var winLine in this.gameConfig.WinLines)
            {
                int[] symbolIndex = winLine.SymbolsPosition;

                for (int i = 0; i < symbolIndex.Length; i++)
                {
                    _winLineSymbols[i] = this.reels[i].VisibleSymbols[symbolIndex[i]];
                }
            
                bool isWinningLine = CompareSymbols();

                if (isWinningLine)
                {
                    _trueWinLines.Add(symbolIndex);
                    winSymbolCost.Add(_winLineSymbols[0].SymbolInfo.Cost);
                }
            }

            prizeCalculator.PrizeCalculate(winSymbolCost);
            return _trueWinLines;
        }
    
        private bool CompareSymbols()
        {
            var firstSymbol = _winLineSymbols[0].SymbolInfo;

            for (int i = 1; i < reels.Length; i++)
            {
                var currentSymbol = _winLineSymbols[i].SymbolInfo;

                if (firstSymbol != currentSymbol)
                {
                    return false;
                }
            }

            return true;
        }
    }
}