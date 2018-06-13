using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Translations;

namespace OmiyaGames.Menu
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
        [Header("Components")]
        [SerializeField]
        protected Button defaultButton = null;

        // FIXME: think real hard here, do we *really* need these?
        [SerializeField]
        protected TranslatedText completeLabel = null;
        [SerializeField]
        protected TranslatedText failedLabel = null;
        [SerializeField]
        protected TranslatedText nextSceneLabel = null;
        [SerializeField]
        protected TranslatedText restartLabel = null;
        [SerializeField]
        protected TranslatedText returnToMenuLabel = null;

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

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // FIXME: think real hard here, do we *really* need these?
            Manager.SetLabelTextToCompletedCurrentScene(completeLabel);
            Manager.SetLabelTextToFailedCurrentScene(failedLabel);
            Manager.SetLabelTextToNextScene(nextSceneLabel);
            Manager.SetLabelTextToRestartCurrentScene(restartLabel);
            Manager.SetLabelTextToReturnToMenu(returnToMenuLabel);
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Call base method
            base.OnStateChanged(from, to);

            if (PauseOnShow == true)
            {
                if(to == VisibilityState.Visible)
                {
                    // Stop time
                    Singleton.Get<TimeManager>().IsManuallyPaused = true;
                }
                else if(to == VisibilityState.Hidden)
                {
                    // Resume the time
                    Singleton.Get<TimeManager>().IsManuallyPaused = false;
                }
            }
        }

        public void OnRestartClicked()
        {
            Hide();

            // Transition to the current level
            SceneChanger.ReloadCurrentScene();
        }

        public void OnReturnToMenuClicked()
        {
            if(IsListeningToEvents == true)
            {
                // Transition to the menu
                SceneChanger.LoadMainMenu();

                IsListeningToEvents = false;
            }
        }
    }
}