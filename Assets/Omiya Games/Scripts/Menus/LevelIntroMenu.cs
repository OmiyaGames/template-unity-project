using UnityEngine;
using UnityEngine.UI;
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
            OnlyIfMouseIsLocked,
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
        [UnityEngine.Serialization.FormerlySerializedAs("defaultButton")]
        Button startButton = null;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("messageLabel")]
        TranslatedTextMeshPro customMessageLabel = null;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("WebGlNoteLabel")]
        TranslatedTextMeshPro mouseLockMessageLabel = null;

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
                Type returnType = Type.ManagedMenu;
                if (CurrentSettings.StartState != StateOnStart.Hidden)
                {
                    returnType = Type.DefaultManagedMenu;
                }
                return returnType;
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

        public PlatformSettings CurrentSettings
        {
            get
            {
                if (cachedSettings == null)
                {
                    cachedSettings = defaultSettings;
                    if ((otherPlatformSettings != null) && (otherPlatformSettings.Length > 0))
                    {
                        foreach (CustomPlatformSettings settings in otherPlatformSettings)
                        {
                            // FIXME: check the mouse state on WebGL...somehow
                            if ((settings != null) && (settings.Platform.IsThisBuildSupported() == true))
                            {
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
        }

        void OnHide()
        {
            // Check if we should stop time
            if (CurrentSettings.StartState == StateOnStart.PauseOnStart)
            {
                // Resume the time
                Singleton.Get<TimeManager>().IsManuallyPaused = false;

                // Update the cursor mode
                if ((SceneChanger != null) && (SceneChanger.CurrentScene != null))
                {
                    Scenes.SceneTransitionManager.CursorMode = SceneChanger.CurrentScene.LockMode;
                }
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
    }
}
