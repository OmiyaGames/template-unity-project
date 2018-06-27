using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using OmiyaGames.Translations;
using OmiyaGames.Global;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LevelIntroMenu.cs" company="Omiya Games">
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu indicating important information before starting a level. You can retrieve
    /// this menu from the singleton script, <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    public class LevelIntroMenu : IMenu
    {
        public enum StateOnStart
        {
            Hidden,
            PauseOnStart,
            DisplayForDuration
        }

        public enum MouseState
        {
            Both,
            OnlyIfMouseIsUnlocked
        }

        #region Helper Containers
        [System.Serializable]
        public class PlatformSettings
        {
            [SerializeField]
            bool showCustomMessage;
            [SerializeField]
            bool showMouseLockMessageLabel;
            [SerializeField]
            BackgroundMenu.BackgroundType background;
            [SerializeField]
            [UnityEngine.Serialization.FormerlySerializedAs("startState")]
            StateOnStart startState;
            [SerializeField]
            [Range(0.1f, 20f)]
            float displayDuration;

            public StateOnStart StartState
            {
                get
                {
                    return startState;
                }
            }

            public bool ShowCustomMessage
            {
                get
                {
                    return showCustomMessage;
                }
            }

            public bool ShowMouseLockMessageLabel
            {
                get
                {
                    return showMouseLockMessageLabel;
                }
            }

            public float DisplayDuration
            {
                get
                {
                    return displayDuration;
                }
            }

            public BackgroundMenu.BackgroundType Background
            {
                get
                {
                    return background;
                }
            }
        }

        [System.Serializable]
        public class CustomPlatformSettings : PlatformSettings
        {
            [SerializeField]
            SupportedPlatforms platform;
            [SerializeField]
            MouseState mouseLockState;

            public SupportedPlatforms Platform
            {
                get
                {
                    return platform;
                }
            }

            public MouseState MouseLockState
            {
                get
                {
                    return mouseLockState;
                }
            }
        }
        #endregion

        [Header("Components")]
        [SerializeField]
        TranslatedTextMeshPro levelNameLabel = null;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("messageLabel")]
        TranslatedTextMeshPro customMessageLabel = null;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("WebGlNoteLabel")]
        TranslatedTextMeshPro mouseLockMessageLabel = null;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("defaultButton")]
        Button startButton = null;

        [Header("Platform Settings")]
        [SerializeField]
        PlatformSettings defaultSettings;
        [SerializeField]
        CustomPlatformSettings[] otherPlatformSettings;

        System.Action<float> checkAnyKey = null;
        PlatformSettings cachedSettings = null;

        #region Properties
        public override Type MenuType
        {
            get
            {
                switch(CurrentSettings.StartState)
                {
                    case StateOnStart.DisplayForDuration:
                    case StateOnStart.PauseOnStart:
                        return Type.DefaultManagedMenu;
                    default:
                        return Type.ManagedMenu;
                }
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                GameObject returnObject = null;
                if (CurrentSettings.StartState == StateOnStart.PauseOnStart)
                {
                    returnObject = startButton.gameObject;
                }
                return returnObject;
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return CurrentSettings.Background;
            }
        }

        public override bool IsPausingEnabledWhileVisible
        {
            get
            {
                return true;
            }
        }

        public override bool IsPopUpEnabledWhileVisible
        {
            get
            {
                return true;
            }
        }

        public PlatformSettings CurrentSettings
        {
            get
            {
                if (cachedSettings == null)
                {
                    // Grab default setting
                    cachedSettings = defaultSettings;

                    // Check if there are any other settings
                    if ((otherPlatformSettings != null) && (otherPlatformSettings.Length > 0))
                    {
                        // Go through all settings
                        foreach (CustomPlatformSettings settings in otherPlatformSettings)
                        {
                            // Verify if this settings is the current platform
                            if (IsSettingsApplicable(settings) == true)
                            {
                                // Return this setting immediately
                                cachedSettings = settings;
                                break;
                            }
                        }
                    }
                }
                return cachedSettings;
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // Setup all labels, if available
            if ((levelNameLabel != null) && (SceneChanger.CurrentScene != null))
            {
                levelNameLabel.TranslationKey = SceneChanger.CurrentScene.DisplayName.TranslationKey;
            }

            // Show the start button if pause is implemented.
            startButton.gameObject.SetActive(CurrentSettings.StartState == StateOnStart.PauseOnStart);

            // Show the custom message if pause is implemented.
            customMessageLabel.gameObject.SetActive(CurrentSettings.ShowCustomMessage);

            // Show the custom message if pause is implemented.
            mouseLockMessageLabel.gameObject.SetActive(CurrentSettings.ShowMouseLockMessageLabel);
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Call base method
            base.OnStateChanged(from, to);

            // Unbind to Singleton's update function
            StopListeningToUpdate();
            if (to == VisibilityState.Visible)
            {
                OnShow();
            }
            else if (to == VisibilityState.Hidden)
            {
                OnHide();
            }
        }

        void OnShow()
        {
            // Check if we should stop time
            if (CurrentSettings.StartState == StateOnStart.PauseOnStart)
            {
                // Stop time
                Singleton.Get<TimeManager>().IsManuallyPaused = true;

                // Bind to Singleton's update function
                checkAnyKey = new System.Action<float>(CheckForAnyKey);
                Singleton.Instance.OnUpdate += checkAnyKey;
            }
            else
            {
                // Since no interactive buttons are shown on-screen, revert cursor mode
                SceneChanger.RevertCursorLockMode();

                // Check if we want to auto-hide
                if (CurrentSettings.StartState == StateOnStart.DisplayForDuration)
                {
                    StartCoroutine(DelayCallingHide(CurrentSettings.DisplayDuration));
                }
            }
        }

        void OnHide()
        {
            // Check if we should stop time
            if (CurrentSettings.StartState == StateOnStart.PauseOnStart)
            {
                // Resume the time
                Singleton.Get<TimeManager>().IsManuallyPaused = false;
            }
        }

        void StopListeningToUpdate()
        {
            // Check if the action is not null
            if (checkAnyKey != null)
            {
                // Remove the binding to Singleton's update function
                Singleton.Instance.OnUpdate -= checkAnyKey;

                // Set the action to null
                checkAnyKey = null;
            }
        }

        void CheckForAnyKey(float deltaTime)
        {
            if ((IsListeningToEvents == true) && (Input.anyKeyDown == true))
            {
                Hide();
            }
        }

        bool IsSettingsApplicable(CustomPlatformSettings settings)
        {
            // Check setting is not null, and supported
            bool returnFlag = false;
            if ((settings != null) && (settings.Platform.IsThisBuildSupported() == true))
            {
                if (settings.MouseLockState == MouseState.Both)
                {
                    // Switch the flag to true
                    returnFlag = true;
                }
                else
                {
                    // Check if the current scene is supposed to have the cursor locked, and wasn't locked from the previous scene
                    returnFlag = ((SceneChanger != null) && (SceneChanger.CurrentScene != null) && (SceneChanger.CurrentScene.LockMode == CursorLockMode.Locked) && (Scenes.SceneTransitionManager.LastCursorMode != CursorLockMode.Locked));
                }
            }
            return returnFlag;
        }

        IEnumerator DelayCallingHide(float autoHideAfterSeconds)
        {
            // Wait for a couple of seconds
            yield return new WaitForSeconds(autoHideAfterSeconds);

            // Make sure we're not in standby, if somehow this menu ended up in the MenuManager stack
            while(CurrentVisibility == VisibilityState.StandBy)
            {
                yield return null;
            }

            // Hide the menu.
            Hide();
        }
    }
}
