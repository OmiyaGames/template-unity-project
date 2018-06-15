using UnityEngine;
using UnityEngine.UI;

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
        #region Serialized Fields
        [Header("Features to Enable")]
        [SerializeField]
        SupportedPlatforms enableKeyboardControls = SupportedPlatforms.AllPlatforms;
        [SerializeField]
        SupportedPlatforms enableMouseControls = SupportedPlatforms.AllPlatforms;
        [SerializeField]
        SupportedPlatforms enableScrollWheelControls = SupportedPlatforms.AllPlatforms;

        [Header("Keyboard Sensitivity")]
        [SerializeField]
        ControllerSensitivityControls keyboardSensitivitySet;
        [SerializeField]
        GameObject keyboardToMouseDivider = null;

        [Header("Mouse Sensitivity")]
        [SerializeField]
        ControllerSensitivityControls mouseSensitivitySet;
        [SerializeField]
        GameObject MouseToScrollWheelDivider = null;

        [Header("Scroll Wheel")]
        [SerializeField]
        LabeledSlider scrollWheelSensitivity;
        [SerializeField]
        Toggle scrollWheelInvert;
        [SerializeField]
        GameObject[] scrollWheelControls;
        #endregion

        #region Properties
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
                if(enableKeyboardControls.IsThisBuildSupported() == true)
                {
                    return keyboardSensitivitySet.DefaultGameObject;
                }
                else if (enableMouseControls.IsThisBuildSupported() == true)
                {
                    return mouseSensitivitySet.DefaultGameObject;
                }
                else
                {
                    return scrollWheelSensitivity.gameObject;
                }
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Setup controls
            base.OnSetup();

            // Update how keyboard controls are enabled
            SetupKeyboardSensitivityControls();

            // Toggle displaying the divider
            keyboardToMouseDivider.SetActive(enableKeyboardControls.IsThisBuildSupported());

            // Update how mouse controls are enabled
            SetupMouseSensitivityControls();

            // Toggle displaying the divider
            keyboardToMouseDivider.SetActive(enableMouseControls.IsThisBuildSupported() && enableScrollWheelControls.IsThisBuildSupported());

            // Update how scroll wheel controls are enabled
            SetupScrollWheelControls();
        }

        #region UI events
        #region Keyboard Controls
        void KeyboardSensitivitySet_OnInvertVerticalAxisCheckboxChanged(ControllerSensitivityControls source, bool isChecked)
        {
            if(IsListeningToEvents == true)
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
        void MouseSensitivitySet_OnInvertVerticalAxisCheckboxChanged(ControllerSensitivityControls source, bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                Settings.IsMouseYAxisInverted = isChecked;
            }
        }

        void MouseSensitivitySet_OnInvertHorizontalAxisCheckboxChanged(ControllerSensitivityControls source, bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                Settings.IsMouseXAxisInverted = isChecked;
            }
        }

        void MouseSensitivitySet_OnVerticalSensitivitySlider(ControllerSensitivityControls source, float value)
        {
            if (IsListeningToEvents == true)
            {
                Settings.MouseYAxisSensitivity = value;
            }
        }

        void MouseSensitivitySet_OnHorizontalSensitivitySlider(ControllerSensitivityControls source, float value)
        {
            if (IsListeningToEvents == true)
            {
                Settings.MouseXAxisSensitivity = value;
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

        void SetupMouseSensitivityControls()
        {
            // Check whether to show the keyboard controls set or not
            bool enabled = enableMouseControls.IsThisBuildSupported();

            // Toggle the display of the controls
            mouseSensitivitySet.gameObject.SetActive(enabled);

            // Check if enabled
            if (enabled == true)
            {
                // If so, setup the UI
                mouseSensitivitySet.Setup(Settings.MouseXAxisSensitivity, Settings.MouseYAxisSensitivity, Settings.IsMouseAxisSensitivitySplit, Settings.IsMouseXAxisInverted, Settings.IsMouseYAxisInverted);

                // Bind to the events on any changes to the control set
                mouseSensitivitySet.OnBothAxisSensitivitySlider += MouseSensitivitySet_OnHorizontalSensitivitySlider;
                mouseSensitivitySet.OnHorizontalSensitivitySlider += MouseSensitivitySet_OnHorizontalSensitivitySlider;
                mouseSensitivitySet.OnVerticalSensitivitySlider += MouseSensitivitySet_OnVerticalSensitivitySlider;
                mouseSensitivitySet.OnInvertHorizontalAxisCheckboxChanged += MouseSensitivitySet_OnInvertHorizontalAxisCheckboxChanged;
                mouseSensitivitySet.OnInvertVerticalAxisCheckboxChanged += MouseSensitivitySet_OnInvertVerticalAxisCheckboxChanged;
            }
        }

        void SetupScrollWheelControls()
        {
            // Check whether to show the keyboard controls set or not
            bool enabled = enableMouseControls.IsThisBuildSupported();

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
        #endregion
    }
}
