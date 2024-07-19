using Coffee.UIExtensions;
using Data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ReelsLogic
{
    public class Reel : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private RectTransform[] symbolsOnReel;
        [SerializeField] private RectTransform[] visibleSymbolsRT;
        [SerializeField] private Symbol[] visibleSymbols;
        [SerializeField] private UIParticle[] particles;
        
        [SerializeField] private RectTransform mainCanvasRT;

        [SerializeField] private int reelID;
        
        [SerializeField] private float exitPosition = 387;
        private float _symbolHeight;
        private float _mainCanvasScale;

        [SerializeField] private WinChecker winChecker;
        
        private int _currentSymbolIndex = 0;
        private int _currentFinalScreen = 0;

        private ReelState _reelState = ReelState.Stop;
    
        public ReelState ReelState
        {
            get => _reelState;
            set => _reelState = value;
        }

        public RectTransform[] VisibleSymbolsRTOnReel => visibleSymbolsRT;
        public Symbol[] VisibleSymbols => visibleSymbols;
        public UIParticle[] Particles => particles;
        
        public float SymbolHeight => _symbolHeight;
        public float ReelID => reelID;
    
        private void Start()
        {
             Debug.Log(symbolsOnReel[0].transform.position);
            _symbolHeight = symbolsOnReel[0].rect.height;
            //_winChecker = new WinChecker(visibleSymbols.Length, gameConfig, symbolsOnReel, _particles);
            _mainCanvasScale = mainCanvasRT.lossyScale.y;
        }

        private void Update()
        {
            foreach (var symbol in symbolsOnReel)
            {
                if (symbol.position.y <= exitPosition * _mainCanvasScale)
                {
                    MoveSymbolUp(symbol);
                    ChangeSymbol(symbol);
                }
            }
        }

        private void ChangeSymbol(RectTransform symbol)
        {
            if (_reelState == ReelState.Stopping && symbol != symbolsOnReel[^1])
            {
                symbol.GetComponent<Symbol>().SymbolInfo = GetFinalSprite();
                symbol.GetComponent<Image>().sprite = symbol.GetComponent<Symbol>().SymbolInfo.Sprite;
            }
            else
            {
                symbol.GetComponent<Symbol>().SymbolInfo = GetRandomSprite();
                symbol.GetComponent<Image>().sprite = symbol.GetComponent<Symbol>().SymbolInfo.Sprite;
            }
        }

        private SymbolData GetRandomSprite()
        {
            var random = Random.Range(0, gameConfig.Symbols.Length);
            return gameConfig.Symbols[random];
        }

        private SymbolData GetFinalSprite()
        {
            var finalScreenSymbolIndex = _currentSymbolIndex + (reelID - 1) * gameConfig.VisibleSymbolsOnReel;
            var currentFinalScreen = gameConfig.FinalScreen[_currentFinalScreen].FinalScreen;

            if (_currentFinalScreen > currentFinalScreen.Length)
                finalScreenSymbolIndex = 0;

            var newSymbolData = gameConfig.Symbols[currentFinalScreen[finalScreenSymbolIndex]];
            _currentSymbolIndex++;
            return newSymbolData;
        }

        private void MoveSymbolUp(RectTransform symbol)
        {
            var offset = symbol.position.y + _symbolHeight * _mainCanvasScale * symbolsOnReel.Length;
            var newPos = new Vector3(symbol.position.x, offset);
            symbol.position = newPos;
        }

        public void ResetSymbolPosition(float traveledDistance)
        {
            _currentSymbolIndex = 0;

            if (_currentFinalScreen < gameConfig.FinalScreen.Length - 1)
                _currentFinalScreen++;
            else
                _currentFinalScreen = 0;
            
            foreach (var symbol in symbolsOnReel)
            {
                var currentSymbolPosition = symbol.localPosition;
                var correction = Mathf.Round(currentSymbolPosition.y - traveledDistance);
                var correctedPosition = new Vector3(currentSymbolPosition.x, correction);
                symbol.localPosition = correctedPosition;
            }
        }
    }
}
