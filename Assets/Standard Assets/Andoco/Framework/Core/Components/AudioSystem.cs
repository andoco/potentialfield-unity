using System;
using UnityEngine;
using System.Collections.Generic;

namespace Andoco.Unity.Framework.Core
{
    public class AudioSystem : MonoBehaviour
    {
        public AudioSource audioSource;

        [Range(0f, 1f)]
        [Tooltip("Sets the AudioListener.volume value")]
        public float globalVolume = 1f;

        void Awake()
        {
            if (this.audioSource == null)
            {
                this.audioSource = this.GetComponent<AudioSource>();
            }

            AudioListener.volume = this.globalVolume;
        }

        void Update()
        {
            AudioListener.volume = this.globalVolume;
        }

        public void PlayClip(AudioBoard board, string key)
        {
            var item = board.GetItem(key);

            if (item.global)
            {
                this.audioSource.PlayOneShot(item.clip);
            }
            else
            {
                board.PlayClip(item.clip);
            }
        }
    }
}
