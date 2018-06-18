﻿using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Audio;
using OmiyaGames.Settings;
using OmiyaGames.Translations;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsAccessibilityMenu.cs" company="Omiya Games">
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
    /// Menu that provides accessibility options.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public class OptionsAccessibilityMenu : IMenu
    {
        #region Serialized Fields
        [Header("Features to Enable")]
        [SerializeField]
        SupportedPlatforms enableTextSize = SupportedPlatforms.AllPlatforms;
        [SerializeField]
        SupportedPlatforms enableTimeScale = SupportedPlatforms.AllPlatforms;
        [SerializeField]
        GameObject[] dividers;

        [Header("Text Size Controls")]
        [SerializeField]
        float defaultTextSize = 1f;
        [SerializeField]
        Slider textSizeSlider;
        [SerializeField]
        Button resetTextSize;

        [Header("Time Scale Controls")]
        [SerializeField]
        float defaultTimeScale = 1f;
        [SerializeField]
        AudioVolumeControls timeScaleSlider;
        [SerializeField]
        Button resetTimeScale;
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
                GameObject returnUi = null;
                if (enableTextSize.IsThisBuildSupported() == true)
                {
                    returnUi = textSizeSlider.gameObject;
                }
                else if (enableTimeScale.IsThisBuildSupported() == true)
                {
                    returnUi = timeScaleSlider.Checkbox.gameObject;
                }
                return returnUi;
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // FIXME: setup sliders
            //timeScaleSlider.Setup();

            // Setup reset buttons
            // FIXME: fix these arguments to match the settings.
            //UpdateResetTimeScaleButton(false, defaultTimeScale);
            //UpdateResetTextSizeButton(defaultTextSize);

            // Setup control events
            timeScaleSlider.OnCheckboxUpdated += TimeScaleSlider_OnCheckboxUpdated;
            timeScaleSlider.OnSliderValueUpdated += TimeScaleSlider_OnSliderValueUpdated;
            timeScaleSlider.OnSliderReleaseUpdated += TimeScaleSlider_OnSliderValueUpdated;

            // Setup dividers
            OptionsMenuUtility.SetupDividers(dividers, enableTextSize, enableTimeScale);
        }

        #region UI events
        public void OnResetTextSizeClicked()
        {
            if (IsListeningToEvents == true)
            {
                // Reset the text size slider to default
                textSizeSlider.value = defaultTextSize;
            }
        }

        public void OnResetTimeScaleClicked()
        {
            if (IsListeningToEvents == true)
            {
                // Reset the time scale slider to default
                timeScaleSlider.Slider.value = defaultTimeScale;
                timeScaleSlider.Checkbox.isOn = false;
            }
        }

        public void OnTextSizeSliderChanged(float percent)
        {
            if (IsListeningToEvents == true)
            {
                // Check slider value
                if (Mathf.Approximately(percent, defaultTextSize) == false)
                {
                    // FIXME: Set the text size on TextSizeResizer
                }
                else
                {
                    // FIXME: Set the text size on TextSizeResizer to default
                }

                // Check slider value
                UpdateResetTextSizeButton(percent);
            }
        }

        void TimeScaleSlider_OnCheckboxUpdated(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update the reset time scale button
                UpdateResetTimeScaleButton(isChecked, timeScaleSlider.Slider.value);
            }
        }

        void TimeScaleSlider_OnSliderValueUpdated(float percent)
        {
            if (IsListeningToEvents == true)
            {
                // Check slider value
                if (Mathf.Approximately(percent, defaultTimeScale) == false)
                {
                    // FIXME: Set the time scale on time manager to slider
                }
                else
                {
                    // FIXME: Set the time scale on time manager to default
                }

                // Update the reset time scale button
                UpdateResetTimeScaleButton(timeScaleSlider.Checkbox.isOn, percent);
            }
        }
        #endregion

        #region Helper Methods
        void UpdateResetTimeScaleButton(bool isChecked, float percent)
        {
            // Check if this checkbox is checked, and the slider is of a different value from default
            resetTimeScale.interactable = ((isChecked == true) && (Mathf.Approximately(percent, defaultTextSize) == false));
        }

        void UpdateResetTextSizeButton(float percent)
        {
            // Check if the slider is of a different value from default
            resetTextSize.interactable = (Mathf.Approximately(percent, defaultTextSize) == false);
        }
        #endregion
    }
}
