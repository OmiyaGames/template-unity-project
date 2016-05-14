using UnityEngine;
using UnityEngine.SocialPlatforms;
using System;
using System.Text;
using System.Collections.Generic;

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
    /// <date>8/18/2015</date>
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
        /// Stores high score information
        /// </summary>
        /// <seealso cref="GameSettings"/>
        public struct HighScore
        {
            const char Divider = ',';

            public readonly int score;
            public readonly int appVersion;
            public readonly string name;
            public readonly DateTime dateAchievedUtc;

            internal HighScore(int score, string name)
            {
                // Setup all the member variables
                this.score = score;
                this.appVersion = AppVersion;
                this.dateAchievedUtc = DateTime.UtcNow;

                // Record name (don't allow null)
                if(string.IsNullOrEmpty(name) == false)
                {
                    this.name = name;
                }
                else
                {
                    this.name = string.Empty;
                }
            }

            internal HighScore(string pastRecord)
            {
                if(string.IsNullOrEmpty(pastRecord) == true)
                {
                    throw new ArgumentNullException("pastRecord must be provided");
                }

                // Split the information
                string[] info = pastRecord.Split(Divider);
                if ((info == null) || (info.Length <= 0))
                {
                    throw new ArgumentException("Could not parse pastRecord: " + pastRecord);
                }

                // Grab app version
                byte index = 0;
                if(int.TryParse(info[index], out appVersion) == false)
                {
                    throw new ArgumentException("Could not parse the version number from pastRecord: " + pastRecord);
                }

                // TODO: decrypt the rest of the information based on app version
                // Grab score
                ++index;
                if(int.TryParse(info[index], out score) == false)
                {
                    throw new ArgumentException("Could not parse the score from pastRecord: " + pastRecord);
                }

                // Grab name
                ++index;
                name = info[index];

                // Grab time
                long ticks;
                ++index;
                if (long.TryParse(info[index], out ticks) == true)
                {
                    dateAchievedUtc = new DateTime(ticks);
                }
                else
                {
                    throw new ArgumentException("Could not parse the date from pastRecord: " + pastRecord);
                }
            }

            public string GetRecord(StringBuilder builder)
            {
                // Create a record out of this information
                builder.Length = 0;

                // TODO: check the app version before recording everything else
                // Add app version
                builder.Append(appVersion);
                builder.Append(Divider);

                // Add score
                builder.Append(score);
                builder.Append(Divider);

                // Add name
                builder.Append(name);
                builder.Append(Divider);

                // Add date of record in UTC
                builder.Append(dateAchievedUtc.Ticks);

                // Store this achievement as a string record
                return builder.ToString();
            }
        }

        /// <summary>
        /// The app version.  Must be positive.
        /// Increment every time a new build is released.
        /// Useful for backwards compatibility.
        /// </summary>
        public const int AppVersion = 0;

        public const int DefaultNumLevelsUnlocked = 1;
        public const int LocalHighScoresMaxListSize = 10;
        const char ScoreDivider = '\n';

        public const float DefaultMusicVolume = 1;
        public const float DefaultSoundVolume = 1;
        public const string DefaultLanguage = "";
        public const int DefaultBestScore = 0;

        public const string VersionKey = "AppVersion";
        public const string NumLevelsUnlockedKey = "Number of Unlocked Levels";
        public const string MusicVolumeKey = "Music Volume";
        public const string MusicMutedKey = "Music Muted";
        public const string SoundVolumeKey = "Sound Volume";
        public const string SoundMutedKey = "Sound Muted";
        public const string LanguageKey = "Language";
        public const string LocalHighScoresKey = "Local High Scores";
        public const string LeaderboardUserScopeKey = "Leaderboard User Scope";

        readonly StringBuilder listEntryBuilder = new StringBuilder(),
            fullListBuilder = new StringBuilder();
        readonly List<HighScore> bestScores = new List<HighScore>(LocalHighScoresMaxListSize);
        int numLevelsUnlocked = 1;
        float musicVolume = 0, soundVolume = 0;
        bool musicMuted = false, soundMuted = false;
        AppStatus status = AppStatus.Replaying;
        string language = DefaultLanguage;
        UserScope leaderboardUserScope;
        Comparison<int> sortScoresBy = null;

        #region Properties
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

        public int NumLevelsUnlocked
        {
            get
            {
                return numLevelsUnlocked;
            }
            set
            {
                numLevelsUnlocked = value;
                PlayerPrefs.SetInt(NumLevelsUnlockedKey, numLevelsUnlocked);
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
                SetBool(MusicMutedKey, musicMuted);
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
                SetBool(SoundMutedKey, soundMuted);
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

        public int BestScore
        {
            get
            {
                int returnScore = DefaultBestScore;
                if(bestScores.Count > 0)
                {
                    returnScore = bestScores[0].score;
                }
                return returnScore;
            }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<HighScore> HighScores
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
                if(sortScoresBy == null)
                {
                    sortScoresBy = new Comparison<int>(DescendingOrder);
                }
                return sortScoresBy;
            }
            set
            {
                if((value != null) && (sortScoresBy != value))
                {
                    sortScoresBy = value;
                }
            }
        }
        #endregion

        public static bool GetBool(string key, bool defaultValue)
        {
            return (PlayerPrefs.GetInt(key, (defaultValue ? 1 : 0)) != 0);
        }

        public static void SetBool(string key, bool setValue)
        {
            PlayerPrefs.SetInt(key, (setValue ? 1 : 0));
        }

        /*
        public static ENUM GetEnum<ENUM>(string key, ENUM defaultValue) where ENUM : struct, IConvertible
        {
            if (!typeof(ENUM).IsEnum) 
            {
                throw new NotSupportedException("T must be an enum");
            }
            return (ENUM)PlayerPrefs.GetInt(key, defaultValue.ToInt32(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
        }

        public static void SetEnum<ENUM>(string key, ENUM setValue) where ENUM : struct, IConvertible
        {
            if (!typeof(ENUM).IsEnum) 
            {
                throw new NotSupportedException("T must be an enum");
            }
            PlayerPrefs.SetInt(key, setValue.ToInt32(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
        }
        */

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

        public virtual void RetrieveFromSettings()
        {
            // Grab the the app version
            string tempString;
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

            // Grab the number of levels unlocked
            numLevelsUnlocked = PlayerPrefs.GetInt(NumLevelsUnlockedKey, DefaultNumLevelsUnlocked);

            // Grab the music settings
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
            musicMuted = GetBool(MusicMutedKey, false);

            // Grab the sound settings
            soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, DefaultSoundVolume);
            soundMuted = GetBool(SoundMutedKey, false);

            // Grab the language
            language = PlayerPrefs.GetString(LanguageKey, DefaultLanguage);

            // Grab leaderboard user scope
            leaderboardUserScope = (UserScope)PlayerPrefs.GetInt(LeaderboardUserScopeKey, (int)DefaultLeaderboardUserScope);

            // Grab the best score
            tempString = PlayerPrefs.GetString(LocalHighScoresKey, null);
            bestScores.Clear();
            if (string.IsNullOrEmpty(tempString) == false)
            {
                RetrieveHighScores(tempString);
            }

            // NOTE: Feel free to add more stuff here
        }

        public virtual void SaveSettings()
        {
            // Save the number of levels unlocked
            PlayerPrefs.SetInt(NumLevelsUnlockedKey, NumLevelsUnlocked);

            // Save the music settings
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            SetBool(MusicMutedKey, musicMuted);

            // Save the sound settings
            PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
            SetBool(SoundMutedKey, soundMuted);

            // Set the language
            PlayerPrefs.SetString(LanguageKey, language);

            // Set leaderboard's user scope variable
            PlayerPrefs.SetInt(LeaderboardUserScopeKey, (int)leaderboardUserScope);

            // Set the best score
            PlayerPrefs.SetString(LocalHighScoresKey, GenerateHighScoresString());

            // NOTE: Feel free to add more stuff here

            // Save the preferences
            PlayerPrefs.Save();
        }

        public virtual void ClearSettings()
        {
            // Grab the the app version
            int currentVersion = PlayerPrefs.GetInt(VersionKey, -1);

            // Delete all stored preferences
            PlayerPrefs.DeleteAll();

			// Store settings that are part of options.
			// Since member variables are unchanged up to this point, we can re-use them here.

            // Set the version
			PlayerPrefs.SetInt(VersionKey, currentVersion);

            // Save the music settings
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            SetBool(MusicMutedKey, musicMuted);
            
            // Save the sound settings
            PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
            SetBool(SoundMutedKey, soundMuted);

            // Set leaderboard's user scope variable
            PlayerPrefs.SetInt(LeaderboardUserScopeKey, (int)leaderboardUserScope);

            // Set the language
            PlayerPrefs.SetString(LanguageKey, language);

			// Reset all other member variables
            RetrieveFromSettings();
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
                PlayerPrefs.SetString(LocalHighScoresKey, GenerateHighScoresString());
            }
            return returnRank;
        }

        #region Helper Methods
        int DescendingOrder(int left, int right)
        {
            return (left - right);
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
                fullListBuilder.Append(bestScores[i].GetRecord(listEntryBuilder));
            }
            return fullListBuilder.ToString();
        }
        #endregion
    }
}
