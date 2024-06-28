using System.Linq;
using Data;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace Reel
{
    public class Reel : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private RectTransform[] symbolsOnReel;
        [SerializeField] private RectTransform[] visibleSymbols;

        [SerializeField] private RectTransform mainCanvasRT;

        [SerializeField] private int reelID;

        [SerializeField] private Image winFade;
        
        private float _exitPosition = 390f;
        private float _symbolHeight;
        private float _mainCanvasScale;

        private int _currentSymbolIndex = 0;
        private int _currentFinalScreen = 0;

        private ReelState _reelState = ReelState.Stop;
    
        public ReelState ReelState
        {
            get => _reelState;
            set => _reelState = value;
        }
    
        public float SymbolHeight => _symbolHeight;
        public float ReelID => reelID;
    
        private void Start()
        {
            _symbolHeight = symbolsOnReel[0].rect.height;
            _mainCanvasScale = mainCanvasRT.lossyScale.y;
        }

        private void Update()
        {
            foreach (var symbol in symbolsOnReel)
            {
                if (symbol.position.y <= _exitPosition * _mainCanvasScale)
                {
                    MoveSymbolUp(symbol);
                    //ChangeSymbol(symbol);
                }
            }
        }

        private void ChangeSymbol(RectTransform symbol)
        {
            if (_reelState == ReelState.Stopping)
                symbol.GetComponent<Image>().sprite = GetFinalSprite();

            else
                symbol.GetComponent<Image>().sprite = GetRandomSprite();

        }

        private Sprite GetRandomSprite()
        {
            var random = Random.Range(0, gameConfig.Symbols.Length);
            var newSprite = gameConfig.Symbols[random].Sprite;
            return newSprite;
        }

        private Sprite GetFinalSprite()
        {
            var finalScreenSymbolIndex = _currentSymbolIndex + (reelID - 1) * gameConfig.VisibleSymbolsOnReel;
            var currentFinalScreen = gameConfig.FinalScreen[_currentFinalScreen].FinalScreen;

            if (_currentFinalScreen >= currentFinalScreen.Length)
                finalScreenSymbolIndex = 0;

            var newSprite = gameConfig.Symbols[currentFinalScreen[finalScreenSymbolIndex]];
            _currentSymbolIndex++;
            return newSprite.Sprite;
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
