using System.Collections;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopUp
{
    public class PopUpView : MonoBehaviour
    {
        [SerializeField] private Image fade; // Объект для затемнения экрана
        [SerializeField] private RectTransform bonusGamePopUp; // Попап для бонусной игры
        [SerializeField] private RectTransform totalWinPopUp; // Попап для общего выигрыша
        [SerializeField] private TextMeshProUGUI totalWinText; // Текст для отображения общего выигрыша
        [SerializeField] private TextMeshProUGUI balanceText; // Текст для отображения баланса
        

        public void ShowWinPopUp(int startBalance, AnimationManager animationManager)
        {
            int totalWin = PlayerPrefs.GetInt("Balance", 0) - startBalance;
            totalWinText.text = totalWin + " $";
            ShowPopUp(totalWinPopUp);
            StartCoroutine(HideWinPopUp(totalWinPopUp, animationManager));
        }

        public void ShowBonusGamePopUp(int freeSpinsCounter)
        {
            ChangeFreeSpinsCount(freeSpinsCounter);
            ShowPopUp(bonusGamePopUp);
            StartCoroutine(HideBonusPopUp(bonusGamePopUp));
        }

        public void ChangeFreeSpinsCount(int freeSpinsCounter)
        {
            balanceText.text = freeSpinsCounter + " FS";
        }

        private void ShowPopUp(RectTransform popup)
        {
                fade.rectTransform.DOScale(Vector3.one, 0f);
                fade.DOFade(0.83f, 0.5f);
                popup.DOScale(Vector3.one, 0.5f);
        }

        private IEnumerator HideWinPopUp(RectTransform popup, AnimationManager animationManager)
        {
            yield return new WaitForSeconds(3f);

            fade.DOFade(0f, 0.5f);
            fade.rectTransform.DOScale(Vector3.zero, 0f);
            popup.DOScale(Vector3.zero, 0.5f).OnComplete(() => animationManager.StartChangeBalanceAnimation());
        }

        private IEnumerator HideBonusPopUp(RectTransform popup)
            {
                yield return new WaitForSeconds(3f);

                fade.rectTransform.DOScale(Vector3.zero, 0f);
                fade.DOFade(0f, 0.5f);
                popup.DOScale(Vector3.zero, 0.5f);
            }
        }
    }