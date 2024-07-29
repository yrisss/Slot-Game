using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Infastructure.Management;
using Infastructure.Services;
using UnityEngine;
using UnityEngine.UI;
using View.PopUp;


namespace Reels
{
    public class FreeSpinGame
    {
        private ReelsScroll _reelsScroll;

        private RectTransform[] _reelsRT;
        private Dictionary<RectTransform, Reel> _reelsDictionary;

        private Button _stopButton;
        private RectTransform _freeSpinsCountFrameRT;

        private int _freeSpinsCount;

        private PopUpView _popUpView;
        private AnimationManager _animationManager;
        private SoundManager _soundManager;
        private WinChecker _winChecker;
        private ScatterChecker _scatterChecker;

        private bool isFreeSpin;
        private int _freeSpinsCounter;
        private List<int[]> _trueWinLines;
        private int _startBalance = 0;


        public FreeSpinGame(ReelsScroll reelsScroll, RectTransform[] reelsRT,
            Dictionary<RectTransform, Reel> reelsDictionary, Button stopButton, int freeSpinsCount, PopUpView popUpView,
            AnimationManager animationManager, SoundManager soundManager, WinChecker winChecker,
            ScatterChecker scatterChecker, RectTransform freeSpinsCountFrameRT)
        {
            _reelsScroll = reelsScroll;
            _reelsRT = reelsRT;
            _reelsDictionary = reelsDictionary;
            _stopButton = stopButton;
            _freeSpinsCount = freeSpinsCount;
            _popUpView = popUpView;
            _animationManager = animationManager;
            _soundManager = soundManager;
            _winChecker = winChecker;
            _scatterChecker = scatterChecker;
            _freeSpinsCountFrameRT = freeSpinsCountFrameRT;
        }

        public bool TryStartFreeSpins(RectTransform reelRT)
        {
            if (_reelsDictionary[reelRT].ReelState == ReelState.Stop)
            {
                if (isFreeSpin)
                {
                    FreeSpinScroll();
                    return true;
                }

                if (_scatterChecker.FreeSpinsChecker(_reelsDictionary[reelRT].ReelID - 1) >= 3)
                {
                    _trueWinLines = _winChecker.CheckResult();
                    if (_trueWinLines.Count != 0)
                    {
                        _stopButton.interactable = true;
                        _animationManager.StartWinAnimation(_trueWinLines);
                        _animationManager.ONWinAnimationComplete = null;
                        _animationManager.ONWinAnimationComplete += _animationManager.StartChangeBalanceAnimation;
                        _animationManager.ONWinAnimationComplete += StartFreeSpin;
                    }
                    else
                    {
                        _stopButton.interactable = true;
                        StartFreeSpin();
                    }

                    return true;
                }
                else
                    return false;
            }

            return false;
        }

        private void StartFreeSpin()
        {
            _soundManager.StopMusic(SoundType.BackMusic);
            _soundManager.PlayMusic(SoundType.FreeSpinsMusic);
            _reelsScroll.StartCoroutine(StartFreeSpins());
        }

        private void FreeSpinScroll()
        {
            if (_freeSpinsCounter > 0)
            {
                FreeSpinStop();
                _animationManager.ChangeFreeSpinsCount(_freeSpinsCounter);
                _freeSpinsCounter--;
            }
            else
            {
                _reelsScroll.isFreeSpinGame = false;
                _freeSpinsCountFrameRT.localScale = Vector3.zero;
                _soundManager.StopMusic(SoundType.FreeSpinsMusic);
                _soundManager.PlayMusic(SoundType.BackMusic);
                isFreeSpin = false;
                _animationManager.ChangeFreeSpinsCount(_freeSpinsCounter);
                _animationManager.ONWinAnimationComplete = null;
                _animationManager.ONWinAnimationComplete += _reelsScroll.ShowPlayButton;
                _animationManager.ONWinAnimationComplete += ShowTotalWinPopUp;
                _trueWinLines = _winChecker.CheckResult();
                if (_trueWinLines.Count != 0)
                {
                    _stopButton.interactable = true;
                    _animationManager.StartWinAnimation(_trueWinLines);
                }
                else
                {
                    _popUpView.ONHideWinPopUpComplete += () => _reelsScroll.ShowPlayButton();
                    ShowTotalWinPopUp();
                }
            }
        }


        private void ShowTotalWinPopUp()
        {
            _popUpView.ShowWinPopUp(_animationManager);
        }

        private IEnumerator StartFreeSpins()
        {
            _freeSpinsCountFrameRT.localScale = Vector3.one;
            _reelsScroll.HidePlayButton();
            _stopButton.interactable = false;
            _startBalance = PlayerPrefs.GetInt("Balance", 0);
            isFreeSpin = true;
            _reelsScroll.isFreeSpinGame = true;
            _animationManager.ONWinAnimationComplete = null;
            _animationManager.ONWinAnimationComplete += _reelsScroll.ScrollStart;
            _freeSpinsCounter = _freeSpinsCount;
            _animationManager.ChangeFreeSpinsCount(_freeSpinsCounter);
            _popUpView.ShowBonusGamePopUp();
            yield return new WaitForSeconds(3f);
            _reelsScroll.ScrollStart();
            _freeSpinsCounter--;
        }

        private void FreeSpinStop()
        {
            _trueWinLines = _winChecker.CheckResult();
            if (_trueWinLines.Count > 0)
            {
                _stopButton.interactable = true;
                _animationManager.StartWinAnimation(_trueWinLines);
            }
            else
                _reelsScroll.ScrollStart();
        }
    }
}