using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="CreditsMenu.cs" company="Omiya Games">
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

        [Header("Background Settings")]
        [SerializeField]
        BackgroundMenu.BackgroundType background = BackgroundMenu.BackgroundType.GradientRightToLeft;

        System.Action<float> checkAnyKey = null;
        float contentSize = 0, normalizedPosition = 0;

        #region Overrides
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

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return background;
            }
        }

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // Start scrolling the contents
            StartCoroutine(ScrollCredits());
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            base.OnStateChanged(from, to);

            // Remove the binding to Singleton's update function
            StopListeningToUpdate();

            if (to == VisibilityState.Visible)
            {
                // If menu is becoming visible
                // Bind to Singleton's update function
                checkAnyKey = new System.Action<float>(CheckForAnyKey);
                Singleton.Instance.OnUpdate += checkAnyKey;
            }
            else if(to == VisibilityState.Hidden)
            {
                // If menu is hidden
                // Remove the binding to Singleton's update function
                StopListeningToUpdate();

                // Return to the menu
                SceneChanger.LoadMainMenu();
            }
        }
        #endregion

        #region Helper Methods
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

        void StopListeningToUpdate()
        {
            // Check if the action is not null
            if(checkAnyKey != null)
            {
                // Remove the binding to Singleton's update function
                Singleton.Instance.OnUpdate -= checkAnyKey;

                // Set the action to null
                checkAnyKey = null;
            }
        }
        #endregion
    }
}
