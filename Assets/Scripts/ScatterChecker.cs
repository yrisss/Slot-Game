using System.Collections;
using System.Collections.Generic;
using Animation;
using Data;
using ReelsLogic;
using UnityEngine;

public class ScatterChecker : MonoBehaviour
{
    [SerializeField] private ReelsLogic.Reel[] _reels;
    [SerializeField] private GameConfig _gameConfig;

    private int totalWin;
    private int newBalance;
    private int startBalance;

    private int scattersOnReel;
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

    public int CheckAnticipation(Reel reel)
    {
        scattersOnReel = 0;
        for (int i = 0; i < reel.VisibleSymbolsRTOnReel.Length; i++)
        {
            if (reel.VisibleSymbols[i].SymbolInfo.Type == SymbolType.Scatter)
                scattersOnReel++;
        }

        return scattersOnReel;
    }

    public int FreeSpinsChecker()
    {
        scattersOnReel = 0;

        foreach (var reel in _reels)
        {

            for (int i = 0; i < reel.VisibleSymbolsRTOnReel.Length; i++)
            {
                if (reel.VisibleSymbols[i].SymbolInfo.Type == SymbolType.Scatter)
                    scattersOnReel++;
            }
        }

        return scattersOnReel;
    }
}
