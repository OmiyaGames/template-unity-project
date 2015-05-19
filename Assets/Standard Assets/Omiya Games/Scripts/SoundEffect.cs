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
        static readonly HashSet<SoundEffect> allSoundEffects = new HashSet<SoundEffect>();

        /// <summary>
        /// A series of clips to play at random
        /// </summary>
        [Tooltip("A randomized list of clips to play. Note that the clip set on the AudioSource on start will be added to this list automatically.")]
        [SerializeField]
        RandomList<AudioClip> clipVariations = new RandomList<AudioClip>();
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

            // Add the audio source's clip to the random list
            if(Audio.clip != null)
            {
                clipVariations.Add(Audio.clip);
            }

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
            if (clipVariations.Count > 1)
            {
                Audio.clip = clipVariations.CurrentElement;
            }
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
