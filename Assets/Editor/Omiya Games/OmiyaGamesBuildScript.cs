using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class OmiyaGamesBuildScript
{
	private static readonly string[] AllScenes = FindEnabledEditorScenes();
	private const BuildOptions Options = BuildOptions.None;
	private const string BuildDirectory = "Builds";
	
	[MenuItem ("Omiya Games/Build All")]
	public static void BuildAllPlatforms()
	{
		PerformWindows32Build();
		PerformWindows64Build();
		PerformMac32Build();
		PerformMac64Build();
		PerformLinux32Build();
		PerformLinux64Build();
	}
	
	[MenuItem ("Omiya Games/Build Web")]
	public static void PerformWebBuild()
	{
		GenericBuild(BuildDirectory + "\\Web\\" + PlayerSettings.productName, BuildTarget.WebPlayer);
	}
	
	[MenuItem ("Omiya Games/Build Windows 32")]
	public static void PerformWindows32Build()
	{
		GenericBuild(BuildDirectory + "\\Windows 32-bit\\" + PlayerSettings.productName + ".exe", BuildTarget.StandaloneWindows);
	}
	
	[MenuItem ("Omiya Games/Build Windows 64")]
	public static void PerformWindows64Build()
	{
		GenericBuild(BuildDirectory + "\\Windows 64-bit\\" + PlayerSettings.productName + ".exe", BuildTarget.StandaloneWindows64);
	}
	
	[MenuItem ("Omiya Games/Build Mac 32")]
	public static void PerformMac32Build()
	{
		GenericBuild(BuildDirectory + "\\Mac 32-bit\\" + PlayerSettings.productName + ".app", BuildTarget.StandaloneOSXIntel);
	}
	
	[MenuItem ("Omiya Games/Build Mac 64")]
	public static void PerformMac64Build()
	{
		GenericBuild(BuildDirectory + "\\Mac 64-bit\\" + PlayerSettings.productName + ".app", BuildTarget.StandaloneOSXIntel64);
	}
	
	[MenuItem ("Omiya Games/Build Linux 32")]
	public static void PerformLinux32Build()
	{
		GenericBuild(BuildDirectory + "\\Linux 32-bit\\" + PlayerSettings.productName, BuildTarget.StandaloneLinux);
	}
	
	[MenuItem ("Omiya Games/Build Linux 64")]
	public static void PerformLinux64Build()
	{
		GenericBuild(BuildDirectory + "\\Linux 64-bit\\" + PlayerSettings.productName, BuildTarget.StandaloneLinux64);
	}
	
	[MenuItem ("Omiya Games/Build iOS")]
	public static void PerformIosBuild()
	{
		GenericBuild(BuildDirectory + "\\iOS\\" + PlayerSettings.productName, BuildTarget.iPhone);
	}
	
	[MenuItem ("Omiya Games/Build Android")]
	public static void PerformAndroidBuild()
	{
		GenericBuild(BuildDirectory + "\\Android\\" + PlayerSettings.productName, BuildTarget.Android);
	}
	
	[MenuItem ("Omiya Games/Build Windows 8")]
	public static void PerformWp8Build()
	{
		GenericBuild(BuildDirectory + "\\Windows 8\\" + PlayerSettings.productName, BuildTarget.WP8Player);
	}
	
	private static void GenericBuild(string targetDirectory, BuildTarget buildTarget)
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
		string res = BuildPipeline.BuildPlayer(AllScenes, targetDirectory, buildTarget, Options);
		if (res.Length > 0)
		{
			throw new Exception("Failed to build to " + targetDirectory + ":\n" + res);
		}
	}
	
	private static string[] FindEnabledEditorScenes()
	{
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
