using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Global;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="StartMenu.cs" company="Omiya Games">
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
    /// Menu that appears on the start of the game, allowing you to change options,
    /// select a level, or quit the game. You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public class StartMenu : ISceneChangingMenu
    {
        public enum LevelSelectButtonBehavior
        {
            DefaultStartFirstLevel,
            AlwaysShowLevelSelect,
            AlwaysStartFirstLevel
        }

        [Header("Start Menu")]
        [SerializeField]
        private LevelSelectButtonBehavior startBehavior = LevelSelectButtonBehavior.DefaultStartFirstLevel;
        [SerializeField]
        private Button creditsButton;
        [SerializeField]
        private Button quitButton;
        [SerializeField]
        private string titleTranslationKey = "Title";

        #region Properties
        public override Type MenuType
        {
            get
            {
                return Type.DefaultManagedMenu;
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return background;
            }
        }

        public override string TitleTranslationKey
        {
            get
            {
                return titleTranslationKey;
            }
        }

        public override bool PauseOnShow
        {
            get
            {
                return false;
            }
        }

        bool IsStartingOnFirstLevel
        {
            get
            {
                // Use the startBehavior and game settings to return the proper flag
                switch(startBehavior)
                {
                    case LevelSelectButtonBehavior.AlwaysStartFirstLevel:
                        // Always return true if we're supposed to always start the first level
                        return true;
                    case LevelSelectButtonBehavior.DefaultStartFirstLevel:
                        // Return true if we haven't unlocked any levels
                        return (Settings.NumLevelsUnlocked <= Settings.DefaultNumLevelsUnlocked);
                    default:
                        return false;
                }
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Call the base method
            base.OnSetup();

            // Check if we should remove the quit button (you can't quit out of a webplayer)
            if (Singleton.Instance.IsWebApp == true)
            {
                // Disable the quit button entirely
                quitButton.gameObject.SetActive(false);
            }

            // Setup default button
            SetupStartButton();
        }

        public void SetupStartButton()
        {
            // Setup the start button
            if (IsStartingOnFirstLevel == true)
            {
                // Update which button to activate
                defaultButton.gameObject.SetActive(true);
                levelSelectButton.gameObject.SetActive(false);

                // Select the start button by default
                CurrentDefaultUi = defaultButton;
            }
            else
            {
                // Update which button to activate
                defaultButton.gameObject.SetActive(false);
                levelSelectButton.gameObject.SetActive(true);

                // Select the level select button by default
                CurrentDefaultUi = levelSelectButton;
            }
        }

        #region Button Events
        public void OnStartClicked()
        {
            // Make sure the menu is active
            if (IsListeningToEvents == true)
            {
                // Load the first level automatically
                SceneChanger.LoadNextLevel();

                // Indicate button is clicked
                CurrentDefaultUi = defaultButton;
            }
        }

        public void OnCreditsClicked()
        {
            // Make sure the menu is active
            if (IsListeningToEvents == true)
            {
                // Transition to the credits
                SceneChanger.LoadScene(SceneChanger.Credits);

                // Indicate we've clicked on a button
                CurrentDefaultUi = creditsButton;

                // Since we're changing scenes, forcefully prevent
                // the other buttons from listening to the events.
                IsListeningToEvents = false;
            }
        }

        public void OnQuitClicked()
        {
            // Make sure the menu is active
            if (IsListeningToEvents == true)
            {
                // Quit the application
                Application.Quit();

                // Indicate we've clicked on a button
                CurrentDefaultUi = quitButton;

                // Since we're closing the application, forcefully prevent
                // the other buttons from listening to the events.
                IsListeningToEvents = false;
            }
        }
        #endregion
    }
}
