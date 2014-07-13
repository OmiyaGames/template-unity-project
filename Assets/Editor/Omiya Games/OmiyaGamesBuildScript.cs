using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
	
	[MenuItem ("Omiya Games/Build All")]
	public static void BuildAllPlatforms()
	{
		PerformWebBuild();

		PerformWindows32Build();
		PerformWindows64Build();

		PerformMac32Build();
		PerformMac64Build();

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

		//PerformAndroidBuild();
	}
	
	[MenuItem ("Omiya Games/Build For/Web")]
	public static void PerformWebBuild()
	{
		GenericBuild("Web", "", BuildTarget.WebPlayer);
	}
	
	[MenuItem ("Omiya Games/Build For/Windows 32-bit")]
	public static void PerformWindows32Build()
	{
		GenericBuild("Windows 32-bit", ".exe", BuildTarget.StandaloneWindows);
	}
	
	[MenuItem ("Omiya Games/Build For/Windows 64-bit")]
	public static void PerformWindows64Build()
	{
		GenericBuild("Windows 64-bit", ".exe", BuildTarget.StandaloneWindows64);
	}
	
	[MenuItem ("Omiya Games/Build For/Mac 32-bit")]
	public static void PerformMac32Build()
	{
		GenericBuild("Mac 32-bit", ".app", BuildTarget.StandaloneOSXIntel);
	}
	
	[MenuItem ("Omiya Games/Build For/Mac 64-bit")]
	public static void PerformMac64Build()
	{
		GenericBuild("Mac 64-bit", ".app", BuildTarget.StandaloneOSXIntel64);
	}
	
	[MenuItem ("Omiya Games/Build For/Linux 32-bit")]
	public static void PerformLinux32Build()
	{
		GenericBuild("Linux 32-bit", "", BuildTarget.StandaloneLinux);
	}
	
	[MenuItem ("Omiya Games/Build For/Linux 64-bit")]
	public static void PerformLinux64Build()
	{
		GenericBuild("Linux 64-bit", "", BuildTarget.StandaloneLinux64);
	}
	
	[MenuItem ("Omiya Games/Build For/iOS")]
	public static void PerformIosBuild()
	{
		GenericBuild("iOS", "", BuildTarget.iPhone);
	}
	
	[MenuItem ("Omiya Games/Build For/Android")]
	public static void PerformAndroidBuild()
	{
		GenericBuild("Android", ".apk", BuildTarget.Android);
	}
	
	[MenuItem ("Omiya Games/Build For/Windows 8")]
	public static void PerformWp8Build()
	{
		GenericBuild("Windows 8", "", BuildTarget.WP8Player);
	}
	
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
