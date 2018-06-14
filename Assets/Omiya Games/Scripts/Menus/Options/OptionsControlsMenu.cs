using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Audio;
using OmiyaGames.Translations;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsControlsMenu.cs" company="Omiya Games">
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
    /// Menu that provides controls options.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public class OptionsControlsMenu : IMenu
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
        [SerializeField]
        SupportedPlatforms enableMusicVolumeControls;
        [SerializeField]
        SupportedPlatforms enableSoundEffectVolumeControls;

        [Header("Mouse Sensitivity")]
        [SerializeField]
        CompoundSensitivityControls mouseSensitivity;

        [Header("Invert Mouse")]
        [SerializeField]
        ToggleControls mouseXInvert;
        [SerializeField]
        ToggleControls mouseYInvert;
        [SerializeField]
        GameObject[] invertMouseLabelsAndDividers;

        [Header("Keyboard Sensitivity")]
        [SerializeField]
        CompoundSensitivityControls keyboardSensitivity;

        [Header("Invert Keyboard")]
        [SerializeField]
        ToggleControls keyboardXInvert;
        [SerializeField]
        ToggleControls keyboardYInvert;
        [SerializeField]
        GameObject[] invertKeyboardLabelsAndDividers;

        [Header("Scroll Wheel")]
        [SerializeField]
        SensitivityControls scrollWheelSensitivity;
        [SerializeField]
        ToggleControls scrollWheelInvert;
        [SerializeField]
        GameObject[] scrollWheelLabelsAndDividers;
        #endregion

        SoundEffect audioCache;
        bool inSetupMode = false;

        System.Action<OptionsControlsMenu> hideAction = null;

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
                // FIXME: assign a control
                return null;
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

            // Update how keyboard controls are enabled
            SetupKeyboardSensitivityControls();
            SetupInvertKeyboardControls();

            // Update how mouse controls are enabled
            SetupMouseSensitivityControls();
            SetupInvertMouseControls();

            // Update how scroll wheel controls are enabled
            SetupScrollWheelControls();
            inSetupMode = false;
        }

        protected override void OnStateChanged(IMenu.VisibilityState from, IMenu.VisibilityState to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            if ((from == VisibilityState.Visible) && (to == VisibilityState.Hidden))
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
        static string Percent(float val)
        {
            return val.ToString("0%");
        }

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

        #region Mouse Sensitivity
        public void OnSplitMouseAxisToggled(bool splitAxis)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                Settings.IsMouseAxisSensitivitySplit = splitAxis;

                // Toggle which sliders will be showing up
                mouseSensitivity.UpdateAxisSensitivityControls();
                if (splitAxis == true)
                {
                    mouseSensitivity.XAxisSensitivity.SensitivitySlider.value = mouseSensitivity.OverallSensitivity.SensitivitySlider.value;
                    mouseSensitivity.YAxisSensitivity.SensitivitySlider.value = mouseSensitivity.OverallSensitivity.SensitivitySlider.value;
                }
                else
                {
                    mouseSensitivity.OverallSensitivity.SensitivitySlider.value = mouseSensitivity.XAxisSensitivity.SensitivitySlider.value;
                }

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnMouseOverallSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                Settings.MouseXAxisSensitivity = sliderValue;
                Settings.MouseYAxisSensitivity = sliderValue;

                mouseSensitivity.OverallSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnMouseXAxisSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                Settings.MouseXAxisSensitivity = sliderValue;

                mouseSensitivity.XAxisSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnMouseYAxisSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                Settings.MouseYAxisSensitivity = sliderValue;

                mouseSensitivity.YAxisSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }
        #endregion

        #region Mouse Inverted
        public void OnInvertMouseXAxisToggled(bool invert)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                Settings.IsMouseXAxisInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnInvertMouseYAxisToggled(bool invert)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                Settings.IsMouseYAxisInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion

        #region Keyboard Sensitivity
        public void OnSplitKeyboardAxisToggled(bool splitAxis)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                Settings.IsKeyboardAxisSensitivitySplit = splitAxis;

                // Toggle which sliders will be showing up
                keyboardSensitivity.UpdateAxisSensitivityControls();
                if (splitAxis == true)
                {
                    keyboardSensitivity.XAxisSensitivity.SensitivitySlider.value = keyboardSensitivity.OverallSensitivity.SensitivitySlider.value;
                    keyboardSensitivity.YAxisSensitivity.SensitivitySlider.value = keyboardSensitivity.OverallSensitivity.SensitivitySlider.value;
                }
                else
                {
                    keyboardSensitivity.OverallSensitivity.SensitivitySlider.value = keyboardSensitivity.XAxisSensitivity.SensitivitySlider.value;
                }

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnKeyboardOverallSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                Settings.KeyboardXAxisSensitivity = sliderValue;
                Settings.KeyboardYAxisSensitivity = sliderValue;
                keyboardSensitivity.OverallSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnKeyboardXAxisSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                Settings.KeyboardXAxisSensitivity = sliderValue;

                keyboardSensitivity.XAxisSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnKeyboardYAxisSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                Settings.KeyboardYAxisSensitivity = sliderValue;

                keyboardSensitivity.YAxisSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }
        #endregion

        #region Keyboard Inverted
        public void OnInvertKeyboardXAxisToggled(bool invert)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                Settings.IsKeyboardXAxisInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnInvertKeyboardYAxisToggled(bool invert)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                Settings.IsKeyboardYAxisInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion

        #region Scroll Wheel
        public void OnScrollWheelSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                Settings.ScrollWheelSensitivity = sliderValue;

                // Update label
                scrollWheelSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnInvertScrollWheelToggled(bool invert)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                Settings.IsScrollWheelInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion
        #endregion

        #region Helper Methods
        void SetupKeyboardSensitivityControls()
        {
            // Update keyboard sensitivity
            keyboardSensitivity.Update(Settings.IsKeyboardAxisSensitivitySplit, Settings.KeyboardXAxisSensitivity, Settings.KeyboardYAxisSensitivity);
            keyboardSensitivity.IsActive = AllFlags.EnableKeyboardSensitivityControls;
        }

        void SetupInvertKeyboardControls()
        {
            // Update keyboard inverting controls
            keyboardXInvert.IsInverted = Settings.IsKeyboardXAxisInverted;
            keyboardYInvert.IsInverted = Settings.IsKeyboardYAxisInverted;

            // Activate or deactivate all controls
            keyboardXInvert.IsActive = AllFlags.EnableKeyboardInvertedControls;
            keyboardYInvert.IsActive = AllFlags.EnableKeyboardInvertedControls;
            foreach (GameObject parent in invertKeyboardLabelsAndDividers)
            {
                parent.SetActive(AllFlags.EnableKeyboardInvertedControls);
            }
        }

        void SetupMouseSensitivityControls()
        {
            // Update keyboard sensitivity
            mouseSensitivity.Update(Settings.IsMouseAxisSensitivitySplit, Settings.MouseXAxisSensitivity, Settings.MouseYAxisSensitivity);
            mouseSensitivity.IsActive = AllFlags.EnableMouseSensitivityControls;
        }

        void SetupInvertMouseControls()
        {
            // Update keyboard inverting controls
            mouseXInvert.IsInverted = Settings.IsMouseXAxisInverted;
            mouseYInvert.IsInverted = Settings.IsMouseYAxisInverted;

            // Activate or deactivate all controls
            mouseXInvert.IsActive = AllFlags.EnableMouseInvertedControls;
            mouseYInvert.IsActive = AllFlags.EnableMouseInvertedControls;
            foreach (GameObject parent in invertMouseLabelsAndDividers)
            {
                parent.SetActive(AllFlags.EnableMouseInvertedControls);
            }
        }

        void SetupScrollWheelControls()
        {
            // Update scroll wheel sensitivity
            scrollWheelSensitivity.Update(Settings.ScrollWheelSensitivity);
            scrollWheelSensitivity.IsActive = AllFlags.EnableScrollWheelSensitivityControls;

            // Update scroll wheel inverted
            scrollWheelInvert.IsInverted = Settings.IsScrollWheelInverted;
            scrollWheelInvert.IsActive = AllFlags.EnableScrollWheelInvertedControls;

            // Update visiblilty
            bool specialEffectsEnabled = AllFlags.EnableScrollWheelSensitivityControls && AllFlags.EnableScrollWheelInvertedControls;
            foreach (GameObject controls in scrollWheelLabelsAndDividers)
            {
                controls.SetActive(specialEffectsEnabled);
            }
        }
        #endregion
    }
}
