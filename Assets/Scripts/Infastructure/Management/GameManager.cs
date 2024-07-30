using TMPro;
using UnityEngine;

namespace Infastructure.Management
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI balance;

        private void Start()
        {
            var currentBalance = PlayerPrefs.GetInt("Balance", 0);
            balance.text = currentBalance + " $";
        }
    }
}