using Data;
using TMPro;
using UnityEngine;

namespace View
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private SlotsGameData slotsGameData;
        [SerializeField] private TextMeshProUGUI balance;

        private MainCanvas currentSlotCanvas;
        private GameObject currentSlotObject;
        
        public void LoadSlot(int index)
        {
            currentSlotObject = Instantiate(slotsGameData.SlotsGame[index]);
            currentSlotCanvas = currentSlotObject.GetComponent<MainCanvas>();
            currentSlotCanvas.ONExit += UpdateBalance;
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
