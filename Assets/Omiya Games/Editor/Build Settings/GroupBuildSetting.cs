using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GroupBuildSetting.cs" company="Omiya Games">
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
    /// A list of <code>IChildBuildSettings</code> to run sequentially.
    /// </summary>
    public class GroupBuildSetting : IChildBuildSetting
    {
        [Header("Folder Settings")]
        [SerializeField]
        private bool createEmbeddedFolder = true;
        [SerializeField]
        private CustomFileName folderName = new CustomFileName(false, new CustomFileName.Prefill(CustomFileName.PrefillType.BuildSettingName));

        [Header("Child Settings")]
        [SerializeField]
        private List<IChildBuildSetting> allSettings = new List<IChildBuildSetting>();

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

        public override IBuildSetting Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                base.Parent = value;
                foreach (IChildBuildSetting setting in allSettings)
                {
                    setting.Parent = this;
                }
            }
        }

        protected override void Build(BuildPlayersResult results)
        {
            // Check the group first
            string message;
            foreach (IChildBuildSetting setting in allSettings)
            {
                // Check if prebuild check failed
                if (setting.PreBuildCheck(out message) == false)
                {
                    // Display a message
                    DisplayPreBuildCheckFailed(message);

                    // Stop building entirely
                    return;
                }
            }

            // Indicate group build started
            using (new BuildPlayersResult.GroupBuildScope(results, this))
            {
                // Build the list of settings
                BuildGroup(allSettings, results);
            }
        }

        public override string GetPathPreview(System.Text.StringBuilder builder, char pathDivider)
        {
            // Get the parent's path
            string returnPath = null;
            if (Parent != null)
            {
                returnPath = Parent.GetPathPreview(builder, pathDivider);
            }

            // Check if we need to append the group folder name
            if (createEmbeddedFolder == true)
            {
                // Setup builder with parent path
                builder.Clear();
                builder.Append(returnPath);

                // Append this group's name
                if (builder[builder.Length - 1] != pathDivider)
                {
                    builder.Append(pathDivider);
                }
                builder.Append(folderName.ToString(this));

                // Update return variable
                returnPath = builder.ToString();
            }
            return returnPath;
        }

        public override bool PreBuildCheck(out string message)
        {
            message = null;
            foreach (IChildBuildSetting setting in allSettings)
            {
                if (setting.PreBuildCheck(out message) == false)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        public string FolderName
        {
            get
            {
                return folderName.ToString();
            }
        }

        public bool IsInEmbeddedFolder
        {
            get
            {
                return createEmbeddedFolder;
            }
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
