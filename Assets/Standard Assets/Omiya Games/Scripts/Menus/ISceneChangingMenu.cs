using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ISceneChangingMenu.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
    /// A common interface for menus that swithes scenes.
    /// </summary>
    public abstract class ISceneChangingMenu : IMenu
    {
        [Header("Components")]
        [SerializeField]
        protected Button defaultButton = null;
        [SerializeField]
        protected Text completeLabel = null;
        [SerializeField]
        protected Text failedLabel = null;
        [SerializeField]
        protected Text nextSceneLabel = null;
        [SerializeField]
        protected Text restartLabel = null;
        [SerializeField]
        protected Text returnToMenuLabel = null;

        abstract public bool PauseOnShow
        {
            get;
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
                return defaultButton.gameObject;
            }
        }

        protected virtual void Start()
        {
            // Update the labels on each button
            MenuManager manager = Singleton.Get<MenuManager>();

            // Setup all labels, if available
            if (completeLabel != null)
            {
                completeLabel.text = manager.CompletedCurrentSceneText;
            }
            if (failedLabel != null)
            {
                failedLabel.text = manager.FailedCurrentSceneText;
            }
            if (nextSceneLabel != null)
            {
                nextSceneLabel.text = manager.NextSceneText;
            }
            if (restartLabel != null)
            {
                restartLabel.text = manager.RestartCurrentSceneText;
            }
            if (returnToMenuLabel != null)
            {
                returnToMenuLabel.text = manager.ReturnToMenuText;
            }
        }

        public override void Show(System.Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Unlock the cursor
            Cursor.lockState = CursorLockMode.None;

            // Check if we should stop time
            if (PauseOnShow == true)
            {
                // Stop time
                Singleton.Get<TimeManager>().IsManuallyPaused = true;
            }
        }

        public override void Hide()
        {
            // Call base function
            base.Hide();

            // Lock the cursor to what the scene is set to
            Cursor.lockState = Singleton.Get<SceneManager>().CurrentScene.LockMode;

            // Check if we should stop time
            if (PauseOnShow == true)
            {
                // Resume the time
                Singleton.Get<TimeManager>().IsManuallyPaused = false;
            }
        }

        public void OnRestartClicked()
        {
            Hide();

            // Transition to the current level
            Singleton.Get<SceneManager>().ReloadCurrentScene();
        }

        public void OnReturnToMenuClicked()
        {
            Hide();

            // Transition to the menu
            Singleton.Get<SceneManager>().LoadMainMenu();
        }
    }
}