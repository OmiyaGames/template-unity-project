using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    public class GameSettings : ISingletonScript
    {
        public const int DefaultNumLevelsUnlocked = 1;

        public const float DefaultMusicVolume = 1;
        public const float DefaultSoundVolume = 1;

        public const string NumLevelsUnlockedKey = "Number of Unlocked Levels";
        public const string MusicVolumeKey = "Music Volume";
        public const string MusicMutedKey = "Music Muted";
        public const string SoundVolumeKey = "Sound Volume";
        public const string SoundMutedKey = "Sound Muted";

        [SerializeField]
        bool simulateWebplayer = false;

        int numLevelsUnlocked = 1;
        float musicVolume = 0, soundVolume = 0;
        bool musicMuted = false, soundMuted = false;

        #region Properties
        public bool IsWebplayer
        {
            get
            {
                bool returnIsWebplayer = false;
                if (simulateWebplayer == true)
                {
                    returnIsWebplayer = true;
                }
                else
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.WindowsWebPlayer:
                        case RuntimePlatform.OSXWebPlayer:
                        case RuntimePlatform.WebGLPlayer:
                            returnIsWebplayer = true;
                            break;
                    }
                }
                return returnIsWebplayer;
            }
        }

        public int NumLevelsUnlocked
        {
            get
            {
                return numLevelsUnlocked;
            }
            internal set
            {
                PlayerPrefs.SetInt(NumLevelsUnlockedKey, NumLevelsUnlocked);
            }
        }

        public float MusicVolume
        {
            get
            {
                return musicVolume;
            }
            internal set
            {
                musicVolume = value;
                PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            }
        }

        public bool IsMusicMuted
        {
            get
            {
                return musicMuted;
            }
            internal set
            {
                musicMuted = value;
                PlayerPrefs.SetInt(MusicMutedKey, (musicMuted ? 1 : 0));
            }
        }

        public float SoundVolume
        {
            get
            {
                return soundVolume;
            }
            internal set
            {
                soundVolume = value;
                PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
            }
        }

        public bool IsSoundMuted
        {
            get
            {
                return soundMuted;
            }
            internal set
            {
                soundMuted = value;
                PlayerPrefs.SetInt(SoundMutedKey, (soundMuted ? 1 : 0));
            }
        }
        #endregion

        public override void SingletonAwake(Singleton instance)
        {
            // Load settings
            RetrieveFromSettings();
        }

        public override void SceneAwake(Singleton instance)
        {
        }

        void OnApplicationQuit()
        {
            SaveSettings();
        }

        public void RetrieveFromSettings()
        {
            // Grab the number of levels unlocked
            numLevelsUnlocked = PlayerPrefs.GetInt(NumLevelsUnlockedKey, DefaultNumLevelsUnlocked);

            // Grab the music settings
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
            musicMuted = (PlayerPrefs.GetInt(MusicMutedKey, 0) != 0);

            // Grab the sound settings
            soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, DefaultSoundVolume);
            soundMuted = (PlayerPrefs.GetInt(SoundMutedKey, 0) != 0);

            // NOTE: Feel free to add more stuff here

        }

        public void SaveSettings()
        {
            // Save the number of levels unlocked
            PlayerPrefs.SetInt(NumLevelsUnlockedKey, NumLevelsUnlocked);

            // Save the music settings
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            PlayerPrefs.SetInt(MusicMutedKey, (musicMuted ? 1 : 0));

            // Save the sound settings
            PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
            PlayerPrefs.SetInt(SoundMutedKey, (soundMuted ? 1 : 0));

            // NOTE: Feel free to add more stuff here

            PlayerPrefs.Save();
        }

        public void ClearSettings()
        {
            PlayerPrefs.DeleteAll();
            RetrieveFromSettings();
        }
    }
}
