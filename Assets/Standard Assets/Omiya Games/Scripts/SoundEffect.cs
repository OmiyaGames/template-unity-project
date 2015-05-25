using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SoundEffect.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <author>Taro Omiya</author>
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A script for playing sound effects, with extra options such as clip, pitch,
    /// and volume mutation. Also allows configuring sound effects' volume.
    /// 
    /// Note: for the <code>OptionsMenu</code> to work, ALL <code>AudioSource</code>s
    /// must have a <code>SoundEffect</code> script attached, or managed by
    /// <code>BackgroundMusic</code>.
    /// </summary>
    /// <seealso cref="AudioSource"/>
    /// <seealso cref="BackgroundMusic"/>
    /// <seealso cref="OptionsMenu"/>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffect : MonoBehaviour
    {
        public const float MinPitch = -3, MaxPitch = 3;
        public const float MinVolume = 0, MaxVolume = 1;

        static readonly HashSet<SoundEffect> allSoundEffects = new HashSet<SoundEffect>();

        /// <summary>
        /// A series of clips to play at random
        /// </summary>
        [Tooltip("A randomized list of clips to play. Note that the clip set on the AudioSource on start will be added to this list automatically.")]
        [SerializeField]
        List<AudioClip> clipVariations = new List<AudioClip>();
        /// <summary>
        /// Whether this sound effect's pitch should be mutated
        /// </summary>
        [SerializeField]
        bool mutatePitch = false;
        /// <summary>
        /// The allowed range the pitch can mutate from the center pitch
        /// </summary>
        [SerializeField]
        Vector2 pitchMutationRange = new Vector2(0.5f, 1.5f);
        /// <summary>
        /// Whether this sound effect's volume should be mutated
        /// </summary>
        [SerializeField]
        bool mutateVolume = false;
        /// <summary>
        /// The allowed range the volume can mutate from the center pitch
        /// </summary>
        [SerializeField]
        Vector2 volumeMutationRange = new Vector2(0.5f, 1f);

        float lastVolumeRatio = 1, globalVolume = 1;
        bool mute = false;
        AudioSource audioCache = null;
        RandomList<AudioClip> clipRandomizer = null;

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

        public float CenterVolume
        {
            get
            {
                float returnVolume = Audio.volume;
                if(mutateVolume == true)
                {
                    returnVolume = (volumeMutationRange.x + volumeMutationRange.y) / 2f;
                }
                return returnVolume;
            }
        }

        public float CenterPitch
        {
            get
            {
                float returnPitch = Audio.pitch;
                if (mutateVolume == true)
                {
                    returnPitch = (pitchMutationRange.x + pitchMutationRange.y) / 2f;
                }
                return returnPitch;
            }
        }

        public List<AudioClip> ClipVariations
        {
            get
            {
                return clipVariations;
            }
        }
        #endregion

        #region Unity Events
        void Start()
        {
            // Grab the original values
            mute = Audio.mute;
            lastVolumeRatio = Audio.volume / globalVolume;
            
            // Add the audio source's clip to the random list, if it isn't in there already
            if ((Audio.clip != null) && (ClipVariations.Contains(Audio.clip) == false))
            {
                ClipVariations.Add(Audio.clip);
            }
            clipRandomizer = new RandomList<AudioClip>(ClipVariations);

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
            if (clipRandomizer.Count > 1)
            {
                Audio.clip = clipRandomizer.RandomElement;
            }
            if(mutatePitch == true)
            {
                // Change the audio's pitch
                Audio.pitch = Random.Range(pitchMutationRange.x, pitchMutationRange.y);
            }

            // Update the volume
            globalVolume = GlobalVolume;
            if(mutateVolume == true)
            {
                // Change the audio's volume
                lastVolumeRatio = Random.Range(volumeMutationRange.x, volumeMutationRange.y);
                Audio.volume = Mathf.Clamp01(lastVolumeRatio * globalVolume);
            }
            else
            {
                // Update the last played volume
                lastVolumeRatio = Audio.volume / globalVolume;
            }

            // Play the audio
            Audio.Play();
        }

        void UpdateAudio(GameSettings settings)
        {
            // Update the volume
            Audio.volume = Mathf.Clamp01(lastVolumeRatio * settings.SoundVolume);

            // Update mute
            Audio.mute = (mute || settings.IsSoundMuted);
        }
    }
}
