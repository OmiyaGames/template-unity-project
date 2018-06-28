using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Global;
using OmiyaGames.Translations;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PauseMenu.cs" company="Omiya Games">
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
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that appears when you press the pause button (based on the "Pause"
    /// Input Manager field). You can also retrieve this menu from the singleton
    /// script, <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    /// <seealso cref="TimeManager"/>
    public class PauseMenu : ISceneChangingMenu
    {
        [Header("Pause Menu")]
        [SerializeField]
        BackgroundMenu.BackgroundType background = BackgroundMenu.BackgroundType.GradientRightToLeft;
        [SerializeField]
        TranslatedTextMeshPro mRestartLabel = null;
        [SerializeField]
        TranslatedTextMeshPro mReturnToMenuLabel = null;

        [Header("Buttons")]
        [SerializeField]
        Button optionsButton = null;
        [SerializeField]
        Button howToPlayButton = null;
        [SerializeField]
        Button highScoresButton = null;
        [SerializeField]
        Button levelSelectButton = null;

        public override bool PauseOnShow
        {
            get
            {
                return true;
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return background;
            }
        }

        protected override void OnSetup()
        {
            base.OnSetup();

            // Update labels
            Manager.SetLabelTextToRestartCurrentScene(mRestartLabel);
            Manager.SetLabelTextToReturnToMenu(mReturnToMenuLabel);
        }

        void OnApplicationPause(bool isPaused)
        {
            if ((isPaused == true) && (CurrentVisibility == VisibilityState.Hidden) && (Singleton.Get<TimeManager>().IsManuallyPaused == false))
            {
                Show();
            }
        }

        #region Button Events
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
                CurrentDefaultUi = optionsButton.gameObject;
            }
        }

        public void OnHowToPlayClicked()
        {
            if(IsListeningToEvents == true)
            {
                Manager.Show<HowToPlayMenu>();

                // Set the default UI
                CurrentDefaultUi = howToPlayButton.gameObject;
            }
        }

        public void OnHighScoresClicked()
        {
            if (IsListeningToEvents == true)
            {
                // FIXME: show high scores
                //Manager.Show<HighScoresMenu>();

                // Set the default UI
                CurrentDefaultUi = highScoresButton.gameObject;
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
                CurrentDefaultUi = levelSelectButton.gameObject;
            }
        }
        #endregion
    }
}
