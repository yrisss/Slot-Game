using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Game Config", menuName = "Game Config")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]private FinalScreensData[] finalScreens;
        public FinalScreensData[] FinalScreen => finalScreens;

        [SerializeField] private SymbolData[] symbolData;
        public SymbolData[] Symbols => symbolData;

        [SerializeField] private WinLinesData[] winLinesData;
        public WinLinesData[] WinLines => winLinesData;

        [SerializeField] private int visibleSymbolsOnReel;
        public int VisibleSymbolsOnReel => visibleSymbolsOnReel;
    }
}