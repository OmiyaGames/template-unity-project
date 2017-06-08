using System;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ISettingsRecorder.cs" company="Omiya Games">
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
    /// <date>12/8/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An interface for storing settings. Useful for indicating where a game's settings should be saved.
    /// </summary>
    /// <seealso cref="GameSettings"/>
    public interface ISettingsRecorder
    {
        bool GetBool(string key, bool defaultValue);
        void SetBool(string key, bool value);

        int GetInt(string key, int defaultValue);
        void SetInt(string key, int value);

        float GetFloat(string key, float defaultValue);
        void SetFloat(string key, float value);

        string GetString(string key, string defaultValue);
        void SetString(string key, string value);

        ENUM GetEnum<ENUM>(string key, ENUM defaultValue) where ENUM : struct, IConvertible;
        void SetEnum<ENUM>(string key, ENUM value) where ENUM : struct, IConvertible;

        DateTime GetDateTimeUtc(string key, DateTime defaultValue);
        void SetDateTimeUtc(string key, DateTime value);

        TimeSpan GetTimeSpan(string key, TimeSpan defaultValue);
        void SetTimeSpan(string key, TimeSpan value);

        void Save();
        bool HasKey(string key);
        void DeleteKey(string key);
        void DeleteAll();
    }
}
