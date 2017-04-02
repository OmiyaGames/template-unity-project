using UnityEngine.SocialPlatforms;
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GameSettings.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
    /// <date>12/8/2016</date>
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

        public event Action<GameSettings> OnRetrieveSettings;
        public event Action<GameSettings> OnSaveSettings;
        public event Action<GameSettings> OnClearSettings;

        /// <summary>
        /// The app version.  Must be positive.
        /// Increment every time a new build is released.
        /// Useful for backwards compatibility.
        /// </summary>
        public const int AppVersion = 0;
        public const string VersionKey = "AppVersion";
        public static readonly ISettings DefaultSettings = new PlayerPrefsSettings();
        AppStatus status = AppStatus.Replaying;

        #region Version 0 Settings Consts
        public const int DefaultNumLevelsUnlocked = 1;
        public const int LocalHighScoresMaxListSize = 10;
        const char ScoreDivider = '\n';

        public const float DefaultMusicVolume = 1;
        public const float DefaultSoundVolume = 1;
        public const string DefaultLanguage = "";
        public const int DefaultBestScore = 0;
        public const float DefaultSensitivity = 0.5f;

        public const string NumLevelsUnlockedKey = "Number of Unlocked Levels";
        public const string MusicVolumeKey = "Music Volume";
        public const string MusicMutedKey = "Music Muted";
        public const string SoundVolumeKey = "Sound Volume";
        public const string SoundMutedKey = "Sound Muted";
        public const string LanguageKey = "Language";
        public const string LocalHighScoresKey = "Local High Scores";
        public const string LeaderboardUserScopeKey = "Leaderboard User Scope";
        public const string NumberOfTimesAppOpenedKey = "Number of Times App Open";
        public const string TotalPlayTimeKey = "Total Play Time";

        public const string SplitKeyboardAxisKey = "Split Keyboard Axis";
        public const string KeyboardXAxisSensitivityKey = "Keyboard X-Axis Sensitivity";
        public const string KeyboardYAxisSensitivityKey = "Keyboard Y-Axis Sensitivity";

        public const string IsKeyboardXAxisInvertedKey = "Keyboard X-Axis is Inverted";
        public const string IsKeyboardYAxisInvertedKey = "Keyboard Y-Axis is Inverted";

        public const string SplitMouseAxisKey = "Split Mouse Axis";
        public const string MouseXAxisSensitivityKey = "Mouse X-Axis Sensitivity";
        public const string MouseYAxisSensitivityKey = "Mouse Y-Axis Sensitivity";

        public const string IsMouseXAxisInvertedKey = "Mouse X-Axis is Inverted";
        public const string IsMouseYAxisInvertedKey = "Mouse Y-Axis is Inverted";

        public const string ScrollWheelSensitivityKey = "Scroll Wheel Sensitivity";
        public const string IsScrollWheelInvertedKey = "Scroll Wheel is Inverted";

        public const string IsSmoothCameraEnabledKey = "Is Smooth Camera Enabled";
        public const string IsBobbingCameraEnabledKey = "Is Bobbing Camera Enabled";
        public const string IsFlashesEnabledKey = "Is Flashes Enabled";
        public const string IsMotionBlursEnabledKey = "Is Motion Blurs Enabled";
        public const string IsBloomEnabledKey = "Is Bloom Enabled";
        #endregion

        #region Version 0 Settings Member Variables
        readonly StringBuilder fullListBuilder = new StringBuilder();
        readonly List<HighScore> bestScores = new List<HighScore>(LocalHighScoresMaxListSize);
        int numLevelsUnlocked = 1;
        float musicVolume = 0;
        float soundVolume = 0;
        bool musicMuted = false;
        bool soundMuted = false;
        string language = DefaultLanguage;
        UserScope leaderboardUserScope;
        Comparison<int> sortScoresBy = null;
        DateTime lastTimeOpen;
        TimeSpan lastPlayTime = TimeSpan.Zero;
        int numberOfTimesAppOpened = 0;

        bool splitKeyboardAxis = false;
        float keyboardXAxisSensitivity = DefaultSensitivity;
        float keyboardYAxisSensitivity = DefaultSensitivity;
        bool isKeyboardXAxisInverted = false;
        bool isKeyboardYAxisInverted = false;

        bool splitMouseAxis = false;
        float mouseXAxisSensitivity = DefaultSensitivity;
        float mouseYAxisSensitivity = DefaultSensitivity;
        bool isMouseXAxisInverted = false;
        bool isMouseYAxisInverted = false;

        float scrollWheelSensitivity = DefaultSensitivity;
        bool isScrollWheelInverted = false;

        bool isSmoothCameraEnabled = true;
        bool isBobbingCameraEnabled = true;
        bool isFlashesEnabled = true;
        bool isMotionBlursEnabled = true;
        bool isBloomEnabled = true;
        #endregion

        public static UserScope DefaultLeaderboardUserScope
        {
            get
            {
#if (UNITY_IOS || UNITY_ANDROID || UNITY_WINRT)
                return UserScope.FriendsOnly;
#else
                return UserScope.Global;
#endif
            }
        }

        public AppStatus Status
        {
            get
            {
                return status;
            }
        }

        public virtual ISettings Settings
        {
            get
            {
                return DefaultSettings;
            }
        }

        #region Version 0 Settings Properties
        public int NumLevelsUnlocked
        {
            get
            {
                return numLevelsUnlocked;
            }
            set
            {
                numLevelsUnlocked = value;
                Settings.SetInt(NumLevelsUnlockedKey, numLevelsUnlocked);
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
                Settings.SetFloat(MusicVolumeKey, musicVolume);
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
                Settings.SetBool(MusicMutedKey, musicMuted);
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
                Settings.SetFloat(SoundVolumeKey, soundVolume);
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
                Settings.SetBool(SoundMutedKey, soundMuted);
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
                Settings.SetString(LanguageKey, language);
            }
        }

        public UserScope LeaderboardUserScope
        {
            get
            {
                return leaderboardUserScope;
            }
            set
            {
                if (leaderboardUserScope != value)
                {
                    leaderboardUserScope = value;
                    Settings.SetInt(LeaderboardUserScopeKey, (int)leaderboardUserScope);
                }
            }
        }

        public int BestScore
        {
            get
            {
                int returnScore = DefaultBestScore;
                if (bestScores.Count > 0)
                {
                    returnScore = bestScores[0].score;
                }
                return returnScore;
            }
        }

        public ReadOnlyCollection<HighScore> HighScores
        {
            get
            {
                return bestScores.AsReadOnly();
            }
        }

        /// <summary>
        /// The comparison used to sort the high scores.
        /// </summary>
        public Comparison<int> SortScoresBy
        {
            get
            {
                if (sortScoresBy == null)
                {
                    sortScoresBy = (int left, int right) => { return (left - right); };
                }
                return sortScoresBy;
            }
            set
            {
                if ((value != null) && (sortScoresBy != value))
                {
                    sortScoresBy = value;
                }
            }
        }

        public int NumberOfTimesAppOpened
        {
            get
            {
                return numberOfTimesAppOpened;
            }
            private set
            {
                if (numberOfTimesAppOpened != value)
                {
                    numberOfTimesAppOpened = value;
                    Settings.SetInt(NumberOfTimesAppOpenedKey, numberOfTimesAppOpened);
                }
            }
        }

        public TimeSpan TotalPlayTime
        {
            get
            {
                return lastPlayTime.Add(DateTime.UtcNow - lastTimeOpen);
            }
        }

        public bool IsKeyboardAxisSensitivitySplit
        {
            get
            {
                return splitKeyboardAxis;
            }
            set
            {
                if (splitKeyboardAxis != value)
                {
                    splitKeyboardAxis = value;
                    Settings.SetBool(SplitKeyboardAxisKey, splitKeyboardAxis);
                }
            }
        }

        public float KeyboardXAxisSensitivity
        {
            get
            {
                return keyboardXAxisSensitivity;
            }
            set
            {
                if (keyboardXAxisSensitivity != value)
                {
                    keyboardXAxisSensitivity = value;
                    Settings.SetFloat(KeyboardXAxisSensitivityKey, keyboardXAxisSensitivity);
                }
            }
        }

        public float KeyboardYAxisSensitivity
        {
            get
            {
                return keyboardYAxisSensitivity;
            }
            set
            {
                if (keyboardYAxisSensitivity != value)
                {
                    keyboardYAxisSensitivity = value;
                    Settings.SetFloat(KeyboardYAxisSensitivityKey, keyboardYAxisSensitivity);
                }
            }
        }

        public bool IsKeyboardXAxisInverted
        {
            get
            {
                return isKeyboardXAxisInverted;
            }
            set
            {
                if (isKeyboardXAxisInverted != value)
                {
                    isKeyboardXAxisInverted = value;
                    Settings.SetBool(IsKeyboardXAxisInvertedKey, isKeyboardXAxisInverted);
                }
            }
        }

        public bool IsKeyboardYAxisInverted
        {
            get
            {
                return isKeyboardYAxisInverted;
            }
            set
            {
                if (isKeyboardYAxisInverted != value)
                {
                    isKeyboardYAxisInverted = value;
                    Settings.SetBool(IsKeyboardYAxisInvertedKey, isKeyboardYAxisInverted);
                }
            }
        }

        public bool IsMouseAxisSensitivitySplit
        {
            get
            {
                return splitMouseAxis;
            }
            set
            {
                if (splitMouseAxis != value)
                {
                    splitMouseAxis = value;
                    Settings.SetBool(SplitMouseAxisKey, splitMouseAxis);
                }
            }
        }

        public float MouseXAxisSensitivity
        {
            get
            {
                return mouseXAxisSensitivity;
            }
            set
            {
                if (mouseXAxisSensitivity != value)
                {
                    mouseXAxisSensitivity = value;
                    Settings.SetFloat(MouseXAxisSensitivityKey, mouseXAxisSensitivity);
                }
            }
        }

        public float MouseYAxisSensitivity
        {
            get
            {
                return mouseYAxisSensitivity;
            }
            set
            {
                if (mouseYAxisSensitivity != value)
                {
                    mouseYAxisSensitivity = value;
                    Settings.SetFloat(MouseYAxisSensitivityKey, mouseYAxisSensitivity);
                }
            }
        }

        public bool IsMouseXAxisInverted
        {
            get
            {
                return isMouseXAxisInverted;
            }
            set
            {
                if (isMouseXAxisInverted != value)
                {
                    isMouseXAxisInverted = value;
                    Settings.SetBool(IsMouseXAxisInvertedKey, isMouseXAxisInverted);
                }
            }
        }

        public bool IsMouseYAxisInverted
        {
            get
            {
                return isMouseYAxisInverted;
            }
            set
            {
                if (isMouseYAxisInverted != value)
                {
                    isMouseYAxisInverted = value;
                    Settings.SetBool(IsMouseYAxisInvertedKey, isMouseYAxisInverted);
                }
            }
        }

        public float ScrollWheelSensitivity
        {
            get
            {
                return scrollWheelSensitivity;
            }
            set
            {
                if (scrollWheelSensitivity != value)
                {
                    scrollWheelSensitivity = value;
                    Settings.SetFloat(ScrollWheelSensitivityKey, scrollWheelSensitivity);
                }
            }
        }

        public bool IsScrollWheelInverted
        {
            get
            {
                return isScrollWheelInverted;
            }
            set
            {
                if (isScrollWheelInverted != value)
                {
                    isScrollWheelInverted = value;
                    Settings.SetBool(IsScrollWheelInvertedKey, isScrollWheelInverted);
                }
            }
        }

        public bool IsFlashesEnabled
        {
            get
            {
                return isFlashesEnabled;
            }
            set
            {
                if (isFlashesEnabled != value)
                {
                    isFlashesEnabled = value;
                    Settings.SetBool(IsFlashesEnabledKey, isFlashesEnabled);
                }
            }
        }

        public bool IsMotionBlursEnabled
        {
            get
            {
                return isMotionBlursEnabled;
            }
            set
            {
                if (isMotionBlursEnabled != value)
                {
                    isMotionBlursEnabled = value;
                    Settings.SetBool(IsMotionBlursEnabledKey, isMotionBlursEnabled);
                }
            }
        }

        public bool IsBloomEnabled
        {
            get
            {
                return isBloomEnabled;
            }
            set
            {
                if (isBloomEnabled != value)
                {
                    isBloomEnabled = value;
                    Settings.SetBool(IsBloomEnabledKey, isBloomEnabled);
                }
            }
        }

        public bool IsSmoothCameraEnabled
        {
            get
            {
                return isSmoothCameraEnabled;
            }
            set
            {
                if (isSmoothCameraEnabled != value)
                {
                    isSmoothCameraEnabled = value;
                    Settings.SetBool(IsSmoothCameraEnabledKey, isSmoothCameraEnabled);
                }
            }
        }

        public bool IsBobbingCameraEnabled
        {
            get
            {
                return isBobbingCameraEnabled;
            }
            set
            {
                if (isBobbingCameraEnabled != value)
                {
                    isBobbingCameraEnabled = value;
                    Settings.SetBool(IsBobbingCameraEnabledKey, isBobbingCameraEnabled);
                }
            }
        }
        #endregion

        #region Singleton Overrides
        public override void SingletonAwake(Singleton instance)
        {
            // Load settings
            RetrieveFromSettings();
        }

        public override void SceneAwake(Singleton instance)
        {
        }
        #endregion

        #region Unity Events
        void OnApplicationQuit()
        {
            SaveSettings();
        }
        #endregion

        public void RetrieveFromSettings()
        {
            RetrieveFromSettings(true);
        }

        public void SaveSettings()
        {
            SaveSettings(true);
        }

        public void ClearSettings()
        {
            ClearSettings(true);
        }

        /// <summary>
        /// Adds the score to the local high scores list.
        /// </summary>
        /// <returns>The index of the rank the score placed in, starting with 0 as the top.</returns>
        /// <param name="newScore">New score.</param>
        /// <param name="name">Name of person achieving score.</param>
        public int AddScore(int newScore, string name)
        {
            // Check to see if there are any high scores recorded
            int returnRank = 0;
            if (bestScores.Count > 0)
            {
                // Go through each high score, and see if the new score exceeds or equals any
                returnRank = -1;
                for (int i = 0; i < bestScores.Count; ++i)
                {
                    if (SortScoresBy(newScore, bestScores[i].score) >= 0)
                    {
                        // If so, return this rank
                        returnRank = i;
                        break;
                    }
                }
            }

            // Make sure the high score belongs to the list
            if (returnRank >= 0)
            {
                // If it does, check if we need to trim excess
                while (bestScores.Count >= LocalHighScoresMaxListSize)
                {
                    bestScores.RemoveAt(bestScores.Count - 1);
                }

                // Insert the score
                bestScores.Insert(returnRank, new HighScore(newScore, name));

                // Save this information
                Settings.SetString(LocalHighScoresKey, GenerateHighScoresString());
            }
            return returnRank;
        }

        #region Virtual Methods
        protected virtual void RetrieveFromSettings(bool runEvent)
        {
            // Grab the the app version
            int currentVersion = Settings.GetInt(VersionKey, -1);

            // Update the app status
            status = AppStatus.Replaying;
            if (currentVersion < 0)
            {
                status = AppStatus.FirstTimeOpened;
            }
            else if (currentVersion < AppVersion)
            {
                status = AppStatus.RecentlyUpdated;
            }

            // Set the version
            Settings.SetInt(VersionKey, AppVersion);
            Settings.Save();

            // NOTE: Feel free to add more stuff here
            RetrieveVersion0Settings();

            // Run events
            if ((runEvent == true) && (OnRetrieveSettings != null))
            {
                OnRetrieveSettings(this);
            }
        }

        protected virtual void SaveSettings(bool runEvent)
        {
            // NOTE: Feel free to add more stuff here
            SaveVersion0Settings();

            // Save the preferences
            Settings.Save();

            // Run events
            if ((runEvent == true) && (OnSaveSettings != null))
            {
                OnSaveSettings(this);
            }
        }

        protected virtual void ClearSettings(bool runEvent)
        {
            // Grab the the app version
            int currentVersion = Settings.GetInt(VersionKey, -1);

            // Delete all stored preferences
            Settings.DeleteAll();

            // Set the version
            Settings.SetInt(VersionKey, currentVersion);

            // Store settings that are part of options.
            // Since member variables are unchanged up to this point, we can re-use them here.
            // NOTE: Feel free to add more stuff here
            RevertVersion0SettingsClearedSettings();

            // Reset all other member variables
            RetrieveFromSettings(false);

            // Run events
            if ((runEvent == true) && (OnClearSettings != null))
            {
                OnClearSettings(this);
            }
        }
        #endregion

        #region Version 0 Settings Methods
        void RetrieveVersion0Settings()
        {
            // Grab the number of levels unlocked
            numLevelsUnlocked = Settings.GetInt(NumLevelsUnlockedKey, DefaultNumLevelsUnlocked);

            // Grab the music settings
            musicVolume = Settings.GetFloat(MusicVolumeKey, DefaultMusicVolume);
            musicMuted = Settings.GetBool(MusicMutedKey, false);

            // Grab the sound settings
            soundVolume = Settings.GetFloat(SoundVolumeKey, DefaultSoundVolume);
            soundMuted = Settings.GetBool(SoundMutedKey, false);

            // Grab the language
            language = Settings.GetString(LanguageKey, DefaultLanguage);

            // Grab leaderboard user scope
            leaderboardUserScope = Settings.GetEnum(LeaderboardUserScopeKey, DefaultLeaderboardUserScope);

            // Grab number of plays
            numberOfTimesAppOpened = Settings.GetInt(NumberOfTimesAppOpenedKey, 0);

            // Grab the best score
            string tempString = Settings.GetString(LocalHighScoresKey, null);
            bestScores.Clear();
            if (string.IsNullOrEmpty(tempString) == false)
            {
                RetrieveHighScores(tempString);
            }

            // Grab Keyboard Sensitivity information
            splitKeyboardAxis = Settings.GetBool(SplitKeyboardAxisKey, false);

            // Grab how long we've played this game
            lastTimeOpen = DateTime.UtcNow;
            lastPlayTime = Settings.GetTimeSpan(TotalPlayTimeKey, TimeSpan.Zero);

            // Get Keyboard Sensitivity information
            splitKeyboardAxis = Settings.GetBool(SplitKeyboardAxisKey, false);
            keyboardXAxisSensitivity = Settings.GetFloat(KeyboardXAxisSensitivityKey, DefaultSensitivity);
            keyboardYAxisSensitivity = Settings.GetFloat(KeyboardYAxisSensitivityKey, DefaultSensitivity);
            isKeyboardXAxisInverted = Settings.GetBool(IsKeyboardXAxisInvertedKey, false);
            isKeyboardYAxisInverted = Settings.GetBool(IsKeyboardYAxisInvertedKey, false);

            // Get Mouse Sensitivity information
            splitMouseAxis = Settings.GetBool(SplitMouseAxisKey, false);
            mouseXAxisSensitivity = Settings.GetFloat(MouseXAxisSensitivityKey, DefaultSensitivity);
            mouseYAxisSensitivity = Settings.GetFloat(MouseYAxisSensitivityKey, DefaultSensitivity);
            isMouseXAxisInverted = Settings.GetBool(IsMouseXAxisInvertedKey, false);
            isMouseYAxisInverted = Settings.GetBool(IsMouseYAxisInvertedKey, false);

            // Get Mouse Wheel Sensitivity information
            scrollWheelSensitivity = Settings.GetFloat(ScrollWheelSensitivityKey, DefaultSensitivity);
            isScrollWheelInverted = Settings.GetBool(IsScrollWheelInvertedKey, false);

            // Get Special Effects information
            isSmoothCameraEnabled = Settings.GetBool(IsSmoothCameraEnabledKey, false);
            isBobbingCameraEnabled = Settings.GetBool(IsBobbingCameraEnabledKey, false);
            isFlashesEnabled = Settings.GetBool(IsFlashesEnabledKey, true);
            isMotionBlursEnabled = Settings.GetBool(IsMotionBlursEnabledKey, true);
            isBloomEnabled = Settings.GetBool(IsBloomEnabledKey, true);
        }

        void SaveVersion0Settings()
        {
            // Save the number of levels unlocked
            Settings.SetInt(NumLevelsUnlockedKey, NumLevelsUnlocked);

            // Save the music settings
            Settings.SetFloat(MusicVolumeKey, MusicVolume);
            Settings.SetBool(MusicMutedKey, IsMusicMuted);

            // Save the sound settings
            Settings.SetFloat(SoundVolumeKey, SoundVolume);
            Settings.SetBool(SoundMutedKey, IsSoundMuted);

            // Set the language
            Settings.SetString(LanguageKey, Language);

            // Set leaderboard's user scope variable
            Settings.SetEnum(LeaderboardUserScopeKey, LeaderboardUserScope);

            // Set the best score
            Settings.SetString(LocalHighScoresKey, GenerateHighScoresString());

            // Set number of plays variables
            Settings.SetInt(NumberOfTimesAppOpenedKey, numberOfTimesAppOpened);

            // Set the play time
            Settings.SetTimeSpan(TotalPlayTimeKey, TotalPlayTime);

            // Set Keyboard Sensitivity information
            Settings.SetBool(SplitKeyboardAxisKey, splitKeyboardAxis);
            Settings.SetFloat(KeyboardXAxisSensitivityKey, keyboardXAxisSensitivity);
            Settings.SetFloat(KeyboardYAxisSensitivityKey, keyboardYAxisSensitivity);
            Settings.SetBool(IsKeyboardXAxisInvertedKey, isKeyboardXAxisInverted);
            Settings.SetBool(IsKeyboardYAxisInvertedKey, isKeyboardYAxisInverted);

            // Set Mouse Sensitivity information
            Settings.SetBool(SplitMouseAxisKey, splitMouseAxis);
            Settings.SetFloat(MouseXAxisSensitivityKey, mouseXAxisSensitivity);
            Settings.SetFloat(MouseYAxisSensitivityKey, mouseYAxisSensitivity);
            Settings.SetBool(IsMouseXAxisInvertedKey, isMouseXAxisInverted);
            Settings.SetBool(IsMouseYAxisInvertedKey, isMouseYAxisInverted);

            // Set Mouse Wheel Sensitivity information
            Settings.SetFloat(ScrollWheelSensitivityKey, scrollWheelSensitivity);
            Settings.SetBool(IsScrollWheelInvertedKey, isScrollWheelInverted);

            // Set Special Effects information
            Settings.SetBool(IsSmoothCameraEnabledKey, isSmoothCameraEnabled);
            Settings.SetBool(IsBobbingCameraEnabledKey, isBobbingCameraEnabled);
            Settings.SetBool(IsFlashesEnabledKey, isFlashesEnabled);
            Settings.SetBool(IsMotionBlursEnabledKey, isMotionBlursEnabled);
            Settings.SetBool(IsBloomEnabledKey, isBloomEnabled);
        }

        void RevertVersion0SettingsClearedSettings()
        {
            // Save the music settings
            Settings.SetFloat(MusicVolumeKey, musicVolume);
            Settings.SetBool(MusicMutedKey, musicMuted);

            // Save the sound settings
            Settings.SetFloat(SoundVolumeKey, soundVolume);
            Settings.SetBool(SoundMutedKey, soundMuted);

            // Set leaderboard's user scope variable
            Settings.SetInt(LeaderboardUserScopeKey, (int)leaderboardUserScope);

            // Set the language
            Settings.SetString(LanguageKey, language);

            // Set Keyboard Sensitivity information
            Settings.SetBool(SplitKeyboardAxisKey, splitKeyboardAxis);

            // Set Keyboard Sensitivity information
            Settings.SetBool(SplitKeyboardAxisKey, splitKeyboardAxis);
            Settings.SetFloat(KeyboardXAxisSensitivityKey, keyboardXAxisSensitivity);
            Settings.SetFloat(KeyboardYAxisSensitivityKey, keyboardYAxisSensitivity);
            Settings.SetBool(IsKeyboardXAxisInvertedKey, isKeyboardXAxisInverted);
            Settings.SetBool(IsKeyboardYAxisInvertedKey, isKeyboardYAxisInverted);

            // Set Mouse Sensitivity information
            Settings.SetBool(SplitMouseAxisKey, splitMouseAxis);
            Settings.SetFloat(MouseXAxisSensitivityKey, mouseXAxisSensitivity);
            Settings.SetFloat(MouseYAxisSensitivityKey, mouseYAxisSensitivity);
            Settings.SetBool(IsMouseXAxisInvertedKey, isMouseXAxisInverted);
            Settings.SetBool(IsMouseYAxisInvertedKey, isMouseYAxisInverted);

            // Set Mouse Wheel Sensitivity information
            Settings.SetFloat(ScrollWheelSensitivityKey, scrollWheelSensitivity);
            Settings.SetBool(IsScrollWheelInvertedKey, isScrollWheelInverted);

            // Set Special Effects information
            Settings.SetBool(IsSmoothCameraEnabledKey, isSmoothCameraEnabled);
            Settings.SetBool(IsBobbingCameraEnabledKey, isBobbingCameraEnabled);
            Settings.SetBool(IsFlashesEnabledKey, isFlashesEnabled);
            Settings.SetBool(IsMotionBlursEnabledKey, isMotionBlursEnabled);
            Settings.SetBool(IsBloomEnabledKey, isBloomEnabled);
        }

        void RetrieveHighScores(string highScoresString)
        {
            // Split the string
            string[] highScoresArray = highScoresString.Split(ScoreDivider);

            // Clear the list
            bestScores.Clear();

            // Add elements to the list
            for (int i = 0; i < highScoresArray.Length; ++i)
            {
                bestScores.Add(new HighScore(highScoresArray[i]));
            }
        }

        string GenerateHighScoresString()
        {
            fullListBuilder.Length = 0;

            // Make sure there's things in the unlock list
            for (int i = 0; i < bestScores.Count; ++i)
            {
                if (i > 0)
                {
                    fullListBuilder.Append(ScoreDivider);
                }
                fullListBuilder.Append(bestScores[i].ToString());
            }
            return fullListBuilder.ToString();
        }
        #endregion
    }
}
