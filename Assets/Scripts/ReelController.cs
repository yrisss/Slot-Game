using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ReelController : MonoBehaviour
{
    [SerializeField] private RectTransform[] reelsRT;
    [SerializeField] private Reel[] reels;
    [SerializeField] private Button playButton;
    
    [SerializeField] private RectTransform playButtonRT;    
    
    [SerializeField] private float delay;
    [SerializeField] private Ease startEase;
    [SerializeField] private Ease stopEase;
    [SerializeField] private float boostDistance, linearDistance; 
    [SerializeField] private float boostDuration, linearDuration, stoppingDuration;

    [SerializeField] private int visibleSymbolsOnReel;
    
    private Dictionary<RectTransform, Reel> _reelsDictionary;
    private float _reelStartPositionY;
    private float _symbolHeight;

    private void Start()
    {
        _symbolHeight = reels[0].SymbolHeight;
        _reelStartPositionY = reelsRT[0].localPosition.y;
        _reelsDictionary = new Dictionary<RectTransform, Reel>();
        for (int i = 0; i < reelsRT.Length; i++)
        {
            _reelsDictionary.Add(reelsRT[i], reels[i]);
        }
    }

    public void ScrollStart()
    {
        for (int i = 0; i < reelsRT.Length; i++)
        {
            var reelRT = reelsRT[i];
            reelsRT[i].DOAnchorPosY(boostDistance, boostDuration)
                .SetDelay(i * delay)
                .SetEase(startEase)
                .OnComplete(() => ScrollLinear(reelRT));
        }
    }

    private void ScrollLinear(RectTransform reelRT)
    {
        DOTween.Kill(reelRT);
        reelRT.DOAnchorPosY(linearDistance, linearDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => ReelCorrection(reelRT));
    }

    private void ReelCorrection(RectTransform reelRT)
    {
        DOTween.Kill(reelRT);
        var currentPosition = reelRT.localPosition.y;
        var extraDistance = CalculateExtraDistance(currentPosition);
        var correctionDistance = currentPosition - extraDistance;
        var correctionDuration = extraDistance / -(linearDistance / linearDuration);
        reelRT.DOAnchorPosY(correctionDistance, correctionDuration)
            .OnComplete(() => ScrollStop(reelRT));
    }

    private void ScrollStop(RectTransform reelRT)
    {
        DOTween.Kill(reelRT);
        var currentPosY = reelRT.localPosition.y;
        var stoppingDistance = currentPosY - _symbolHeight * visibleSymbolsOnReel;
        reelRT.DOAnchorPosY(stoppingDistance, stoppingDuration)
            .SetEase(stopEase)
            .OnComplete(() => PrepareReel(reelRT));
    }

    private float CalculateExtraDistance(float reelCurrentPositionY)
    {
        var traveledDistance = _reelStartPositionY - reelCurrentPositionY;
        var symbolUpperPart = traveledDistance % _symbolHeight;
        var extraDistance = _symbolHeight - symbolUpperPart;
        
        return extraDistance;
    }

    private void PrepareReel(RectTransform reelRT)
    {
        var currentReelPosY = reelRT.localPosition.y;
        var traveledDistance = -(_reelStartPositionY + currentReelPosY);
        reelRT.localPosition = new Vector3(reelRT.localPosition.x, _reelStartPositionY);
        _reelsDictionary[reelRT].ResetSymbolPosition(traveledDistance);
    }
}


