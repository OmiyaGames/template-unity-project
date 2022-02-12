using UnityEngine;
using OmiyaGames.Global;
using OmiyaGames.Translations;
using OmiyaGames.GameFeel;

namespace OmiyaGames.Menus
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
        TranslatedTextMeshPro mRestartLabel = null;
        [SerializeField]
        TranslatedTextMeshPro mReturnToMenuLabel = null;

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
            if ((isPaused == true) && (CurrentVisibility == VisibilityState.Hidden) && (TimeManager.IsManuallyPaused == false))
            {
                Show();
            }
        }
    }
}
