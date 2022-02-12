using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Global;
using OmiyaGames.Managers;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ISceneChangingMenu.cs" company="Omiya Games">
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
    /// A common interface for menus that swithes scenes.
    /// </summary>
    public abstract class ISceneChangingMenu : IMenu
    {
        [Header("Common Settings")]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("showBackground")]
        protected BackgroundMenu.BackgroundType background = BackgroundMenu.BackgroundType.GradientRightToLeft;
        [SerializeField]
        MenuNavigator navigator;
        [Header("Buttons")]
        [SerializeField]
        protected Button defaultButton = null;
        [SerializeField]
        protected Button optionsButton = null;
        [SerializeField]
        protected Button howToPlayButton = null;
        [SerializeField]
        protected Button highScoresButton = null;
        [SerializeField]
        protected Button levelSelectButton = null;

        abstract public bool PauseOnShow
        {
            get;
        }

				public virtual void Reset()
				{
					navigator = GetComponent<MenuNavigator>();
				}

        #region Non-abstract Properties
        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return background;
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override Selectable DefaultUi
        {
            get
            {
                Selectable returnObject = null;
                if (CurrentDefaultUi != null)
                {
                    returnObject = CurrentDefaultUi;
                }
                else if(defaultButton != null)
                {
                    returnObject = defaultButton;
                }
                return returnObject;
            }
        }

        public override MenuNavigator Navigator
        {
            get
            {
                return navigator;
            }
        }

        protected Selectable CurrentDefaultUi
        {
            get;
            set;
        }
        #endregion

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Check if this menu is going from hidden to visible
            if ((from == VisibilityState.Hidden) && (to == VisibilityState.Visible))
            {
                // Set the Default UI to the default button
                CurrentDefaultUi = null;
            }

            // Call base method
            base.OnStateChanged(from, to);

            if (PauseOnShow == true)
            {
                if (to == VisibilityState.Visible)
                {
                    // Stop time
                    TimeManager.IsManuallyPaused = true;
                }
                else if (to == VisibilityState.Hidden)
                {
                    // Resume the time
                    TimeManager.IsManuallyPaused = false;
                }
            }
        }

        #region Button Events
        public void OnNextLevelClicked()
        {
            if (IsListeningToEvents == true)
            {
                // Transition to the next level
                SceneChanger.LoadNextLevel();
                Hide();
            }
        }

        public void OnRestartClicked()
        {
            if (IsListeningToEvents == true)
            {
                // FIXME: Notify the player that they'll lose their unsaved progress.
                // Transition to the current level
                SceneChanger.ReloadCurrentScene();
                Hide();
            }
        }

        public void OnReturnToMenuClicked()
        {
            if(IsListeningToEvents == true)
            {
                // FIXME: Notify the player that they'll lose their unsaved progress.
                // Transition to the menu
                SceneChanger.LoadMainMenu();
                Hide();
            }
        }

        public void OnOptionsClicked()
        {
            if (IsListeningToEvents == true)
            {
                // Open the options dialog
                OptionsListMenu menu = Manager.GetMenu<OptionsListMenu>();
                if (menu != null)
                {
                    menu.UpdateDialog(this);
                    menu.Show();
                }

                // Set the default UI
                CurrentDefaultUi = optionsButton;
            }
        }

        public void OnHowToPlayClicked()
        {
            if (IsListeningToEvents == true)
            {
                Manager.Show<HowToPlayMenu>();

                // Set the default UI
                CurrentDefaultUi = howToPlayButton;
            }
        }

        public void OnHighScoresClicked()
        {
            if (IsListeningToEvents == true)
            {
                // Show high scores
                Manager.Show<HighScoresMenu>();

                // Set the default UI
                CurrentDefaultUi = highScoresButton;
            }
        }

        public void OnLevelSelectClicked()
        {
            // Make sure the menu is active
            if (IsListeningToEvents == true)
            {
                // Open the Level Select menu
                LevelSelectMenu levelSelect = Manager.GetMenu<LevelSelectMenu>();
                if (levelSelect != null)
                {
                    levelSelect.UpdateDialog(this);
                    levelSelect.Show();
                }

                // Set the default UI
                CurrentDefaultUi = levelSelectButton;
            }
        }
        #endregion
    }
}
