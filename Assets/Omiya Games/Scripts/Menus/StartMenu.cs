using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    using Settings;

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
    public class StartMenu : IMenu
    {
        public enum LevelSelectButtonBehavior
        {
            DefaultStartFirstLevel,
            AlwaysShowLevelSelect,
            AlwaysStartFirstLevel
        }

        [Header("Start Menu")]
        [SerializeField]
        LevelSelectButtonBehavior startBehavior = LevelSelectButtonBehavior.DefaultStartFirstLevel;

        [Header("Buttons")]
        [SerializeField]
        Button startButton;
        [SerializeField]
        Button levelSelectButton;
        [SerializeField]
        Button howToPlayButton;
        [SerializeField]
        Button optionsButton;
        [SerializeField]
        Button highScoresButton;
        [SerializeField]
        Button creditsButton;
        [SerializeField]
        Button quitButton;

        GameObject defaultButton = null;
        bool isButtonLocked = false;

        public override Type MenuType
        {
            get
            {
                return Type.DefaultManagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return defaultButton;
            }
        }

        GameSettings Settings
        {
            get
            {
                return Singleton.Get<GameSettings>();
            }
        }

        bool IsStartingOnFirstLevel
        {
            get
            {
                // Use the startBehavior and game settings to return the proper flag
                bool returnFlag = false;
                switch(startBehavior)
                {
                    case LevelSelectButtonBehavior.AlwaysStartFirstLevel:
                        // Always return true if we're supposed to always start the first level
                        returnFlag = true;
                        break;
                    case LevelSelectButtonBehavior.DefaultStartFirstLevel:
                        // Return true if we haven't unlocked any levels
                        returnFlag = (Settings.NumLevelsUnlocked <= Settings.DefaultNumLevelsUnlocked);
                        break;
                }
                return returnFlag;
            }
        }

        void Start()
        {
            // Check if we should remove the quit button (you can't quit out of a webplayer)
            if (Singleton.Instance.IsWebApp == true)
            {
                // Disable the quit button entirely
                quitButton.gameObject.SetActive(false);
            }

            // Select the level select button by default
            defaultButton = levelSelectButton.gameObject;
            Singleton.Get<MenuManager>().SelectGuiGameObject(defaultButton);

            // Update Select
            startButton.gameObject.SetActive(IsStartingOnFirstLevel);
            levelSelectButton.gameObject.SetActive(IsStartingOnFirstLevel == false);
        }

        #region Button Events
        public void OnStartClicked()
        {
            if (isButtonLocked == false)
            {
                // Load the first level automatically
                Singleton.Get<SceneTransitionManager>().LoadNextLevel();

                // Indicate button is clicked
                defaultButton = startButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnLevelSelectClicked()
        {
            if (isButtonLocked == false)
            {
                // Open the Level Select menu
                Manager.Show<LevelSelectMenu>();

                // Indicate we've clicked on a button
                defaultButton = levelSelectButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnOptionsClicked()
        {
            if (isButtonLocked == false)
            {
                // Open the options menu
                Manager.Show<OptionsListMenu>();

                // Indicate we've clicked on a button
                defaultButton = optionsButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnCreditsClicked()
        {
            if (isButtonLocked == false)
            {
                // Transition to the credits
                Singleton.Get<SceneTransitionManager>().LoadScene(Singleton.Get<SceneTransitionManager>().Credits);

                // Change the menu to stand by
                CurrentState = State.StandBy;

                // Indicate we've clicked on a button
                defaultButton = creditsButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnQuitClicked()
        {
            if (isButtonLocked == false)
            {
                // Quit the application
                Application.Quit();

                // Leave the menu as-is
                // CurrentState = State.StandBy;

                // Indicate we've clicked on a button
                defaultButton = quitButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnHowToPlayClicked()
        {
            if (isButtonLocked == false)
            {
                // Open the how to play menu
                Manager.Show<HowToPlayMenu>();

                // Indicate we've clicked on a button
                defaultButton = howToPlayButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnHighScoresClicked()
        {
            if (isButtonLocked == false)
            {
                // FIXME: Open the high scores menu
                //Manager.Show<HighScoresMenu>();

                // Indicate we've clicked on a button
                defaultButton = highScoresButton.gameObject;
                isButtonLocked = true;
            }
        }
        #endregion

        protected override void OnStateChanged(State from, State to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            // If this menu is visible again, release the button lock
            if(to == State.Visible)
            {
                isButtonLocked = false;
            }
        }
    }
}
