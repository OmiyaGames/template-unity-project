using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="WindowsBuildSetting.cs" company="Omiya Games">
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
    /// Build settings for Windows platform.
    /// </summary>
    public class WindowsBuildSetting : IChildBuildSetting
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

        [Serializable]
        public struct CustomName
        {
            public string ToString(IChildBuildSetting setting)
            {
                return base.ToString();
            }
        }

        [Serializable]
        public struct SceneSettings
        {
            [SerializeField]
            bool enabled;
            [SerializeField]
            string[] scenePaths;

            public bool IsEnabled
            {
                get
                {
                    return enabled;
                }
            }

            public string[] Paths
            {
                get
                {
                    return scenePaths;
                }
            }
        }

        [Serializable]
        public struct ArchiveSettings
        {
            [SerializeField]
            bool enabled;
            [SerializeField]
            bool includeParentFolder;
            [SerializeField]
            CustomName fileName;
            [SerializeField]
            bool deleteOriginals;

            public ArchiveSettings(CustomName fileName, bool includeParentFolder)
            {
                // Setup member variables
                this.fileName = fileName;
                this.includeParentFolder = includeParentFolder;

                // Setup defaults
                enabled = false;
                deleteOriginals = false;
            }

            public bool IsEnabled
            {
                get
                {
                    return enabled;
                }
            }

            public bool IsParentFolderIncluded
            {
                get
                {
                    return includeParentFolder;
                }
            }

            public CustomName FileName
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
        }

        [SerializeField]
        Architecture architecture = Architecture.Build64Bit;
        [SerializeField]
        CompressionType compression = CompressionType.Default;
        [SerializeField]
        bool enableStrictMode = false;

        [Header("Scene Settings")]
        [SerializeField]
        SceneSettings customScenes;

        [Header("Archive Configuration")]
        [SerializeField]
        ArchiveSettings archiveSettings = new ArchiveSettings(new CustomName(), true);

        #region Overrides
        internal override int MaxNumberOfResults
        {
            get
            {
                if(archiveSettings.IsEnabled == true)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }

        protected override void BuildBaseOnSettings(RootBuildSetting root, BuildPlayersResult results)
        {
            // Get options
            BuildPlayerOptions options = GetPlayerOptions(root);

            // Build the player
            BuildReport res = BuildPipeline.BuildPlayer(options);

            // Add the latest results
            results.AddReport(res, this);

            // Consider making a post build
            if ((results.LastReport.State == BuildPlayersResult.Status.Success) && (archiveSettings.IsEnabled == true))
            {
                if(ArchiveBuild(root) == true)
                {
                    results.AddPostBuildReport(BuildPlayersResult.Status.Success, results.Concatenate("Successfully archived: ", name), this);
                }
                else
                {
                    results.AddPostBuildReport(BuildPlayersResult.Status.Success, results.Concatenate("Failed to archive: ", name), this);
                }
            }
        }
        #endregion

        public BuildPlayerOptions GetPlayerOptions(RootBuildSetting root)
        {
            // Setup all options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

            // Update the location to build this player
            buildPlayerOptions.locationPathName = root.BuildPath;

            // Update the scenes to build
            if (customScenes.IsEnabled == true)
            {
                buildPlayerOptions.scenes = customScenes.Paths;
            }
            else
            {
                buildPlayerOptions.scenes = root.DefaultScenes;
            }

            // Update the platform target
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
            buildPlayerOptions.target = BuildTarget.StandaloneWindows;
            if (architecture == Architecture.Build64Bit)
            {
                buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            }

            // Update the options
            BuildOptions options = BuildOptions.None;
            if (enableStrictMode == true)
            {
                options |= BuildOptions.StrictMode;
            }
            options |= GetBuildOption(ref buildPlayerOptions, compression);
            buildPlayerOptions.options = options;
            return buildPlayerOptions;
        }

        public bool ArchiveBuild(RootBuildSetting root)
        {
            return false;
        }

        private static BuildOptions GetBuildOption(ref BuildPlayerOptions buildPlayerOptions, CompressionType compression)
        {
            BuildOptions options = BuildOptions.None;
            if (compression == CompressionType.LZ4HighCompression)
            {
                switch (buildPlayerOptions.targetGroup)
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
                switch (buildPlayerOptions.targetGroup)
                {
                    case BuildTargetGroup.Standalone:
                    case BuildTargetGroup.Android:
                    case BuildTargetGroup.iOS:
                        options |= BuildOptions.CompressWithLz4;
                        break;
                }
            }
            return options;
        }
    }
}
