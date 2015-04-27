using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffect : MonoBehaviour
    {
        static readonly HashSet<SoundEffect> allSoundEffects = new HashSet<SoundEffect>();

        /// <summary>
        /// Whether this sound effect's pitch should be mutated
        /// </summary>
        [SerializeField]
        bool mutatePitch = false;
        /// <summary>
        /// The allowed range the pitch can mutate from the center pitch
        /// </summary>
        [SerializeField]
        Vector2 pitchMutationRange = new Vector2(-0.5f, 0.5f);
        /// <summary>
        /// Whether this sound effect's volume should be mutated
        /// </summary>
        [SerializeField]
        bool mutateVolume = false;
        /// <summary>
        /// The allowed range the volume can mutate from the center pitch
        /// </summary>
        [SerializeField]
        Vector2 volumeMutationRange = new Vector2(-0.5f, 0f);

        float centerVolume = 0, centerPitch = 0;
        bool mute = false;
        AudioSource audioCache = null;

        #region Static Properties
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
                foreach (SoundEffect effect in allSoundEffects)
                {
                    effect.UpdateAudio(settings);
                }
            }
        }
        #endregion

        #region Local Properties
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
                return centerVolume;
            }
            set
            {
                centerVolume = value;
                UpdateAudio(Singleton.Get<GameSettings>());
            }
        }

        public float Pitch
        {
            get
            {
                return centerPitch;
            }
            set
            {
                centerPitch = value;
            }
        }
        #endregion

        #region Unity Events
        void Start()
        {
            // Grab the original values
            centerPitch = Audio.pitch;
            centerVolume = Audio.volume;
            mute = Audio.mute;

            // Calculate how the audio should behave
            allSoundEffects.Add(this);
        }

        void OnDestroy()
        {
            allSoundEffects.Remove(this);
        }
        #endregion

        public void Play()
        {
            // Stop the audio
            Audio.Stop();

            // Apply mutation
            if(mutatePitch == true)
            {
                // Change the audio's pitch
                Audio.pitch = centerPitch + Random.Range(pitchMutationRange.x, pitchMutationRange.y);
            }
            if(mutateVolume == true)
            {
                // Change the audio's volume
                Audio.volume = Mathf.Clamp01(Volume + (Random.Range(volumeMutationRange.x, volumeMutationRange.y) * GlobalVolume));
            }

            // Play the audio
            Audio.Play();
        }

        void UpdateAudio(GameSettings settings)
        {
            // Update the volume
            Audio.volume = Mathf.Clamp01(centerVolume * settings.SoundVolume);

            // Update mute
            Audio.mute = (mute || settings.IsSoundMuted);
        }
    }
}
