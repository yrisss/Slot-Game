using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Reel : MonoBehaviour
{ 
    [SerializeField] private RectTransform[] symbolsOnReel;
    [SerializeField] private Sprite[] possibleSymbols;

    [SerializeField] private RectTransform mainCanvasRT;
    

    private float _exitPosition = 520;
    private float _symbolHeight;
    private float _mainCanvasScale;

    public float SymbolHeight => _symbolHeight;
    
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
                ChangeSymbol(symbol);
            }
        }
    }
    
    private void ChangeSymbol(RectTransform symbol)
    {
        symbol.GetComponent<Image>().sprite = GetRandomSprite();
    }

    private Sprite GetRandomSprite()
    {
        var random = Random.Range(0, possibleSymbols.Length);
        var newSprite = possibleSymbols[random];
        return newSprite;
    }

    private void MoveSymbolUp(RectTransform symbol)
    {
        var offset = symbol.position.y + _symbolHeight * _mainCanvasScale * symbolsOnReel.Length;
        var newPos = new Vector3(symbol.position.x, offset);
        symbol.position = newPos;
    }

    public void ResetSymbolPosition(float traveledDistance)
    {
        foreach (var symbol in symbolsOnReel)
        {
            var currentSymbolPosition = symbol.localPosition;
            var correction = Mathf.Round(currentSymbolPosition.y - traveledDistance);
            var correctedPosition = new Vector3(currentSymbolPosition.x, correction);
            symbol.localPosition = correctedPosition;
        }
    }
}
