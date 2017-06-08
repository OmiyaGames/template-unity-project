using System;
using System.Collections.Generic;
using System.Collections;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SingleSettingsGeneratorArgs.cs" company="Omiya Games">
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
    /// Arguments providing a collection of <see cref="IGenerator"/>
    /// </summary>
    /// <seealso cref="GameSettings"/>
    public class SettingsGeneratorArgs : EventArgs, IEnumerable<KeyValuePair<int, ICollection<SettingsGeneratorArgs.SingleSettingsInfo>>>
    {
        public struct SingleSettingsInfo
        {
            public readonly int versionArrayIndex;
            public readonly IGenerator generator;
            public readonly bool isStoredSetting;

            public SingleSettingsInfo(int versionArrayIndex, IGenerator generator)
            {
                this.versionArrayIndex = versionArrayIndex;
                this.generator = generator;
                isStoredSetting = (generator is IStoredSetting);
            }

            public string propertyName
            {
                get
                {
                    return generator.PropertyName;
                }
            }
        }

        readonly Dictionary<string, SingleSettingsInfo> allProperties = new Dictionary<string, SingleSettingsInfo>();
        readonly HashSet<string> allPropertyNames = new HashSet<string>();

        public bool AddSetting(int versionArrayIndex, IGenerator generator, out string errorMessage)
        {
            bool returnFlag = false;
            errorMessage = null;
            if (string.IsNullOrEmpty(generator.Key) == true)
            {
                errorMessage = "Key from generator is null or empty. Please populate them.";
            }
            else if (string.IsNullOrEmpty(generator.PropertyName) == true)
            {
                errorMessage = "PropertyName from generator is null or empty. Please populate them.";
            }
            else if (allProperties.ContainsKey(generator.Key) == true)
            {
                errorMessage = "Key \"" + generator.Key + "\" is already in the arguments. Consider using ModifySetting(ISingleGenerator, out string) instead.";
            }
            else if (allPropertyNames.Contains(generator.PropertyName) == true)
            {
                errorMessage = "Property/Function \"" + generator.PropertyName + "\" is already in the arguments. Provide a unique property name, instead.";
            }
            else
            {
                allProperties.Add(generator.Key, new SingleSettingsInfo(versionArrayIndex, generator));
                allPropertyNames.Add(generator.PropertyName);
                returnFlag = true;
            }
            return returnFlag;
        }

        public bool RemoveSetting(string generatorKey, out string errorMessage)
        {
            bool returnFlag = false;
            errorMessage = null;

            if (string.IsNullOrEmpty(generatorKey) == true)
            {
                errorMessage = "Key is null or empty.";
            }
            else if(RemoveSettings(generatorKey) == false)
            {
                errorMessage = "Key \"" + generatorKey + "\" is not in the arguments.";
            }
            else
            {
                returnFlag = true;
            }
            return returnFlag;
        }

        public IEnumerator<KeyValuePair<int, ICollection<SingleSettingsInfo>>> GetEnumerator()
        {
            return GroupedInfo.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GroupedInfo.GetEnumerator();
        }

        #region Helper Methods
        private SortedDictionary<int, ICollection<SingleSettingsInfo>> GroupedInfo
        {
            get
            {
                SortedDictionary<int, ICollection<SingleSettingsInfo>> groupedInfo = new SortedDictionary<int, ICollection<SingleSettingsInfo>>();
                ICollection<SingleSettingsInfo> infoGroup = null;
                foreach (SingleSettingsInfo info in allProperties.Values)
                {
                    if(groupedInfo.TryGetValue(info.versionArrayIndex, out infoGroup) == false)
                    {
                        infoGroup = new List<SingleSettingsInfo>();
                        groupedInfo.Add(info.versionArrayIndex, infoGroup);
                    }
                    infoGroup.Add(info);
                }
                return groupedInfo;
            }
        }

        private bool RemoveSettings(string key)
        {
            bool returnFlag = false;
            SingleSettingsInfo generator;

            if (allProperties.TryGetValue(key, out generator) == true)
            {
                allPropertyNames.Remove(generator.propertyName);
                allProperties.Remove(key);
                returnFlag = true;
            }
            return returnFlag;
        }
        #endregion
    }
}
