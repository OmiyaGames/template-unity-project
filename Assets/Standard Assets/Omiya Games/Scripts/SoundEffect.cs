using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    static readonly HashSet<SoundEffect> allSoundEffects = new HashSet<SoundEffect>();

    float volume = 0;
    bool mute = false;

    AudioSource audioCache = null;

    public static float GlobalVolume
    {
        get
        {
            return Mathf.Clamp01(Singleton.Get<GameSettings>().SoundVolume);
        }
        set
        {
            // First, set the sound volume
            GameSettings settings = Singleton.Get<GameSettings>();
            settings.SoundVolume = Mathf.Clamp01(value);

            // Go through every instance of SoundEffect and update their settings
            foreach (SoundEffect effect in allSoundEffects)
            {
                effect.UpdateAudio(settings);
            }
        }
    }

    public static bool GlobalMute
    {
        get
        {
            return Singleton.Get<GameSettings>().IsSoundMuted;
        }
        set
        {
            // First, set the sound setting
            GameSettings settings = Singleton.Get<GameSettings>();
            settings.IsSoundMuted = value;

            // Go through every instance of SoundEffect and update their settings
            foreach(SoundEffect effect in allSoundEffects)
            {
                effect.UpdateAudio(settings);
            }
        }
    }

    public AudioSource Audio
    {
        get
        {
            if (audioCache == null)
            {
                audioCache = GetComponent<AudioSource>();
            }
            return audioCache;
        }
    }

    public bool IsMuted
    {
        get
        {
            return mute;
        }
        set
        {
            mute = value;
            UpdateAudio(Singleton.Get<GameSettings>());
        }
    }

    public float Volume
    {
        get
        {
            return volume;
        }
        set
        {
            volume = value;
            UpdateAudio(Singleton.Get<GameSettings>());
        }
    }

    void Start()
    {
        // Grab the original values
        volume = Audio.volume;
        mute = Audio.mute;

        // Calculate how the audio should behave
        allSoundEffects.Add(this);
    }

    void OnDestroy()
    {
        allSoundEffects.Remove(this);
    }

    void UpdateAudio(GameSettings settings)
    {
        // Update the volume
        Audio.volume = Mathf.Clamp01(volume * settings.SoundVolume);

        // Update mute
        Audio.mute = (mute || settings.IsSoundMuted);
    }
}
