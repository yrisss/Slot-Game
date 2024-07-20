using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using Infastructure.Management;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    public class AnticipationAnimation : MonoBehaviour
    {
        [SerializeField] private Image fade;
        [SerializeField] private Image frame;
        [SerializeField] private List<UIParticle> antisipationParticle;
        [SerializeField] private RectTransform reel;
        [SerializeField] private SoundManager soundManager;

        public void StartAnticipationAnim(float duration)
        {
            soundManager.PlayMusic(SoundType.AntisipationMusic);
            var position = reel.position;
            position = new Vector3(position.x, position.y, 1);
            reel.position = position;
            
                fade.rectTransform.DOScale(Vector3.one, 0f);
                foreach (var particle in antisipationParticle)
                {
                    particle.gameObject.SetActive(true);
                    particle.Play();
                }

                fade.rectTransform.DOScale(Vector3.one, 0f);
                fade.DOFade(0.83f, duration/2);
                frame.DOFade(1f, duration/2);
                StartCoroutine(StopAnticipationAnim(duration));
        }

        private IEnumerator StopAnticipationAnim(float duration)
        {
            yield return new WaitForSeconds(duration);
            
            var position = reel.position;
            position = new Vector3(position.x, position.y, 0);
            reel.position = position;
            
            foreach (var particle in antisipationParticle)
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