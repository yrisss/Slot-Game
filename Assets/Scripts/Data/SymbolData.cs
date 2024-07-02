using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Symbol Data", menuName = "Symbol Data")]
    public class SymbolData : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        public Sprite Sprite => sprite;
        
        [SerializeField] private int cost;
        public int Cost => cost;

        [SerializeField] private SymbolType symbolType;
        public SymbolType Type => symbolType;

    }
}