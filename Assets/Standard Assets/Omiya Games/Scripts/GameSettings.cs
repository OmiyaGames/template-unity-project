using UnityEngine;
using System.Collections;

public class GameSettings : ISingletonScript
{
    public const int DefaultNumLevelsUnlocked = 1;

    public const float DefaultMusicVolume = 1;
    public const float DefaultSoundVolume = 1;

    public const string NumLevelsUnlockedKey = "Number of Unlocked Levels";
    public const string MusicVolumeKey = "Music Volume";
    public const string MusicMutedKey = "Music Muted";
    public const string SoundVolumeKey = "Sound Volume";
    public const string SoundMutedKey = "Sound Muted";

    [System.Serializable]
    public class LevelInfo
    {
        [SerializeField]
        string sceneName;
        [SerializeField]
        string displayName;

        internal int ordinal;

        public LevelInfo(string scene, string display)
        {
            sceneName = scene;
            displayName = display;
        }

        public string SceneName
        {
            get
            {
                return sceneName;
            }
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public int Ordinal
        {
            get
            {
                return ordinal;
            }
        }
    }

    [SerializeField]
    bool simulateWebplayer = false;
    [SerializeField]
    string returnToMenuText = "Return to {0}";
    [SerializeField]
    LevelInfo[] levels;

    int numLevelsUnlocked = 1;
    float musicVolume = 0, soundVolume = 0;
    bool musicMuted = false, soundMuted = false;

    #region Properties
    public bool IsWebplayer
    {
        get
        {
            bool returnIsWebplayer = false;
            if(simulateWebplayer == true)
            {
                returnIsWebplayer = true;
            }
            else
            {
                switch(Application.platform)
                {
                    case RuntimePlatform.WindowsWebPlayer:
                    case RuntimePlatform.OSXWebPlayer:
                    case RuntimePlatform.WebGLPlayer:
                        returnIsWebplayer = true;
                        break;
                }
            }
            return returnIsWebplayer;
        }
    }

    public string ReturnToMenuText
    {
        get
        {
            return returnToMenuText;
        }
    }

    public LevelInfo[] Levels
    {
        get
        {
            return levels;
        }
    }

    public int NumLevels
    {
        get
        {
            return Levels.Length;
        }
    }

    public LevelInfo CurrentLevel
    {
        get
        {
            return levels[Application.loadedLevel];
        }
    }

    public LevelInfo NextLevel
    {
        get
        {
            LevelInfo returnLevel = null;
            if ((Application.loadedLevel + 1) < NumLevels)
            {
				returnLevel = Levels[(Application.loadedLevel + 1)];
            }
            return returnLevel;
        }
    }

	/// <summary>
	/// The menu level is assumed to be the first scene.
	/// </summary>
	/// <value>The menu level.</value>
    public LevelInfo MenuLevel
    {
        get
        {
			return Levels[0];
        }
    }

	/// <summary>
	/// The credits level is assumed to be the first scene.
	/// </summary>
	/// <value>The menu level.</value>
	public LevelInfo CreditsLevel
	{
		get
		{
			return Levels[NumLevels - 1];
		}
	}

	public int NumLevelsUnlocked
    {
        get
        {
            return numLevelsUnlocked;
        }
        set
        {
            numLevelsUnlocked = Mathf.Clamp(value, 1, NumLevels);
            PlayerPrefs.SetInt(NumLevelsUnlockedKey, NumLevelsUnlocked);
        }
    }

    internal float MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            musicVolume = value;
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        }
    }

    internal bool IsMusicMuted
    {
        get
        {
            return musicMuted;
        }
        set
        {
            musicMuted = value;
            PlayerPrefs.SetInt(MusicMutedKey, (musicMuted ? 1 : 0));
        }
    }

    internal float SoundVolume
    {
        get
        {
            return soundVolume;
        }
        set
        {
            soundVolume = value;
            PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
        }
    }

    internal bool IsSoundMuted
    {
        get
        {
            return soundMuted;
        }
        set
        {
            soundMuted = value;
            PlayerPrefs.SetInt(SoundMutedKey, (soundMuted ? 1 : 0));
        }
    }
    #endregion

    public override void SingletonStart(Singleton instance)
    {
        // Update level information
        for (int index = 0; index < Levels.Length; ++index)
        {
            Levels[index].ordinal = index;
        }

        // Load settings
        RetrieveFromSettings();
    }
    
    public override void SceneStart(Singleton instance)
    {
    }

    void OnApplicationQuit()
    {
        SaveSettings();
    }

    public void RetrieveFromSettings()
    {
        // Grab the number of levels unlocked
        numLevelsUnlocked = PlayerPrefs.GetInt(NumLevelsUnlockedKey, DefaultNumLevelsUnlocked);
        numLevelsUnlocked = Mathf.Clamp(numLevelsUnlocked, 1, NumLevels);

        // Grab the music settings
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
        musicMuted = (PlayerPrefs.GetInt(MusicMutedKey, 0) != 0);

        // Grab the sound settings
        soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, DefaultSoundVolume);
        soundMuted = (PlayerPrefs.GetInt(SoundMutedKey, 0) != 0);

        // NOTE: Feel free to add more stuff here

    }

    public void SaveSettings()
    {
        // Save the number of levels unlocked
        PlayerPrefs.SetInt(NumLevelsUnlockedKey, NumLevelsUnlocked);

        // Save the music settings
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.SetInt(MusicMutedKey, (musicMuted ? 1 : 0));

        // Save the sound settings
        PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);
        PlayerPrefs.SetInt(SoundMutedKey, (soundMuted ? 1 : 0));
        
        // NOTE: Feel free to add more stuff here

        PlayerPrefs.Save();
    }

    public void ClearSettings()
    {
        PlayerPrefs.DeleteAll();
        RetrieveFromSettings();
    }

#if UNITY_EDITOR
    [ContextMenu("Setup Levels (using Level Numbers)")]
    public void SetupLevelsUsingLevelNumber()
    {
        SetupLevels("Level {0}");
    }

    [ContextMenu("Setup Levels (using Scene Names)")]
    public void SetupLevelsUsingSceneName()
    {
        SetupLevels();
    }

    [ContextMenu("Setup Levels")]
    void SetupLevels(string formatText = null)
    {
        // Create a new list
        int numScenes = UnityEditor.EditorBuildSettings.scenes.Length;
        levels = new LevelInfo[numScenes];

        // Go through each level
        UnityEditor.EditorBuildSettingsScene scene = null;
        string sceneName, displayName;
        for (int index = 0; index < numScenes; ++index)
        {
            // Grab the scene
            scene = UnityEditor.EditorBuildSettings.scenes[index];

            // Get the scene name
            sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

            // Get the display name
            displayName = GetDisplayName(index, sceneName, formatText);

            levels[index] = new LevelInfo(sceneName, displayName);
        }
    }

    static string GetDisplayName(int index, string sceneName, string formatText)
    {
        string displayName;
        if(string.IsNullOrEmpty(formatText) == true)
        {
            displayName = sceneName;
        }
        else if (index <= 0)
        {
            displayName = "Menu";
        }
        else
        {
            displayName = string.Format(formatText, index);
        }
        return displayName;
    }
#endif
}
