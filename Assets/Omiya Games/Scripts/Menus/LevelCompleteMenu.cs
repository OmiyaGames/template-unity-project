using UnityEngine;
using OmiyaGames.Translations;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LevelCompleteMenu.cs" company="Omiya Games">
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
    /// <date>12/8/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu indicating you've completed a level. You can retrieve this menu from
    /// the singleton script, <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    public class LevelCompleteMenu : ISceneChangingMenu
    {
        [Header("Level Complete Menu")]
        [SerializeField]
        bool pauseGameOnShow = false;
        [SerializeField]
        TranslatedTextMeshPro mLevelCompleteLabel = null;
        [SerializeField]
        TranslatedTextMeshPro mRestartLabel = null;
        [SerializeField]
        TranslatedTextMeshPro mReturnToMenuLabel = null;

        public override bool PauseOnShow
        {
            get
            {
                return pauseGameOnShow;
            }
        }

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // Update labels
            Manager.SetLabelTextToCompletedCurrentScene(mLevelCompleteLabel);
            Manager.SetLabelTextToRestartCurrentScene(mRestartLabel);
            Manager.SetLabelTextToReturnToMenu(mReturnToMenuLabel);

            // Check if we need to disable the next level button
            if ((defaultButton != null) && (SceneChanger.UpcomingScene == null))
            {
                defaultButton.interactable = false;
            }
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Call base method
            base.OnStateChanged(from, to);

            // Check if we're making this menu visible and unlocked a new level
            if ((from == VisibilityState.Hidden) && (to == VisibilityState.Visible) && (SceneChanger.UnlockNextLevel() == true))
            {
                // Update the level select menu as well
                LevelSelectMenu levelMenu = Manager.GetMenu<LevelSelectMenu>();
                if (levelMenu != null)
                {
                    levelMenu.SetButtonsEnabled(true);
                }
            }
        }
    }
}
