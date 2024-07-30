using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Game Config", menuName = "Game Config")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private FinalScreensData[] finalScreens;
        [SerializeField] private SymbolData[] symbolData;
        [SerializeField] private WinLinesData[] winLinesData;
        [SerializeField] private int visibleSymbolsOnReel;

        public FinalScreensData[] FinalScreen => finalScreens;
        public SymbolData[] Symbols => symbolData;
        public WinLinesData[] WinLines => winLinesData;
        public int VisibleSymbolsOnReel => visibleSymbolsOnReel;
    }
}