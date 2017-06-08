using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="BackgroundMusicChanger.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>A simple test script: changes the background music when the scene starts</summary>
    /// <seealso cref="BackgroundMusic"/>
    [RequireComponent(typeof(AudioClip))]
    public class AdaptiveMusic : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Check this box if this music should be affected by the Options Menu.")]
        bool bindToOptions = true;
        [SerializeField]
        [Tooltip("Check this box if this music's pitch should be affected by the Time Manager.")]
        bool bindToTimeManager = true;
        [SerializeField]
        bool silenceBackgroundMusicOnAwake = true;
        //[Tooltip("The transition length (in seconds) between 2 background musics. Set to -1 if you want no transition.")]
        //[SerializeField]
        //[UnityEngine.Serialization.FormerlySerializedAs("transitionDuration")]
        //float defaultTransitionDuration = 1;

        [Header("Required Properties")]
        [SerializeField]
        AudioMixer mixer;
        [SerializeField]
        string masterVolumeFieldName = "Volume";
        [SerializeField]
        string masterPitchFieldName = "Pitch";
        [SerializeField]
        string duckLevelFieldName = "Duck Level";
        [SerializeField]
        float muteVolume = -80f;

        float volumeDb = 0f;
        float pitchPercent = 1f;
        bool isMuted = false;

        #region Properties
        public bool IsMuted
        {
            get
            {
                float volume = 0f;
                if(Mixer.GetFloat(masterVolumeFieldName, out volume) == false)
                {
                    volume = 0f;
                }
                return volume.CompareTo(muteVolume) <= 0;
            }
            set
            {
                if(IsBoundToOptions == true)
                {
                    Debug.LogWarning("Cannot mute this music through this property: use BackgroundMusic.GlobalMute property.", this);
                }
                else if (isMuted != value)
                {
                    SetMute(value);
                }
            }
        }

        public float VolumeDb
        {
            get
            {
                return volumeDb;
            }
            set
            {
                if (IsBoundToOptions == true)
                {
                    Debug.LogWarning("Cannot change this music's volume through this property: use BackgroundMusic.GlobalVolume property.", this);
                }
                else if (Mathf.Approximately(volumeDb, value) == false)
                {
                    SetVolumeDb(value);
                }
            }
        }

        public float VolumePercent
        {
            get
            {
                return Mathf.InverseLerp(muteVolume, 0, volumeDb);
            }
            set
            {
                if (IsBoundToOptions == true)
                {
                    Debug.LogWarning("Cannot change this music's volume through this property: use BackgroundMusic.GlobalVolume property.", this);
                }
                else if(Mathf.Approximately(VolumePercent, value) == false)
                {
                    SetVolumePercent(value);
                }
            }
        }

        public float PitchPercent
        {
            get
            {
                return pitchPercent;
            }
            set
            {
                if (IsBoundToOptions == true)
                {
                    Debug.LogWarning("Cannot change this music's pitch through this property: use BackgroundMusic.GlobalPitch property.", this);
                }
                else if (Mathf.Approximately(pitchPercent, value) == false)
                {
                    SetPitchPercent(value);
                }
            }
        }

        //public float DefaultTransitionDuration
        //{
        //    get
        //    {
        //        return defaultTransitionDuration;
        //    }
        //    set
        //    {
        //        defaultTransitionDuration = value;
        //    }
        //}

        public bool IsBoundToOptions
        {
            get
            {
                return bindToOptions;
            }
            set
            {
                if(bindToOptions != value)
                {
                    bindToOptions = value;
                    UpdateBindingToOptions();
                }
            }
        }

        public bool IsBoundToTimeManager
        {
            get
            {
                return bindToTimeManager;
            }
            set
            {
                if(bindToTimeManager != value)
                {
                    bindToTimeManager = value;
                    UpdateBindingToTimeManager();
                }
            }
        }

        public AudioMixer Mixer
        {
            get
            {
                return mixer;
            }
        }
        #endregion

        void Awake()
        {
            // Grab the volume, if possible
            if(Mixer.GetFloat(masterVolumeFieldName, out volumeDb) == true)
            {
                // Check whether the music is muted or not
                isMuted = volumeDb.CompareTo(muteVolume) <= 0;
            }
            else
            {
                // If failed to get volume, indicate error
                Debug.LogError("Unrecognized volume field, \"" + masterVolumeFieldName + "\"");
            }

            // Grab the pitch, if possible
            if (Mixer.GetFloat(masterPitchFieldName, out pitchPercent) == false)
            {
                // If failed to get pitch, indicate error
                Debug.LogError("Unrecognized pitch field, \"" + masterPitchFieldName + "\"");
            }

            if (silenceBackgroundMusicOnAwake == true)
            {
                // Silence the music
                Singleton.Get<BackgroundMusic>().CurrentMusic = null;
            }
        }

        void Start()
        {
            if(IsBoundToOptions == true)
            {
                UpdateBindingToOptions();
            }
            if(IsBoundToTimeManager == true)
            {
                UpdateBindingToTimeManager();
            }
        }

        #region Helper Methods and Events
        private TimeManager TimeManager
        {
            get
            {
                return Singleton.Get<TimeManager>();
            }
        }

        private void UpdateBindingToOptions()
        {
            if (IsBoundToOptions == true)
            {
                // Bind to the global events
                BackgroundMusic.OnGlobalVolumePercentChange += SetVolumePercent;
                BackgroundMusic.OnGlobalMuteChange += SetMute;
                BackgroundMusic.OnGlobalPitchPercentChange += SetPitchPercent;

                // Setup the mute, pitch, and volume
                SetVolumePercent(BackgroundMusic.GlobalVolume);
                SetPitchPercent(BackgroundMusic.GlobalPitch);
                SetMute(BackgroundMusic.GlobalMute);
            }
            else
            {
                // Unbind to the global events
                BackgroundMusic.OnGlobalVolumePercentChange -= SetVolumePercent;
                BackgroundMusic.OnGlobalMuteChange -= SetMute;
                BackgroundMusic.OnGlobalPitchPercentChange -= SetPitchPercent;
            }
        }

        private void UpdateBindingToTimeManager()
        {
            if (IsBoundToTimeManager == true)
            {
                // Bind to TimeManager
                TimeManager.OnManuallyPausedChanged += ToggleDuckLevel;
                ToggleDuckLevel(TimeManager);
            }
            else
            {
                // Unbind to TimeManager
                TimeManager.OnManuallyPausedChanged -= ToggleDuckLevel;
            }
        }

        private void SetVolumePercent(float valuePercent)
        {
            SetVolumeDb(Mathf.Lerp(muteVolume, 0, valuePercent));
        }

        private void SetVolumeDb(float valueDb)
        {
            volumeDb = valueDb;
            if (IsMuted == false)
            {
                Mixer.SetFloat(masterVolumeFieldName, volumeDb);
            }
        }

        private void SetPitchPercent(float value)
        {
            pitchPercent = value;
            Mixer.SetFloat(masterPitchFieldName, pitchPercent);
        }

        private void SetMute(bool value)
        {
            isMuted = value;
            if (isMuted == true)
            {
                Mixer.SetFloat(masterVolumeFieldName, muteVolume);
            }
            else
            {
                Mixer.SetFloat(masterVolumeFieldName, VolumeDb);
            }
        }

        private void ToggleDuckLevel(TimeManager obj)
        {
            if(obj.IsManuallyPaused == true)
            {
                Mixer.SetFloat(duckLevelFieldName, 0f);
            }
            else
            {
                Mixer.SetFloat(duckLevelFieldName, muteVolume);
            }
        }
        #endregion
    }
}
