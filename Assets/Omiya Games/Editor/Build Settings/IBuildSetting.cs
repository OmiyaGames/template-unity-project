using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IBuildSetting.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
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
    /// <date>10/29/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Assets holding settings for creating builds.
    /// </summary>
    public abstract class IBuildSetting : ScriptableObject
    {
        public struct BuildResult
        {
            public enum Status
            {
                Info,
                Success,
                Warning,
                Error
            }

            public BuildResult(Status state, string message)
            {
                State = state;
                Message = message;
            }

            public Status State
            {
                get;
            }

            public string Message
            {
                get;
            }
        }

        /// <summary>
        /// Returns the maximum number of results possible,
        /// returned by a single build call.
        /// </summary>
        public abstract int MaxNumberOfResults
        {
            get;
        }

        /// <summary>
        /// Recursively creates builds.
        /// </summary>
        /// <param name="results">List of statuses indicating the results</param>
        /// <returns></returns>
        public bool Build(out List<BuildResult> results)
        {
            results = new List<BuildResult>(MaxNumberOfResults);
            return BuildBaseOnSettings(null, ref results);
        }

        /// <summary>
        /// Creates builds specific to this setting.
        /// </summary>
        /// <param name="results">List of statuses indicating the results</param>
        /// <returns>True if the build was successful.</returns>
        protected abstract bool BuildBaseOnSettings(RootBuildSetting root, ref List<BuildResult> results);

        #region Helper Methods
        /// <summary>
        /// Helper method to build a list of settings.
        /// </summary>
        /// <param name="settings">All the build settings to build from.</param>
        /// <param name="results">List of statuses indicating the results</param>
        /// <returns>True if the build was successful.</returns>
        protected static bool BuildGroup(RootBuildSetting root, IList<IChildBuildSetting> settings, ref List<BuildResult> results)
        {
            bool returnFlag = true;

            // Go through all settings
            foreach (IChildBuildSetting setting in settings)
            {
                // Attempt to make a build
                if (setting.BuildBaseOnSettings(root, ref results) == false)
                {
                    // If the build failed, update return flag
                    returnFlag = false;

                    // Check if we need to halt
                    if (setting.RootSetting.HaltOnFirstError == true)
                    {
                        break;
                    }
                }
            }
            return returnFlag;
        }

        protected static void AddSetting(IBuildSetting parent, List<IChildBuildSetting> allSettings, IChildBuildSetting setting)
        {
            setting.Parent = parent;
            allSettings.Add(setting);
        }

        protected static IChildBuildSetting RemoveSetting(List<IChildBuildSetting> allSettings, int index)
        {
            // Grab the return value
            IChildBuildSetting returnChild = allSettings[index];

            // Reset parent
            returnChild.Parent = null;

            // Remove the element
            allSettings.RemoveAt(index);
            return returnChild;
        }
        #endregion
    }
}
