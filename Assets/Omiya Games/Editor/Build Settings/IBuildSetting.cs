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
        public const string DefaultDateTimeText = "yyyy.MM.dd-HH.mm";

        /// <summary>
        /// Returns the maximum number of results possible,
        /// returned by a single build call.
        /// </summary>
        internal abstract int MaxNumberOfResults
        {
            get;
        }

        /// <summary>
        /// Returns the root-most parent.
        /// </summary>
        internal abstract RootBuildSetting RootSetting
        {
            get;
        }

        /// <summary>
        /// Returns stored number of times this setting created a build.
        /// </summary>
        internal virtual int BuildNumber
        {
            get
            {
                return -1;
            }
            set
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Grabs a preview of the path to build to
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public abstract string GetPathPreview(System.Text.StringBuilder builder, char pathDivider);

        /// <summary>
        /// Grabs a preview of the path to build to
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public string GetPathPreview(System.Text.StringBuilder builder)
        {
            return GetPathPreview(builder, Helpers.PathDivider);
        }

        /// <summary>
        /// Recursively creates builds.
        /// </summary>
        /// <param name="results">List of statuses indicating the results</param>
        /// <returns></returns>
        public virtual BuildPlayersResult Build()
        {
            BuildPlayersResult results = SetupResults();
            Build(results);
            return results;
        }

        /// <summary>
        /// Creates builds specific to this setting.
        /// </summary>
        /// <param name="results">List of statuses indicating the results</param>
        /// <returns>True if the build was successful.</returns>
        protected abstract void Build(BuildPlayersResult results);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected BuildPlayersResult SetupResults()
        {
            return new BuildPlayersResult(RootSetting, this);
        }

        #region Helper Methods
        protected static void DisplayPreBuildCheckFailed(string message)
        {
            // Display a message
            UnityEditor.EditorUtility.DisplayDialog("Pre-Build Check Failed", message, "OK");
        }

        /// <summary>
        /// Helper method to build a list of settings.
        /// </summary>
        /// <param name="settings">All the build settings to build from.</param>
        /// <param name="results">List of statuses indicating the results</param>
        /// <returns>True if the build was successful.</returns>
        protected static void BuildGroup(IEnumerable<IChildBuildSetting> settings, BuildPlayersResult results)
        {
            // Go through all settings
            foreach (IChildBuildSetting setting in settings)
            {
                // First, check if we didn't cancel the build
                if (results.IsAllBuildsCancelled == true)
                {
                    return;
                }

                // Make a build
                setting.Build(results);
                if (results.LastReport.State == BuildPlayersResult.Status.Error)
                {
                    UpdateResults(results, true);
                }
                else if (results.LastReport.State == BuildPlayersResult.Status.Cancelled)
                {
                    UpdateResults(results, true);
                }
            }
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

        private static void UpdateResults(BuildPlayersResult results, bool isError)
        {
            // Grab the result's state
            RootBuildSetting.BuildProgression state = results.OnBuildCancelled;
            if (isError == true)
            {
                state = results.OnBuildFailed;
            }

            // Check if we need to display a dialog
            if (state == RootBuildSetting.BuildProgression.AskWhetherToContinue)
            {
                // Display the dialog, and update the build failed state
                state = results.DisplayBuildProgressionDialog(isError);

                // Update the result as well
                if (isError == true)
                {
                    results.OnBuildFailed = state;
                }
                else
                {
                    results.OnBuildCancelled = state;
                }
            }

            // Check if we need to halt building
            if (state == RootBuildSetting.BuildProgression.HaltImmediately)
            {
                // Halt immediately
                results.IsAllBuildsCancelled = true;
            }
        }
        #endregion
    }
}
