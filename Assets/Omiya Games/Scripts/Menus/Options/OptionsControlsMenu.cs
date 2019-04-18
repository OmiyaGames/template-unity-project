using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menus
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
    public class OptionsControlsMenu : IOptionsMenu
    {
        #region Serialized Fields
        [Header("Features to Enable")]
        [SerializeField]
        private SupportedPlatforms enableKeyboardControls = SupportedPlatforms.AllPlatforms;
        [SerializeField]
        private SupportedPlatforms enableCameraSensitivityControls = SupportedPlatforms.AllPlatforms;
        [SerializeField]
        private SupportedPlatforms enableCameraSmoothingControls = SupportedPlatforms.AllPlatforms;
        [SerializeField]
        private SupportedPlatforms enableScrollWheelControls = SupportedPlatforms.AllPlatforms;
        [SerializeField]
        private GameObject[] allDividers;

        [Header("Keyboard Sensitivity")]
        [SerializeField]
        private ControllerSensitivityControls keyboardSensitivitySet;

        [Header("Camera Sensitivity")]
        [SerializeField]
        private ControllerSensitivityControls cameraSensitivitySet;

        [Header("Camera Smoothing")]
        [SerializeField]
        private AudioVolumeControls cameraSmoothingSet;
        [SerializeField]
        private GameObject[] cameraSmoothingControls;

        [Header("Scroll Wheel")]
        [SerializeField]
        private LabeledSlider scrollWheelSensitivity;
        [SerializeField]
        private Toggle scrollWheelInvert;
        [SerializeField]
        private GameObject[] scrollWheelControls;
        #endregion

        #region Properties
        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override Selectable DefaultUi
        {
            get
            {
                if (enableKeyboardControls.IsThisBuildSupported() == true)
                {
                    return keyboardSensitivitySet.DefaultSelectable;
                }
                else if (enableCameraSensitivityControls.IsThisBuildSupported() == true)
                {
                    return cameraSensitivitySet.DefaultSelectable;
                }
                else
                {
                    return scrollWheelSensitivity.Slider;
                }
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return BackgroundMenu.BackgroundType.SolidColor;
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Setup controls
            base.OnSetup();

            // Update how keyboard controls are enabled
            SetupKeyboardSensitivityControls();

            // Update how camera controls are enabled
            SetupCameraSensitivityControls();

            // Update how camera smoothing are enabled
            SetupCameraSmoothing();

            // Update how scroll wheel controls are enabled
            SetupScrollWheelControls();

            // Update how dividers appear
            SetupDividers(allDividers,
                enableKeyboardControls,
                enableCameraSensitivityControls,
                enableCameraSmoothingControls,
                enableScrollWheelControls);
        }

        #region UI events
        #region Keyboard Controls
        void KeyboardSensitivitySet_OnInvertVerticalAxisCheckboxChanged(ControllerSensitivityControls source, bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                Settings.IsKeyboardYAxisInverted = isChecked;
            }
        }

        void KeyboardSensitivitySet_OnInvertHorizontalAxisCheckboxChanged(ControllerSensitivityControls source, bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                Settings.IsKeyboardXAxisInverted = isChecked;
            }
        }

        void KeyboardSensitivitySet_OnVerticalSensitivitySlider(ControllerSensitivityControls source, float value)
        {
            if (IsListeningToEvents == true)
            {
                Settings.KeyboardYAxisSensitivity = value;
            }
        }

        void KeyboardSensitivitySet_OnHorizontalSensitivitySlider(ControllerSensitivityControls source, float value)
        {
            if (IsListeningToEvents == true)
            {
                Settings.KeyboardXAxisSensitivity = value;
            }
        }
        #endregion

        #region Mouse Controls
        void CameraSensitivitySet_OnInvertVerticalAxisCheckboxChanged(ControllerSensitivityControls source, bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                Settings.IsMouseYAxisInverted = isChecked;
            }
        }

        void CameraSensitivitySet_OnInvertHorizontalAxisCheckboxChanged(ControllerSensitivityControls source, bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                Settings.IsMouseXAxisInverted = isChecked;
            }
        }

        void CameraSensitivitySet_OnVerticalSensitivitySlider(ControllerSensitivityControls source, float value)
        {
            if (IsListeningToEvents == true)
            {
                Settings.MouseYAxisSensitivity = value;
            }
        }

        void CameraSensitivitySet_OnHorizontalSensitivitySlider(ControllerSensitivityControls source, float value)
        {
            if (IsListeningToEvents == true)
            {
                Settings.MouseXAxisSensitivity = value;
            }
        }

        private void CameraSmoothingSet_OnSliderValueUpdated(float value)
        {
            if (IsListeningToEvents == true)
            {
                Settings.SmoothCameraFactorOption = value;
            }
        }

        private void CameraSmoothingSet_OnCheckboxUpdated(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                Settings.IsSmoothCameraEnabled = isChecked;
            }
        }
        #endregion

        #region Scroll Wheel
        public void OnScrollWheelSensitivityChanged(float sliderValue)
        {
            if (IsListeningToEvents == true)
            {
                // Setup settings
                Settings.ScrollWheelSensitivity = sliderValue;
            }
        }

        public void OnInvertScrollWheelToggled(bool invert)
        {
            if (IsListeningToEvents == true)
            {
                // Store this settings
                Settings.IsScrollWheelInverted = invert;
            }
        }
        #endregion
        #endregion

        #region Helper Methods
        void SetupKeyboardSensitivityControls()
        {
            // Check whether to show the keyboard controls set or not
            bool enabled = enableKeyboardControls.IsThisBuildSupported();

            // Toggle the display of the controls
            keyboardSensitivitySet.gameObject.SetActive(enabled);

            // Check if enabled
            if (enabled == true)
            {
                // If so, setup the UI
                keyboardSensitivitySet.Setup(Settings.KeyboardXAxisSensitivity, Settings.KeyboardYAxisSensitivity, Settings.IsKeyboardAxisSensitivitySplit, Settings.IsKeyboardXAxisInverted, Settings.IsKeyboardYAxisInverted);

                // Bind to the events on any changes to the control set
                keyboardSensitivitySet.OnBothAxisSensitivitySlider += KeyboardSensitivitySet_OnHorizontalSensitivitySlider;
                keyboardSensitivitySet.OnHorizontalSensitivitySlider += KeyboardSensitivitySet_OnHorizontalSensitivitySlider;
                keyboardSensitivitySet.OnVerticalSensitivitySlider += KeyboardSensitivitySet_OnVerticalSensitivitySlider;
                keyboardSensitivitySet.OnInvertHorizontalAxisCheckboxChanged += KeyboardSensitivitySet_OnInvertHorizontalAxisCheckboxChanged;
                keyboardSensitivitySet.OnInvertVerticalAxisCheckboxChanged += KeyboardSensitivitySet_OnInvertVerticalAxisCheckboxChanged;
            }
        }

        void SetupCameraSensitivityControls()
        {
            // Check whether to show the keyboard controls set or not
            bool enabled = enableCameraSensitivityControls.IsThisBuildSupported();

            // Toggle the display of the controls
            cameraSensitivitySet.gameObject.SetActive(enabled);

            // Check if enabled
            if (enabled == true)
            {
                // If so, setup the UI
                cameraSensitivitySet.Setup(Settings.MouseXAxisSensitivity, Settings.MouseYAxisSensitivity, Settings.IsMouseAxisSensitivitySplit, Settings.IsMouseXAxisInverted, Settings.IsMouseYAxisInverted);

                // Bind to the events on any changes to the control set
                cameraSensitivitySet.OnBothAxisSensitivitySlider += CameraSensitivitySet_OnHorizontalSensitivitySlider;
                cameraSensitivitySet.OnHorizontalSensitivitySlider += CameraSensitivitySet_OnHorizontalSensitivitySlider;
                cameraSensitivitySet.OnVerticalSensitivitySlider += CameraSensitivitySet_OnVerticalSensitivitySlider;
                cameraSensitivitySet.OnInvertHorizontalAxisCheckboxChanged += CameraSensitivitySet_OnInvertHorizontalAxisCheckboxChanged;
                cameraSensitivitySet.OnInvertVerticalAxisCheckboxChanged += CameraSensitivitySet_OnInvertVerticalAxisCheckboxChanged;
            }
        }

        void SetupScrollWheelControls()
        {
            // Check whether to show the keyboard controls set or not
            bool enabled = enableCameraSensitivityControls.IsThisBuildSupported();

            // Check if enabled
            if (enabled == true)
            {
                // If so, setup the UI
                scrollWheelSensitivity.Slider.value = Settings.ScrollWheelSensitivity;
                scrollWheelInvert.isOn = Settings.IsScrollWheelInverted;
            }

            // Toggle the display of the controls
            foreach (GameObject control in scrollWheelControls)
            {
                control.SetActive(enabled);
            }
        }

        void SetupCameraSmoothing()
        {
            // Check to see if we want to show camera smoothing controls
            bool enable = enableCameraSmoothingControls.IsThisBuildSupported();

            // Update the controls active state
            foreach (GameObject controls in cameraSmoothingControls)
            {
                controls.SetActive(enable);
            }

            // If enabled, setup the controls
            if (enable == true)
            {
                cameraSmoothingSet.Setup(Settings.SmoothCameraFactorOption, Settings.IsSmoothCameraEnabled);
                cameraSmoothingSet.OnCheckboxUpdated += CameraSmoothingSet_OnCheckboxUpdated;
                cameraSmoothingSet.OnSliderValueUpdated += CameraSmoothingSet_OnSliderValueUpdated;
                cameraSmoothingSet.OnSliderReleaseUpdated += CameraSmoothingSet_OnSliderValueUpdated;
            }
        }
        #endregion
    }
}
