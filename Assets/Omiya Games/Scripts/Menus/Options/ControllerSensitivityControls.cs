using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ControllerSensitivityControls.cs" company="Omiya Games">
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
    /// A helper script to handle controller sensitivity sliders and checkbox combination.
    /// </summary>
    [DisallowMultipleComponent]
    public class ControllerSensitivityControls : MonoBehaviour
    {
        public event System.Action<ControllerSensitivityControls, float> OnHorizontalSensitivitySlider;
        public event System.Action<ControllerSensitivityControls, float> OnVerticalSensitivitySlider;
        public event System.Action<ControllerSensitivityControls, float> OnBothAxisSensitivitySlider;
        public event System.Action<ControllerSensitivityControls, bool> OnSplitAxisCheckboxChanged;
        public event System.Action<ControllerSensitivityControls, bool> OnInvertHorizontalAxisCheckboxChanged;
        public event System.Action<ControllerSensitivityControls, bool> OnInvertVerticalAxisCheckboxChanged;

        [Header("Sensitivity Controls")]
        [SerializeField]
        Slider horizontalSensitivitySlider;
        [SerializeField]
        Slider verticalSensitivitySlider;
        [SerializeField]
        Slider bothSensitivitySlider;
        [SerializeField]
        Toggle splitAxisCheckbox;

        [Header("Sensitivity Controls")]
        [SerializeField]
        Toggle invertHorizontalAxisCheckbox;
        [SerializeField]
        Toggle invertVerticalAxisCheckbox;

        #region Properties
        bool IsListeningToEvents
        {
            get;
            set;
        } = true;

        public Slider HorizontalSensitivitySlider
        {
            get
            {
                return horizontalSensitivitySlider;
            }

            set
            {
                horizontalSensitivitySlider = value;
            }
        }

        public Slider VerticalSensitivitySlider
        {
            get
            {
                return verticalSensitivitySlider;
            }

            set
            {
                verticalSensitivitySlider = value;
            }
        }

        public Slider BothSensitivitySlider
        {
            get
            {
                return bothSensitivitySlider;
            }

            set
            {
                bothSensitivitySlider = value;
            }
        }

        public Toggle SplitAxisCheckbox
        {
            get
            {
                return splitAxisCheckbox;
            }

            set
            {
                splitAxisCheckbox = value;
            }
        }

        public Toggle InvertHorizontalAxisCheckbox
        {
            get
            {
                return invertHorizontalAxisCheckbox;
            }

            set
            {
                invertHorizontalAxisCheckbox = value;
            }
        }

        public Toggle InvertVerticalAxisCheckbox
        {
            get
            {
                return invertVerticalAxisCheckbox;
            }

            set
            {
                invertVerticalAxisCheckbox = value;
            }
        }

        public GameObject DefaultGameObject
        {
            get
            {
                GameObject defaultSlider = BothSensitivitySlider.gameObject;
                if (SplitAxisCheckbox.isOn == true)
                {
                    defaultSlider = HorizontalSensitivitySlider.gameObject;
                }
                return defaultSlider;
            }
        }
        #endregion

        public void Setup(float xAxisSensitivity, float yAxisSensitivity, bool splitAxisSensitivity, bool invertXAxis, bool invertYAxis)
        {
            // Update state
            IsListeningToEvents = false;

            // Update the sensitivity toggle first
            ToggleSensitivityControls(splitAxisSensitivity, false);

            // Update the sensitivity sliders
            BothSensitivitySlider.value = xAxisSensitivity;
            HorizontalSensitivitySlider.value = xAxisSensitivity;
            VerticalSensitivitySlider.value = yAxisSensitivity;

            // Update the inverted checkbox
            InvertHorizontalAxisCheckbox.isOn = invertXAxis;
            InvertVerticalAxisCheckbox.isOn = invertYAxis;

            // Update state
            IsListeningToEvents = true;
        }

        #region Button Events
        public void OnHorizontalSensitivityChanged(float value)
        {
            if (IsListeningToEvents == true)
            {
                OnHorizontalSensitivitySlider?.Invoke(this, value);
            }
        }

        public void OnVerticalSensitivityChanged(float value)
        {
            if (IsListeningToEvents == true)
            {
                OnVerticalSensitivitySlider?.Invoke(this, value);
            }
        }

        public void OnBothAxisSensitivityChanged(float value)
        {
            if (IsListeningToEvents == true)
            {
                OnBothAxisSensitivitySlider?.Invoke(this, value);
            }
        }

        public void OnSplitAxisCheckboxToggled(bool isChecked)
        {
            if(IsListeningToEvents == true)
            {
                ToggleSensitivityControls(isChecked, true);
                OnSplitAxisCheckboxChanged?.Invoke(this, isChecked);
            }
        }

        public void OnInvertHorizontalAxisCheckboxToggled(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                OnInvertHorizontalAxisCheckboxChanged?.Invoke(this, isChecked);
            }
        }

        public void OnInvertVerticalAxisCheckboxToggled(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                OnInvertVerticalAxisCheckboxChanged?.Invoke(this, isChecked);
            }
        }
        #endregion

        #region Helper Methods
        private void ToggleSensitivityControls(bool isChecked, bool updateSliders)
        {
            // Toggle activating controls
            HorizontalSensitivitySlider.gameObject.SetActive(isChecked == true);
            VerticalSensitivitySlider.gameObject.SetActive(isChecked == true);
            BothSensitivitySlider.gameObject.SetActive(isChecked == false);

            // Check if slider values needs to be updated as well
            if (updateSliders == true)
            {
                // Check the direction to update
                if (isChecked == true)
                {
                    // If we're splitting axis, make horizontal and vertical match the value of "both axis" slider
                    VerticalSensitivitySlider.value = BothSensitivitySlider.value;
                    HorizontalSensitivitySlider.value = BothSensitivitySlider.value;
                }
                else
                {
                    // If we're combining the axis, set the "both axis" slider the same value as the horizontal slider
                    BothSensitivitySlider.value = HorizontalSensitivitySlider.value;
                }
            }
        }
        #endregion
    }
}
