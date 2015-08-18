using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GameSettings.cs" company="Omiya Games">
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
    /// A singleton script to retrieve settings stored in <code>PlayerPrefs</code>.
    /// Currently only stores the last unlocked level, music volume, and sound volume.
    /// </summary>
    /// <seealso cref="Singleton"/>
    /// <seealso cref="PlayerPrefs"/>
    public class GameSettings : ISingletonScript
    {
        public enum AppStatus
        {
            FirstTimeOpened,
            RecentlyUpdated,
            Replaying
        }

        /// <summary>
        /// The app version.  Must be positive.
        /// Increment every time a new build is released.
        /// Useful for backwards compatibility.
        /// </summary>
        public const int AppVersion = 0;

        public const int BeginnerBestScoreThreshold = 10;
        public const int NumberOfBeginnerGames = 5;

        public const float DefaultMusicVolume = 1;
        public const float DefaultSoundVolume = 1;
        public const int DefaultBestScore = 0;
        public const string DefaultLanguage = "";

        public const string VersionKey = "AppVersion";
        public const string MusicVolumeKey = "Music Volume";
        public const string MusicMutedKey = "Music Muted";
        public const string SoundVolumeKey = "Sound Volume";
        public const string SoundMutedKey = "Sound Muted";
        public const string BestScoreKey = "PersonalBest";
        public const string LanguageKey = "Language";

        [SerializeField]
        bool simulateWebplayer = false;

        int bestScore = DefaultBestScore;
        float musicVolume = DefaultMusicVolume, soundVolume = DefaultSoundVolume;
        bool musicMuted = false, soundMuted = false;
        AppStatus status = AppStatus.Replaying;
        string language = DefaultLanguage;

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

        public AppStatus Status
        {
            get
            {
                return status;
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

        public int BestScore
        {
            get
            {
                return bestScore;
            }
            set
            {
                if(bestScore < value)
                {
                    bestScore = value;
                    PlayerPrefs.SetInt(BestScoreKey, bestScore);
                }
            }
        }

        public string Language
        {
            get
            {
                return language;
            }
            internal set
            {
                language = value;
                PlayerPrefs.SetString(LanguageKey, language);
            }
        }

        public bool IsBeginnerModeOn
        {
            get
            {
                return (BestScore < BeginnerBestScoreThreshold);
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
            // Grab the the app version
            int currentVersion = PlayerPrefs.GetInt(VersionKey, -1);

            // Update the app status
            status = AppStatus.Replaying;
            if(currentVersion < 0)
            {
                status = AppStatus.FirstTimeOpened;
            }
            else if(currentVersion < AppVersion)
            {
                status = AppStatus.RecentlyUpdated;
            }

            // Set the version
            PlayerPrefs.SetInt(VersionKey, AppVersion);

            // Grab the music settings
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
            musicMuted = (PlayerPrefs.GetInt(MusicMutedKey, 0) != 0);

            // Grab the sound settings
            soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, DefaultSoundVolume);
            soundMuted = (PlayerPrefs.GetInt(SoundMutedKey, 0) != 0);

            // Grab the best score
            bestScore = PlayerPrefs.GetInt(BestScoreKey, DefaultBestScore);

            // Grab the language
            language = PlayerPrefs.GetString(LanguageKey, DefaultLanguage);
        }

        public void SaveSettings()
        {
            // Save the music settings
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            PlayerPrefs.SetInt(MusicMutedKey, (musicMuted ? 1 : 0));

            // Save the sound settings
            PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
            PlayerPrefs.SetInt(SoundMutedKey, (soundMuted ? 1 : 0));

            // Set the best score
            PlayerPrefs.SetInt(BestScoreKey, bestScore);

            // Set the language
            PlayerPrefs.SetString(LanguageKey, language);

            PlayerPrefs.Save();
        }

        public void ClearSettings()
        {
            PlayerPrefs.DeleteAll();
            RetrieveFromSettings();
        }
    }
}
