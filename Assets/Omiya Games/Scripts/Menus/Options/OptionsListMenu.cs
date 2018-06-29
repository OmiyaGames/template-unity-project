using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
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
        const BackgroundMenu.BackgroundType DefaultBackground = BackgroundMenu.BackgroundType.GradientRightToLeft;

        [Header("Options List")]
        [SerializeField]
        ScrollRect scrollMenu;
        [SerializeField]
        Button backButton;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("allButtons")]
        PlatformSpecificButton[] allOptionsButtons;
        [SerializeField]
        string resetDataMessage = "Options Reset Message";

        GameObject cachedDefaultButton = null;
        BackgroundSettings background = new BackgroundSettings();

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
                if(cachedDefaultButton == null)
                {
                    foreach(PlatformSpecificButton button in allOptionsButtons)
                    {
                        if(button.EnabledFor.IsThisBuildSupported() == true)
                        {
                            cachedDefaultButton = button.Component.gameObject;
                            break;
                        }
                    }
                }
                return cachedDefaultButton;
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return background.BackgroundState;
            }
        }

        public override string TitleTranslationKey
        {
            get
            {
                return background.TitleTranslationKey;
            }
        }

        public override object[] TitleTranslationArgs
        {
            get
            {
                return background.TitleTranslationArgs;
            }
        }

        public override ScrollRect ScrollToDefaultUi
        {
            get
            {
                return scrollMenu;
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // Setup every button
            foreach (PlatformSpecificButton button in allOptionsButtons)
            {
                button.Setup();
            }
        }

        /// <summary>
        /// Sets up the dialog background based off of another menu.
        /// </summary>
        public void UpdateDialog(IMenu copyBackgroundSettings)
        {
            // Check the parameter
            if (copyBackgroundSettings != null)
            {
                background.CopySettings(copyBackgroundSettings);
            }
        }

        /// <summary>
        /// Sets up the dialog with the proper message and time on when to select the default dialog selection
        /// </summary>
        /// <param name="messageTranslatedKey"></param>
        /// <param name="automaticallySelectDefaultAfterSeconds"></param>
        public void UpdateDialog(BackgroundMenu.BackgroundType backgroundType = DefaultBackground, string titleTranslationKey = null, params object[] titleTranslationArgs)
        {
            // Update background
            background.Update(backgroundType, titleTranslationKey, titleTranslationArgs);
        }

        #region UI Events
        public void OnAudioClicked()
        {
            // Make sure the button isn't locked yet
            if (IsListeningToEvents == true)
            {
                // Show the audio options
                Manager.Show<OptionsAudioMenu>();
            }
        }

        public void OnControlsClicked()
        {
            // Make sure the button isn't locked yet
            if (IsListeningToEvents == true)
            {
                // Show the controls options
                Manager.Show<OptionsControlsMenu>();
            }
        }

        public void OnGraphicsClicked()
        {
            // Make sure the button isn't locked yet
            if (IsListeningToEvents == true)
            {
                // Show the graphics options
                Manager.Show<OptionsGraphicsMenu>();
            }
        }

        public void OnAccessibilityClicked()
        {
            // Make sure the button isn't locked yet
            if (IsListeningToEvents == true)
            {
                // Show the accessibility options
                Manager.Show<OptionsAccessibilityMenu>();
            }
        }

        public void OnLanguageClicked()
        {
            // Make sure the button isn't locked yet
            if (IsListeningToEvents == true)
            {
                // Show the language options
                Manager.Show<OptionsLanguageMenu>();
            }
        }

        public void OnResetDataClicked()
        {
            // Make sure the button isn't locked yet
            if (IsListeningToEvents == true)
            {
                ConfirmationMenu menu = Manager.GetMenu<ConfirmationMenu>();
                if (menu != null)
                {
                    // Display confirmation dialog
                    menu.DefaultToYes = false;
                    menu.UpdateDialog(this, resetDataMessage);
                    menu.Show(CheckResetSavedDataConfirmation);
                }
            }
        }
        #endregion

        #region Helper Methods
        void CheckResetSavedDataConfirmation(IMenu source, VisibilityState from, VisibilityState to)
        {
            if ((source is ConfirmationMenu) && (to == VisibilityState.Hidden) && (((ConfirmationMenu)source).IsYesSelected == true))
            {
                // Clear settings
                Settings.ClearSettings();

                // Update the start menu, if one is available
                StartMenu start = Manager.GetMenu<StartMenu>();
                if (start != null)
                {
                    start.SetupStartButton();
                }

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
