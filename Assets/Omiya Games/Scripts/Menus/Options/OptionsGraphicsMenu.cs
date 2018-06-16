using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Audio;
using OmiyaGames.Settings;
using OmiyaGames.Translations;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsGraphicsMenu.cs" company="Omiya Games">
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
    /// <date>6/15/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that provides graphics options.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public class OptionsGraphicsMenu : IMenu
    {
        #region Serialized Fields
        [Header("Features to Enable")]
        [SerializeField]
        SupportedPlatforms enableCameraShakes;
        [SerializeField]
        SupportedPlatforms enableHeadBobbing;
        [SerializeField]
        SupportedPlatforms enableScreenFlashing;
        [SerializeField]
        SupportedPlatforms enableMotionBlurs;
        [SerializeField]
        SupportedPlatforms enableBloom;

        [Header("All Checkboxes")]
        [SerializeField]
        Toggle languageDropDown;
        [SerializeField]
        GameObject[] languageParents;

        [Header("Audio Controls")]
        [SerializeField]
        AudioControls musicControls;
        [SerializeField]
        AudioControls soundEffectsControls;

        [Header("Special Effects Controls")]
        [SerializeField]
        ToggleControls smoothCameraControls;
        [SerializeField]
        ToggleControls bobbingCameraControls;
        [SerializeField]
        ToggleControls motionBlursControls;
        [SerializeField]
        ToggleControls flashesControls;
        [SerializeField]
        ToggleControls bloomControls;
        [SerializeField]
        GameObject[] specialEffectsParents;

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

        [Header("Scroll Wheel")]
        [SerializeField]
        SensitivityControls scrollWheelSensitivity;
        [SerializeField]
        ToggleControls scrollWheelInvert;
        [SerializeField]
        GameObject[] scrollWheelLabelsAndDividers;

        [Header("Other controls")]
        [SerializeField]
        GameObject resetAllDataParent;
        #endregion

        SoundEffect audioCache;
        bool inSetupMode = false;

        System.Action<OptionsGraphicsMenu> hideAction = null;

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

            // Update how the languages are enabled
            SetupLanguageControls();

            // Update how music controls are enabled
            SetupAudioControls();

            // Update how special effects controls are enabled
            SetupSpecialEffectsControls();

            // Update how keyboard controls are enabled
            SetupKeyboardSensitivityControls();
            SetupInvertKeyboardControls();

            // Update how mouse controls are enabled
            SetupMouseSensitivityControls();
            SetupInvertMouseControls();

            // Update how scroll wheel controls are enabled
            SetupScrollWheelControls();

            // Update whether the rest of the controls are enabled
            SetupOtherControls();
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

        #region Special Effects Group
        public void OnEnableSmoothCameraToggled(bool enable)
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                settings.IsSmoothCameraEnabled = enable;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnEnableBobbingCameraToggled(bool enable)
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                settings.IsBobbingCameraEnabled = enable;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnEnableFlashesToggled(bool enable)
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                settings.IsFlashesEnabled = enable;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnEnableMotionBlursToggled(bool enable)
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                settings.IsMotionBlursEnabled = enable;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnEnableBloomToggled(bool enable)
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                settings.IsBloomEnabled = enable;

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
                settings.IsKeyboardAxisSensitivitySplit = splitAxis;

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
                settings.KeyboardXAxisSensitivity = sliderValue;
                settings.KeyboardYAxisSensitivity = sliderValue;
                keyboardSensitivity.OverallSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnKeyboardXAxisSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                settings.KeyboardXAxisSensitivity = sliderValue;

                keyboardSensitivity.XAxisSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnKeyboardYAxisSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                settings.KeyboardYAxisSensitivity = sliderValue;

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
                settings.IsKeyboardXAxisInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnInvertKeyboardYAxisToggled(bool invert)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                settings.IsKeyboardYAxisInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion

        #region Mouse Sensitivity
        public void OnSplitMouseAxisToggled(bool splitAxis)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                settings.IsMouseAxisSensitivitySplit = splitAxis;

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
                settings.MouseXAxisSensitivity = sliderValue;
                settings.MouseYAxisSensitivity = sliderValue;

                mouseSensitivity.OverallSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnMouseXAxisSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                settings.MouseXAxisSensitivity = sliderValue;

                mouseSensitivity.XAxisSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnMouseYAxisSensitivityChanged(float sliderValue)
        {
            if (inSetupMode == false)
            {
                // Setup settings
                settings.MouseYAxisSensitivity = sliderValue;

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
                settings.IsMouseXAxisInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnInvertMouseYAxisToggled(bool invert)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                settings.IsMouseYAxisInverted = invert;

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
                settings.ScrollWheelSensitivity = sliderValue;

                // Update label
                scrollWheelSensitivity.SensitivityPercentLabel.text = Percent(sliderValue);
            }
        }

        public void OnInvertScrollWheelToggled(bool invert)
        {
            if (inSetupMode == false)
            {
                // Store this settings
                settings.IsScrollWheelInverted = invert;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion
        #endregion

        #region Helper Methods
        static string Percent(float val)
        {
            return val.ToString("0%");
        }

        void SetupLanguageControls()
        {
            if (languageDropDown.IsSetup == false)
            {
                // Setup the drop down
                languageDropDown.Setup();
            }

            // Update whether the controls are visible or not
            foreach (GameObject controls in languageParents)
            {
                controls.SetActive(AllFlags.EnableLanguageControls);
            }
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

        void SetupSpecialEffectsControls()
        {
            // Update Motion Blurs controls
            smoothCameraControls.IsInverted = settings.IsSmoothCameraEnabled;
            smoothCameraControls.IsActive = AllFlags.EnableSmoothCameraToggle;

            // Update Motion Blurs controls
            bobbingCameraControls.IsInverted = settings.IsBobbingCameraEnabled;
            bobbingCameraControls.IsActive = AllFlags.EnableBobbingCameraToggle;

            // Update Motion Blurs controls
            motionBlursControls.IsInverted = settings.IsMotionBlursEnabled;
            motionBlursControls.IsActive = AllFlags.EnableMotionBlursToggle;

            // Update Flashing controls
            flashesControls.IsInverted = settings.IsFlashesEnabled;
            flashesControls.IsActive = AllFlags.EnableFlashingEffectsToggle;

            // Update Bloom controls
            bloomControls.IsInverted = settings.IsBloomEnabled;
            bloomControls.IsActive = AllFlags.EnableBloomToggle;

            // Update visiblilty
            bool specialEffectsEnabled = AllFlags.EnableMotionBlursToggle && AllFlags.EnableFlashingEffectsToggle;
            foreach (GameObject controls in specialEffectsParents)
            {
                controls.SetActive(specialEffectsEnabled);
            }
        }

        void SetupKeyboardSensitivityControls()
        {
            // Update keyboard sensitivity
            keyboardSensitivity.Update(settings.IsKeyboardAxisSensitivitySplit, settings.KeyboardXAxisSensitivity, settings.KeyboardYAxisSensitivity);
            keyboardSensitivity.IsActive = AllFlags.EnableKeyboardSensitivityControls;
        }

        void SetupInvertKeyboardControls()
        {
            // Update keyboard inverting controls
            keyboardXInvert.IsInverted = settings.IsKeyboardXAxisInverted;
            keyboardYInvert.IsInverted = settings.IsKeyboardYAxisInverted;

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
            mouseSensitivity.Update(settings.IsMouseAxisSensitivitySplit, settings.MouseXAxisSensitivity, settings.MouseYAxisSensitivity);
            mouseSensitivity.IsActive = AllFlags.EnableMouseSensitivityControls;
        }

        void SetupInvertMouseControls()
        {
            // Update keyboard inverting controls
            mouseXInvert.IsInverted = settings.IsMouseXAxisInverted;
            mouseYInvert.IsInverted = settings.IsMouseYAxisInverted;

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
            scrollWheelSensitivity.Update(settings.ScrollWheelSensitivity);
            scrollWheelSensitivity.IsActive = AllFlags.EnableScrollWheelSensitivityControls;

            // Update scroll wheel inverted
            scrollWheelInvert.IsInverted = settings.IsScrollWheelInverted;
            scrollWheelInvert.IsActive = AllFlags.EnableScrollWheelInvertedControls;

            // Update visiblilty
            bool specialEffectsEnabled = AllFlags.EnableScrollWheelSensitivityControls && AllFlags.EnableScrollWheelInvertedControls;
            foreach (GameObject controls in scrollWheelLabelsAndDividers)
            {
                controls.SetActive(specialEffectsEnabled);
            }
        }

        void SetupOtherControls()
        {
            resetAllDataParent.SetActive(AllFlags.EnableResetDataButton);
        }
        #endregion
    }
}
