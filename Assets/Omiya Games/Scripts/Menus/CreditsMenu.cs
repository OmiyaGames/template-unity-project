using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="CreditsMenu.cs" company="Omiya Games">
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
    /// Scrolling credits. You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    public class CreditsMenu : IMenu
    {
        [Header("Components")]
        [SerializeField]
        Button defaultButton = null;
        [SerializeField]
        ScrollRect scrollable = null;
        [SerializeField]
        RectTransform content = null;

        [Header("Behavior")]
        [SerializeField]
        float startDelay = 1f;
        [SerializeField]
        float scrollSpeed = 20f;
        [SerializeField]
        float endDelay = 1f;

        float contentSize = 0, normalizedPosition = 0;
        System.Action<float> checkInput = null;

        public override Type MenuType
        {
            get
            {
                return Type.DefaultManagedMenu;
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
            // Start scrolling the contents
            StartCoroutine(ScrollCredits());
        }

        public override void Show(System.Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Unlock the cursor
            //SceneManager.CursorMode = CursorLockMode.None;

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
            bool wasVisible = (CurrentState == State.Visible);

            // Call base function
            base.Hide();

            if (wasVisible == true)
            {
                // Lock the cursor to what the scene is set to
                SceneTransitionManager manager = Singleton.Get<SceneTransitionManager>();
                //SceneManager.CursorMode = manager.CurrentScene.LockMode;

                // Unbind to Singleton's update function
                if (checkInput != null)
                {
                    Singleton.Instance.OnUpdate -= checkInput;
                    checkInput = null;
                }

                // Return to the menu
                manager.LoadMainMenu();
            }
        }

        IEnumerator ScrollCredits()
        {
            // Wait for a bit before starting the credits
            yield return new WaitForSeconds(startDelay);

            // Adjust the size of the scroll panel content
            contentSize = content.sizeDelta.y;
            normalizedPosition = 1;

            // Check what the scroll panel condition is so far
            while (Mathf.Approximately(normalizedPosition, 0) == false)
            {
                // Scroll the panel
                normalizedPosition -= (scrollSpeed * Time.deltaTime) / contentSize;
                normalizedPosition = Mathf.Clamp01(normalizedPosition);
                scrollable.verticalNormalizedPosition = normalizedPosition;

                // Wait for a frame
                yield return null;
            }

            // Wait for a bit before hiding the credits
            yield return new WaitForSeconds(endDelay);
            Hide();
        }

        void CheckForAnyKey(float deltaTime)
        {
            if(Input.anyKeyDown == true)
            {
                Hide();
            }
        }
    }
}
