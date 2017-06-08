using System;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SortedRecordSettingGenerator.cs" company="Omiya Games">
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
    /// Generates a <see cref="ISortedRecords{T}"/> property in <see cref="GameSettings"/>.
    /// </summary>
    public class SortedRecordSettingGenerator<T> : PropertyStoredSettingsGenerator<ISortedRecords<T>> where T : IComparable<T>
    {
        public SortedRecordSettingGenerator(string key, ISortedRecords<T> defaultValue) : base(key, defaultValue)
        {
        }

        #region Overrides
        public override bool IsSameValue(ISortedRecords<T> compareValue)
        {
            return Value == compareValue;
        }

        public override ISortedRecords<T> DefaultSettingsRetrieval(ISettingsRecorder settings, int recordedVersion, ISortedRecords<T> defaultValue)
        {
            string pastRecords = settings.GetString(Key, string.Empty);
            Value.RetrieveRecords(pastRecords, recordedVersion);
            return Value;
        }

        public override void OnSaveSetting(ISettingsRecorder settings, int latestVersion)
        {
            settings.SetString(Key, Value.ToString());
        }

        public override string SetterCode
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
