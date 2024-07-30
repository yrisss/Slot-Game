using System.Collections;
using DG.Tweening;
using Infastructure.Management;
using Reels;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    public class AnticipationAnimation : MonoBehaviour
    {
        [SerializeField] private Image fade;
        [SerializeField] private Image frame;
        [SerializeField] private SoundManager soundManager;

        public void StartAnticipationAnim(Reel[] reels, Reel reel, RectTransform reelRT, int currentReelIndex,
            float duration)
        {
            soundManager.PlayMusic(SoundType.AntisipationMusic);
            var position = reelRT.position;
            position = new Vector3(position.x, position.y, 1);
            reelRT.position = position;

            fade.rectTransform.DOScale(Vector3.one, 0f);

            for (int i = currentReelIndex + 1; i < reels.Length; i++)
            {
                foreach (var symbol in reels[i].VisibleSymbolsRTOnReel)
                {
                    Image symbolImage = symbol.GetComponent<Image>();
                    if (symbolImage != null)
                    {
                        symbolImage.DOFade(0.23f, 0f);
                    }
                }
            }

            foreach (var particle in reel.AntisipationParticles)
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }

            fade.rectTransform.DOScale(Vector3.one, 0f);
            fade.DOFade(0.83f, duration / 2);
            frame.DOFade(1f, duration / 2);
            StartCoroutine(StopAnticipationAnim(reels, reel, reelRT, currentReelIndex, duration));
        }

        private IEnumerator StopAnticipationAnim(Reel[] reels, Reel reel, RectTransform reelRT, int currentReelIndex,
            float duration)
        {
            yield return new WaitForSeconds(duration);

            var position = reelRT.position;
            position = new Vector3(position.x, position.y, 0);
            reelRT.position = position;

            for (int i = currentReelIndex + 1; i < reels.Length; i++)
            {
                foreach (var symbol in reels[i].VisibleSymbolsRTOnReel)
                {
                    Image symbolImage = symbol.GetComponent<Image>();
                    if (symbolImage != null)
                    {
                        symbolImage.DOFade(1f, 0.5f);
                    }
                }
            }

            foreach (var particle in reel.AntisipationParticles)
            {
                particle.gameObject.SetActive(false);
                particle.Stop();
            }

            fade.rectTransform.DOScale(Vector3.one, 0f);
            fade.DOFade(0f, 0.5f).OnComplete(() => fade.rectTransform.DOScale(Vector3.zero, 0f));
            frame.DOFade(0f, 0.5f);

            soundManager.StopMusic(SoundType.AntisipationMusic);
        }
    }
}