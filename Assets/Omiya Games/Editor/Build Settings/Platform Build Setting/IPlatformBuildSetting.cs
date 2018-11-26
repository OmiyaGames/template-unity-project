using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IPlatformBuildSetting.cs" company="Omiya Games">
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
    /// Base build settings for any platform.
    /// </summary>
    public abstract class IPlatformBuildSetting : IChildBuildSetting
    {
        public enum Architecture
        {
            BuildUniversal,
            Build64Bit,
            Build32Bit
        }

        public enum CompressionType
        {
            Default,
            LZ4,
            LZ4HighCompression
        }

        public enum ArchiveType
        {
            Zip
        }

        [Serializable]
        public struct ArchiveSettings
        {
            [SerializeField]
            bool enable;
            [SerializeField]
            ArchiveType type;
            [SerializeField]
            bool includeParentFolder;
            [SerializeField]
            bool deleteOriginals;
            [SerializeField]
            CustomFileName fileName;

            public ArchiveSettings(bool includeParentFolder)
            {
                // Setup member variables
                this.includeParentFolder = includeParentFolder;

                // Setup defaults
                enable = false;
                deleteOriginals = false;
                type = ArchiveType.Zip;

                // Setup name
                string extension;
                switch (type)
                {
                    // TODO: when new archive types are added, add file extensions
                    default:
                        extension = ".zip";
                        break;
                }

                CustomFileName.Prefill appName = new CustomFileName.Prefill(CustomFileName.PrefillType.AppName);
                CustomFileName.Prefill fileExtension = new CustomFileName.Prefill(CustomFileName.PrefillType.Literal, extension);
                fileName = new CustomFileName(false, appName, fileExtension);
            }

            public bool IsEnabled
            {
                get
                {
                    return enable;
                }
            }

            public bool IsParentFolderIncluded
            {
                get
                {
                    return includeParentFolder;
                }
            }

            public CustomFileName FileName
            {
                get
                {
                    return fileName;
                }
            }

            public bool DeleteOriginals
            {
                get
                {
                    return deleteOriginals;
                }
            }

            public ArchiveType Type
            {
                get
                {
                    return type;
                }
            }
        }

        [Serializable]
        public struct DevelopmentSettings
        {
            [SerializeField]
            bool enable;
            [SerializeField]
            bool enableDebuggingScripts;
            [SerializeField]
            bool buildScriptsOnly;

            public DevelopmentSettings(bool allowDebuggingScripts)
            {
                this.enable = false;
                this.enableDebuggingScripts = allowDebuggingScripts;
                this.buildScriptsOnly = false;
            }

            public bool IsEnabled
            {
                get
                {
                    return enable;
                }
            }

            public bool IsBuildingScriptsOnly
            {
                get
                {
                    return buildScriptsOnly;
                }
            }

            public bool IsScriptDebuggingEnabled
            {
                get
                {
                    return enableDebuggingScripts;
                }
            }
        }

        public class LastPlayerSettings
        {
            public string LastScriptDefineSymbols
            {
                get;
            }

            public LastPlayerSettings(BuildTargetGroup target)
            {
                // Setup member variables
                LastScriptDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            }
        }

        [SerializeField]
        [Tooltip("Name of the executable file.")]
        protected CustomFileName fileName = new CustomFileName(false, new CustomFileName.Prefill(CustomFileName.PrefillType.AppName));
        [SerializeField]
        [Tooltip("Name of the folder the executables will be in.")]
        protected CustomFileName folderName = new CustomFileName(false,
            new CustomFileName.Prefill(CustomFileName.PrefillType.AppName),
            new CustomFileName.Prefill(CustomFileName.PrefillType.Literal, " ("),
            new CustomFileName.Prefill(CustomFileName.PrefillType.BuildSettingName),
            new CustomFileName.Prefill(CustomFileName.PrefillType.Literal, ")"));
        [SerializeField]
        protected bool enableStrictMode = false;
        /// <summary>
        /// Only effective if debugSettings is disabled.
        /// </summary>
        [SerializeField]
        protected bool enableAssertions = false;
        [SerializeField]
        protected SceneSetting customScenes = new SceneSetting();
        [SerializeField]
        protected DevelopmentSettings debugSettings = new DevelopmentSettings(true);
        [SerializeField]
        protected ArchiveSettings archiveSettings = new ArchiveSettings(true);
        [SerializeField]
        protected ScriptDefineSymbolsSetting customScriptDefineSymbols;

        #region Overrides
        internal override int MaxNumberOfResults
        {
            get
            {
                if (archiveSettings.IsEnabled == true)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }

        protected override void Build(BuildPlayersResult results)
        {
            // Check if prebuild check succeeded
            string message;
            if (PreBuildCheck(out message) == true)
            {
                // Setup the folders
                if (System.IO.Directory.Exists(results.FolderName) == false)
                {
                    System.IO.Directory.CreateDirectory(results.FolderName);
                }

                // Get options
                BuildPlayerOptions options = GetPlayerOptions(results);

                // Build the player
                LastPlayerSettings lastSettings = SetupPlayerSettings();
                BuildReport res = BuildPipeline.BuildPlayer(options);
                RevertPlayerSettings(lastSettings);

                // Add the latest results
                results.AddReport(res, this);

                // Consider making a post build
                if (results.LastReport.State == BuildPlayersResult.Status.Success)
                {
                    ArchiveBuild(results);
                }
            }
            else
            {
                // Display a message
                DisplayPreBuildCheckFailed(message);
            }
        }

        public override string GetPathPreview(System.Text.StringBuilder builder, char pathDivider)
        {
            // Get the parent's path
            string parentPath = null;
            if (Parent != null)
            {
                parentPath = Parent.GetPathPreview(builder, pathDivider);
            }

            // Setup builder with parent path
            builder.Clear();
            if (string.IsNullOrEmpty(parentPath) == false)
            {
                builder.Append(parentPath);
            }

            // Grab the folder name
            parentPath = folderName.ToString(this);
            if (string.IsNullOrEmpty(parentPath) == false)
            {
                // Check if we need to add a path divider
                if (builder[builder.Length - 1] != pathDivider)
                {
                    builder.Append(pathDivider);
                }

                // Append this folder name
                builder.Append(parentPath);
            }
            return builder.ToString();
        }

        public override bool PreBuildCheck(out string message)
        {
            message = null;
            return true;
        }
        #endregion

        protected abstract BuildTargetGroup TargetGroup
        {
            get;
        }

        protected abstract BuildTarget Target
        {
            get;
        }

        public virtual string FileExtension
        {
            get
            {
                return null;
            }
        }

        protected virtual BuildOptions Options
        {
            get
            {
                BuildOptions options = BuildOptions.None;

                // Update strict mode
                if (enableStrictMode == true)
                {
                    options |= BuildOptions.StrictMode;
                }

                // Check debug settings
                if (debugSettings.IsEnabled == true)
                {
                    // Update debug settings
                    options |= BuildOptions.Development;
                    if (debugSettings.IsScriptDebuggingEnabled == true)
                    {
                        options |= BuildOptions.AllowDebugging;
                    }
                    if (debugSettings.IsBuildingScriptsOnly == true)
                    {
                        options |= BuildOptions.BuildScriptsOnly;
                    }
                }
                else if (enableAssertions == true)
                {
                    // Update assertion mode
                    options |= BuildOptions.ForceEnableAssertions;
                }
                return options;
            }
        }

        /// <summary>
        /// Note: don't forget to override <code>RevertPlayerSettings()</code>!
        /// </summary>
        protected virtual LastPlayerSettings SetupPlayerSettings()
        {
            // Setup return variable
            LastPlayerSettings returnSettings = new LastPlayerSettings(TargetGroup);

            // Check if we want to update the player settings
            if (customScriptDefineSymbols.IsEnabled == true)
            {
                // Update player settings
                PlayerSettings.SetScriptingDefineSymbolsForGroup(TargetGroup, customScriptDefineSymbols.CustomValue);
            }
            return returnSettings;
        }

        /// <summary>
        /// Note: don't forget to override <code>SetupPlayerSettings()</code>!
        /// </summary>
        protected virtual void RevertPlayerSettings(LastPlayerSettings lastSettings)
        {
            // Check if we want to update the player settings
            if (customScriptDefineSymbols.IsEnabled == true)
            {
                // Revert player settings
                PlayerSettings.SetScriptingDefineSymbolsForGroup(TargetGroup, lastSettings.LastScriptDefineSymbols);
            }
        }

        protected virtual void ArchiveBuild(BuildPlayersResult results)
        {
            if (archiveSettings.IsEnabled == true)
            {
                // FIXME: to ZIP the folder that's generated
                throw new System.NotImplementedException();
                if (false)
                {
                    results.AddPostBuildReport(BuildPlayersResult.Status.Success, results.Concatenate("Successfully archived: ", name), this);
                }
                else
                {
                    results.AddPostBuildReport(BuildPlayersResult.Status.Success, results.Concatenate("Failed to archive: ", name), this);
                }
            }
        }

        private BuildPlayerOptions GetPlayerOptions(BuildPlayersResult results)
        {
            // Setup all options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

            // Update the location to build this player
            buildPlayerOptions.locationPathName = results.ConcatenateFolders(results.FolderName, folderName.ToString(this), fileName.ToString(this));
            if (string.IsNullOrEmpty(FileExtension) == false)
            {
                buildPlayerOptions.locationPathName = results.Concatenate(buildPlayerOptions.locationPathName, FileExtension);
            }

            // Update the scenes to build
            if (customScenes.IsEnabled == true)
            {
                buildPlayerOptions.scenes = customScenes.CustomValue;
            }
            else
            {
                buildPlayerOptions.scenes = results.DefaultScenes;
            }

            // Update the rest
            buildPlayerOptions.targetGroup = TargetGroup;
            buildPlayerOptions.target = Target;
            buildPlayerOptions.options = Options;
            return buildPlayerOptions;
        }

        protected static void SetBuildOption(ref BuildOptions options, BuildTargetGroup targetGroup, CompressionType compression)
        {
            if (compression == CompressionType.LZ4HighCompression)
            {
                switch (targetGroup)
                {
                    case BuildTargetGroup.Standalone:
                    case BuildTargetGroup.Android:
                    case BuildTargetGroup.iOS:
                    case BuildTargetGroup.WebGL:
                        options |= BuildOptions.CompressWithLz4HC;
                        break;
                }
            }
            else if (compression == CompressionType.LZ4)
            {
                switch (targetGroup)
                {
                    case BuildTargetGroup.Standalone:
                    case BuildTargetGroup.Android:
                    case BuildTargetGroup.iOS:
                        options |= BuildOptions.CompressWithLz4;
                        break;
                }
            }
        }
    }
}
