using System.Collections;
using Infastructure.Management;
using TMPro;
using UnityEngine;

namespace Animation
{
    public class ChangeBalanceAnimation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI balance;
        [SerializeField] private int duration;
        [SerializeField] private int countFPS;
        [SerializeField] private SoundManager soundManager;
    
        private int _currentBalance;
        private int _newBalance;
    
        public IEnumerator ChangeBalance()
        {
            soundManager.PlayMusic(SoundType.ChangeBalanceSound);
            WaitForSeconds Wait = new WaitForSeconds(1f / countFPS);
            int stepAmount;

            stepAmount = 1;//Mathf.CeilToInt((newBalance - currentBalance) / (_duration));
        
            while (_currentBalance < _newBalance)
            {
                _currentBalance += stepAmount;
            
                if (_currentBalance > _newBalance)
                    _currentBalance = _newBalance;

                balance.SetText(_currentBalance + " $");

                yield return Wait;
            }
            soundManager.StopMusic(SoundType.ChangeBalanceSound);
        }

        public void SetValues(int startBalance, int newBalance)
        {
            _currentBalance = startBalance;
            _newBalance = newBalance;
        }
    }
}
