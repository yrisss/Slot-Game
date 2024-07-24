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

        private bool isFreeSpinGame;
        private int _freeSpinsCounter;
        private List<int[]> _trueWinLines;
        private int _startBalance = 0;

        public bool IsFreeSpinGame => isFreeSpinGame;

        public FreeSpinGame(ReelsScroll reelsScroll, RectTransform[] reelsRT,
            Dictionary<RectTransform, Reel> reelsDictionary, Button stopButton, int freeSpinsCount, PopUpView popUpView,
            AnimationManager animationManager, SoundManager soundManager, WinChecker winChecker,
            ScatterChecker scatterChecker)
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

        public bool TryStartFreeSpins(RectTransform reelRT)
        {
            isFreeSpinGame = false;
            
            if (_reelsDictionary[reelRT].ReelID == _reelsRT.Length &&
                _reelsDictionary[reelRT].ReelState == ReelState.Stop)
            {
                _soundManager.StopMusic(SoundType.ScrollingSound);

                if (_scatterChecker.FreeSpinsChecker(_reelsDictionary[reelRT].ReelID - 1) >= 3)
                {
                    _soundManager.StopMusic(SoundType.BackMusic);
                    _soundManager.PlayMusic(SoundType.FreeSpinsMusic);
                    _freeSpinsCounter = _freeSpinsCount;
                    isFreeSpinGame = true;
                }
                else
                {
                    isFreeSpinGame = false;
                }
            }

            return isFreeSpinGame;
        }

        public void StartFreeSpin()
        {
            if (_freeSpinsCounter == _freeSpinsCount)
                _reelsScroll.StartCoroutine(StartFreeSpinsCoroutine());
            
            else if (_freeSpinsCounter > 0)
            {
                _reelsScroll.ScrollStart();
                _freeSpinsCounter--;
                _popUpView.ChangeFreeSpinsCount(_freeSpinsCounter);
            }

            else
            {
                isFreeSpinGame = false;
                
                _soundManager.StopMusic(SoundType.FreeSpinsMusic);
                _soundManager.PlayMusic(SoundType.BackMusic);
                
                _popUpView.ChangeFreeSpinsCount(_freeSpinsCounter);
                
                _animationManager.ONWinAnimationComplete = null;
                _animationManager.ONWinAnimationComplete += ShowTotalWinPopUp;
                _animationManager.ONWinAnimationComplete += _reelsScroll.ShowPlayButton;
                
                _trueWinLines = _winChecker.CheckResult();
                if (_trueWinLines.Count != 0)
                {
                    _stopButton.interactable = true;
                    _animationManager.StartWinAnimation(_trueWinLines);
                }
                else
                {
                    _animationManager.ONWinAnimationComplete?.Invoke();
                }
            }
        }


        private void ShowTotalWinPopUp()
        {
            _popUpView.ShowWinPopUp(_startBalance, _animationManager);
        }

        private IEnumerator StartFreeSpinsCoroutine()
        {
            _stopButton.interactable = false;
            _startBalance = PlayerPrefs.GetInt("Balance", 0);
            
            _animationManager.ONWinAnimationComplete = null;
            _animationManager.ONWinAnimationComplete += FreeSpin;
            
            _popUpView.ShowBonusGamePopUp(_freeSpinsCounter);
            yield return new WaitForSeconds(3f);
            
            _reelsScroll.ScrollStart();
            _freeSpinsCounter--;
            _popUpView.ChangeFreeSpinsCount(_freeSpinsCounter);
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
            {
                StartFreeSpin();
            }
        }
    }
}