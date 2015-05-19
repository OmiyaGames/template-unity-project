using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="BackgroundMusic.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <author>Taro Omiya</author>
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that allows smooth transitions between 2 background musics.
    /// </summary>
    /// <seealso cref="Singleton"/>
    public class BackgroundMusic : MonoBehaviour
    {
        [System.Serializable]
        public class MusicInfo
        {
            [SerializeField]
            AudioSource source = null;
            [SerializeField]
            AudioMixerSnapshot snapshot = null;

            public AudioSource Source
            {
                get
                {
                    return source;
                }
            }

            public AudioClip Clip
            {
                get
                {
                    return source.clip;
                }
            }

            public AudioMixerSnapshot Snapshot
            {
                get
                {
                    return snapshot;
                }
            }

            public void ChangeClip(AudioClip clip, float transitionTime)
            {
                if(Source.clip != null)
                {
                    Source.Stop();
                }
                source.clip = clip;
                if(clip != null)
                {
                    Source.Play();
                }
                Snapshot.TransitionTo(transitionTime);
            }
        }

        [Tooltip("The transition length (in seconds) between 2 background musics. Set to -1 if you want no transition.")]
        [SerializeField]
        float transitionDuration = 1;
        [SerializeField]
        MusicInfo music1 = null;
        [SerializeField]
        MusicInfo music2 = null;

        bool isPlayingMusic1 = true;
        int index = 0;

        /// <summary>
        /// Gets or sets the volume of the background music, which is a value between 0 and 1.
        /// </summary>
        /// <value>The background music's volume.</value>
        public float Volume
        {
            get
            {
                return Mathf.Clamp01(Singleton.Get<GameSettings>().MusicVolume);
            }
            set
            {
                // Set volume
                GameSettings settings = Singleton.Get<GameSettings>();
                settings.MusicVolume = Mathf.Clamp01(value);

                // Update audio sources
                music1.Source.volume = settings.MusicVolume;
                music2.Source.volume = settings.MusicVolume;
            }
        }

        public bool IsMuted
        {
            get
            {
                return Singleton.Get<GameSettings>().IsMusicMuted;
            }
            set
            {
                // Set mute
                GameSettings settings = Singleton.Get<GameSettings>();
                settings.IsMusicMuted = value;

                // Update audio sources
                music1.Source.mute = value;
                music2.Source.mute = value;
            }
        }

        public AudioClip CurrentMusic
        {
            get
            {
                return CurrentAudioSource.Clip;
            }
            set
            {
                // Check if this is a different clip
                if (CurrentAudioSource.Clip != value)
                {
                    // Swap to the next audio source
                    isPlayingMusic1 = !isPlayingMusic1;
                    if(isPlayingMusic1 == true)
                    {
                        music1.ChangeClip(value, transitionDuration);
                    }
                    else
                    {
                        music2.ChangeClip(value, transitionDuration);
                    }
                }
            }
        }

        MusicInfo CurrentAudioSource
        {
            get
            {
                if(isPlayingMusic1 == true)
                {
                    return music1;
                }
                else
                {
                    return music2;
                }
            }
        }

        MusicInfo TransitionAudioSource
        {
            get
            {
                if(isPlayingMusic1 == true)
                {
                    return music2;
                }
                else
                {
                    return music1;
                }
            }
        }
    }
}
