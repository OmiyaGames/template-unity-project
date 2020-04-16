using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Scenes;

namespace OmiyaGames.Menus
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

        protected struct BackgroundSettings
        {
            public BackgroundMenu.BackgroundType BackgroundState
            {
                get;
                set;
            }

            public string TitleTranslationKey
            {
                get;
                set;
            }

            public object[] TitleTranslationArgs
            {
                get;
                set;
            }

            public bool IsVisible
            {
                get
                {
                    bool returnFlag = false;
                    if (BackgroundState != BackgroundMenu.BackgroundType.Hidden)
                    {
                        returnFlag = true;
                    }
                    else if (string.IsNullOrEmpty(TitleTranslationKey) == false)
                    {
                        returnFlag = true;
                    }
                    return returnFlag;
                }
            }

            public void Update(BackgroundMenu.BackgroundType backgroundType, string titleTranslationKey = null, params object[] titleTranslationArgs)
            {
                BackgroundState = backgroundType;
                TitleTranslationKey = titleTranslationKey;
                TitleTranslationArgs = titleTranslationArgs;
            }

            public void CopySettings(IMenu copyFrom)
            {
                BackgroundState = copyFrom.Background;
                TitleTranslationKey = copyFrom.TitleTranslationKey;
                TitleTranslationArgs = copyFrom.TitleTranslationArgs;
            }
        }

        [Header("Animator Info")]
        [SerializeField]
        string stateField = "State";

        VisibilityState currentState = VisibilityState.Hidden;
        Animator animatorCache = null;
        bool isListeningToEvents = true;
        VisibilityChanged onStateChangedWhileManaged = null;

        #region Properties
        protected static MenuManager Manager => Singleton.Get<MenuManager>();

        protected static Settings.GameSettings Settings => Singleton.Get<Settings.GameSettings>();

        protected static SceneTransitionManager SceneChanger => Singleton.Get<SceneTransitionManager>();

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
                if ((CurrentSetupState == SetupState.Ready)

                    // Then check if this menu is actually visible
                    && (CurrentVisibility == VisibilityState.Visible)

                    // Finally, make sure we're not in the middle of transitioing to or from a new scene
                    && (SceneChanger.IsLoadingScene == false))
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

        public Animator Animator => Helpers.GetComponentCached(this, ref animatorCache);

        /// <summary>
        /// Setting this ScrollRect will make the menu center to the default UI when the Show() method is called.
        /// </summary>
        public virtual MenuNavigator Navigator
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The type of background to display behind this menu.
        /// </summary>
        public virtual BackgroundMenu.BackgroundType Background
        {
            get
            {
                return BackgroundMenu.BackgroundType.Hidden;
            }
        }

        /// <summary>
        /// The translation key for the title to display in this menu.
        /// </summary>
        public virtual string TitleTranslationKey
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The translation arguments for the title to display in this menu (in the situation the translation text expects them, e.g. containing {0}).
        /// </summary>
        public virtual object[] TitleTranslationArgs
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Indicates whether one can pause while this menu is in the MenuManager stack.
        /// </summary>
        public virtual bool IsPausingEnabledWhileVisible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether one can show pop-ups while this menu is in the MenuManager stack.
        /// </summary>
        public virtual bool IsPopUpEnabledWhileVisible
        {
            get
            {
                return (MenuType != Type.UnmanagedMenu);
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

        /// <summary>
        /// Indicates whether this menu is managed or not.
        /// </summary>
        public abstract Type MenuType
        {
            get;
        }

        /// <summary>
        /// The default UI to highlight.
        /// </summary>
        public abstract Selectable DefaultUi
        {
            get;
        }

        /// <summary>
        /// Name of the animator's state field
        /// </summary>
        protected string StateField
        {
            get => stateField;
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
        /// <seealso cref="Hide(bool)"/>
        public void Hide()
        {
            Hide(false);
        }

        /// <summary>
        /// Makes the menu hidden.
        /// </summary>
        public void Hide(bool force)
        {
            // Make sure the menu is Visible, and listening to events/being forced to be hidden.
            if ((CurrentVisibility == VisibilityState.Visible) && ((IsListeningToEvents == true) || (force == true)))
            {
                // Make the menu hidden
                CurrentVisibility = VisibilityState.Hidden;
            }
        }

        public void ScrollToDefaultUi(bool forceCenter)
        {
            ScrollToDefaultUi(VisibilityState.Visible, forceCenter);
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
            Animator.SetInteger(StateField, (int)to);

            // Check to see if we're visible
            if (to == VisibilityState.Visible)
            {
                // Run setup when made visible
                OnVisibilityChangedToVisible(from, true);
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
            // Setup the navigator, if one is assigned.
            if(Navigator != null)
            {
                Navigator.BindToEvents();
                Navigator.UpdateNavigation();
            }
        }

        #region Helper Methods
        void OnVisibilityChangedToVisible(VisibilityState from, bool forceCenter)
        {
            // Check if we've been setup
            if (CurrentSetupState == SetupState.NotSetup)
            {
                // If not, start setting up
                Setup();
            }

            // Check if there's a default UI
            ScrollToDefaultUi(from, forceCenter);
        }

        void ScrollToDefaultUi(VisibilityState from, bool forceCenter)
        {
            if (DefaultUi != null)
            {
                // Check if we have scrolling to be concerned about
                if (Navigator != null)
                {
                    UiEventNavigation uiToNavigateTo = DefaultUi.GetComponent<UiEventNavigation>();
                    if (uiToNavigateTo != null)
                    {
                        if (from == VisibilityState.Hidden)
                        {
                            // Scroll to the default UI
                            Navigator.ScrollToSelectable(uiToNavigateTo, forceCenter);

                            // If so, update the menu manager to select the default UI
                            Manager.SelectGui(DefaultUi);
                        }
                        else
                        {
                            // Scroll to the last selected element
                            uiToNavigateTo = Navigator.ScrollToLastSelectedElement(uiToNavigateTo, forceCenter);
                            if((uiToNavigateTo !=null) && (uiToNavigateTo.Selectable != null))
                            {
                                // Select the last selected UI
                                Manager.SelectGui(uiToNavigateTo.Selectable);
                            }
                            else
                            {
                                // If so, update the menu manager to select the default UI
                                Manager.SelectGui(DefaultUi);
                            }
                        }
                    }
                }
                else
                {
                    // If so, update the menu manager to select the default UI
                    Manager.SelectGui(DefaultUi);
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
