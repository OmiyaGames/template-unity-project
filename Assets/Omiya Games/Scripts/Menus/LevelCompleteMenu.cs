using UnityEngine;

namespace OmiyaGames.Menu
{
    using Settings;

    ///-----------------------------------------------------------------------
    /// <copyright file="LevelCompleteMenu.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
        [Header("Behavior")]
        [SerializeField]
        bool pauseGameOnShow = false;
        [SerializeField]
        bool unlockNextLevel = true;

        public override bool PauseOnShow
        {
            get
            {
                return pauseGameOnShow;
            }
        }

        public SceneTransitionManager TransitionManager
        {
            get
            {
                return Singleton.Get<SceneTransitionManager>();
            }
        }

        public GameSettings Settings
        {
            get
            {
                return Singleton.Get<GameSettings>();
            }
        }

        protected override void Start()
        {
            base.Start();

            // Check if we need to disable the next level button
            if ((defaultButton != null) && (TransitionManager.NextScene == null))
            {
                defaultButton.interactable = false;
            }
        }

        public override void Show(System.Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Check if we need to unlock the next level
            if (unlockNextLevel == true)
            {
                // Check which level to unlock
                int nextLevelUnlocked = TransitionManager.CurrentScene.Ordinal;
                if (TransitionManager.NextScene != null)
                {
                    // Unlock the next level
                    nextLevelUnlocked += 1;
                }

                // Check if this level hasn't been unlocked already
                if (nextLevelUnlocked > Settings.NumLevelsUnlocked)
                {
                    // Unlock this level
                    Settings.NumLevelsUnlocked = nextLevelUnlocked;
                }
            }
        }

        public void OnNextLevelClicked()
        {
            Hide();

            // Transition to the current level
            TransitionManager.LoadNextLevel();

            // Indicate the button was clicked
            Manager.ButtonClick.Play();
        }
    }
}
