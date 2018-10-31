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
    /// Build settings for Windows platform.
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
            CustomFileName fileName;
            [SerializeField]
            bool deleteOriginals;

            public ArchiveSettings(CustomFileName fileName, bool includeParentFolder)
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
        }

        [Header("Common Settings")]
        [SerializeField]
        protected CustomFileName fileName = new CustomFileName();
        [SerializeField]
        protected CustomFileName folderName = new CustomFileName();
        [SerializeField]
        protected Architecture architecture = Architecture.Build64Bit;
        [SerializeField]
        protected CompressionType compression = CompressionType.Default;
        [SerializeField]
        protected bool enableStrictMode = false;

        [Header("Scene Settings")]
        [SerializeField]
        protected SceneSettings customScenes;

        [Header("Archive Settings")]
        [SerializeField]
        protected ArchiveSettings archiveSettings = new ArchiveSettings(new CustomFileName(), true);

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
            BuildPlayerOptions options = GetPlayerOptions(results);

            // Build the player
            BuildReport res = BuildPipeline.BuildPlayer(options);

            // Add the latest results
            results.AddReport(res, this);

            // Consider making a post build
            if (results.LastReport.State == BuildPlayersResult.Status.Success)
            {
                if (archiveSettings.IsEnabled == true)
                {
                    if (ArchiveBuild(results) == true)
                    {
                        results.AddPostBuildReport(BuildPlayersResult.Status.Success, results.Concatenate("Successfully archived: ", name), this);
                    }
                    else
                    {
                        results.AddPostBuildReport(BuildPlayersResult.Status.Success, results.Concatenate("Failed to archive: ", name), this);
                    }
                }

                RenameBuild(options, results);
            }
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

        protected abstract BuildOptions Options
        {
            get;
        }

        protected virtual void RenameBuild(BuildPlayerOptions options, BuildPlayersResult results)
        {
            string newFolderName = results.ConcatenateFolders(results.FolderName, folderName.ToString());
            FileUtil.MoveFileOrDirectory(options.locationPathName, newFolderName);
        }

        protected virtual bool ArchiveBuild(BuildPlayersResult results)
        {
            // FIXME: to ZIP the folder that's generated
            throw new System.NotImplementedException();
        }

        private BuildPlayerOptions GetPlayerOptions(BuildPlayersResult results)
        {
            // Setup all options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

            // Update the location to build this player
            buildPlayerOptions.locationPathName = results.ConcatenateFolders(results.FolderName, fileName.ToString());

            // Update the scenes to build
            if (customScenes.IsEnabled == true)
            {
                buildPlayerOptions.scenes = customScenes.Paths;
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
