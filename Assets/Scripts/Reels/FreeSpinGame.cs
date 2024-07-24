using System.Collections;
using System.Collections.Generic;
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


        public FreeSpinGame(ReelsScroll reelsScroll, RectTransform[] reelsRT, Dictionary<RectTransform, Reel> reelsDictionary, Button stopButton, int freeSpinsCount, PopUpView popUpView, AnimationManager animationManager, SoundManager soundManager, WinChecker winChecker, ScatterChecker scatterChecker)
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
        }

        public void TryStartFreeSpins(RectTransform reelRT)
        {
            if (_reelsDictionary[reelRT].ReelID == _reelsRT.Length &&
                _reelsDictionary[reelRT].ReelState == ReelState.Stop)
            {
                _soundManager.StopMusic(SoundType.ScrollingSound);
                if (isFreeSpin && _freeSpinsCounter > 0)
                {
                    FreeSpin();
                    _popUpView.ChangeFreeSpinsCount(_freeSpinsCounter);
                    _freeSpinsCounter--;
                }
                else if (isFreeSpin && _freeSpinsCounter == 0)
                {
                    _soundManager.StopMusic(SoundType.FreeSpinsMusic);
                    _soundManager.PlayMusic(SoundType.BackMusic);
                    isFreeSpin = false;
                    _animationManager.ONWinAnimationComplete = null;
                    _popUpView.ChangeFreeSpinsCount(_freeSpinsCounter);
                    _animationManager.ONWinAnimationComplete += ShowTotalWinPopUp;
                    _animationManager.ONWinAnimationComplete += _reelsScroll.ShowPlayButton;
                    _trueWinLines = _winChecker.CheckResult();
                    if (_trueWinLines.Count != 0)
                    {
                        _stopButton.interactable = true;
                        _animationManager.StartWinAnimation(_trueWinLines);
                    }
                }
                else if (_scatterChecker.FreeSpinsChecker(_reelsDictionary[reelRT].ReelID - 1) >= 3 && !isFreeSpin)
                {
                    _soundManager.StopMusic(SoundType.BackMusic);
                    _soundManager.PlayMusic(SoundType.FreeSpinsMusic);
                    _reelsScroll.StartCoroutine(StartFreeSpins());
                }
                else
                {
                    _animationManager.ONWinAnimationComplete += _animationManager.StartChangeBalanceAnimation;
                    _animationManager.ONWinAnimationComplete += _reelsScroll.ShowPlayButton;
                    _trueWinLines = _winChecker.CheckResult();
                    if (_trueWinLines.Count != 0)
                    {
                        _reelsScroll.HidePlayButton();
                        _animationManager.StartWinAnimation(_trueWinLines);
                    }
                }
            }
        }


        private void ShowTotalWinPopUp()
        {
            _popUpView.ShowWinPopUp(_startBalance, _animationManager);
        }

        private IEnumerator StartFreeSpins()
        {
            _stopButton.interactable = false;
            _animationManager.ONWinAnimationComplete = null;
            _startBalance = PlayerPrefs.GetInt("Balance", 0);
            isFreeSpin = true;
            _animationManager.ONWinAnimationComplete += _reelsScroll.ScrollStart;
            _freeSpinsCounter = _freeSpinsCount;
            _popUpView.ShowBonusGamePopUp(_freeSpinsCounter);
            yield return new WaitForSeconds(3f);
            _reelsScroll.ScrollStart();
        }

        private void FreeSpin()
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