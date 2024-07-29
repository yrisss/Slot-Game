using System.Collections.Generic;
using Data;
using DG.Tweening;
using Reels;
using UnityEngine;

namespace Infastructure.Services
{
    public class ScatterChecker : MonoBehaviour
    {
        [SerializeField] private Reel[] reels;
        [SerializeField] private GameConfig gameConfig;

        private int _totalWin;
        private int _newBalance;
        private int _startBalance;

        private int _scattersOnReel;
        private List<int[]> _trueWinLines;

        private Symbol[] _winLineSymbols;

        public int CheckAnticipation(Reel reel)
        {
            _scattersOnReel = 0;
            for (int i = 0; i < reel.VisibleSymbolsRTOnReel.Length; i++)
            {
                if (reel.VisibleSymbols[i].SymbolInfo.Type == SymbolType.Scatter)
                {
                    _scattersOnReel++;
                    reel.VisibleSymbolsRTOnReel[i].DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f)
                        .SetLoops(2, LoopType.Yoyo);
                }
            }

            return _scattersOnReel;
        }

        public int FreeSpinsChecker(int currentReelIndex)
        {
            _scattersOnReel = 0;

            for (int i = 0; i <= currentReelIndex; i++)
            {
                for (int j = 0; j < reels[i].VisibleSymbolsRTOnReel.Length; j++)
                {
                    if (reels[i].VisibleSymbols[j].SymbolInfo.Type == SymbolType.Scatter)
                        _scattersOnReel++;
                }
            }
            
            return _scattersOnReel;
        }
    }
}