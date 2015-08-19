using UnityEngine;
using UnityEngine.Audio;

namespace OmiyaGames
{
    public class AudioMixerReference : ISingletonScript
    {
        [SerializeField]
        float muteVolumeDb = -80;
        [SerializeField]
        AudioMixer mixer = null;
        [SerializeField]
        string[] backgroundMusicVolume = new string[] { "Music Volume" };
        [SerializeField]
        string[] soundEffectsVolume = new string[] { "Sound Effects Volume" };
        [SerializeField]
        string[] backgroundMusicPitch = new string[] { "Music Pitch" };
        [SerializeField]
        string[] soundEffectsPitch = new string[] { "Sound Effects Pitch" };

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

                // Check if the background music was muted
                if(settings.IsMusicMuted == true)
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
                if(settings.IsSoundMuted == true)
                {
                    // Mute the background music
                    SoundEffectsVolumeDb = MuteVolumeDb;
                }
                else
                {
                    // Set the background music volume based on settings
                    SoundEffectsVolumeNormalized = settings.SoundVolume;
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
    }
}