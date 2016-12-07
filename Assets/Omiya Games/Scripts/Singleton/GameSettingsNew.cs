using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// FIXME: Consider creating a GameSettingsNew code generator
// FIXME: Consider generating different versions of GameSettingsNew
// FIXME: https://github.com/nickgravelyn/UnityToolbag/blob/master/UnityConstants/Editor/UnityConstantsGenerator.cs
namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GameSettingsNew.cs" company="Omiya Games">
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
    [DisallowMultipleComponent]
    public class GameSettingsNew : ISingletonScript
    {
        public enum AppStatus
        {
            FirstTimeOpened,
            RecentlyUpdated,
            Replaying
        }

        public enum GameOverNextPopUp
        {
            Tutorial,
            ReviewDialog,
            None
        }

        public enum UpgradeType
        {
            Theme = 0,
            ExtraLives,
            LuckyCoinUpgrade,
            ExtraShopEntry,
            NumTypes
        }

        public enum ShowGesturesSettings
        {
            Auto = 0,
            Always,
            Never
        }

        public event Action<GameSettingsNew> OnRetrieveSettings;
        public event Action<GameSettingsNew> OnSaveSettings;
        public event Action<GameSettingsNew> OnClearSettings;

        /// <summary>
        /// The app version.  Must be positive.
        /// Increment every time a new build is released.
        /// Useful for backwards compatibility.
        /// </summary>
        public const int AppVersion = 2;
        /// <summary>
        /// The date the Loading Screen's EULA revision has been posted
        /// </summary>
        public static readonly DateTime EulaRevision = new DateTime(2016, 9, 23);

        // --------------- Added during AppVersion == 0 ------------------
        #region Consts for AppVersion == 0
        public const int BeginnerBestScoreThreshold = 15;
        public const int LocalHighScoresMaxListSize = 10;
        public const string InvalidTimeString = "Invalid";
        public static readonly TimeSpan StartingPleaseRateMeNextTimeGap = new TimeSpan(0, 20, 0);
        public static readonly TimeSpan NextPleaseRateMeNextTimeGap = new TimeSpan(0, 30, 0);

        public const float DefaultMusicVolume = 1;
        public const float DefaultSoundVolume = 1;
        public const string DefaultLanguage = "";
        public const int MaximumCurrency = 9999;
        public const int DefaultNumberOfBoughtUnlockables = 0;

        public const string VersionKey = "AppVersion";
        public const string MusicVolumeKey = "Music Volume";
        public const string MusicMutedKey = "Music Muted";
        public const string SoundVolumeKey = "Sound Volume";
        public const string SoundMutedKey = "Sound Muted";
        public const string LocalHighScoresKey = "Local High Scores";
        public const string LanguageKey = "Language";
        public const string CurrencyKey = "Currency";
        public const string UnlockListKey = "Unlock List";
        public const string ShowMiniGameDebugButtonKey = "Show MiniGame Debug Button";
        public const string ShowFpsCounterKey = "Show FPS Counter";
        public const string NumberOfTimesAppOpenedKey = "Number of Times App Open";
        public const string NumberOfTimesPlayedKey = "Number of Times Played";
        public const string LeaderboardUserScopeKey = "Leaderboard User Scope";
        public const string TotalPlayTimeKey = "Total Play Time";
        public const string PleaseRateMeNextTimeKey = "Show Please Rate Me Next Time";
        public const string GameOverPopUpKey = "Game Over Pop-Up";
        public const string NumberOfTimesLuckyCoinAppearedKey = "Lucky Coin Appearance";
        public const string NumberOfTimesLuckyCoinWasTappedKey = "Lucky Coin Tapped";
        public const string TimeEulaWasAgreedKey = "Time Eula Was Agreed";
        #endregion

        // --------------- Added during AppVersion == 1 ------------------
        #region Consts for AppVersion == 1
        public const int TopTargetFrameRate = -1;
        public const int TopMobileTargetFrameRate = 60;
        public const int LowPowerTargetFrameRate = 30;
        public const float DefaultTimeScale = 1f;
        public const float DefaultTimerExtends = 1.5f;

        public const string IsSlowMoModeOnKey = "Is Slow-Mo Mode On";
        public const string TimeScaleKey = "Time Scale";
        public const string TimerExtendSecondsKey = "Timer Extend";
        public const string IsPowerSavingsOnKey = "Is Power Savings On";
#if !DEMO
        public const string IsExtraLifeFreeKey = "Is Extra Life Free";
#endif
        #endregion

        // --------------- Added during AppVersion == 2 ------------------
        #region Consts for AppVersion == 2
        public const ShowGesturesSettings DefaultShowGestureSettings = ShowGesturesSettings.Auto;
        public const string ShowGestureSettingsKey = "Show Gestures Settings";
        #endregion

        static readonly StringBuilder builder = new StringBuilder();

        // --------------- Added during AppVersion == 0 ------------------
        #region Member Variables for AppVersion == 0
        readonly List<HighScore> bestScores = new List<HighScore>(LocalHighScoresMaxListSize);
        int currency = 0, numberOfTimesAppOpened = 0, numberOfTimesPlayed = 0, numberOfTimesLuckyCoinAppeared = 0, numberOfTimesLuckyCoinWasTapped = 0;
        float musicVolume = DefaultMusicVolume, soundVolume = DefaultSoundVolume;
        bool musicMuted = false, soundMuted = false;
        AppStatus status = AppStatus.Replaying;
        GameOverNextPopUp gameOverPopUp = GameOverNextPopUp.Tutorial;
        string language = DefaultLanguage;
        DateTime lastTimeOpen;
        TimeSpan lastPlayTime = TimeSpan.Zero;
        DateTime? lastUpdatedUnlockUtc = null;
        TimeSpan nextTimeToDisplayPleaseRateMe = TimeSpan.Zero;
        string[] unlockList = null;
        UserScope leaderboardUserScope;
#if CHEATS
        bool isMiniGameDebugButtonVisible = false, isFpsCounterVisible = false;
#endif
        DateTime timeEulaWasAgreed;
        #endregion

        // --------------- Added during AppVersion == 1 ------------------
        #region Member Variables for AppVersion == 1
        bool isSlowMoModeOn = false;
        float timeScale = 1f;
        float timerExtendSeconds = 0f;
        bool isPowerSavingsOn = false;
#if !DEMO
        bool isExtraLifeFree = false;
#endif
        #endregion

        // --------------- Added during AppVersion == 2 ------------------
        #region Member Variables for AppVersion == 2
        ShowGesturesSettings showGestureSettings = DefaultShowGestureSettings;
        #endregion

        #region Properties
        public static UserScope DefaultLeaderboardUserScope
        {
            get
            {
                // Switch to friends-only if on mobile
                UserScope returnUserScope = UserScope.Global;
                if(Application.isMobilePlatform == true)
                {
                    returnUserScope = UserScope.FriendsOnly;
                }
                return returnUserScope;
            }
        }

        public AppStatus Status
        {
            get
            {
                return status;
            }
        }

        // --------------- Added during AppVersion == 0 ------------------
        #region Properties for AppVersion == 0
        public int NumLevelsUnlocked
        {
            get
            {
                return 1;
            }
            set
            {
                // Do nothing; there are no levels in this game.
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
                PlayerPrefs.SetInt(MusicMutedKey, BoolToInt(musicMuted));
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
                PlayerPrefs.SetInt(SoundMutedKey, BoolToInt(soundMuted));
            }
        }

        public int BestScore
        {
            get
            {
                int returnScore = 0;
                if(bestScores.Count > 0)
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

        public int CurrencyCents
        {
            get
            {
                return currency;
            }
            set
            {
                if(currency != value)
                {
                    currency = Mathf.Clamp(value, 0, MaximumCurrency);
                    PlayerPrefs.SetInt(CurrencyKey, currency);
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
                if(numberOfTimesAppOpened != value)
                {
                    numberOfTimesAppOpened = value;
                    PlayerPrefs.SetInt(NumberOfTimesAppOpenedKey, numberOfTimesAppOpened);
                }
            }
        }

        public int NumberOfTimesPlayed
        {
            get
            {
                return numberOfTimesPlayed;
            }
            set
            {
                if(numberOfTimesPlayed != value)
                {
                    numberOfTimesPlayed = value;
                    PlayerPrefs.SetInt(NumberOfTimesPlayedKey, numberOfTimesPlayed);
                }
            }
        }

        public string CurrencyDisplayString
        {
            get
            {
                return CentsToString(CurrencyCents);
            }
        }

        public DateTime? LastUpdatedUnlockUtc
        {
            get
            {
                return lastUpdatedUnlockUtc;
            }
        }

        public string[] UnlockListAppKeys
        {
            get
            {
                return unlockList;
            }
            set
            {
                if (GameOverPopUp != GameOverNextPopUp.Tutorial)
                {
                    unlockList = value;
                    lastUpdatedUnlockUtc = DateTime.UtcNow;

                    // Set last unlock update
                    PlayerPrefs.SetString(UnlockListKey, GenerateUnlockString());
                }
            }
        }

        public int NumberOfTimesLuckyCoinAppeared
        {
            get
            {
                return numberOfTimesLuckyCoinAppeared;
            }
            set
            {
                numberOfTimesLuckyCoinAppeared = value;
                if(numberOfTimesLuckyCoinAppeared < 0)
                {
                    numberOfTimesLuckyCoinAppeared = 0;
                }

                // Set last unlock update
                PlayerPrefs.SetInt(NumberOfTimesLuckyCoinAppearedKey, numberOfTimesLuckyCoinAppeared);
            }
        }

        public int NumberOfTimesLuckyCoinWasTapped
        {
            get
            {
                return numberOfTimesLuckyCoinWasTapped;
            }
            set
            {
                numberOfTimesLuckyCoinWasTapped = value;
                if (numberOfTimesLuckyCoinWasTapped < 0)
                {
                    numberOfTimesLuckyCoinWasTapped = 0;
                }

                // Set last unlock update
                PlayerPrefs.SetInt(NumberOfTimesLuckyCoinWasTappedKey, numberOfTimesLuckyCoinWasTapped);
            }
        }

#if CHEATS
        public bool IsMiniGameDebugButtonVisible
        {
            get
            {
                return isMiniGameDebugButtonVisible;
            }
            set
            {
                if(isMiniGameDebugButtonVisible != value)
                {
                    isMiniGameDebugButtonVisible = value;
                    PlayerPrefs.SetInt(ShowMiniGameDebugButtonKey, BoolToInt(isMiniGameDebugButtonVisible));
                }
            }
        }

        public bool IsFpsCounterVisible
        {
            get
            {
                return isFpsCounterVisible;
            }
            set
            {
                if (isFpsCounterVisible != value)
                {
                    isFpsCounterVisible = value;
                    PlayerPrefs.SetInt(ShowFpsCounterKey, BoolToInt(isFpsCounterVisible));
                }
            }
        }
#endif

        public UserScope LeaderboardUserScope
        {
            get
            {
                return leaderboardUserScope;
            }
            set
            {
                if(leaderboardUserScope != value)
                {
                    leaderboardUserScope = value;
                    PlayerPrefs.SetInt(LeaderboardUserScopeKey, (int)leaderboardUserScope);
                }
            }
        }

        public GameOverNextPopUp GameOverPopUp
        {
            get
            {
                return gameOverPopUp;
            }
            set
            {
                if (gameOverPopUp != value)
                {
                    gameOverPopUp = value;
                    PlayerPrefs.SetInt(GameOverPopUpKey, (int)gameOverPopUp);
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

        public TimeSpan NextTimeToDisplayPleaseRateMe
        {
            get
            {
                return nextTimeToDisplayPleaseRateMe;
            }
            set
            {
                nextTimeToDisplayPleaseRateMe = value;
                PlayerPrefs.SetString(PleaseRateMeNextTimeKey, nextTimeToDisplayPleaseRateMe.Ticks.ToString());
            }
        }

        public DateTime TimeEulaWasAgreed
        {
            get
            {
                return timeEulaWasAgreed;
            }
            set
            {
                timeEulaWasAgreed = value;
                SetDateTime(TimeEulaWasAgreedKey, timeEulaWasAgreed);
            }
        }
        #endregion

        // --------------- Added during AppVersion == 1 ------------------
        #region Properties for AppVersion == 1
        // FIXME: use this property!
        public bool IsSlowMoModeOn
        {
            get
            {
                return isSlowMoModeOn;
            }
            set
            {
                if (isSlowMoModeOn != value)
                {
                    isSlowMoModeOn = value;
                    PlayerPrefs.SetInt(IsSlowMoModeOnKey, BoolToInt(isSlowMoModeOn));
                }
            }
        }

        // FIXME: use this property!
        public float TimeScale
        {
            get
            {
                if(IsSlowMoModeOn == false)
                {
                    return 1f;
                }
                else
                {
                    return timeScale;
                }
            }
            set
            {
                if (Mathf.Approximately(timeScale, value) == false)
                {
                    timeScale = value;
                    PlayerPrefs.SetFloat(TimeScaleKey, timeScale);
                }
            }
        }

        // FIXME: use this property!
        public float TimerExtendSeconds
        {
            get
            {
                if (IsSlowMoModeOn == false)
                {
                    return 0f;
                }
                else
                {
                    return timerExtendSeconds;
                }
            }
            set
            {
                if(Mathf.Approximately(timerExtendSeconds, value) == false)
                {
                    timerExtendSeconds = value;
                    PlayerPrefs.SetFloat(TimerExtendSecondsKey, timerExtendSeconds);
                }
            }
        }

        public bool IsPowerSavingsOn
        {
            get
            {
                return isPowerSavingsOn;
            }
            set
            {
                if (isPowerSavingsOn != value)
                {
                    // Set value
                    isPowerSavingsOn = value;

                    // Update target frame rate
                    RecalculateTargetFrameRate();

                    // Set the stored value
                    PlayerPrefs.SetInt(IsPowerSavingsOnKey, BoolToInt(isPowerSavingsOn));
                }
            }
        }

#if !DEMO
        public bool IsExtraLifeFree
        {
            get
            {
                return isExtraLifeFree;
            }
            set
            {
                isExtraLifeFree = value;
                PlayerPrefs.SetInt(IsExtraLifeFreeKey, BoolToInt(isExtraLifeFree));
            }
        }
#endif
        #endregion

        // --------------- Added during AppVersion == 2 ------------------
        #region Properties for AppVersion == 2
        public ShowGesturesSettings ShowGestureSettings
        {
            get
            {
                return showGestureSettings;
            }
            set
            {
                if (showGestureSettings != value)
                {
                    showGestureSettings = value;
                    PlayerPrefs.SetInt(ShowGestureSettingsKey, (int)showGestureSettings);
                }
            }
        }
        #endregion
        #endregion

        #region Other Methods Accessing Single Setting
        public void ResetUnlockList()
        {
            // Set last unlock update
            PlayerPrefs.DeleteKey(UnlockListKey);

            // Reset variables
            lastUpdatedUnlockUtc = null;
            unlockList = null;
        }

        public int AddScore(int newScore)
        {
            // By default, return -1
            int returnRank = -1;

            // Make sure the new score to be recorded is positive
            if (newScore > 0)
            {
                // Check to see if there are any high scores recorded
                if (bestScores.Count > 0)
                {
                    // Go through each high score, and see if the new score exceeds or equals any
                    for (int i = 0; i < bestScores.Count; ++i)
                    {
                        if (newScore >= bestScores[i].score)
                        {
                            // If so, return this rank
                            returnRank = i;
                            break;
                        }
                    }
                }
                else
                {
                    // If not, add the score to the top of the list
                    returnRank = 0;
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
                    bestScores.Insert(returnRank, new HighScore(newScore));

                    // Save this information
                    PlayerPrefs.SetString(LocalHighScoresKey, GenerateHighScoresString());
                }
            }
            return returnRank;
        }
        #endregion

        #region Public Static Methods
        public static string CentsToString(int cents)
        {
            builder.Length = 0;
            int tempCurrency = (cents / 100);
            builder.Append(tempCurrency.ToString("N0"));
            builder.Append('.');
            tempCurrency = (cents % 100);
            builder.Append(tempCurrency.ToString("00"));
            return builder.ToString();
        }

        public static bool IntToBool(int number)
        {
            return (number != 0);
        }

        public static int BoolToInt(bool boolean)
        {
            return (boolean ? 1 : 0);
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            return IntToBool(PlayerPrefs.GetInt(key, BoolToInt(defaultValue)));
        }

        public static void SetBool(string key, bool setValue)
        {
            PlayerPrefs.SetInt(key, BoolToInt(setValue));
        }

        public static long GetLong(string key, long defaultValue)
        {
            long returnLong = defaultValue;
            string record = PlayerPrefs.GetString(key, defaultValue.ToString());
            if (long.TryParse(record, out returnLong) == false)
            {
                returnLong = defaultValue;
            }
            return returnLong;
        }

        public static void SetLong(string key, long setValue)
        {
            PlayerPrefs.SetString(key, setValue.ToString());
        }

        public static DateTime GetDateTime(string key, DateTime defaultValue)
        {
            return new DateTime(GetLong(key, defaultValue.Ticks));
        }

        public static void SetDateTime(string key, DateTime setValue)
        {
            SetLong(key, setValue.Ticks);
        }
        #endregion

        #region Public Static Methods
        public static bool HasKey(string prepend, string append)
        {
            return PlayerPrefs.HasKey(CombineKey(prepend, append));
        }

        public static bool GetBool(string prepend, string append, bool defaultValue)
        {
            return GetBool(CombineKey(prepend, append), defaultValue);
        }

        public static void SetBool(string prepend, string append, bool setValue)
        {
            SetBool(CombineKey(prepend, append), setValue);
        }

        public static int GetInt(string prepend, string append, int defaultValue)
        {
            return PlayerPrefs.GetInt(CombineKey(prepend, append), defaultValue);
        }

        public static void SetInt(string prepend, string append, int setValue)
        {
            PlayerPrefs.SetInt(CombineKey(prepend, append), setValue);
        }

        public static long GetLong(string prepend, string append, long defaultValue)
        {
            return GetLong(CombineKey(prepend, append), defaultValue);
        }

        public static void SetLong(string prepend, string append, long setValue)
        {
            SetLong(CombineKey(prepend, append), setValue);
        }

        public static string GetString(string prepend, string append, string defaultValue)
        {
            return PlayerPrefs.GetString(CombineKey(prepend, append), defaultValue);
        }

        public static void SetString(string prepend, string append, string setValue)
        {
            PlayerPrefs.SetString(CombineKey(prepend, append), setValue);
        }

        public static void DeleteKey(string prepend, string append)
        {
            PlayerPrefs.DeleteKey(CombineKey(prepend, append));
        }
        #endregion

        #region Singleton Overrides
        public override void SingletonAwake(Singleton instance)
        {
            // Load settings
            lastTimeOpen = DateTime.UtcNow;
            RetrieveFromSettings();
        }

        public override void SceneAwake(Singleton instance)
        {
        }
        #endregion

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
            if (currentVersion < 0)
            {
                status = AppStatus.FirstTimeOpened;
            }
            else if (currentVersion < AppVersion)
            {
                status = AppStatus.RecentlyUpdated;
            }

            // Set the version
            PlayerPrefs.SetInt(VersionKey, AppVersion);

            // --------------- Added during AppVersion == 0 ------------------
            RetrieveVersion0Settings();

            // --------------- Added during AppVersion == 1 ------------------
            RetrieveVersion1Settings(currentVersion);

            // --------------- Added during AppVersion == 2 ------------------
            RetrieveVersion2Settings(currentVersion);

            // Run any events
            if (OnRetrieveSettings != null)
            {
                OnRetrieveSettings(this);
            }

            // Increment the number of times played
            NumberOfTimesAppOpened += 1;

            // Save
            PlayerPrefs.Save();
        }

        public void SaveSettings()
        {
            // --------------- Added during AppVersion == 0 ------------------
            SaveVersion0Settings();

            // --------------- Added during AppVersion == 1 ------------------
            SaveVersion1Settings();

            // --------------- Added during AppVersion == 2 ------------------
            SaveVersion2Settings();

            // Run any events
            if (OnSaveSettings != null)
            {
                OnSaveSettings(this);
            }

            // Save
            PlayerPrefs.Save();
        }

        public void ClearSettings()
        {
            // Delete all settings (all the temporary variables should be unchanged)
            PlayerPrefs.DeleteAll();

            // Retain certain settings
            // --------------- Added during AppVersion == 0 ------------------
            RetainAfterClearingSettingsVersion0();

            // --------------- Added during AppVersion == 1 ------------------
            RetainAfterClearingSettingsVersion1();

            // --------------- Added during AppVersion == 2 ------------------
            RetainAfterClearingSettingsVersion2();

            // Run any events
            if (OnClearSettings != null)
            {
                OnClearSettings(this);
            }

            // Reset every other value
            RetrieveFromSettings();
        }

        public void RecalculateTargetFrameRate()
        {
            if (IsPowerSavingsOn == false)
            {
                if(Application.isMobilePlatform == true)
                {
                    Application.targetFrameRate = TopMobileTargetFrameRate;
                }
                else
                {
                    Application.targetFrameRate = TopTargetFrameRate;
                }
            }
            else
            {
                Application.targetFrameRate = LowPowerTargetFrameRate;
            }
        }

        #region Helper Methods
        static string CombineKey(string prepend, string append)
        {
            builder.Length = 0;
            builder.Append(prepend);
            builder.Append('.');
            builder.Append(append);
            return builder.ToString();
        }

        // --------------- Added during AppVersion == 0 ------------------
        #region Save & Retrieve for AppVersion == 0
        void RetrieveVersion0Settings()
        {
            // Grab the music settings
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
            musicMuted = IntToBool(PlayerPrefs.GetInt(MusicMutedKey, 0));

            // Grab the sound settings
            soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, DefaultSoundVolume);
            soundMuted = IntToBool(PlayerPrefs.GetInt(SoundMutedKey, 0));

            // Grab the best score
            string tempString = PlayerPrefs.GetString(LocalHighScoresKey, null);
            if (string.IsNullOrEmpty(tempString) == false)
            {
                RetrieveHighScores(tempString);
            }
            else
            {
                bestScores.Clear();
            }

            // Grab the language
            language = PlayerPrefs.GetString(LanguageKey, DefaultLanguage);

            // Grab the currency
            currency = Mathf.Clamp(PlayerPrefs.GetInt(CurrencyKey, 0), 0, MaximumCurrency);

#if CHEATS
            // Get cheat variables
            isMiniGameDebugButtonVisible = IntToBool(PlayerPrefs.GetInt(ShowMiniGameDebugButtonKey, 0));
            isFpsCounterVisible = IntToBool(PlayerPrefs.GetInt(ShowFpsCounterKey, 0));
#endif

            // Set unlock list
            tempString = PlayerPrefs.GetString(UnlockListKey, null);
            if (string.IsNullOrEmpty(tempString) == false)
            {
                RetrieveUnlockList(tempString);
            }

            // Grab number of plays
            numberOfTimesAppOpened = PlayerPrefs.GetInt(NumberOfTimesAppOpenedKey, 0);
            numberOfTimesPlayed = PlayerPrefs.GetInt(NumberOfTimesPlayedKey, 0);

            // Grab luck coin count
            numberOfTimesLuckyCoinAppeared = PlayerPrefs.GetInt(NumberOfTimesLuckyCoinAppearedKey, 0);
            numberOfTimesLuckyCoinWasTapped = PlayerPrefs.GetInt(NumberOfTimesLuckyCoinWasTappedKey, 0);

            // Grab leaderboard user scope
            leaderboardUserScope = (UserScope)PlayerPrefs.GetInt(LeaderboardUserScopeKey, (int)DefaultLeaderboardUserScope);

            // Grab how long we've played this game
            long numberOfTicks;
            lastPlayTime = TimeSpan.Zero;
            tempString = PlayerPrefs.GetString(TotalPlayTimeKey, null);
            if ((string.IsNullOrEmpty(tempString) == false) && (long.TryParse(tempString, out numberOfTicks) == true))
            {
                lastPlayTime = new TimeSpan(numberOfTicks);
            }

            // Grab when we should display Please Rate This Game dialog
            tempString = PlayerPrefs.GetString(PleaseRateMeNextTimeKey, null);
            nextTimeToDisplayPleaseRateMe = StartingPleaseRateMeNextTimeGap;
            if ((string.IsNullOrEmpty(tempString) == false) && (long.TryParse(tempString, out numberOfTicks) == true))
            {
                nextTimeToDisplayPleaseRateMe = new TimeSpan(numberOfTicks);
            }

            // Get Game Over pop-up status
            gameOverPopUp = (GameOverNextPopUp)PlayerPrefs.GetInt(GameOverPopUpKey, (int)GameOverNextPopUp.Tutorial);

            // Get time user agreed to the eula type
            timeEulaWasAgreed = GetDateTime(TimeEulaWasAgreedKey, DateTime.MinValue);
        }

        void SaveVersion0Settings()
        {
            // Save the music settings
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            PlayerPrefs.SetInt(MusicMutedKey, BoolToInt(musicMuted));

            // Save the sound settings
            PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
            PlayerPrefs.SetInt(SoundMutedKey, BoolToInt(soundMuted));

            // Set the best score
            PlayerPrefs.SetString(LocalHighScoresKey, GenerateHighScoresString());

            // Set the language
            PlayerPrefs.SetString(LanguageKey, language);

            // Set the currency
            PlayerPrefs.SetInt(CurrencyKey, currency);

            // Set last unlock update
            PlayerPrefs.SetString(UnlockListKey, GenerateUnlockString());

#if CHEATS
            // Set cheat variables
            PlayerPrefs.SetInt(ShowMiniGameDebugButtonKey, BoolToInt(isMiniGameDebugButtonVisible));
            PlayerPrefs.SetInt(ShowFpsCounterKey, BoolToInt(isFpsCounterVisible));
#endif

            // Set number of plays variables
            PlayerPrefs.SetInt(NumberOfTimesAppOpenedKey, numberOfTimesAppOpened);
            PlayerPrefs.SetInt(NumberOfTimesPlayedKey, numberOfTimesPlayed);

            // Grab luck coin count
            PlayerPrefs.SetInt(NumberOfTimesLuckyCoinAppearedKey, numberOfTimesLuckyCoinAppeared);
            PlayerPrefs.SetInt(NumberOfTimesLuckyCoinWasTappedKey, numberOfTimesLuckyCoinWasTapped);

            // Set leaderboard's user scope variable
            PlayerPrefs.SetInt(LeaderboardUserScopeKey, (int)leaderboardUserScope);

            // Set the play time
            lastPlayTime = TotalPlayTime;
            PlayerPrefs.SetString(TotalPlayTimeKey, lastPlayTime.Ticks.ToString());
            lastTimeOpen = DateTime.UtcNow;

            // Grab when we should display Please Rate This Game dialog
            PlayerPrefs.SetString(PleaseRateMeNextTimeKey, nextTimeToDisplayPleaseRateMe.Ticks.ToString());

            // Set Game Over pop-up status
            PlayerPrefs.SetInt(GameOverPopUpKey, (int)gameOverPopUp);

            // Set time user agreed to the eula type
            SetDateTime(TimeEulaWasAgreedKey, timeEulaWasAgreed);
        }

        void RetainAfterClearingSettingsVersion0()
        {
            // Save the temporarily-held music settings
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            PlayerPrefs.SetInt(MusicMutedKey, (musicMuted ? 1 : 0));

            // Save the temporarily-held sound settings
            PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
            PlayerPrefs.SetInt(SoundMutedKey, (soundMuted ? 1 : 0));

            // Set the temporarily-held language
            PlayerPrefs.SetString(LanguageKey, language);

            // Set the temporarily-held leaderboard's user scope
            PlayerPrefs.SetInt(LeaderboardUserScopeKey, (int)leaderboardUserScope);

            // Set the temporarily-held Game Over pop-up status
            //PlayerPrefs.SetInt(GameOverPopUpKey, (int)gameOverPopUp);

            // Set the temporarily-held time user agreed to the eula type
            SetDateTime(TimeEulaWasAgreedKey, timeEulaWasAgreed);

            // Set the play time
            lastPlayTime = TotalPlayTime;
            PlayerPrefs.SetString(TotalPlayTimeKey, lastPlayTime.Ticks.ToString());
            lastTimeOpen = DateTime.UtcNow;

            // Manually clearing out the app keys
            ResetUnlockList();
        }
        #endregion

        // --------------- Added during AppVersion == 1 ------------------
        #region Save & Retrieve for AppVersion == 1
        void RetrieveVersion1Settings(int currentVersion)
        {
            // Get slow-mo mode settings
            isSlowMoModeOn = IntToBool(PlayerPrefs.GetInt(IsSlowMoModeOnKey, BoolToInt(false)));
            timeScale = PlayerPrefs.GetFloat(TimeScaleKey, DefaultTimeScale);
            timerExtendSeconds = PlayerPrefs.GetFloat(TimerExtendSecondsKey, DefaultTimerExtends);

            // Set power savings
            isPowerSavingsOn = IntToBool(PlayerPrefs.GetInt(IsPowerSavingsOnKey, BoolToInt(false)));

#if !DEMO
            // Check the version number
            if (currentVersion == 0)
            {
                // There isn't a good way to figure out if the player unlocked Extra Life #1 as the first upgrade
                // So for now, we'll assume they didn't unless they're still in beginner mode
                isExtraLifeFree = IntToBool(PlayerPrefs.GetInt(IsExtraLifeFreeKey, BoolToInt(IsBeginnerModeOn)));
            }
            else
            {
                // Set the tutorial flag to false
                isExtraLifeFree = IntToBool(PlayerPrefs.GetInt(IsExtraLifeFreeKey, BoolToInt(false)));
            }
#endif
        }

        void SaveVersion1Settings()
        {
            // Set slow-mo mode settings
            PlayerPrefs.SetInt(IsSlowMoModeOnKey, BoolToInt(isSlowMoModeOn));
            PlayerPrefs.SetFloat(TimeScaleKey, timeScale);
            PlayerPrefs.SetFloat(TimerExtendSecondsKey, timerExtendSeconds);

            // Set power savings
            PlayerPrefs.SetInt(IsPowerSavingsOnKey, BoolToInt(isPowerSavingsOn));

#if !DEMO
            // Set the tutorial flag
            PlayerPrefs.SetInt(IsExtraLifeFreeKey, BoolToInt(isExtraLifeFree));
#endif
        }

        void RetainAfterClearingSettingsVersion1()
        {
            // Set slow-mo mode settings
            PlayerPrefs.SetInt(IsSlowMoModeOnKey, BoolToInt(isSlowMoModeOn));
            PlayerPrefs.SetFloat(TimeScaleKey, timeScale);
            PlayerPrefs.SetFloat(TimerExtendSecondsKey, timerExtendSeconds);

            // Set power savings
            PlayerPrefs.SetInt(IsPowerSavingsOnKey, BoolToInt(isPowerSavingsOn));
        }
        #endregion

        // --------------- Added during AppVersion == 2 ------------------
        #region Save & Retrieve for AppVersion == 2
        void RetrieveVersion2Settings(int currentVersion)
        {
            // Get slow-mo mode settings
            showGestureSettings = (ShowGesturesSettings)PlayerPrefs.GetInt(ShowGestureSettingsKey, (int)DefaultShowGestureSettings);
        }

        void SaveVersion2Settings()
        {
            // Set slow-mo mode settings
            PlayerPrefs.SetInt(ShowGestureSettingsKey, (int)showGestureSettings);
        }

        void RetainAfterClearingSettingsVersion2()
        {
            PlayerPrefs.SetInt(ShowGestureSettingsKey, (int)showGestureSettings);
        }
        #endregion

        void RetrieveUnlockList(string unlockListString)
        {
            // Split the string
            string[] unlockListArray = unlockListString.Split('\n');

            // Parse the time
            long ticks = 0;
            if (long.TryParse(unlockListArray[0], out ticks) == true)
            {
                lastUpdatedUnlockUtc = new DateTime(ticks);
            }

            // Parse rest of the list
            if ((unlockList == null) || (unlockList.Length != (unlockListArray.Length - 1)))
            {
                unlockList = new string[(unlockListArray.Length - 1)];
            }
            for(int i = 0; i < unlockList.Length; ++i)
            {
                unlockList[i] = unlockListArray[i + 1];
            }
        }

        string GenerateUnlockString()
        {
            builder.Length = 0;
            // Make sure there's things in the unlock list
            if((lastUpdatedUnlockUtc.HasValue == true) && (unlockList != null) && (unlockList.Length > 0))
            {
                builder.Append(lastUpdatedUnlockUtc.Value.Ticks);
                for (int i = 0; i < unlockList.Length; ++i)
                {
                    builder.Append('\n');
                    builder.Append(unlockList[i]);
                }
            }
            return builder.ToString();
        }

        void RetrieveHighScores(string highScoresString)
        {
            // Split the string
            string[] highScoresArray = highScoresString.Split('\n');

            // Clear the list
            bestScores.Clear();

            // Add elements to the list
            for (int i = 0; i < highScoresArray.Length; ++i)
            {
                bestScores.Add(new HighScore(highScoresArray[i], AppVersion));
            }
        }

        string GenerateHighScoresString()
        {
            builder.Length = 0;
            // Make sure there's things in the unlock list
            for (int i = 0; i < bestScores.Count; ++i)
            {
                if (i > 0)
                {
                    builder.Append('\n');
                }
                builder.Append(bestScores[i].ToString());
            }
            return builder.ToString();
        }
        #endregion
    }
}
