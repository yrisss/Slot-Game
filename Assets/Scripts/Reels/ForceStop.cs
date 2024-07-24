using System.Collections.Generic;
using DG.Tweening;
using Infastructure.Management;
using UnityEngine;
using UnityEngine.UI;

namespace Reels
{
    public class ForceStop
    {
        private ReelsScroll _reelsScroll;
        private RectTransform[] _reelsRT;
        private Dictionary<RectTransform, Reel> _reelsDictionary;
        private Button _stopButton;
        private AnimationManager _animationManager;

        private bool isForceStop;

        public ForceStop(ReelsScroll reelsScroll, RectTransform[] reelsRT,
            Dictionary<RectTransform, Reel> reelsDictionary, Button stopButton, AnimationManager animationManager)
        {
            _reelsScroll = reelsScroll;
            _reelsRT = reelsRT;
            _reelsDictionary = reelsDictionary;
            _stopButton = stopButton;
            _animationManager = animationManager;
        }

        public bool StartForceStop()
        {
            isForceStop = false;

            foreach (var reelRT in _reelsRT)
            {
                if (_reelsDictionary[reelRT].ReelID == _reelsRT.Length)
                {
                    if (_reelsDictionary[reelRT].ReelState == ReelState.Stop)
                    {
                        ForceAnimStop();
                    }
                    
                    else
                    {
                        isForceStop = true;
                        ForceScrollStop();
                    }
                }
            }


            return isForceStop;
        }

        private void ForceAnimStop()
        {
            _stopButton.interactable = false;
            _animationManager.ForceStopWinAnimation();
        }
        
        private void ForceScrollStop()
        {
            _stopButton.interactable = false;

            foreach (var reelRT in _reelsRT)
            {
                if (_reelsDictionary[reelRT].ReelState == ReelState.Spin)
                {
                    _reelsDictionary[reelRT].ReelState = ReelState.ForceStopping;
                    // DOTween.Kill(reelRT);
                    _reelsScroll.ReelCorrection(reelRT);
                }
            }
        }
        //
        // public void ForceStop()
        // {
        //     foreach (var reelRT in reelsRT)
        //     {
        //         if (_reelsDictionary[reelRT].ReelState == ReelState.Stop)
        //         {
        //             stopButton.interactable = false;
        //             animationManager.ForceStopWinAnimation(_reelsDictionary[reelRT].VisibleSymbolsRTOnReel);
        //         }
        //         else
        //         {
        //             isForceStop = true;
        //             ForceScrollStop();
        //         }
        //
        //     }
        // }
        //
        // private void ForceScrollStop()
        // {
        //     stopButton.interactable = false;
        //
        //     foreach (var reelRT in reelsRT)
        //     {
        //         if (_reelsDictionary[reelRT].ReelState == ReelState.Spin)
        //         {
        //             _reelsDictionary[reelRT].ReelState = ReelState.ForceStopping;
        //             ReelCorrection(reelRT);
        //         }
        //     }
        // }
    }
}