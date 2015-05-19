using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="StartMenu.cs" company="Omiya Games">
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
    /// Menu that appears on the start of the game, allowing you to change options,
    /// select a level, or quit the game. You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public class StartMenu : IMenu
    {
        [SerializeField]
        Button levelSelectButton;
        [SerializeField]
        Button optionsButton;
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

        void Start()
        {
            // Check if we should remove the quit button (you can't quit out of a webplayer)
            GameSettings gameSettings = Singleton.Get<GameSettings>();
            if (gameSettings.IsWebplayer == true)
            {
                // Disable the quit button entirely
                quitButton.gameObject.SetActive(false);
            }

            // Select the level select button by default
            defaultButton = levelSelectButton.gameObject;
            Singleton.Get<UnityEngine.EventSystems.EventSystem>().firstSelectedGameObject = defaultButton;
        }

        #region Button Events
        public void OnLevelSelectClicked()
        {
            if (isButtonLocked == false)
            {
                // Open the Level Select menu
                Singleton.Get<MenuManager>().GetMenu<LevelSelectMenu>().CurrentState = IMenu.State.Visible;

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
                Singleton.Get<MenuManager>().Show<OptionsMenu>();

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
                Singleton.Get<SceneManager>().LoadScene(Singleton.Get<SceneManager>().Credits);

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

                // Change the menu to stand by
                CurrentState = State.StandBy;

                // Indicate we've clicked on a button
                defaultButton = quitButton.gameObject;
                isButtonLocked = true;
            }
        }
        #endregion

        protected override void OnStateChanged(IMenu.State from, IMenu.State to)
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
