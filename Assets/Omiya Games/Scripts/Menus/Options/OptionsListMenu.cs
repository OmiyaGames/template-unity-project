using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    using System;
    using Settings;

    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsListMenu.cs" company="Omiya Games">
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
    /// Menu that provides a list of option categories.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class OptionsListMenu : IMenu
    {
        [SerializeField]
        Button defaultButton;

        /// <summary>
        /// Flag indicating a button in this menu has already been clicked.
        /// Since this menu leads to other menus, this flag is used to prevent double-clicking.
        /// The value will be reset when the panel is made visible again.
        /// </summary>
        bool isButtonLocked = false;

        #region Overridden Properties
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
                return null;
            }
        }
        #endregion

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            // If this menu is visible again, release the button lock
            if (to == VisibilityState.Visible)
            {
                isButtonLocked = false;
            }
        }

        #region UI Events
        public void OnAudioClicked()
        {
            // Make sure the button isn't locked yet
            if (isButtonLocked == false)
            {
                // Show the audio options
                Manager.Show<OptionsAudioMenu>();

                // Indicate the button has been clicked.
                isButtonLocked = true;
            }
        }

        public void OnControlsClicked()
        {
            // Make sure the button isn't locked yet
            if (isButtonLocked == false)
            {
                // Show the controls options
                Manager.Show<OptionsControlsMenu>();

                // Indicate the button has been clicked.
                isButtonLocked = true;
            }
        }

        public void OnGraphicsClicked()
        {
            // Make sure the button isn't locked yet
            if (isButtonLocked == false)
            {
                // Show the graphics options
                Manager.Show<OptionsGraphicsMenu>();

                // Indicate the button has been clicked.
                isButtonLocked = true;
            }
        }

        public void OnAccessibilityClicked()
        {
            // Make sure the button isn't locked yet
            if (isButtonLocked == false)
            {
                // Show the accessibility options
                Manager.Show<OptionsAccessibilityMenu>();

                // Indicate the button has been clicked.
                isButtonLocked = true;
            }
        }

        public void OnLanguageClicked()
        {
            // Make sure the button isn't locked yet
            if (isButtonLocked == false)
            {
                // Show the language options
                Manager.Show<OptionsLanguageMenu>();

                // Indicate the button has been clicked.
                isButtonLocked = true;
            }
        }

        public void OnResetDataClicked()
        {
            // Make sure the button isn't locked yet
            if (isButtonLocked == false)
            {
                ConfirmationMenu menu = Manager.GetMenu<ConfirmationMenu>();
                if (menu != null)
                {
                    // Display confirmation dialog
                    menu.DefaultToYes = false;
                    menu.Show(CheckResetSavedDataConfirmation);

                    // Indicate the button has been clicked.
                    isButtonLocked = true;
                }
            }
        }
        #endregion

        #region Helper Methods
        void CheckResetSavedDataConfirmation(IMenu source, VisibilityState from, VisibilityState to)
        {
            if ((source is ConfirmationMenu) && (((ConfirmationMenu)source).IsYesSelected == true))
            {
                // Clear settings
                Singleton.Get<GameSettings>().ClearSettings();

                // Update the level select menu, if one is available
                LevelSelectMenu levelSelect = Manager.GetMenu<LevelSelectMenu>();
                if (levelSelect != null)
                {
                    levelSelect.SetButtonsEnabled(true);
                }
            }
        }
        #endregion
    }
}
