using System;
using UnityEngine;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PlayerPrefsSettingsRecorder.cs" company="Omiya Games">
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
    /// <date>5/16/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An implementation of <code>ISettingsRecorder</code> using <code>PlayerPrefs</code>.
    /// </summary>
    /// <seealso cref="ISettingsRecorder"/>
    /// <seealso cref="PlayerPrefs"/>
    /// <seealso cref="GameSettings"/>
    public class PlayerPrefsSettingsRecorder : SettingsRecorderDecorator
    {
        #region Int Settings
        public override int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public override void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        #endregion

        #region Float Settings
        public override float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public override void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        #endregion

        #region String Settings
        public override string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public override void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        #endregion

        #region Delete Settings
        public override void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public override void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
        #endregion

        public override void Save()
        {
            PlayerPrefs.Save();
        }

        public override bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}
