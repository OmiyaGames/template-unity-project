/// Comment or uncomment the preprocessor directives below
/// to adjust what "Build All" context menu will build

// FIXME: consider moving preprocessor directives into PlayerPrefs, and have a window specific to editing that.
#define BUILD_TO_MAJOR_DESKTOP_OS
//#define BUILD_32_BIT_AND_64_BIT_SEPARATELY
#define SPRITE_PACK_MAJOR_DESKTOP_OS

//#define BUILD_TO_MAJOR_MOBILE_OS
#define SPRITE_PACK_MAJOR_MOBILE_OS

#define BUILD_TO_WEBGL
#define SPRITE_PACK_WEBGL

//#define BUILD_TO_FACEBOOK_WINDOWS
//#define BUILD_TO_FACEBOOK_WEBGL
#define SPRITE_PACK_FACEBOOK

#define APPEND_DATE_IN_FOLDER_NAME
//#define APPEND_VERSION_IN_FOLDER_NAME

#define STOP_REST_OF_THE_BUILDS_IF_ANY_FAIL

using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OmiyaGamesBuildScript.cs" company="Omiya Games">
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
    /// <date>10/29/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Script that builds for a specific platform.  It even adds several menu items
    /// under "Build" in the file menu bar to quickly build to a different
    /// platform.
    /// 
    /// Also useful for continuous integration, like Jenkins.
    /// Use the Unity command feature to run this script, such as:
    /// <code>unity -batchmode -quit -executeMethod OmiyaGamesBuildScript.BuildAllPlatforms</code>
    /// </summary>
    public class Build
    {
        private struct BuildInfo
        {
            readonly public string platformName;
            readonly public string fileExtension;

            public BuildInfo(string platform, string extension)
            {
                platformName = platform;
                fileExtension = extension;
            }
        }

        private struct BuildSet
        {
            readonly public BuildTargetGroup targetGroup;
            readonly public BuildTarget buildTarget;

            public BuildSet(BuildTargetGroup group, BuildTarget target)
            {
                targetGroup = group;
                buildTarget = target;
            }

            public BuildSet(BuildTarget target) :
                this(GetDefaultBuildTarget(target), target)
            { }
        }

        /// <summary>
        /// Format for data string when appended to the folder.
        /// </summary>
        // FIXME: consider moving this to PlayerPrefs, and have a window specific to editing this setting
        public const string DateFormat = "yyyy.MM.dd-HH.mm.ss";
        /// <summary>
        /// The build option for every platform. Feel free to edit this variable.
        /// </summary>
        // FIXME: consider moving this to PlayerPrefs, and have a window specific to editing this setting
        public const BuildOptions OptionsAll = BuildOptions.None;
        /// <summary>
        /// The build option for WebGL. Feel free to edit this variable.
        /// </summary>
        // FIXME: consider moving this to PlayerPrefs, and have a window specific to editing this setting
        public const BuildOptions OptionsWeb = BuildOptions.None;
        /// <summary>
        /// The folder where the game will be built to. If this string is either null or empty, a save folder dialog will pop-up instead. Feel free to edit this variable.
        /// </summary>
        // FIXME: consider moving this to PlayerPrefs, and have a window specific to editing this setting
        public const string BuildDirectory = null;
        /// <summary>
        /// All the supported build targets, and their information. Feel free to edit this variable.
        /// </summary>
        #region All Build Info
        private static readonly Dictionary<BuildTarget, BuildInfo> AllBuildInfo = new Dictionary<BuildTarget, BuildInfo>()
        {
            // Web platforms
            { BuildTarget.WebGL, new BuildInfo("WebGL", "") },

            // Windows platform
            { BuildTarget.StandaloneWindows, new BuildInfo("Windows 32-bit", ".exe") },
            { BuildTarget.StandaloneWindows64, new BuildInfo("Windows 64-bit", ".exe") },

            // Mac platform
            { BuildTarget.StandaloneOSXUniversal, new BuildInfo("Mac", ".app") },
            { BuildTarget.StandaloneOSXIntel, new BuildInfo("Mac 32-bit", ".app") },
            { BuildTarget.StandaloneOSXIntel64, new BuildInfo("Mac 64-bit", ".app") },

            // Linux platform
            { BuildTarget.StandaloneLinuxUniversal, new BuildInfo("Linux", "") },
            { BuildTarget.StandaloneLinux, new BuildInfo("Linux 32-bit", "") },
            { BuildTarget.StandaloneLinux64, new BuildInfo("Linux 64-bit", "") },

            // Mobile platform
            { BuildTarget.iOS, new BuildInfo("iOS", "") },
            { BuildTarget.Android, new BuildInfo("Android", ".apk") },
            // FIMXE: add Tizen platform in the future!
            //{ BuildTarget.Tizen, new BuildInfo("Tizen", ".tpk") },
            // FIXME: add WSA platform in the future!
            //{ BuildTarget.WSAPlayer, new BuildInfo("WSA", ".ps1") },

            // TV platforms
            // FIMXE: add tvOS platform in the future!
            //{ BuildTarget.tvOS, new BuildInfo("tvOS", "") },
            // FIMXE: add Samsung TV platform in the future!
            //{ BuildTarget.SamsungTV, new BuildInfo("Samsung TV", ".apk") },
        };
        #endregion
        // FIXME: consider moving this to PlayerPrefs, and have a window specific to editing this setting
        #region All Desktop Targets
        private static readonly HashSet<BuildSet> AllDesktopTargets = new HashSet<BuildSet>()
        {
            new BuildSet(BuildTarget.StandaloneWindows),
            new BuildSet(BuildTarget.StandaloneWindows64),
#if BUILD_32_BIT_AND_64_BIT_SEPARATELY
            new BuildSet(BuildTarget.StandaloneOSXIntel),
            new BuildSet(BuildTarget.StandaloneOSXIntel64),
            new BuildSet(BuildTarget.StandaloneLinux),
            new BuildSet(BuildTarget.StandaloneLinux64)
#else
            new BuildSet(BuildTarget.StandaloneOSXUniversal),
            new BuildSet(BuildTarget.StandaloneLinuxUniversal)
#endif
        };
        #endregion
        // FIXME: consider moving this to PlayerPrefs, and have a window specific to editing this setting
        #region All Mobile Targets
        private static readonly HashSet<BuildSet> AllMobileTargets = new HashSet<BuildSet>()
        {
            new BuildSet(BuildTarget.iOS),
            new BuildSet(BuildTarget.Android),
            // FIMXE: add Tizen platform in the future!
            //new BuildSet(BuildTarget.Tizen),
            // FIXME: add WSA platform in the future!
            //new BuildSet(BuildTarget.WSAPlayer)
        };
        #endregion
        // FIXME: consider moving this to PlayerPrefs, and have a window specific to editing this setting
        //private static readonly HashSet<BuildSet> AllTvTargets = new HashSet<BuildSet>()
        //{
        //    new BuildSet(BuildTarget.tvOS),
        //    new BuildSet(BuildTarget.SamsungTV)
        //};

        #region Constants and Read-Onlys
        /// <summary>
        /// The folder where the game will be built to. If this string is either null or empty, a save folder dialog will pop-up instead. Feel free to change this variable.
        /// </summary>
        private const string BuildDirectoryKey = "OmiyaGames.StoredBuildDirectory";
        /// <summary>
        /// All the build targets for this single session
        /// </summary>
        private static readonly List<BuildSet> allBuildTargets = new List<BuildSet>();
        /// <summary>
        /// Cached string builder, useful for generating file names.
        /// </summary>
        private static readonly StringBuilder FileNameGenerator = new StringBuilder();
        /// <summary>
        /// Regular expression to detect /, \, :, *, ?, ", <, >, and |.
        /// </summary>
        private static readonly Regex InvalidFileNameCharacters = new Regex("[.\\\\/:*?\"<>|]");
        /// <summary>
        /// All scenes enabled in the build settings, in order.
        /// </summary>
        private static readonly string[] AllScenes = FindEnabledEditorScenes();
        /// <summary>
        /// The maximum WebGL build name
        /// </summary>
        public const int MaxSlugLength = 45;
        /// <summary>
        /// Common build directory
        /// </summary>
        private static string buildDirectory = null;
        #endregion

        /// <summary>
        /// Function that builds for all platforms.  Edit this function if you want
        /// to add more platforms besides PC, Mac, Linux, and Web.
        /// </summary>
        [MenuItem("Build/Build All")]
        public static void BuildAllPlatforms()
        {
            allBuildTargets.Clear();
#if BUILD_TO_MAJOR_DESKTOP_OS
            allBuildTargets.AddRange(AllDesktopTargets);
#endif
#if BUILD_TO_MAJOR_MOBILE_OS
            allBuildTargets.AddRange(AllMobileTargets);
#endif
#if BUILD_TO_WEBGL
            allBuildTargets.Add(new BuildSet(BuildTarget.WebGL));
#endif
#if BUILD_TO_FACEBOOK_WINDOWS
            allBuildTargets.Add(new BuildSet(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows));
#endif
#if BUILD_TO_FACEBOOK_WEBGL
            allBuildTargets.Add(new BuildSet(BuildTargetGroup.Facebook, BuildTarget.WebGL));
#endif
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Desktop OSs.
        /// </summary>
        [MenuItem("Build/Build Set/Major Desktop OSs")]
        public static void PerformDesktopBuilds()
        {
            allBuildTargets.Clear();
            allBuildTargets.AddRange(AllDesktopTargets);
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Mobile OSs.
        /// </summary>
        [MenuItem("Build/Build Set/Major Mobile OSs")]
        public static void PerformMobileBuilds()
        {
            allBuildTargets.Clear();
            allBuildTargets.AddRange(AllMobileTargets);
            BuildAll();
        }

        #region Platform-Specific Build Functions
        /// <summary>
        /// Function that builds for Web.
        /// </summary>
        [MenuItem("Build/Build For/WebGL")]
        public static void PerformWebGLBuild()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.WebGL));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Windows, 32-bit.
        /// </summary>
        [MenuItem("Build/Build For/Windows 32-bit")]
        public static void PerformWindows32Build()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.StandaloneWindows));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Windows, 64-bit.
        /// </summary>
        [MenuItem("Build/Build For/Windows 64-bit")]
        public static void PerformWindows64Build()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.StandaloneWindows64));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Mac.
        /// </summary>
        [MenuItem("Build/Build For/Mac (Universal)")]
        public static void PerformMacUniversalBuild()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.StandaloneOSXUniversal));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Mac, 32-bit.
        /// </summary>
        [MenuItem("Build/Build For/Mac 32-bit")]
        public static void PerformMac32Build()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.StandaloneOSXIntel));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Mac, 64-bit.
        /// </summary>
        [MenuItem("Build/Build For/Mac 64-bit")]
        public static void PerformMac64Build()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.StandaloneOSXIntel64));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Linux.
        /// </summary>
        [MenuItem("Build/Build For/Linux (Universal)")]
        public static void PerformLinuxUniversalBuild()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.StandaloneLinuxUniversal));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Linux, 32-bit.
        /// </summary>
        [MenuItem("Build/Build For/Linux 32-bit")]
        public static void PerformLinux32Build()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.StandaloneLinux));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Linux, 64-bit.
        /// </summary>
        [MenuItem("Build/Build For/Linux 64-bit")]
        public static void PerformLinux64Build()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.StandaloneLinux64));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for iOS.
        /// </summary>
        [MenuItem("Build/Build For/iOS")]
        public static void PerformIosBuild()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.iOS));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Android.
        /// </summary>
        [MenuItem("Build/Build For/Android")]
        public static void PerformAndroidBuild()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTarget.Android));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Windows Store Apps
        /// </summary>
        //[MenuItem("Build/Build For/Windows Store App")]
        //public static void PerformWsaBuild()
        //{
        //    allBuildTargets.Clear();
        //    allBuildTargets.Add(new BuildSet(BuildTarget.WSAPlayer));
        //    BuildAll();
        //}

        /// <summary>
        /// Function that builds for Web.
        /// </summary>
        [MenuItem("Build/Build For/Facebook WebGL")]
        public static void PerformFacebookWebGLBuild()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTargetGroup.Facebook, BuildTarget.WebGL));
            BuildAll();
        }

        /// <summary>
        /// Function that builds for Windows, 32-bit.
        /// </summary>
        [MenuItem("Build/Build For/Facebook Windows 32-bit")]
        public static void PerformFacebookWindows32Build()
        {
            allBuildTargets.Clear();
            allBuildTargets.Add(new BuildSet(BuildTargetGroup.Facebook, BuildTarget.StandaloneWindows));
            BuildAll();
        }
        #endregion

        [MenuItem("Build/Open Last Builds Folder")]
        public static void OpenBuildsFolder()
        {
            if (string.IsNullOrEmpty(BuildDirectory) == true)
            {
                EditorUtility.RevealInFinder(SavedBuildDirectory);
            }
            else
            {
                EditorUtility.RevealInFinder(BuildDirectory);
            }
        }

        public static bool AndroidCredentialsFilled
        {
            get
            {
                // By default, return true
                bool returnFlag = true;

                // Check if there's an Android keystore name
                if (string.IsNullOrEmpty(PlayerSettings.Android.keystoreName) == false)
                {
                    // If so, by default, return false
                    returnFlag = false;

                    // Make sure all the passwords are filled in
                    if ((string.IsNullOrEmpty(PlayerSettings.keystorePass) == false) && (string.IsNullOrEmpty(PlayerSettings.keyaliasPass) == false))
                    {
                        // We're going to assume it's all good to go!
                        returnFlag = true;
                    }
                }
                return returnFlag;
            }
        }

        #region Helper Methods
        private static string SavedBuildDirectory
        {
            get
            {
                return PlayerPrefs.GetString(BuildDirectoryKey, Path.Combine(Application.dataPath, ".."));
            }
        }

        private static bool IsSpritePackingSupported(BuildTarget target)
        {
            bool returnFlag = false;
#if SPRITE_PACK_MAJOR_DESKTOP_OS
            foreach(BuildSet set in AllDesktopTargets)
            {
                if (set.buildTarget == target)
                {
                    returnFlag = true;
                    break;
                }
            }
#endif
#if SPRITE_PACK_MAJOR_MOBILE_OS
            foreach (BuildSet set in AllMobileTargets)
            {
                if (set.buildTarget == target)
                {
                    returnFlag = true;
                    break;
                }
            }
#endif
#if SPRITE_PACK_WEBGL
            if (target == BuildTarget.WebGL)
            {
                returnFlag = true;
            }
#endif
            return returnFlag;
        }

        private static void BuildAll(AndroidKeystoreCredentialsWindow window)
        {
            if ((allBuildTargets.Count > 0) && (string.IsNullOrEmpty(buildDirectory) == false))
            {
                // Check to see if the current active build target is in the list
                int activeTargetIndex = 0;
                for(int index = 0; index < allBuildTargets.Count; ++index)
                {
                    // Make sure current group settings are correct
                    if((allBuildTargets[index].buildTarget == EditorUserBuildSettings.activeBuildTarget) &&
                        (allBuildTargets[index].targetGroup == EditorUserBuildSettings.selectedBuildTargetGroup))
                    {
                        activeTargetIndex = index;
                        break;
                    }
                }
                if (activeTargetIndex > 0)
                {
                    // If the current active target is found, and not as the first element
                    // Move the target to the beginning of the list
                    BuildSet swapSet = allBuildTargets[activeTargetIndex];
                    allBuildTargets.RemoveAt(activeTargetIndex);
                    allBuildTargets.Insert(0, swapSet);
                }

                // Go through each element of the list
                bool enableSpritePacking = false;
                BuildInfo info;
                StringBuilder allTargets = new StringBuilder();
                allTargets.Append("Build Status:");
                foreach (BuildSet set in allBuildTargets)
                {
                    // Indicate the target we're building to
                    allTargets.AppendLine();
                    allTargets.Append(set);
                    try
                    {
                        // Make sure the build target is supported
                        if(AllBuildInfo.TryGetValue(set.buildTarget, out info) == true)
                        {
                            // Build the game
                            enableSpritePacking = IsSpritePackingSupported(set.buildTarget);
                            GenericBuild(buildDirectory, info.platformName, info.fileExtension, set.targetGroup, set.buildTarget, enableSpritePacking);
                            allTargets.Append(": Success!");
                        }
                        else
                        {
                            // Otherwise, indicate we're unsupported
                            allTargets.Append(": Unsupported...");
                        }
                    }
                    catch (Exception ex)
                    {
                        allTargets.Append(": Failed...");
                        Debug.LogError(ex.Message);

                        // If the user indicated so, stop the rest of the builds due to this 
#if STOP_REST_OF_THE_BUILDS_IF_ANY_FAIL
                        break;
#endif
                    }
                }

                // Open the folder where the builds were made
                OpenBuildsFolder();
            }
        }

        private static void BuildAll()
        {
            // Make sure the build targets aren't empty
            if (allBuildTargets.Count > 0)
            {
                // Grab the build directory
                buildDirectory = BuildDirectory;

                // Check if we should open the save folder dialog
                if (string.IsNullOrEmpty(buildDirectory) == true)
                {
                    // Open the save folder dialog
                    buildDirectory = EditorUtility.SaveFolderPanel("Build project to folder", SavedBuildDirectory, "");

                    // Check if the user selected a folder
                    if (string.IsNullOrEmpty(buildDirectory) == false)
                    {
                        // Store this folder
                        PlayerPrefs.SetString(BuildDirectoryKey, buildDirectory);
                    }
                    else
                    {
                        // If not, cancel this operation entirely
                        return;
                    }
                }

                // If one of these targets are Androids, make sure it has all the credentials filled in
                if ((AndroidCredentialsFilled == false) && (Contains(allBuildTargets, BuildTarget.Android) == true))
                {
                    // If not, prompt the user to fill in the Android credentials
                    AndroidKeystoreCredentialsWindow.Display(BuildAll);
                }
                else
                {
                    // Otherwise, just build to all the platforms listed in allBuildTargets
                    BuildAll(null);
                }
            }
        }

        private static bool Contains(List<BuildSet> allBuildTargets, BuildTarget target)
        {
            bool returnFlag = false;
            foreach(BuildSet set in allBuildTargets)
            {
                if(set.buildTarget == target)
                {
                    returnFlag = true;
                    break;
                }
            }
            return returnFlag;
        }

        /// <summary>
        /// Helper function that generates a build using a file name based off of the file extension.
        /// </summary>
        private static void GenericBuild(string buildDirectory, string platformName, string fileExtension, BuildTargetGroup targetGroup, BuildTarget buildTarget, bool enableSpritePacking)
        {
            // Reset the file name
            FileNameGenerator.Length = 0;

            // Generate the folder for this platform, if there isn't one already
            string sanitizedProductName;
            AppendFolderName(FileNameGenerator, buildDirectory, platformName, targetGroup, out sanitizedProductName);
            Directory.CreateDirectory(FileNameGenerator.ToString());

            // Add file name
            AppendFileName(FileNameGenerator, fileExtension, buildTarget, sanitizedProductName);

            // Change the sprite packing settings
            SpritePackerMode lastMode = EditorSettings.spritePackerMode;
            if (enableSpritePacking == true)
            {
                // Turn on sprite packing on build time
                EditorSettings.spritePackerMode = SpritePackerMode.BuildTimeOnly;
            }
            else
            {
                // Turn off sprite packing on build time
                EditorSettings.spritePackerMode = SpritePackerMode.Disabled;
            }

            try
            {
                // Generate the build
                GenericBuild(FileNameGenerator.ToString(), targetGroup, buildTarget);

                // Printing where the build was created
                FileNameGenerator.Insert(0, "Created build to: ");
                Debug.Log(FileNameGenerator.ToString());
            }
            finally
            {
                // Revert the sprite mode
                EditorSettings.spritePackerMode = lastMode;
            }
        }

        /// <summary>
        /// Helper function that generates a build.
        /// </summary>
        private static BuildTargetGroup GetDefaultBuildTarget(BuildTarget buildTarget)
        {
            BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    targetGroup = BuildTargetGroup.Standalone;
                    break;
                case BuildTarget.Android:
                    targetGroup = BuildTargetGroup.Android;
                    break;
                case BuildTarget.iOS:
                    targetGroup = BuildTargetGroup.iOS;
                    break;
                case BuildTarget.WebGL:
                    targetGroup = BuildTargetGroup.WebGL;
                    break;
                case BuildTarget.WSAPlayer:
                    targetGroup = BuildTargetGroup.WSA;
                    break;
            }
            return targetGroup;
        }

        /// <summary>
        /// Helper function that generates a build.
        /// </summary>
        private static void GenericBuild(string targetDirectory, BuildTargetGroup targetGroup, BuildTarget buildTarget)
        {
            // Import assets for this platform
            if ((EditorUserBuildSettings.activeBuildTarget == buildTarget) || (EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget) == true))
            {
                // Determine the best build option
                BuildOptions buildOption = OptionsAll;
                if (buildTarget == BuildTarget.WebGL)
                {
                    buildOption |= OptionsWeb;
                }

                // Setup all options
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions.scenes = AllScenes;
                buildPlayerOptions.locationPathName = targetDirectory;
                buildPlayerOptions.target = buildTarget;
                buildPlayerOptions.options = buildOption;
                
                // Build everything based on the options
                string res = BuildPipeline.BuildPlayer(buildPlayerOptions);
                if (res.Length > 0)
                {
                    throw new Exception("Failed to build to " + targetDirectory + ":\n" + res);
                }
            }
        }

        /// <summary>
        /// Returns a list of scenes that are enabled in the build settings.
        /// </summary>
        private static string[] FindEnabledEditorScenes()
        {
            // Grab all enabled scenes
            List<string> EditorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled == true)
                {
                    EditorScenes.Add(scene.path);
                }
            }
            return EditorScenes.ToArray();
        }

        private static void AppendFolderName(StringBuilder appendTo, string buildDirectory, string platformName, BuildTargetGroup targetGroup, out string sanitizedProductName)
        {
            // Sanitize the product name
            sanitizedProductName = InvalidFileNameCharacters.Replace(RemoveDiacritics(PlayerSettings.productName), "");
            if (string.IsNullOrEmpty(sanitizedProductName) == true)
            {
                throw new Exception("Product name is not available!");
            }

            // Append the build directory
            appendTo.Append(buildDirectory);
            appendTo.Append(Path.DirectorySeparatorChar);

            // Append the sanitized product name
            appendTo.Append(sanitizedProductName);

#if APPEND_VERSION_IN_FOLDER_NAME
            // Append the version of this application
            appendTo.Append(" v");
            appendTo.Append(Application.version);
#endif
#if APPEND_DATE_IN_FOLDER_NAME
            // Append the date and time
            appendTo.Append(' ');
            appendTo.Append(DateTime.Now.ToString(DateFormat));
#endif

            // Append the platform name
            appendTo.Append(" (");
            appendTo.Append(platformName);

            // Append if it's for any platform
            switch (targetGroup)
            {
                case BuildTargetGroup.Facebook:
                    appendTo.Append(" for Facebook");
                    break;
            }
            appendTo.Append(')');
        }

        private static void AppendFileName(StringBuilder appendTo, string fileExtension, BuildTarget buildTarget, string sanitizedProductName)
        {
            if (buildTarget == BuildTarget.WebGL)
            {
                // Append the slugged product name
                appendTo.Append(Path.DirectorySeparatorChar);
                appendTo.Append(GenerateSlug(sanitizedProductName));
            }
            else
            {
                // Append the sanitized product name
                appendTo.Append(Path.DirectorySeparatorChar);
                appendTo.Append(sanitizedProductName);

                // Append the file extension, if available
                if (string.IsNullOrEmpty(fileExtension) == false)
                {
                    appendTo.Append(fileExtension);
                }
            }
        }

        /// <summary>
        /// Taken from http://predicatet.blogspot.com/2009/04/improved-c-slug-generator-or-how-to.html
        /// </summary>
        public static string GenerateSlug(string originalString)
        {
            // Remove invalid chars
            string returnSlug = Regex.Replace(originalString.ToLower(), @"[^a-z0-9\s-]", "");

            // Convert multiple spaces into one space
            returnSlug = Regex.Replace(returnSlug, @"\s+", " ").Trim();

            // Trim the length of the slug down to MaxSlugLength characters
            if (returnSlug.Length > MaxSlugLength)
            {
                returnSlug = returnSlug.Substring(0, MaxSlugLength).Trim();
            }

            // Replace spaces with hyphens
            returnSlug = Regex.Replace(returnSlug, @"\s", "-");

            return returnSlug;
        }

        /// <summary>
        /// Taken from http://archives.miloush.net/michkap/archive/2007/05/14/2629747.html
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int index = 0; index < normalizedString.Length; ++index)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(normalizedString[index]);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(normalizedString[index]);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        #endregion
    }
}
