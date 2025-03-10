using System.Collections;
using Infastructure.Management;
using TMPro;
using UnityEngine;

namespace Animation
{
    public class ChangeBalanceAnimation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI balance;
        [SerializeField] private TextMeshProUGUI totalWinBalance;
        [SerializeField] private TextMeshProUGUI totalWinTextPopUp;
        [SerializeField] private TextMeshProUGUI freeSpins;
        [SerializeField] private int duration;
        [SerializeField] private int countFPS;
        [SerializeField] private SoundManager soundManager;

        private int _currentBalance;
        private int _newBalance;
        private int _totalWin;
        private int _currentTotalWinBalance;

        private void Awake()
        {
            _newBalance = PlayerPrefs.GetInt("Balance", 0);
        }

        public IEnumerator ChangeBalance()
        {
            _newBalance = _currentBalance + _totalWin;
            PlayerPrefs.SetInt("Balance", _newBalance);

            soundManager.PlayMusic(SoundType.ChangeBalanceSound);
            WaitForSeconds Wait = new WaitForSeconds(1f / countFPS);
            int stepAmount;

            stepAmount = 1;

            while (_currentBalance < _newBalance)
            {
                _currentBalance += stepAmount;
                _currentTotalWinBalance -= stepAmount;

                if (_currentBalance > _newBalance)
                    _currentBalance = _newBalance;

                balance.SetText(_currentBalance + " $");
                totalWinBalance.SetText(_currentTotalWinBalance + " $");

                yield return Wait;
            }

            _totalWin = _currentTotalWinBalance;
            PlayerPrefs.SetInt("TotalWinBalance", _totalWin);
            soundManager.StopMusic(SoundType.ChangeBalanceSound);
        }

        public void ForceChangeBalance()
        {
            StopCoroutine(ChangeBalance());
            _currentBalance = _newBalance;
            _currentTotalWinBalance = 0;

            balance.SetText(_currentBalance + " $");
            totalWinBalance.SetText(_currentTotalWinBalance + " $");

            _totalWin = _currentTotalWinBalance;
            PlayerPrefs.SetInt("TotalWinBalance", _totalWin);
            soundManager.StopMusic(SoundType.ChangeBalanceSound);
        }

        public void ChangeTotalWinBalance()
        {
            totalWinBalance.SetText(_totalWin + " $");

            _currentTotalWinBalance = _totalWin;
            PlayerPrefs.SetInt("TotalWinBalance", _totalWin);
        }

        public void ChangeFreeSpinsCount(int freeSpinsCount)
        {
            freeSpins.SetText(freeSpinsCount + " FS");
        }

        public void SetValues(int startBalance, int totalWin)
        {
            _currentBalance = startBalance;
            _totalWin += totalWin;
            PlayerPrefs.SetInt("TotalWinBalance", _totalWin);
        }
    }
}