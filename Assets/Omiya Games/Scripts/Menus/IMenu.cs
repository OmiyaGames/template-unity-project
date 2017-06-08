using UnityEngine;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IMenu.cs" company="Omiya Games">
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
    /// Abstract class for creating menus. <code>MenuManager</code> will seek for
    /// these scripts at the start of a scene.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public abstract class IMenu : MonoBehaviour
    {
        public const string StateField = "State";
        static MenuManager managerCache = null;

        public enum State
        {
            Hidden  = 0,
            Visible = 1,
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

        State currentState = State.Hidden;
        Animator animatorCache = null;
        protected System.Action<IMenu> onStateChanged = null;

        #region Properties
        protected static MenuManager Manager
        {
            get
            {
                if(managerCache == null)
                {
                    managerCache = Singleton.Get<MenuManager>();
                }
                return managerCache;
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

        public State CurrentState
        {
            get
            {
                return currentState;
            }
            internal set
            {
                if (currentState != value)
                {
                    // Grab the before and after state
                    State lastState = currentState;
                    currentState = value;

                    // Run the event indicating the state changed
                    OnStateChanged(lastState, currentState);
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
                return true;
            }
        }

        public void Show()
        {
            Show(null);
        }

        public virtual void Show(System.Action<IMenu> stateChanged)
        {
            onStateChanged = stateChanged;
            CurrentState = State.Visible;
        }

        public virtual void Hide()
        {
            CurrentState = State.Hidden;
            if(onStateChanged != null)
            {
                onStateChanged = null;
            }
        }

        protected virtual void OnStateChanged(State from, State to)
        {
            // Update the animator
            Animator.SetInteger(StateField, (int)to);

            // Grab the menu manager
            MenuManager manager = Singleton.Get<MenuManager>();

            // Check to see if we're visible
            if((to == State.Visible) && (DefaultUi != null))
            {
                // If so, update the menu manager to select the default UI
                manager.SelectGuiGameObject(DefaultUi);
            }

            // Check if this is managed
            if(MenuType != Type.UnmanagedMenu)
            {
                // Check if we're becoming visible or hidden
                if ((from == State.Hidden) && (to == State.Visible))
                {
                    // If we're going from hidden to visible, add this menu to the managed stack
                    // This will prompt the manager to push the last menu into stand-by
                    manager.PushToManagedStack(this);
                }
                else if ((from == State.Visible) && (to == State.Hidden))
                {
                    // If we're going from visible to hidden, remove this menu from the managed stack
                    // This will prompt the manager to pop the last menu into visible
                    if(manager.LastManagedMenu == this)
                    {
                        manager.PopFromManagedStack();
                    }
                }
            }

            // Check if there's an action associated with this dialog
            if(onStateChanged != null)
            {
                onStateChanged(this);
            }
        }
    }
}
