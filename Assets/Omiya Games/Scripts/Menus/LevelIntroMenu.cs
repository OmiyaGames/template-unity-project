using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LevelIntroMenu.cs" company="Omiya Games">
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu indicating important information before starting a level. You can retrieve
    /// this menu from the singleton script, <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    public class LevelIntroMenu : IMenu
    {
        [Header("Components")]
        [SerializeField]
        Button defaultButton = null;
        [SerializeField]
        TranslatedText levelNameLabel = null;

        [Header("Behavior")]
        [SerializeField]
        bool pauseOnStart = false;
        [SerializeField]
        bool onlyAppearOnWebplayer = false;
        [SerializeField]
        bool requireClickToStart = false;

        System.Action<float> checkInput = null;

        public override Type MenuType
        {
            get
            {
                Type returnType = Type.ManagedMenu;
                if ((Singleton.Instance.IsWebplayer == true) || (onlyAppearOnWebplayer == false))
                {
                    returnType = Type.DefaultManagedMenu;
                }
                return returnType;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return defaultButton.gameObject;
            }
        }

        protected virtual void Start()
        {
            // Setup all labels, if available
            SceneInfo currentScene = Singleton.Get<SceneTransitionManager>().CurrentScene;
            if ((levelNameLabel != null) && (currentScene != null))
            {
                levelNameLabel.TranslationKey = currentScene.DisplayName.TranslationKey;
            }
        }

        public override void Show(System.Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Check if we should stop time
            if (pauseOnStart == true)
            {
                // Stop time
                Singleton.Get<TimeManager>().IsManuallyPaused = true;
            }

            // Check if we've previously binded to the singleton's update function
            if (checkInput != null)
            {
                Singleton.Instance.OnUpdate -= checkInput;
                checkInput = null;
            }

            // Bind to Singleton's update function
            checkInput = new System.Action<float>(CheckForAnyKey);
            Singleton.Instance.OnUpdate += checkInput;
        }

        public override void Hide()
        {
            // Call base function
            base.Hide();

            // Check if we should stop time
            if (pauseOnStart == true)
            {
                // Resume the time
                Singleton.Get<TimeManager>().IsManuallyPaused = false;
            }

            // Unbind to Singleton's update function
            if (checkInput != null)
            {
                Singleton.Instance.OnUpdate -= checkInput;
                checkInput = null;
            }

            // Indicate button is clicked
            Manager.ButtonClick.Play();
        }

        void CheckForAnyKey(float deltaTime)
        {
            if((Input.anyKeyDown == true) && (requireClickToStart == false))
            {
                Hide();
            }
        }
    }
}
