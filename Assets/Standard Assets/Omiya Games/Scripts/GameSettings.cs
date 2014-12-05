using UnityEngine;
using System.Collections;

public class GameSettings : ISingletonScript
{
	public const int MenuLevel = 0;
    public const int CreditsLevel = 7;
	public const int NumLevels = 7;
	
	public bool simulateWebplayer = false;

	public bool IsWebplayer
	{
		get
		{
			return (simulateWebplayer == true) || (Application.isWebPlayer == true);
		}
	}

	public int NumLevelsUnlocked
	{
		get
		{
			return NumLevels;
		}
		set
		{
		}
	}
	
	public override void SingletonStart(Singleton instance)
	{
		RetrieveFromSettings();
	}
	
    public override void SceneStart(Singleton instance)
	{
	}
	
	public void RetrieveFromSettings()
	{
	}
	
	public void SaveSettings()
	{
	}
	
	public void ClearSettings()
	{
	}
}
