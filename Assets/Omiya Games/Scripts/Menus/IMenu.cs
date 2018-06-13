using System;
using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IMenu.cs" company="Omiya Games">
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
    /// Abstract class for creating menus. <code>MenuManager</code> will seek for
    /// these scripts at the start of a scene.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public abstract class IMenu : MonoBehaviour
    {
        public delegate void VisibilityChanged(IMenu source, VisibilityState from, VisibilityState to);
        public event VisibilityChanged OnBeforeVisbibilityChanged;
        public event VisibilityChanged OnAfterVisbibilityChanged;

        public enum SetupState
        {
            NotSetup,
            InProgress,
            Ready
        }

        public enum VisibilityState
        {
            /// <summary>
            /// Indicates menu is not show on-screen.
            /// </summary>
            Hidden = 0,
            /// <summary>
            /// Indicates menu is shown on-screen.
            /// If the menu is managed, indicates its been pushed onto the
            /// top of <code>MenuManager</code>'s stack.
            /// </summary>
            Visible = 1,
            /// <summary>
            /// Indicates menu is not show on-screen.
            /// State only happens to managed menus.
            /// Indicates the menu has been pushed down from the
            /// top of <code>MenuManager</code>'s stack.
            /// </summary>
            StandBy = 2
        }

        public enum Type
        {
            /// <summary>
            /// A type of menu that will not overlap with other managed menus.
            /// Also appears visible on Awake().
            /// </summary>
            DefaultManagedMenu,
            /// <summary>
            /// A type of menu that will not overlap with other managed menus.
            /// Also is hidden on Awake().
            /// </summary>
            ManagedMenu,
            /// <summary>
            /// A type of menu that can appear on top or below other menus.
            /// </summary>
            UnmanagedMenu
        }

        [Header("Animator Info")]
        [SerializeField]
        string stateField = "State";

        [Header("Background Settings")]
        [SerializeField]
        bool showBackground = true;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("projectTitleTranslationKey")]
        string titleTranslationKey = "";

        [Header("Default UI")]
        [SerializeField]
        [Tooltip("Setting this ScrollRect will make the menu center to the default UI when the Show() method is called.")]
        ScrollRect scrollToDefaultUi = null;

        VisibilityState currentState = VisibilityState.Hidden;
        Animator animatorCache = null;
        bool isListeningToEvents = true;
        VisibilityChanged onStateChangedWhileManaged = null;

        #region Properties
        protected static MenuManager Manager
        {
            get
            {
                return Singleton.Get<MenuManager>();
            }
        }

        protected static Settings.GameSettings Settings
        {
            get
            {
                return Singleton.Get<Settings.GameSettings>();
            }
        }

        protected static SceneTransitionManager SceneChanger
        {
            get
            {
                return Singleton.Get<SceneTransitionManager>();
            }
        }

        /// <summary>
        /// Indicates whether the UI is setup or not.
        /// </summary>
        public SetupState CurrentSetupState
        {
            get;
            private set;
        } = SetupState.NotSetup;

        /// <summary>
        /// Indicates whether the Menu should be listening to the events on its own UI elements or not.
        /// Note the getter will check multiple flags first before returning the actual state.
        /// </summary>
        /// <remarks>
        /// It is up to the menu script itself to listen and monitor how to react to this flag changing.
        /// </remarks>
        public bool IsListeningToEvents
        {
            get
            {
                // By default, return false
                bool returnFlag = false;

                // Make sure the menu is setup
                if ((CurrentSetupState != SetupState.Ready)

                    // Then check if this menu is actually visible
                    && (CurrentVisibility == VisibilityState.Visible)

                    // Finally, make sure we're not in the middle of transitioing to or from a new scene
                    && (SceneChanger.State == SceneTransitionManager.TransitionState.None))
                {
                    // Return the current listening state
                    returnFlag = isListeningToEvents;
                }
                return returnFlag;
            }
            protected set
            {
                if (isListeningToEvents != value)
                {
                    isListeningToEvents = value;
                }
            }
        }

        public Animator Animator
        {
            get
            {
                if (animatorCache == null)
                {
                    animatorCache = GetComponent<Animator>();
                }
                return animatorCache;
            }
        }

        protected ScrollRect ScrollToDefaultUi
        {
            get
            {
                return scrollToDefaultUi;
            }
            set
            {
                scrollToDefaultUi = value;
            }
        }

        /// <summary>
        /// The visibility of this menu.
        /// </summary>
        /// <seealso cref="Show(Action<IMenu>)"/>
        /// <seealso cref="Show()"/>
        /// <seealso cref="Hide()"/>
        public VisibilityState CurrentVisibility
        {
            get
            {
                return currentState;
            }
            internal set
            {
                // Make sure the state is actually changing
                if ((currentState != value) && (IsChangeInVisibilityValid(this, currentState, value) == true))
                {
                    // Run the before event
                    OnBeforeVisbibilityChanged?.Invoke(this, currentState, value);

                    // Grab the before and after state
                    VisibilityState lastState = currentState;
                    currentState = value;

                    // Run the event indicating the state changed
                    OnStateChanged(lastState, currentState);

                    // Run the after event
                    OnAfterVisbibilityChanged?.Invoke(this, lastState, currentState);
                }
            }
        }
        #endregion

        public abstract Type MenuType
        {
            get;
        }

        public abstract GameObject DefaultUi
        {
            get;
        }

        public virtual bool ShowBackground
        {
            get
            {
                return showBackground;
            }
        }

        public virtual string TitleTranslationKey
        {
            get
            {
                return titleTranslationKey;
            }
        }

        /// <summary>
        /// Sets up the Menu.
        /// </summary>
        public void Setup()
        {
            // Setup controls
            CurrentSetupState = SetupState.InProgress;

            // Setup the rest of the controls
            OnSetup();
            CurrentSetupState = SetupState.Ready;
        }

        /// <summary>
        /// Makes the menu visible. Also provides one to assign a method to
        /// listen to the menu changing states until it's made hidden.
        /// </summary>
        /// <param name="stateChangedWhileManaged"><code>Action</code></param>
        /// <seealso cref="Show()"/>
        /// <seealso cref="Hide()"/>
        /// <seealso cref="Action<IMenu>"/>
        public void Show(VisibilityChanged stateChangedWhileManaged = null)
        {
            // Make sure the menu is Hidden
            if (CurrentVisibility == VisibilityState.Hidden)
            {
                // Set the run-once action
                onStateChangedWhileManaged = stateChangedWhileManaged;

                // Make the menu visible
                CurrentVisibility = VisibilityState.Visible;
            }
        }

        /// <summary>
        /// Makes the menu hidden.
        /// </summary>
        /// <seealso cref="Show(Action<IMenu>)"/>
        /// <seealso cref="Hide()"/>
        public void Hide()
        {
            // Make sure the menu is Visible
            if (CurrentVisibility == VisibilityState.Visible)
            {
                // Make the menu hidden
                CurrentVisibility = VisibilityState.Hidden;
            }
        }

        /// <summary>
        /// Handles the menu's visiblility changing.
        /// Called after <code>CurrentVisibility</code> has already changed.
        /// </summary>
        /// <param name="from">The last state.</param>
        /// <param name="to">The new state the menu is changing to.</param>
        /// <seealso cref="CurrentVisibility"/>
        protected virtual void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Update the animator
            Animator.SetInteger(stateField, (int)to);

            // Check to see if we're visible
            if (to == VisibilityState.Visible)
            {
                // Run setup when made visible
                OnVisibilityChangedToVisible();
            }

            // Check if this is managed
            if (MenuType != Type.UnmanagedMenu)
            {
                // Update the menu manager
                UpdateMenuManager(this, from, to);
            }

            // Stop listening to managed events if this menu is made to be hidden
            onStateChangedWhileManaged?.Invoke(this, from, to);
            if ((to == VisibilityState.Hidden) && (onStateChangedWhileManaged != null))
            {
                onStateChangedWhileManaged = null;
            }
        }

        /// <summary>
        /// Handles setting up the UI.
        /// </summary>
        protected virtual void OnSetup()
        {
            // Do nothing for now.
        }

        #region Helper Methods
        void OnVisibilityChangedToVisible()
        {
            // Check if we've been setup
            if (CurrentSetupState == SetupState.NotSetup)
            {
                // If not, start setting up
                Setup();
            }

            // Check if there's a default UI
            if (DefaultUi != null)
            {
                // If so, update the menu manager to select the default UI
                Manager.SelectGuiGameObject(DefaultUi);

                // Check if we have scrolling to be concerned about
                if (ScrollToDefaultUi != null)
                {
                    // FIXME: scroll to the default UI
                    //ScrollToDefaultUi.scr
                }
            }
        }

        static void UpdateMenuManager(IMenu menu, VisibilityState from, VisibilityState to)
        {
            // Check if we're becoming visible or hidden
            if ((from == VisibilityState.Hidden) && (to == VisibilityState.Visible))
            {
                // If we're going from hidden to visible, add this menu to the managed stack
                // This will prompt the manager to push the last menu into stand-by
                Manager.PushToManagedStack(menu);
            }
            else if ((from == VisibilityState.Visible) && (to == VisibilityState.Hidden))
            {
                // If we're going from visible to hidden, remove this menu from the managed stack
                // This will prompt the manager to pop the last menu into visible
                if (Manager.LastManagedMenu == menu)
                {
                    Manager.PopFromManagedStack();
                }
            }
        }

        static bool IsChangeInVisibilityValid(IMenu menu, VisibilityState from, VisibilityState to)
        {
            // By default, return false
            bool returnFlag = false;

            // Check argument "from" value
            switch(from)
            {
                case VisibilityState.Hidden:
                case VisibilityState.StandBy:
                    // The only valid state from hidden or standby is Visible
                    returnFlag = (to == VisibilityState.Visible);
                    break;
                case VisibilityState.Visible:
                    // By default, visible can transition to all states.
                    returnFlag = true;
                    if (to == VisibilityState.StandBy)
                    {
                        // Unmanaged menus cannot be set to standby
                        returnFlag = (menu.MenuType != Type.UnmanagedMenu);
                    }
                    break;
            }
            return returnFlag;
        }
        #endregion
    }
}
