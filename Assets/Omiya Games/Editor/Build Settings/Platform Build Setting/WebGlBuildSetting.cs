using UnityEngine;
using UnityEditor;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="WebGlBuildSetting.cs" company="Omiya Games">
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
    /// <date>10/31/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Build settings for WebGL platform.
    /// </summary>
    public class WebGlBuildSetting : IPlatformBuildSetting
    {
        private class LastWebGlPlayerSettings : LastPlayerSettings
        {
            public LastWebGlPlayerSettings(LastPlayerSettings setting, string templatePath) : base(setting.Target)
            {
                // Setup member variables
                TemplatePath = templatePath;
            }

            public string TemplatePath
            {
                get;
            }
        }

        [SerializeField]
        [Tooltip("[Optional] Determines the WebGL template this setting builds to.")]
        private string templatePath;
        // FIXME: work on setting up a drawer for this variable
        [SerializeField]
        protected HostArchiveSetting[] hostSpecificArchiveSettings;
        // FIXME: do more research on the Facebook builds
        //[SerializeField]
        //protected bool forFacebook = false;

        #region Overrides
        internal override int MaxNumberOfResults
        {
            get
            {
                return base.MaxNumberOfResults + hostSpecificArchiveSettings.Length;
            }
        }

        protected override BuildTargetGroup TargetGroup
        {
            get
            {
                //if (forFacebook == true)
                //{
                //    return BuildTargetGroup.Facebook;
                //}
                return BuildTargetGroup.WebGL;
            }
        }

        protected override BuildTarget Target => BuildTarget.WebGL;

        public string TemplatePath
        {
            get => templatePath;
            set => templatePath = value;
        }

        protected override LastPlayerSettings SetupPlayerSettings()
        {
            // Store the old setting
            LastWebGlPlayerSettings returnSetting = new LastWebGlPlayerSettings(base.SetupPlayerSettings(), PlayerSettings.WebGL.template);

            // Change the template path
            if (string.IsNullOrEmpty(TemplatePath) == false)
            {
                PlayerSettings.WebGL.template = TemplatePath;
            }
            return returnSetting;
        }

        protected override void RevertPlayerSettings(LastPlayerSettings lastSettings)
        {
            base.RevertPlayerSettings(lastSettings);

            // Revert the template path
            if (lastSettings is LastWebGlPlayerSettings)
            {
                PlayerSettings.WebGL.template = ((LastWebGlPlayerSettings)lastSettings).TemplatePath;
            }
        }

        protected override async System.Threading.Tasks.Task PostSuccessfulBuild(BuildPlayersResult results)
        {
            if ((hostSpecificArchiveSettings != null) && (hostSpecificArchiveSettings.Length > 0))
            {
                // Indicate we're doing some post-build setup
                results.AddPostBuildReport(BuildPlayersResult.Status.EnterGroup, "Building DomainLists for " + name, this);

                // Grab the folder to archive
                string archiveFolderName = GetBuildFolderName(results);

                // Go through every archive settings
                foreach (HostArchiveSetting setting in hostSpecificArchiveSettings)
                {
                    // Check if the setting is enabled
                    if (setting.IsEnabled == true)
                    {
                        // Build the setting first
                        setting.Build(this, results);

                        // Archive this build, assuming it was successful
                        if (results.LastReport.State == BuildPlayersResult.Status.Success)
                        {
                            await ArchiveBuildHelper(setting, setting.ArchiveSettings, results, archiveFolderName);
                        }
                    }
                }

                // Indicate we finished post-build setup
                results.AddPostBuildReport(BuildPlayersResult.Status.ExitGroup, "Finished building DomainLists for " + name, this);
            }
        }

        public override bool PreBuildCheck(out string message)
        {
            bool returnFlag = base.PreBuildCheck(out message);

            // Append the message to the builder
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (string.IsNullOrEmpty(message) == false)
            {
                builder.AppendLine(message);
            }

            // Check if there are any problems in the archive settings
            foreach (HostArchiveSetting setting in hostSpecificArchiveSettings)
            {
                // Check if prebuild check failed
                if (setting.PreBuildCheck(out message) == false)
                {
                    // Return false and append the message
                    returnFlag = false;
                    builder.AppendLine(message);
                }
            }

            // Setup return values
            message = builder.ToString();
            return returnFlag;
        }
        #endregion

        public string GetBuildFolderName(BuildPlayersResult results)
        {
            // Calculate folder and file name
            return results.ConcatenateFolders(results.FolderName, folderName.ToString(this), FileName.ToString(this));
        }
    }
}
