using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Win Line", menuName = "Win Line Data")]
    public class WinLinesData : ScriptableObject
    {
        [SerializeField] int[] symbolsPosition;
        public int[] SymbolsPosition => symbolsPosition;
    }
}