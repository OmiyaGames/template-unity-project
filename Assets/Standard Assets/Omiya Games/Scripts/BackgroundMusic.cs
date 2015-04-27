using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    /// <summary>
    /// This is a singleton script that allows smooth transitions between 2 background musics.
    /// This script will automatically create 2 audio sources.
    /// </summary>
    public class BackgroundMusic : ISingletonScript
    {
        [Tooltip("The background music's clip. 2D AudioClip is recommended.")]
        [SerializeField]
        AudioClip startingClip = null;
        [Tooltip("The transition length (in seconds) between 2 background musics. Set to -1 if you want no transition.")]
        [SerializeField]
        float transitionDuration = 1;
        [SerializeField]
        bool loopMusic = true;
        [Tooltip("The priority of the audio sources when they're created.")]
        [SerializeField]
        int audioPriority = 128;

        float timePassedInTransition = float.NaN, volumeChangeSpeed = 1;
        int currentAudioSourceIndex = 0, index = 0;
        GameSettings settings = null;
        readonly AudioSource[] allAudioSources = new AudioSource[2];

        /// <summary>
        /// Gets or sets the volume of the background music, which is a value between 0 and 1.
        /// </summary>
        /// <value>The background music's volume.</value>
        public float Volume
        {
            get
            {
                return Mathf.Clamp01(settings.MusicVolume);
            }
            set
            {
                // Set volume
                settings.MusicVolume = Mathf.Clamp01(value);

                // Check if we're transitioning
                if (float.IsNaN(timePassedInTransition) == true)
                {
                    // Update audio sources
                    CurrentAudioSource.volume = settings.MusicVolume;
                }
                else
                {
                    // Reduce the volume of each audio source, if it has a louder volume
                    if (TransitionAudioSource.volume > settings.MusicVolume)
                    {
                        TransitionAudioSource.volume = settings.MusicVolume;
                    }
                    if (CurrentAudioSource.volume > settings.MusicVolume)
                    {
                        CurrentAudioSource.volume = settings.MusicVolume;
                    }
                }
            }
        }

        public bool IsMuted
        {
            get
            {
                return settings.IsMusicMuted;
            }
            set
            {
                settings.IsMusicMuted = value;
            }
        }

        public AudioClip CurrentMusic
        {
            get
            {
                return CurrentAudioSource.clip;
            }
            set
            {
                // Check if this is a different clip
                if (CurrentAudioSource.clip != value)
                {
                    // Check if we want to set the background music immediately
                    if (float.IsNaN(volumeChangeSpeed) == true)
                    {
                        // If not, just set the current audio source to the new clip
                        CurrentAudioSource.clip = value;
                    }
                    else
                    {
                        // Otherwise, switch to the next audio source
                        currentAudioSourceIndex += 1;

                        // Setup the next audio source
                        CurrentAudioSource.Stop();
                        CurrentAudioSource.clip = value;
                        CurrentAudioSource.volume = 0;

                        // Start playing the new audio source
                        CurrentAudioSource.Play();

                        // Flag we're transitioning
                        timePassedInTransition = 0;
                    }
                }
            }
        }

        AudioSource CurrentAudioSource
        {
            get
            {
                return allAudioSources[currentAudioSourceIndex];
            }
        }

        AudioSource TransitionAudioSource
        {
            get
            {
                int nextSource = currentAudioSourceIndex - 1;
                if (nextSource < 0)
                {
                    nextSource = allAudioSources.Length - 1;
                }
                return allAudioSources[nextSource];
            }
        }

        public override void SingletonStart(Singleton instance)
        {
            // Do nothing
        }

        public override void SceneStart(Singleton instance)
        {
            // Don't do anything if this object is already setup
            if (settings != null)
            {
                return;
            }

            // Grab an instance of the settings
            settings = Singleton.Get<GameSettings>();

            // Setup variables
            timePassedInTransition = float.NaN;
            currentAudioSourceIndex = 0;

            // Calculate the volume change speed
            if ((transitionDuration < 0) || (Mathf.Approximately(transitionDuration, 0) == true))
            {
                volumeChangeSpeed = float.NaN;
            }
            else
            {
                volumeChangeSpeed = 1f / transitionDuration;
            }

            // Go through all audio sources
            for (index = 0; index < allAudioSources.Length; ++index)
            {
                // Create audio sources
                allAudioSources[index] = gameObject.AddComponent<AudioSource>();

                // Update this audio's settings
                allAudioSources[index].spatialBlend = 0f;
                allAudioSources[index].mute = IsMuted;
                allAudioSources[index].volume = Volume;
                allAudioSources[index].priority = audioPriority;
                allAudioSources[index].loop = loopMusic;

                // Check if we should play this audio
                if (index == currentAudioSourceIndex)
                {
                    allAudioSources[index].playOnAwake = true;
                    allAudioSources[index].clip = startingClip;
                    allAudioSources[index].Play();
                }
                else
                {
                    allAudioSources[index].playOnAwake = false;
                    allAudioSources[index].Stop();
                }
            }

            // Bind to the singleton's update event
            if (instance != null)
            {
                instance.OnRealTimeUpdate += RealTimeUpdate;
            }
        }

        void RealTimeUpdate(float deltaTime)
        {
            // Check the timePassedInTransition
            if (float.IsNaN(timePassedInTransition) == false)
            {
                // Check if enough time has passed
                if (timePassedInTransition > transitionDuration)
                {
                    // Stop the transition music
                    TransitionAudioSource.Stop();

                    // Snap the volume of the current music
                    CurrentAudioSource.volume = Volume;

                    // Flag we're done transitioning
                    timePassedInTransition = float.NaN;
                }
                else
                {
                    // Reduce the transition audio's volume to 0
                    TransitionAudioSource.volume = Mathf.MoveTowards(
                        TransitionAudioSource.volume, 0, (volumeChangeSpeed * deltaTime));

                    // Increase the current audio's volume to Volume
                    CurrentAudioSource.volume = Mathf.MoveTowards(
                        CurrentAudioSource.volume, Volume, (volumeChangeSpeed * deltaTime));
                }
            }
        }
    }
}
