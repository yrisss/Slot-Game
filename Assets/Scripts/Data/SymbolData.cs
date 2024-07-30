using Reels;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Symbol Data", menuName = "Symbol Data")]
    public class SymbolData : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private int cost;
        [SerializeField] private SymbolType symbolType;

        public Sprite Sprite => sprite;
        public int Cost => cost;
        public SymbolType Type => symbolType;
    }
}