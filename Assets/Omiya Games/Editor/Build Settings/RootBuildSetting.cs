using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="RootBuildSetting.cs" company="Omiya Games">
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
    public class RootBuildSetting : IBuildSetting
    {
        public enum BuildProgression
        {
            AskWhetherToContinue,
            IgnoreAndResumeBuilding,
            HaltImmediately,
        }

        [SerializeField]
        [FolderPath]
        private string rootBuildFolder = "";
        [SerializeField]
        private CustomFileName newBuildFolderName = new CustomFileName(false,
            new CustomFileName.Prefill(CustomFileName.PrefillType.BuildSettingName, null),
            new CustomFileName.Prefill(CustomFileName.PrefillType.Literal, "-"),
            new CustomFileName.Prefill(CustomFileName.PrefillType.DateTime, DefaultDateTimeText));
        [SerializeField]
        private BuildProgression onBuildFailed = BuildProgression.AskWhetherToContinue;
        [SerializeField]
        private BuildProgression onBuildCancelled = BuildProgression.AskWhetherToContinue;
        [SerializeField]
        List<IChildBuildSetting> allSettings = new List<IChildBuildSetting>();

        // TODO: add this optimization flag once we've figured out what platform and debug settings we're on.
        // Not to mention recursively finding a list of similar build settings to reduce script compiling.
        //[SerializeField]
        //[Tooltip("Time-saving flag: If true, builds the current platform first rather than going through them in order")]
        //private bool buildCurrentPlatformFirst = true;

        #region Overrides
        internal override int MaxNumberOfResults
        {
            get
            {
                // Indicate this group drops 2 statuses
                int returnNumber = 2;

                // Increment the max by all the other settings
                foreach (IBuildSetting setting in allSettings)
                {
                    returnNumber += setting.MaxNumberOfResults;
                }
                return returnNumber;
            }
        }

        internal override RootBuildSetting RootSetting
        {
            get
            {
                return this;
            }
        }

        protected override void Build(BuildPlayersResult results)
        {
            // Indicate group build started
            using (new BuildPlayersResult.GroupBuildScope(results, this))
            {
                // Build the list of settings
                BuildGroup(allSettings, results);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public BuildProgression OnBuildFailed
        {
            get
            {
                return onBuildFailed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BuildProgression OnBuildCancelled
        {
            get
            {
                return onBuildCancelled;
            }
        }
        #endregion

        public string GetBuildPath()
        {
            string customPath = rootBuildFolder;
            if (string.IsNullOrEmpty(rootBuildFolder) == true)
            {
                customPath = UnityEditor.EditorUtility.SaveFolderPanel("Build project to folder", "Builds", "");
                if (string.IsNullOrEmpty(customPath) == true)
                {
                    throw new System.Exception("No folder assigned to build to.");
                }
            }
            return System.IO.Path.Combine(customPath, newBuildFolderName.ToString());
        }

        public void Add(IChildBuildSetting addSetting)
        {
            AddSetting(this, allSettings, addSetting);
        }

        public IChildBuildSetting Remove(int index)
        {
            return RemoveSetting(allSettings, index);
        }
    }
}
