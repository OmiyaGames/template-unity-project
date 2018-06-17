using UnityEngine;
using UnityEngine.UI;

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
        [System.Serializable]
        public struct ToggleSet
        {
            [SerializeField]
            SupportedPlatforms enableFor;
            [SerializeField]
            Toggle checkbox;
            [SerializeField]
            GameObject[] parents;

            public SupportedPlatforms EnableFor
            {
                get
                {
                    return enableFor;
                }
            }

            public Toggle Checkbox
            {
                get
                {
                    return checkbox;
                }
            }

            public GameObject[] Parents
            {
                get
                {
                    return parents;
                }
            }

            public bool IsEnabled
            {
                get
                {
                    return EnableFor.IsThisBuildSupported();
                }
            }

            public void Setup()
            {
                bool isActive = IsEnabled;
                if((parents != null) && (parents.Length > 0))
                {
                    foreach(GameObject parent in parents)
                    {
                        parent.SetActive(isActive);
                    }
                }
                else
                {
                    Checkbox.gameObject.SetActive(isActive);
                }
            }
        }

        #region Serialized Fields
        [Header("Special Effects Controls")]
        [SerializeField]
        ToggleSet cameraShakeControls;
        [SerializeField]
        ToggleSet bobbingCameraControls;
        [SerializeField]
        ToggleSet motionBlursControls;
        [SerializeField]
        ToggleSet screenFlashesControls;
        [SerializeField]
        ToggleSet bloomControls;
        #endregion

        ToggleSet[] allControls = null;

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
                foreach(ToggleSet controls in AllControls)
                {
                    if(controls.IsEnabled == true)
                    {
                        returnUi = controls.Checkbox.gameObject;
                        break;
                    }
                }
                return returnUi;
            }
        }

        ToggleSet[] AllControls
        {
            get
            {
                if(allControls == null)
                {
                    allControls = new ToggleSet[]
                    {
                        cameraShakeControls,
                        bobbingCameraControls,
                        motionBlursControls,
                        screenFlashesControls,
                        bloomControls
                    };
                }
                return allControls;
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // Call setup on all controls
            foreach (ToggleSet controls in AllControls)
            {
                controls.Setup();
            }

            // Setup checkbox isOn state
            cameraShakeControls.Checkbox.isOn = Settings.IsCameraShakesEnabled;
            bobbingCameraControls.Checkbox.isOn = Settings.IsHeadBobbingOptionEnabled;
            motionBlursControls.Checkbox.isOn = Settings.IsMotionBlursEnabled;
            screenFlashesControls.Checkbox.isOn = Settings.IsScreenFlashesEnabled;
            bloomControls.Checkbox.isOn = Settings.IsBloomEffectEnabled;

            // Setup checkbox interactable state
            bobbingCameraControls.Checkbox.interactable = cameraShakeControls.Checkbox.isOn;

            // Setup whether to disable bobbing camera or not
            bobbingCameraControls.Checkbox.interactable = cameraShakeControls.Checkbox.isOn;
        }

        #region UI Events
        public void OnCameraShakeClicked(bool isChecked)
        {
            if(IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsCameraShakesEnabled = isChecked;

                // Update bobbing head interactable state
                bobbingCameraControls.Checkbox.interactable = isChecked;
            }
        }

        public void OnHeadBobbingClicked(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsHeadBobbingOptionEnabled = isChecked;
            }
        }

        public void OnMotionBlursClicked(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsMotionBlursEnabled = isChecked;
            }
        }

        public void OnScreenFlashesClicked(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsScreenFlashesEnabled = isChecked;
            }
        }

        public void OnBloomClicked(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsBloomEffectEnabled = isChecked;
            }
        }
        #endregion
    }
}
