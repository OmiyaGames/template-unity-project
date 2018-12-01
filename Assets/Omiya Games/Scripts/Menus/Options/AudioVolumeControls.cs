using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AudioVolumeControls.cs" company="Omiya Games">
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
    /// A helper script to handle audio slider and checkbox.
    /// </summary>
    [DisallowMultipleComponent]
    public class AudioVolumeControls : MonoBehaviour
    {
        public event System.Action<bool> OnCheckboxUpdated;
        public event System.Action<float> OnSliderValueUpdated;
        public event System.Action<float> OnSliderReleaseUpdated;

        [SerializeField]
        Slider slider;
        [SerializeField]
        Toggle checkbox;

        [Header("Slider info")]
        [SerializeField]
        bool conditionToEnableSlider = false;
        [SerializeField]
        float defaultValue = 0;

        #region Properties
        public Slider Slider
        {
            get
            {
                return slider;
            }
            set
            {
                slider = value;
            }
        }

        public Toggle Checkbox
        {
            get
            {
                return checkbox;
            }
            set
            {
                checkbox = value;
            }
        }

        public float Value
        {
            get
            {
                float returnValue = defaultValue;
                if(Slider.IsInteractable() == true)
                {
                    returnValue = Slider.value;
                }
                return returnValue;
            }
        }

        bool IsListeningToEvents
        {
            get;
            set;
        } = true;
        #endregion

        public void Setup(float volumeNormalized, bool isMute)
        {
            // Update state
            IsListeningToEvents = false;

            // Setup controls
            Slider.value = volumeNormalized;
            Checkbox.isOn = isMute;
            UpdateSliderInteractable(isMute);

            // Update state
            IsListeningToEvents = true;
        }

        public void OnCheckboxChanged(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                UpdateSliderInteractable(isChecked);
                OnCheckboxUpdated?.Invoke(isChecked);
            }
        }

        public void OnSliderReleased()
        {
            if (IsListeningToEvents == true)
            {
                OnSliderReleaseUpdated?.Invoke(Slider.value);
            }
        }

        public void OnSliderValueChanged(float newValue)
        {
            if (IsListeningToEvents == true)
            {
                OnSliderReleaseUpdated?.Invoke(newValue);
            }
        }

        private void UpdateSliderInteractable(bool isChecked)
        {
            Slider.interactable = (conditionToEnableSlider == isChecked);
        }
    }
}
