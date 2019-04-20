using UnityEngine;
using OmiyaGames.Translations;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="BackgroundMenu.cs" company="Omiya Games">
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
    /// The GUI background. You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>5/18/2015</description>
    /// <description>Taro</description>
    /// <description>Initial verison.</description>
    /// 
    /// <description>6/5/2016</description>
    /// <description>Taro</description>
    /// <description>Adding title labels.</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="MenuManager"/>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class BackgroundMenu : IMenu
    {
        public enum BackgroundType
        {
            Hidden =                0,
            SolidColor =            1,
            GradientLeftToRight =   2,
            GradientRightToLeft =   3,
        }

        [SerializeField]
        bool forceToBack = true;

        [Header("Labels")]
        [SerializeField]
        TranslatedTextMeshPro titleLabel = null;
        [SerializeField]
        VersionLabel versionLabel = null;
        [SerializeField]
        GameObject divider = null;

        [Header("Animations")]
        [SerializeField]
        public string backgroundTypeField = "Background Type";
        [SerializeField]
        public string titleVisibilityField = "Title Visible";
        [SerializeField]
        public string changeTitleTrigger = "Change Title";

        System.Action<MenuManager> onMenuNumberChanged = null;
        BackgroundSettings nextState = new BackgroundSettings();

        public override Type MenuType
        {
            get
            {
                return Type.UnmanagedMenu;
            }
        }

        public override UnityEngine.UI.Selectable DefaultUi
        {
            get
            {
                return null;
            }
        }

        protected virtual void Start()
        {
            // Update labels
            nextState.TitleTranslationKey = titleLabel.TranslationKey;
            UpdateVersionLabelVisibility();

            // Grab the Menu manager and update the background visibility
            MenuManager manager = Singleton.Get<MenuManager>();
            if(manager != null)
            {
                UpdateBackgroundVisibility(manager);

                // Bind to the manager event
                if (onMenuNumberChanged != null)
                {
                    manager.OnManagedMenusStackChanged -= onMenuNumberChanged;
                    onMenuNumberChanged = null;
                }
                onMenuNumberChanged = new System.Action<MenuManager>(UpdateBackgroundVisibility);
                manager.OnManagedMenusStackChanged += onMenuNumberChanged;
            }

            if (forceToBack == true)
            {
                // Always make this the background
                transform.SetAsFirstSibling();
            }
        }

        protected virtual void OnDestroy()
        {
            MenuManager manager = Singleton.Get<MenuManager>();
            if ((manager != null) && (onMenuNumberChanged != null))
            {
                manager.OnManagedMenusStackChanged -= onMenuNumberChanged;
                onMenuNumberChanged = null;
            }
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            UpdateAnimator(to);
        }

        public void OnTitleHidden()
        {
            // Change the translation key
            if (string.IsNullOrEmpty(nextState.TitleTranslationKey) == false)
            {
                titleLabel.SetTranslationKey(nextState.TitleTranslationKey, nextState.TitleTranslationArgs);
            }
        }

        protected void UpdateBackgroundVisibility(MenuManager manager)
        {
            // Attempt to grab the latest menu
            VisibilityState currentState = VisibilityState.Hidden;
            IMenu menu = manager.PeekFromManagedStack();
            if (menu != null)
            {
                // Setup the next state of the background
                nextState.BackgroundState = menu.Background;
                nextState.TitleTranslationKey = menu.TitleTranslationKey;
                nextState.TitleTranslationArgs = menu.TitleTranslationArgs;

                // Indicate whether we want to show the background
                if (nextState.IsVisible == true)
                {
                    currentState = VisibilityState.Visible;
                }
            }

            // Before changing the state, check if this background is already visible
            if((CurrentVisibility == VisibilityState.Visible) && (currentState == VisibilityState.Visible))
            {
                // If so, force the animator to update
                UpdateAnimator(currentState);
            }

            // Set the current state
            CurrentVisibility = currentState;
        }

        void UpdateVersionLabelVisibility()
        {
            // Update the visibility of the version label
            if ((versionLabel != null) && (versionLabel.IsVisible == true))
            {
                versionLabel.gameObject.SetActive(true);
                if(divider != null)
                {
                    divider.SetActive(true);
                }
            }
            else if (divider != null)
            {
                divider.SetActive(false);
            }
        }

        void AnimateTitleVisibility()
        {
            if (string.IsNullOrEmpty(nextState.TitleTranslationKey) == true)
            {
                // If not, hide the title
                Animator.SetBool(titleVisibilityField, false);
            }
            else if (Animator.GetBool(titleVisibilityField) == false)
            {
                // Check if the title is hidden
                // Change the title text, then make it visible
                titleLabel.SetTranslationKey(nextState.TitleTranslationKey, nextState.TitleTranslationArgs);
                Animator.SetBool(titleVisibilityField, true);
            }
            else if ((string.Equals(titleLabel.TranslationKey, nextState.TitleTranslationKey) == false) || (titleLabel.Arguments != nextState.TitleTranslationArgs))
            {
                // If the strings are different;
                // the animation will trigger changing the label.
                Animator.SetTrigger(changeTitleTrigger);
            }
        }

        void UpdateAnimator(VisibilityState state)
        {
            // Update the animator
            if (state == VisibilityState.Visible)
            {
                // Update the background visibility
                Animator.SetInteger(backgroundTypeField, (int)nextState.BackgroundState);

                // Check whether to show the title
                AnimateTitleVisibility();
            }
            else
            {
                Animator.SetInteger(backgroundTypeField, (int)BackgroundType.Hidden);
                Animator.SetBool(titleVisibilityField, false);
            }
        }
    }
}