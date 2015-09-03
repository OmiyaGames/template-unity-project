#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using OmiyaGames;

public class AudioFinder : MonoBehaviour
{
    [Header("Search through...")]
    public GameObject[] searchThrough;

    [Header("Audio Sources")]
    public List<AudioSource> soundEffects = new List<AudioSource>();
    public List<AudioSource> backgroundMusics = new List<AudioSource>();
    public List<AudioSource> unknownSources = new List<AudioSource>();

    [Header("Audio Sources")]
    public AudioMixerGroup soundEffectsGroup = null;
    public AudioMixerGroup backgroundMusicGroup = null;

    [ContextMenu("Find all audio sources")]
    void FindAllAudioSources()
    {
        // Clear all lists
        soundEffects.Clear();
        backgroundMusics.Clear();
        unknownSources.Clear();

        // Seek for all AudioSources
        foreach(GameObject search in searchThrough)
        {
            AudioSource[] allSources = search.GetComponentsInChildren<AudioSource>(true);
            foreach(AudioSource source in allSources)
            {
                if(source.GetComponent<SoundEffect>() != null)
                {
                    soundEffects.Add(source);
                }
                else if(source.GetComponent<BackgroundMusic>() != null)
                {
                    backgroundMusics.Add(source);
                }
                else
                {
                    unknownSources.Add(source);
                }
            }
        }
    }

    [ContextMenu("Set mixer group")]
    void SetMixerGroup()
    {
        // Set the audio group
        foreach(AudioSource source in soundEffects)
        {
            source.outputAudioMixerGroup = soundEffectsGroup;
        }
        foreach(AudioSource source in backgroundMusics)
        {
            source.outputAudioMixerGroup = backgroundMusicGroup;
        }
    }
}
#endif
