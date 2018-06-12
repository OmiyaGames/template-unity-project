using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    using Settings;

    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsAudioMenu.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
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
    /// <date>6/11/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that provides audio options.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SoundEffect))]
    public class OptionsAudioMenu : IMenu
    {
        // FIXME: take out the unnecessary cruft
        public const float MinimumDisplayedVolume = 0.01f;
        public const float MaximumDisplayedVolume = 1f;

        #region Serialized Containers
        [System.Serializable]
        public class EnableFlags
        {
            [SerializeField]
            bool enableLanguageControls = true;
            [SerializeField]
            bool enableMusicControls = true;
            [SerializeField]
            bool enableSoundEffectControls = true;
            [SerializeField]
            bool enableSmoothCameraToggle = true;
            [SerializeField]
            bool enableBobbingCameraToggle = true;
            [SerializeField]
            bool enableMotionBlursToggle = true;
            [SerializeField]
            bool enableFlashingEffectsToggle = true;
            [SerializeField]
            bool enableBloomToggle = true;
            [SerializeField]
            bool enableKeyboardSensitivityControls = true;
            [SerializeField]
            bool enableKeyboardInvertedControls = true;
            [SerializeField]
            bool enableMouseSensitivityControls = true;
            [SerializeField]
            bool enableMouseInvertedControls = true;
            [SerializeField]
            bool enableScrollWheelSensitivityControls = true;
            [SerializeField]
            bool enableScrollWheelInvertedControls = true;
            [SerializeField]
            bool enableResetDataButton = true;

            public bool EnableLanguageControls
            {
                get
                {
                    return enableLanguageControls;
                }
            }

            public bool EnableMusicControls
            {
                get
                {
                    return enableMusicControls;
                }
            }

            public bool EnableSoundEffectControls
            {
                get
                {
                    return enableSoundEffectControls;
                }
            }

            public bool EnableMotionBlursToggle
            {
                get
                {
                    return enableMotionBlursToggle;
                }
            }

            public bool EnableFlashingEffectsToggle
            {
                get
                {
                    return enableFlashingEffectsToggle;
                }
            }

            public bool EnableBloomToggle
            {
                get
                {
                    return enableBloomToggle;
                }
            }

            public bool EnableKeyboardSensitivityControls
            {
                get
                {
                    return enableKeyboardSensitivityControls;
                }
            }

            public bool EnableKeyboardInvertedControls
            {
                get
                {
                    return enableKeyboardInvertedControls;
                }
            }

            public bool EnableMouseSensitivityControls
            {
                get
                {
                    return enableMouseSensitivityControls;
                }
            }

            public bool EnableMouseInvertedControls
            {
                get
                {
                    return enableMouseInvertedControls;
                }
            }

            public bool EnableScrollWheelSensitivityControls
            {
                get
                {
                    return enableScrollWheelSensitivityControls;
                }
            }

            public bool EnableScrollWheelInvertedControls
            {
                get
                {
                    return enableScrollWheelInvertedControls;
                }
            }

            public bool EnableResetDataButton
            {
                get
                {
                    return enableResetDataButton;
                }
            }

            public bool EnableSmoothCameraToggle
            {
                get
                {
                    return enableSmoothCameraToggle;
                }
            }

            public bool EnableBobbingCameraToggle
            {
                get
                {
                    return enableBobbingCameraToggle;
                }
            }
        }

        [System.Serializable]
        public struct AudioControls
        {
            [SerializeField]
            GameObject[] controlParents;
            [SerializeField]
            Slider volumeSlider;
            [SerializeField]
            Text volumePercentLabel;
            [SerializeField]
            Toggle checkBoxMark;

            public void Update(float volume, bool mute)
            {
                VolumeSlider.value = volume;
                VolumePercentLabel.text = Percent(volume);
                VolumeSlider.interactable = !mute;
                CheckBoxMark.isOn = mute;
            }

            public Slider VolumeSlider
            {
                get
                {
                    return volumeSlider;
                }
            }

            public Text VolumePercentLabel
            {
                get
                {
                    return volumePercentLabel;
                }
            }

            public Toggle CheckBoxMark
            {
                get
                {
                    return checkBoxMark;
                }
            }

            public float MinValue
            {
                get
                {
                    return VolumeSlider.minValue;
                }
            }

            public float MaxValue
            {
                get
                {
                    return VolumeSlider.maxValue;
                }
            }

            public bool IsActive
            {
                get
                {
                    bool returnFlag = false;
                    foreach (GameObject control in controlParents)
                    {
                        if (control != null)
                        {
                            returnFlag = control.activeSelf;
                            break;
                        }
                    }
                    return returnFlag;
                }
                set
                {
                    foreach (GameObject control in controlParents)
                    {
                        if (control != null)
                        {
                            control.SetActive(value);
                        }
                    }
                }
            }
        }

        [System.Serializable]
        public struct SensitivityControls
        {
            [SerializeField]
            GameObject controlParent;
            [SerializeField]
            Slider slider;
            [SerializeField]
            Text percentLabel;

            public void Update(float sensitivity)
            {
                SensitivitySlider.value = sensitivity;
                SensitivityPercentLabel.text = Percent(sensitivity);
            }

            public Slider SensitivitySlider
            {
                get
                {
                    return slider;
                }
            }

            public Text SensitivityPercentLabel
            {
                get
                {
                    return percentLabel;
                }
            }

            public float MinValue
            {
                get
                {
                    return SensitivitySlider.minValue;
                }
            }

            public float MaxValue
            {
                get
                {
                    return SensitivitySlider.maxValue;
                }
            }

            public bool IsActive
            {
                get
                {
                    return controlParent.activeSelf;
                }
                set
                {
                    controlParent.SetActive(value);
                }
            }
        }

        [System.Serializable]
        public struct ToggleControls
        {
            [SerializeField]
            GameObject controlParent;
            [SerializeField]
            Toggle toggle;

            public bool IsInverted
            {
                get
                {
                    return toggle.isOn;
                }
                set
                {
                    toggle.isOn = value;
                }
            }

            public bool IsActive
            {
                get
                {
                    return controlParent.activeSelf;
                }
                set
                {
                    controlParent.SetActive(value);
                }
            }
        }

        [System.Serializable]
        public struct CompoundSensitivityControls
        {
            [SerializeField]
            ToggleControls splitAxisToggle;
            [SerializeField]
            SensitivityControls overallSensitivity;
            [SerializeField]
            SensitivityControls xAxisSensitivity;
            [SerializeField]
            SensitivityControls yAxisSensitivity;
            [SerializeField]
            GameObject[] labelsAndDividers;

            public void Update(bool splitSensitivity, float xSensitivity, float ySensitivity)
            {
                splitAxisToggle.IsInverted = splitSensitivity;

                OverallSensitivity.Update(xSensitivity);
                xAxisSensitivity.Update(xSensitivity);
                yAxisSensitivity.Update(ySensitivity);

                UpdateAxisSensitivityControls();
            }

            public SensitivityControls OverallSensitivity
            {
                get
                {
                    return overallSensitivity;
                }
            }

            public SensitivityControls XAxisSensitivity
            {
                get
                {
                    return xAxisSensitivity;
                }
            }

            public SensitivityControls YAxisSensitivity
            {
                get
                {
                    return yAxisSensitivity;
                }
            }

            public bool IsActive
            {
                get
                {
                    return splitAxisToggle.IsActive;
                }
                set
                {
                    splitAxisToggle.IsActive = value;
                    UpdateAxisSensitivityControls();
                }
            }

            public void UpdateAxisSensitivityControls()
            {
                if (splitAxisToggle.IsActive == false)
                {
                    xAxisSensitivity.IsActive = false;
                    yAxisSensitivity.IsActive = false;
                    overallSensitivity.IsActive = false;

                    foreach (GameObject control in labelsAndDividers)
                    {
                        control.SetActive(false);
                    }
                }
                else
                {
                    if (splitAxisToggle.IsInverted == true)
                    {
                        xAxisSensitivity.IsActive = true;
                        yAxisSensitivity.IsActive = true;
                        overallSensitivity.IsActive = false;
                    }
                    else
                    {
                        overallSensitivity.IsActive = true;
                        xAxisSensitivity.IsActive = false;
                        yAxisSensitivity.IsActive = false;
                    }

                    foreach (GameObject control in labelsAndDividers)
                    {
                        control.SetActive(true);
                    }
                }
            }
        }
        #endregion

        #region Serialized Fields
        [Header("Features to Enable")]
        [SerializeField]
        EnableFlags defaultFlags;
        [SerializeField]
        EnableFlags webglFlags;

        [Header("Audio Controls")]
        [SerializeField]
        AudioControls musicControls;
        [SerializeField]
        AudioControls soundEffectsControls;
        #endregion

        SoundEffect audioCache;
        bool inSetupMode = false;

        System.Action<OptionsAudioMenu> hideAction = null;

        #region Properties
        public SoundEffect TestSoundEffect
        {
            get
            {
                if (audioCache == null)
                {
                    audioCache = GetComponent<SoundEffect>();
                }
                return audioCache;
            }
        }

        GameSettings settings
        {
            get
            {
                return Singleton.Get<GameSettings>();
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return musicControls.VolumeSlider.gameObject;
            }
        }

        EnableFlags AllFlags
        {
            get
            {
#if UNITY_WEBGL
                return webglFlags;
#else
                return defaultFlags;
#endif
            }
        }
        #endregion

        void Start()
        {
            // Setup controls
            inSetupMode = true;

            // Update how music controls are enabled
            SetupAudioControls();
            inSetupMode = false;
        }

        protected override void OnStateChanged(IMenu.State from, IMenu.State to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            if ((from == State.Visible) && (to == State.Hidden))
            {
                // Run the last action
                if (hideAction != null)
                {
                    hideAction(this);
                    hideAction = null;
                }
            }
        }

        #region UI events
        public void OnLanguageSeleced(int selectedIndex)
        {
            if ((inSetupMode == false) && (selectedIndex >= 0))
            {
                // Grab the translator
                TranslationManager translator = Singleton.Get<TranslationManager>();
                if ((translator != null) && (selectedIndex < translator.SupportedLanguages.Count))
                {
                    // Change the language
                    translator.CurrentLanguage = translator.SupportedLanguages[selectedIndex];
                }

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        #region Music Group
        public void OnMusicSliderChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                BackgroundMusic.GlobalVolume = sliderValue;
                musicControls.VolumePercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnMusicMuteToggled(bool mute)
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                BackgroundMusic.GlobalMute = mute;

                // disable the slider
                musicControls.VolumeSlider.interactable = !mute;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion

        #region Sound Effects Group
        public void OnSoundEffectsSliderChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                SoundEffect.GlobalVolume = sliderValue;
                soundEffectsControls.VolumePercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnSoundEffectsSliderPointerUp()
        {
            TestSoundEffect.Play();
        }

        public void OnSoundEffectsMuteToggled(bool mute)
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                SoundEffect.GlobalMute = mute;

                // disable the slider
                soundEffectsControls.VolumeSlider.interactable = !mute;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion
        #endregion

        #region Helper Methods
        public static string Percent(float val)
        {
            return val.ToString("0%");
        }

        void SetupAudioControls()
        {
            // Update music controls
            musicControls.Update(BackgroundMusic.GlobalVolume, BackgroundMusic.GlobalMute);
            musicControls.IsActive = AllFlags.EnableMusicControls;

            // Update sound effect controls
            soundEffectsControls.Update(SoundEffect.GlobalVolume, SoundEffect.GlobalMute);
            soundEffectsControls.IsActive = AllFlags.EnableSoundEffectControls;
        }
        #endregion
    }
}
