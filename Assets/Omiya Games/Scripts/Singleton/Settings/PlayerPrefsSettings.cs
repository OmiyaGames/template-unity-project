using System;
using System.Globalization;
using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ISettings.cs" company="Omiya Games">
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
    /// <date>12/7/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An implementation of <code>ISettings</code> using <code>PlayerPrefs</code>.
    /// </summary>
    /// <seealso cref="ISettings"/>
    /// <seealso cref="PlayerPrefs"/>
    /// <seealso cref="GameSettings"/>
    public class PlayerPrefsSettings : ISettings
    {
        public bool GetBool(string key, bool defaultValue)
        {
            return ToBool(GetInt(key, ToInt(defaultValue)));
        }
        public void SetBool(string key, bool value)
        {
            SetInt(key, ToInt(value));
        }

        public int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public ENUM GetEnum<ENUM>(string key, ENUM defaultValue) where ENUM : struct, IConvertible
        {
            if (typeof(ENUM).IsEnum == false)
            {
                throw new NotSupportedException("Generic type must be an enum");
            }
            return (ENUM)(object)GetInt(key, ToInt(defaultValue));
        }
        public void SetEnum<ENUM>(string key, ENUM value) where ENUM : struct, IConvertible
        {
            if (typeof(ENUM).IsEnum == false)
            {
                throw new NotSupportedException("Generic type must be an enum");
            }
            SetInt(key, ToInt(value));
        }

        public DateTime GetDateTimeUtc(string key, DateTime defaultValue)
        {
            return ToDateTimeUtc(GetString(key, ToString(defaultValue)));
        }
        public void SetDateTimeUtc(string key, DateTime value)
        {
            SetString(key, ToString(value));
        }

        public TimeSpan GetTimeSpan(string key, TimeSpan defaultValue)
        {
            return ToTimeSpan(GetString(key, ToString(defaultValue)));
        }
        public void SetTimeSpan(string key, TimeSpan value)
        {
            SetString(key, ToString(value));
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public static int ToInt(bool flag)
        {
            return (flag ? 1 : 0);
        }
        public static int ToInt(IConvertible value)
        {
            return value.ToInt32(CultureInfo.InvariantCulture.NumberFormat);
        }
        public static bool ToBool(int value)
        {
            if(value != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string ToString(DateTime timeUtc)
        {
            return timeUtc.Ticks.ToString();
        }
        public static string ToString(TimeSpan duration)
        {
            return duration.Ticks.ToString();
        }
        public static DateTime ToDateTimeUtc(string value)
        {
            long ticks;
            DateTime time = DateTime.MinValue;
            if(long.TryParse(value, out ticks) == true)
            {
                time = new DateTime(ticks, DateTimeKind.Utc);
            }
            return time;
        }
        public static TimeSpan ToTimeSpan(string value)
        {
            long ticks;
            TimeSpan span = TimeSpan.Zero;
            if (long.TryParse(value, out ticks) == true)
            {
                span = new TimeSpan(ticks);
            }
            return span;
        }
    }
}
