using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Management
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private AudioSource backMusic;
        [SerializeField] private AudioSource antisipationMusic;
        [SerializeField] private AudioSource freeSpinsMusic;
        [SerializeField] private AudioSource scrollingSound;
        [SerializeField] private AudioSource changeBalanceSound;
        [Header("Clips")]
        [SerializeField] private AudioClip clickButtonSound;
        [SerializeField] private AudioClip scatterSound;
        [SerializeField] private AudioClip winLineSound;
        [SerializeField] private AudioClip stopScrollinggSound;

    
        private AudioSource _source;
        private Dictionary<SoundType, AudioClip> _clips;
        private Dictionary<SoundType, AudioSource> _sources;

        private void Start()
        {
            _source = GetComponent<AudioSource>();
            _clips = new Dictionary<SoundType, AudioClip>()
            {
                {SoundType.ClickButtonSound, clickButtonSound},
                {SoundType.ScatterSound, scatterSound},
                {SoundType.WinLineSound, winLineSound},
                {SoundType.StopScrollingSound, stopScrollinggSound},
            };
        
            _sources = new Dictionary<SoundType, AudioSource>()
            {
                {SoundType.BackMusic, backMusic},
                {SoundType.AntisipationMusic, antisipationMusic},
                {SoundType.FreeSpinsMusic, freeSpinsMusic},
                {SoundType.ChangeBalanceSound, changeBalanceSound},
                {SoundType.ScrollingSound, scrollingSound},
            };
        }
    
        public void PlayMusic(SoundType type)
        {
            var music = _sources[type];
            music.Play();
        }

        public void StopMusic(SoundType type)
        {
            var music = _sources[type];
            music.Stop();
        }
    
        public void PlaySound(SoundType type)
        {
            var sound = _clips[type];
            PlaySound(sound);
        }

        private void PlaySound(AudioClip clip)
        {
            _source.PlayOneShot(clip);
        }

        public void OffSounds()
        {
            AudioListener.volume = AudioListener.volume > 0 ? 0 : 1;
        }

    }


    public enum SoundType
    {
        BackMusic,
        AntisipationMusic,
        FreeSpinsMusic,
        ClickButtonSound,
        ChangeBalanceSound,
        ScatterSound,
        WinLineSound,
        ScrollingSound,
        StopScrollingSound
    }
}