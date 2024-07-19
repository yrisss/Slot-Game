using Data;
using UnityEngine;

namespace Reels
{
    public class Symbol : MonoBehaviour
    {
        [SerializeField] private SymbolData symbolData;

        public SymbolData SymbolInfo {get => symbolData; set => symbolData = value; }
    }
}
