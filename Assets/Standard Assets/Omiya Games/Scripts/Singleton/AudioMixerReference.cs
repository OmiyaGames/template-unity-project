using UnityEngine;
using UnityEngine.Audio;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AudioMixerReference.cs" company="Omiya Games">
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
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script to interface with the AudioMixer.
    /// </summary>
    /// <seealso cref="AudioSource"/>
    /// <seealso cref="SoundEffect"/>
    /// <seealso cref="AmbientMusic"/>
    /// <seealso cref="BackgroundMusic"/>
    public class AudioMixerReference : ISingletonScript
    {
        [SerializeField]
        float muteVolumeDb = -80;
        [SerializeField]
        AudioMixer mixer = null;

        [Header("Volume Settings")]
        [SerializeField]
        string[] backgroundMusicVolume = new string[] { "Music Volume" };
        [SerializeField]
        string[] soundEffectsVolume = new string[] { "Sound Effects Volume" };

        [Header("Pitch Settings")]
        [SerializeField]
        string[] backgroundMusicPitch = new string[] { "Music Pitch" };
        [SerializeField]
        string[] soundEffectsPitch = new string[] { "Sound Effects Pitch" };

        [Header("Pause Effect Settings")]
        [SerializeField]
        string musicDuckField = "Music Duck Level";

        int index = 0;
        bool isSetup = false;

        #region Properties
        public float MuteVolumeDb
        {
            get
            {
                return muteVolumeDb;
            }
        }

        public float BackgroundMusicVolumeDb
        {
            get
            {
                float returnVolume = MuteVolumeDb;
                for (index = 0; index < backgroundMusicVolume.Length; ++index)
                {
                    if (mixer.GetFloat(backgroundMusicVolume[index], out returnVolume) == true)
                    {
                        break;
                    }
                }
                return returnVolume;
            }
            internal set
            {
                for (index = 0; index < backgroundMusicVolume.Length; ++index)
                {
                    mixer.SetFloat(backgroundMusicVolume[index], value);
                }
            }
        }

        public float BackgroundMusicVolumeNormalized
        {
            get
            {
                return Mathf.InverseLerp(MuteVolumeDb, 0f, BackgroundMusicVolumeDb);
            }
            internal set
            {
                BackgroundMusicVolumeDb = Mathf.Lerp(MuteVolumeDb, 0f, value);
            }
        }

        public float SoundEffectsVolumeDb
        {
            get
            {
                float returnVolume = MuteVolumeDb;
                for (index = 0; index < soundEffectsVolume.Length; ++index)
                {
                    if (mixer.GetFloat(soundEffectsVolume[index], out returnVolume) == true)
                    {
                        break;
                    }
                }
                return returnVolume;
            }
            internal set
            {
                for (index = 0; index < soundEffectsVolume.Length; ++index)
                {
                    mixer.SetFloat(soundEffectsVolume[index], value);
                }
            }
        }

        public float SoundEffectsVolumeNormalized
        {
            get
            {
                return Mathf.InverseLerp(MuteVolumeDb, 0f, SoundEffectsVolumeDb);
            }
            internal set
            {
                SoundEffectsVolumeDb = Mathf.Lerp(MuteVolumeDb, 0f, value);
            }
        }

        public float BackgroundMusicPitch
        {
            get
            {
                float returnPitch = 1;
                for (index = 0; index < backgroundMusicPitch.Length; ++index)
                {
                    if (mixer.GetFloat(backgroundMusicPitch[index], out returnPitch) == true)
                    {
                        break;
                    }
                }
                return returnPitch;
            }
            internal set
            {
                for (index = 0; index < backgroundMusicPitch.Length; ++index)
                {
                    mixer.SetFloat(backgroundMusicPitch[index], value);
                }
            }
        }

        public float SoundEffectsPitch
        {
            get
            {
                float returnPitch = 1;
                for (index = 0; index < soundEffectsPitch.Length; ++index)
                {
                    if (mixer.GetFloat(soundEffectsPitch[index], out returnPitch) == true)
                    {
                        break;
                    }
                }
                return returnPitch;
            }
            internal set
            {
                for (index = 0; index < soundEffectsPitch.Length; ++index)
                {
                    mixer.SetFloat(soundEffectsPitch[index], value);
                }
            }
        }
        #endregion

        #region implemented abstract members of ISingletonScript
        public override void SingletonAwake(Singleton instance)
        {
            // Do nothing
        }
        
        public override void SceneAwake(Singleton instance)
        {
            // Check if we need to setup
            if(isSetup == false)
            {
                // Retrieve settings
                GameSettings settings = Singleton.Get<GameSettings>();
                if (settings != null)
                {
                    SetupVolumeAndMute(settings);
                }

                // Check the TimeManager event
                TimeManager manager = Singleton.Get<TimeManager>();
                if(manager != null)
                {
                    manager.OnManuallyPausedChanged += OnPauseChanged;
                }

                // Indicate we don't need to setup anymore
                isSetup = true;
            }
        }
        #endregion

        /// <summary>
        /// Mute both music and sound effects without affecting the Game Settings.
        /// </summary>
        public void SetTemporaryMuteAll(bool mute)
        {
            if(mute == true)
            {
                BackgroundMusicVolumeDb = MuteVolumeDb;
                SoundEffectsVolumeDb = MuteVolumeDb;
            }
            else
            {
                if(BackgroundMusic.GlobalMute == false)
                {
                    BackgroundMusicVolumeNormalized = BackgroundMusic.GlobalVolume;
                }
                if(SoundEffect.GlobalMute == false)
                {
                    SoundEffectsVolumeNormalized = SoundEffect.GlobalVolume;
                }
            }
        }

        #region Helper Methods
        void SetupVolumeAndMute(GameSettings settings)
        {
            // Check if the background music was muted
            if (settings.IsMusicMuted == true)
            {
                // Mute the background music
                BackgroundMusicVolumeDb = MuteVolumeDb;
            }
            else
            {
                // Set the background music volume based on settings
                BackgroundMusicVolumeNormalized = settings.MusicVolume;
            }

            // Check if the background music was muted
            if (settings.IsSoundMuted == true)
            {
                // Mute the background music
                SoundEffectsVolumeDb = MuteVolumeDb;
            }
            else
            {
                // Set the background music volume based on settings
                SoundEffectsVolumeNormalized = settings.SoundVolume;
            }
        }

        void OnPauseChanged(TimeManager pauseCheck)
        {
            if (pauseCheck.IsManuallyPaused == true)
            {
                mixer.SetFloat(musicDuckField, 0f);
            }
            else
            {
                mixer.SetFloat(musicDuckField, MuteVolumeDb);
            }
        }
        #endregion
    }
}