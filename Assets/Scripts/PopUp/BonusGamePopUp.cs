using System.Collections;
using TMPro;
using UnityEngine;

namespace PopUp
{
    public class BonusGamePopUp : MonoBehaviour, IPopUp
    {
        [SerializeField] private RectTransform fade;
        [SerializeField] private RectTransform bonusGamePopUp;
        [SerializeField] private TextMeshProUGUI balanceText;
        
        public void ShowPopUp()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator HidePopUp()
        {
            throw new System.NotImplementedException();
        }
    }
}