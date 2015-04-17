using UnityEngine;
using System.Collections;

public class GameSettings : ISingletonScript
{
    public const int MenuLevel = 0;
    public const int DefaultNumLevelsUnlocked = 1;

    public const float DefaultMusicVolume = 1;
    public const float DefaultSoundVolume = 1;

    public const string NumLevelsUnlockedKey = "Number of Unlocked Levels";
    public const string MusicVolumeKey = "Music Volume";
    public const string SoundVolumeKey = "Sound Volume";

    [SerializeField]
    bool simulateWebplayer = false;
    [SerializeField]
    [ReadOnly]
    string[] levelNames;

    int numLevelsUnlocked = 1;
    float musicVolume = 0, soundVolume = 0;

    #region Properties
    public bool IsWebplayer
    {
        get
        {
            return (simulateWebplayer == true) || (Application.isWebPlayer == true);
        }
    }

    public int NumLevels
    {
        get
        {
            return levelNames.Length;
        }
    }

    public string[] LevelNames
    {
        get
        {
            return levelNames;
        }
    }

    public int NumLevelsUnlocked
    {
        get
        {
            return numLevelsUnlocked;
        }
        internal set
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
    #endregion

    public override void SingletonStart(Singleton instance)
    {
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

        // Grab the music volume
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);

        // Grab the sound volume
        soundVolume = PlayerPrefs.GetFloat(SoundVolumeKey, DefaultSoundVolume);

        // NOTE: Feel free to add more stuff here

    }

    public void SaveSettings()
    {
        // Save the number of levels unlocked
        PlayerPrefs.SetInt(NumLevelsUnlockedKey, NumLevelsUnlocked);

        // Save the music volume
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);

        // Save the sound volume
        PlayerPrefs.SetFloat(SoundVolumeKey, soundVolume);

        // NOTE: Feel free to add more stuff here

        PlayerPrefs.Save();
    }

    public void ClearSettings()
    {
        PlayerPrefs.DeleteAll();
        RetrieveFromSettings();
    }

    public string GetLevelName(int levelIndex)
    {
        string returnString = "Menu";
        if(levelIndex > 0)
        {
            if (levelIndex <= NumLevels)
            {
                returnString = levelNames[levelIndex - 1];
            }
            else
            {
                returnString = "Level " + levelIndex;
            }
        }
        return returnString;
    }
}
