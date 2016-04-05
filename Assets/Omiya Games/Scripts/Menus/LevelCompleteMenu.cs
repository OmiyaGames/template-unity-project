using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LevelCompleteMenu.cs" company="Omiya Games">
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

        protected override void Start()
        {
            base.Start();

            // Check if we need to disable the next level button
            if ((defaultButton != null) && (Singleton.Get<SceneManager>().NextScene == null))
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
                SceneManager manager = Singleton.Get<SceneManager>();
                GameSettings settings = Singleton.Get<GameSettings>();
                if (Singleton.Get<SceneManager>().NextScene != null)
                {
                    // Unlock the next level
                    settings.NumLevelsUnlocked = manager.CurrentScene.Ordinal + 1;
                }
                else
                {
                    // Unlock this level (last one)
                    settings.NumLevelsUnlocked = manager.CurrentScene.Ordinal;
                }
            }
        }

        public void OnNextLevelClicked()
        {
            Hide();

            // Transition to the current level
            Singleton.Get<SceneManager>().LoadNextLevel();

            // Indicate the button was clicked
            Manager.ButtonClick.Play();
        }
    }
}
