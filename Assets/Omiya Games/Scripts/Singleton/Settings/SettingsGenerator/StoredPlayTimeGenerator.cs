using System;
using System.IO;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="StoredPlayTimeGenerator.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2017 Omiya Games
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
    /// <date>5/29/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Generates a <see cref="TimeSpan"/> property in <see cref="GameSettings"/>, providing how long the player has played the game.
    /// </summary>
    public class StoredPlayTimeGenerator : PropertyGenerator, IStoredSettingGenerator
    {
        private static readonly TimeSpan DefaultTimeSpan = TimeSpan.Zero;

        private readonly string key;

        private TimeSpan lastPlayTime = DefaultTimeSpan;
        private DateTime lastTimeOpen = DateTime.UtcNow;

        public StoredPlayTimeGenerator(string key, string propertyName) :
            base(propertyName, typeof(TimeSpan))
        {
            this.key = key;
            GetterCode = "TotalPlayTime";
        }

        #region Properties
        public TimeSpan TotalPlayTime
        {
            get
            {
                return lastPlayTime.Add(DateTime.UtcNow - lastTimeOpen);
            }
        }

        public override string Key
        {
            get
            {
                return key;
            }
        }

        public override string SetterCode
        {
            get
            {
                return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    throw new NotImplementedException("Cannot set the Setter");
                }
            }
        }
        #endregion

        #region Overrides
        public void OnClearSetting(ISettingsRecorder settings, int recordedVersion, int latestVersion)
        {
            OnSaveSetting(settings, latestVersion);
        }

        public void OnRetrieveSetting(ISettingsRecorder settings, int recordedVersion, int latestVersion)
        {
            // Grab how long we've played this game
            lastTimeOpen = DateTime.UtcNow;
            lastPlayTime = settings.GetTimeSpan(Key, TimeSpan.Zero);
        }

        public void OnSaveSetting(ISettingsRecorder settings, int latestVersion)
        {
            // Set the play time
            settings.SetTimeSpan(Key, TotalPlayTime);
        }

#if UNITY_EDITOR
        public override bool CanWriteCodeToInstance
        {
            get
            {
                return true;
            }
        }

        public override void WriteCodeToInstance(StreamWriter writer, int versionArrayIndex, bool includeGeneric)
        {
            GeneratorHelper.WriteCodeToInstance(this, writer, versionArrayIndex, includeGeneric);
        }
#endif
        #endregion
    }
}
