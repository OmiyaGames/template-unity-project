
// This code requires Unity Pro license.
#if UNITY_PRO_LICENSE

using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// Script that builds for a specific platform.
/// Useful for continuous integration, like Jenkins.
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
	private const BuildOptions OptionsWeb = BuildOptions.BuildAdditionalStreamedScenes;
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
		// Build for the Web platform
		PerformWebBuild();

		// Build for the Windows platform
		PerformWindows32Build();
		PerformWindows64Build();

		// Build for the Mac platform
		PerformMac32Build();
		PerformMac64Build();

		// Build for the Linux platform
		PerformLinux32Build();
		PerformLinux64Build();

		// Check the editor's platform
		if(Application.platform == RuntimePlatform.OSXEditor)
		{
			// If on a Mac, build an iOS XCode project
			//PerformIosBuild();
		}
		else if(Application.platform == RuntimePlatform.WindowsEditor)
		{
			// If on Windows 8, build a Windows 8 Visual Studio 2012 project
			//PerformWp8Build();
		}

		// Build for the Android platform
		//PerformAndroidBuild();
	}
	
	/// <summary>
	/// Function that builds for Web.  HTML page not included.
	/// </summary>
	[MenuItem ("Omiya Games/Build For/Web")]
	public static void PerformWebBuild()
	{
		GenericBuild("Web", ".unity3d", BuildTarget.WebPlayer);
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
	/// Function that builds for Mac, 32-bit.
	/// </summary>
	[MenuItem ("Omiya Games/Build For/Mac 32-bit")]
	public static void PerformMac32Build()
	{
		GenericBuild("Mac 32-bit", ".app", BuildTarget.StandaloneOSXIntel);
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
		GenericBuild("iOS", "", BuildTarget.iPhone);
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
		FileNameGenerator.Append(")\\");

		// Append the sanitized product name
		FileNameGenerator.Append(sanitizedProductName);

		// Append the file extension, if available
		if(string.IsNullOrEmpty(fileExtension) == false)
		{
			FileNameGenerator.Append(fileExtension);
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
#endif
