using System;
using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace Andoco.Unity.Framework.Core
{
    public class AudioBoard : MonoBehaviour
    {
        public AudioSource audioSource;

        public List<Item> audioClips;

        void Awake()
        {
            if (this.audioSource == null)
            {
                this.audioSource = this.GetComponent<AudioSource>();
            }
        }

        public AudioClip GetClip(string key)
        {
            return this.GetItem(key).clip;
        }

        public Item GetItem(string key)
        {
            for (int i = 0; i < this.audioClips.Count; i++)
            {
                var item = this.audioClips[i];

                if (string.Equals(key, item.key, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return null;
        }

        public void PlayClip(string key)
        {
            this.audioSource.PlayOneShot(this.GetClip(key));
        }

        public void PlayClip(AudioClip clip)
        {
            this.audioSource.PlayOneShot(clip);
        }

        [Serializable]
        public class Item
        {
            public string key;
            public AudioClip clip;
            public bool global;
        }
    }
}
