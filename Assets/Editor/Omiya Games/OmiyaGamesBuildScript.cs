/*
 * Comment or uncomment the preprocessor directives below
 * to adjust what "Build All" context menu will build
*/
#define BUILD_TO_MAJOR_DESKTOP_OS
//#define BUILD_32_BIT_AND_64_BIT_SEPARATELY

//#define BUILD_TO_MAJOR_MOBILE_OS

#define BUILD_TO_WEBGL
//#define BUILD_TO_WEBPLAYER
//#define BUILD_TO_STREAMED_WEBPLAYER

using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// Script that builds for a specific platform.
/// Useful for continuous integration, like Jenkins.
/// Use the Unity command feature to run this script, such as:
/// <code>unity -batchmode -quit -executeMethod OmiyaGamesBuildScript.BuildAllPlatforms</code>
/// 
/// This code is under the MIT license.
/// For more details, see LICENSE.txt at the root of this project.
/// </summary>
public class OmiyaGamesBuildScript
{
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
    /// The build option for every platform.
    /// </summary>
    private const BuildOptions OptionsAll = BuildOptions.None;
    /// <summary>
    /// The build option for every platform.
    /// </summary>
    private const BuildOptions OptionsWeb = BuildOptions.None;
    /// <summary>
    /// The folder where the game will be built to.
    /// </summary>
    private const string BuildDirectory = "Builds";
    
    /// <summary>
    /// Function that builds for all platforms.  Edit this function if you want
    /// to add more platforms besides PC, Mac, Linux, and Web.
    /// </summary>
    [MenuItem ("Omiya Games/Build All")]
    public static void BuildAllPlatforms()
    {
#if BUILD_TO_MAJOR_DESKTOP_OS
        // Build for Desktop platforms
		PerformDesktopBuilds();
#endif

#if BUILD_TO_MAJOR_MOBILE_OS
		// Build for Mobile platforms
		PerformMobileBuilds();
#endif

#if BUILD_TO_WEBGL
        // Build for the Web platform
        PerformWebGLBuild();
#endif

#if BUILD_TO_WEBPLAYER
        // Build for the Web platform
        PerformWebplayerBuild();
#endif
        
#if BUILD_TO_STREAMED_WEBPLAYER
        // Build for the Web platform
        PerformStreamedWebplayerBuild();
#endif
    }

	/// <summary>
	/// Function that builds for Desktop OSs.
	/// </summary>
	[MenuItem ("Omiya Games/Build Set/Major Desktop OSs")]
	public static void PerformDesktopBuilds()
	{
		// Build for the Windows platform
		PerformWindows32Build();
		PerformWindows64Build();

#if BUILD_32_BIT_AND_64_BIT_SEPARATELY
		// Build for the Mac platform
		PerformMac32Build();
		PerformMac64Build();
		
		// Build for the Linux platform
		PerformLinux32Build();
		PerformLinux64Build();
#else
		// Build for the Mac platform
		PerformMacUniversalBuild();
		
		// Build for the Linux platform
		PerformLinuxUniversalBuild();
#endif
	}

	/// <summary>
	/// Function that builds for Mobile OSs.
	/// </summary>
	[MenuItem ("Omiya Games/Build Set/Major Mobile OSs")]
	public static void PerformMobileBuilds()
	{
		// Check the editor's platform
		if(Application.platform == RuntimePlatform.OSXEditor)
		{
			// If on a Mac, build an iOS XCode project
			PerformIosBuild();
		}
		else if(Application.platform == RuntimePlatform.WindowsEditor)
		{
			// If on Windows 8, build a Windows 8 Visual Studio 2012 project
			PerformWp8Build();
		}
		
		// Build for the Android platform
		PerformAndroidBuild();
	}
    
    /// <summary>
    /// Function that builds for Web.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/WebGL")]
    public static void PerformWebGLBuild()
    {
        GenericBuild("WebGL", "", BuildTarget.WebGL);
    }
    
    /// <summary>
    /// Function that builds for Web.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Streamed Webplayer")]
    public static void PerformStreamedWebplayerBuild()
    {
        GenericBuild("Streamed Webplayer", "", BuildTarget.WebPlayerStreamed);
    }

    /// <summary>
    /// Function that builds for Web.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Webplayer")]
    public static void PerformWebplayerBuild()
    {
        GenericBuild("Webplayer", "", BuildTarget.WebPlayer);
    }
    
    /// <summary>
    /// Function that builds for Windows, 32-bit.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Windows 32-bit")]
    public static void PerformWindows32Build()
    {
        GenericBuild("Windows 32-bit", ".exe", BuildTarget.StandaloneWindows);
    }
    
    /// <summary>
    /// Function that builds for Windows, 64-bit.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Windows 64-bit")]
    public static void PerformWindows64Build()
    {
        GenericBuild("Windows 64-bit", ".exe", BuildTarget.StandaloneWindows64);
    }
    
    /// <summary>
    /// Function that builds for Mac.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Mac (Universal)")]
    public static void PerformMacUniversalBuild()
    {
        GenericBuild("Mac", ".app", BuildTarget.StandaloneOSXIntel);
    }

    /// <summary>
    /// Function that builds for Mac, 32-bit.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Mac 32-bit")]
    public static void PerformMac32Build()
    {
        GenericBuild("Mac 32-bit", ".app", BuildTarget.StandaloneOSXUniversal);
    }
    
    /// <summary>
    /// Function that builds for Mac, 64-bit.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Mac 64-bit")]
    public static void PerformMac64Build()
    {
        GenericBuild("Mac 64-bit", ".app", BuildTarget.StandaloneOSXIntel64);
    }
    
    /// <summary>
    /// Function that builds for Linux.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Linux (Universal)")]
    public static void PerformLinuxUniversalBuild()
    {
        GenericBuild("Linux", "", BuildTarget.StandaloneLinuxUniversal);
    }
    
    /// <summary>
    /// Function that builds for Linux, 32-bit.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Linux 32-bit")]
    public static void PerformLinux32Build()
    {
        GenericBuild("Linux 32-bit", "", BuildTarget.StandaloneLinux);
    }
    
    /// <summary>
    /// Function that builds for Linux, 64-bit.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Linux 64-bit")]
    public static void PerformLinux64Build()
    {
        GenericBuild("Linux 64-bit", "", BuildTarget.StandaloneLinux64);
    }
    
    /// <summary>
    /// Function that builds for iOS.  Note this function only runs on a Mac.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/iOS")]
    public static void PerformIosBuild()
    {
        GenericBuild("iOS", "", BuildTarget.iOS);
    }
    
    /// <summary>
    /// Function that builds for Android.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Android")]
    public static void PerformAndroidBuild()
    {
        GenericBuild("Android", ".apk", BuildTarget.Android);
    }
    
    /// <summary>
    /// Function that builds for Windows 8.  Note this function only runs on Windows 8.
    /// </summary>
    [MenuItem ("Omiya Games/Build For/Windows 8")]
    public static void PerformWp8Build()
    {
        GenericBuild("Windows 8", "", BuildTarget.WP8Player);
    }
    
    /// <summary>
    /// Helper function that generates a build using a file name based off of the file extension.
    /// </summary>
    private static void GenericBuild(string platformName, string fileExtension, BuildTarget buildTarget)
    {
        // Sanitize the product name
        string sanitizedProductName = InvalidFileNameCharacters.Replace(PlayerSettings.productName, "_");
        if(string.IsNullOrEmpty(sanitizedProductName) == true)
        {
            throw new Exception("Product name is not available!");
        }

        // Reset the file name
        FileNameGenerator.Length = 0;

        // Append the build directory
        FileNameGenerator.Append(BuildDirectory);
        FileNameGenerator.Append('\\');

        // Append the sanitized product name
        FileNameGenerator.Append(sanitizedProductName);

        // Append the platform name
        FileNameGenerator.Append(" (");
        FileNameGenerator.Append(platformName);
        FileNameGenerator.Append(')');

        switch(buildTarget)
        {
            case BuildTarget.WebPlayer:
            case BuildTarget.WebPlayerStreamed:
                // Append the file extension, if available
                if(string.IsNullOrEmpty(fileExtension) == false)
                {
                    FileNameGenerator.Append(fileExtension);
                }
                break;
            default:
                // Append the sanitized product name
                FileNameGenerator.Append('\\');
                FileNameGenerator.Append(sanitizedProductName);

                // Append the file extension, if available
                if(string.IsNullOrEmpty(fileExtension) == false)
                {
                    FileNameGenerator.Append(fileExtension);
                }
                break;
        }

        // Generate the build
        GenericBuild(FileNameGenerator.ToString(), buildTarget);
    }

    /// <summary>
    /// Helper function that generates a build.
    /// </summary>
    private static void GenericBuild(string targetDirectory, BuildTarget buildTarget)
    {
        // Import assets to this platform
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);

        // Determine the best build option
        BuildOptions buildOption = OptionsAll;
        if(buildTarget == BuildTarget.WebPlayer)
        {
            buildOption |= OptionsWeb;
        }

        // Build everything based on the options
        string res = BuildPipeline.BuildPlayer(AllScenes, targetDirectory, buildTarget, buildOption);
        if (res.Length > 0)
        {
            throw new Exception("Failed to build to " + targetDirectory + ":\n" + res);
        }
    }
    
    /// <summary>
    /// Returns a list of scenes that are enabled in the build settings.
    /// </summary>
    private static string[] FindEnabledEditorScenes()
    {
        // Grab all enabled scenes
        List<string> EditorScenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled == true)
            {
                EditorScenes.Add(scene.path);
            }
        }
        return EditorScenes.ToArray();
    }
}
