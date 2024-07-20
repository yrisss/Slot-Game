using Data;
using TMPro;
using UnityEngine;

namespace View
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private SlotsGameData slotsGameData;
        [SerializeField] private TextMeshProUGUI balance;
        
        public void LoadSlot(int index)
        {
            Instantiate(slotsGameData.SlotsGame[index]);
            UpdateBalance();
        }

        private void Start()
        {
            UpdateBalance();
        }

        private void UpdateBalance()
        {
            var currentBalance = PlayerPrefs.GetInt("Balance", 0);
            balance.text = currentBalance + " $";
        }
    }
}
