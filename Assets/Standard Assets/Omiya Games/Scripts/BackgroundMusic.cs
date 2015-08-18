using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        static readonly HashSet<BackgroundMusic> allBackgroundMusics = new HashSet<BackgroundMusic>();

        AudioSource audioCache = null;

        #region Static Properties
        public static float GlobalVolume
        {
            get
            {
                return Mathf.Clamp01(Singleton.Get<GameSettings>().MusicVolume);
            }
            set
            {
                // First, set the music volume
                GameSettings settings = Singleton.Get<GameSettings>();
                settings.MusicVolume = Mathf.Clamp01(value);
                
                // Update the AudioMixerReference, if NOT muted
                if(settings.IsMusicMuted == false)
                {
                    Singleton.Get<AudioMixerReference>().BackgroundMusicVolumeNormalized = settings.MusicVolume;
                }
            }
        }
        
        public static bool GlobalMute
        {
            get
            {
                return Singleton.Get<GameSettings>().IsMusicMuted;
            }
            set
            {
                // First, set the music setting
                GameSettings settings = Singleton.Get<GameSettings>();
                settings.IsMusicMuted = value;
                
                // Update the AudioMixerReference to either mute or revert the volume back to settings
                AudioMixerReference audioMixer = Singleton.Get<AudioMixerReference>();
                if(settings.IsMusicMuted == true)
                {
                    audioMixer.BackgroundMusicVolumeDb = audioMixer.MuteVolumeDb;
                }
                else
                {
                    audioMixer.BackgroundMusicVolumeNormalized = settings.MusicVolume;
                }
            }
        }

        public static float GlobalPitch
        {
            get
            {
                return Singleton.Get<AudioMixerReference>().BackgroundMusicPitch;
            }
            set
            {
                Singleton.Get<AudioMixerReference>().BackgroundMusicPitch = value;
            }
        }

        public static IEnumerable<BackgroundMusic> AllBackgroundMusics
        {
            get
            {
                return allBackgroundMusics;
            }
        }
        #endregion

        #region Local Properties
        public AudioSource Audio
        {
            get
            {
                if (audioCache == null)
                {
                    audioCache = GetComponent<AudioSource>();
                }
                return audioCache;
            }
        }

        public float Pitch
        {
            get
            {
                return Audio.pitch;
            }
            set
            {
                Audio.pitch = value;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return Audio.isPlaying;
            }
        }
        #endregion

        #region Unity Events
        void Start()
        {
            // Calculate how the audio should behave
            allBackgroundMusics.Add(this);
        }
        
        void OnDestroy()
        {
            allBackgroundMusics.Remove(this);
        }
        #endregion
        
        public void Play()
        {
            // Play the audio
            Audio.Play();
        }
        
        public void Stop()
        {
            Audio.Stop();
        }

        public void Pause()
        {
            Audio.Pause();
        }

        public void UnPause()
        {
            Audio.UnPause();
        }
    }
}
