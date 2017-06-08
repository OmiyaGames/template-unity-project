using UnityEngine;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="BackgroundMenu.cs" company="Omiya Games">
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
    /// The GUI background. You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public class BackgroundMenu : IMenu
    {
        public const string VisibleField = "Visible";

        [SerializeField]
        bool forceToBack = true;

        System.Action<MenuManager> onMenuNumberChanged = null;

        public override Type MenuType
        {
            get
            {
                return Type.UnmanagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return null;
            }
        }

        protected virtual void Start()
        {
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

        protected override void OnStateChanged(State from, State to)
        {
            // Update the animator
            if (to == State.Visible)
            {
                Animator.SetBool(VisibleField, true);
            }
            else
            {
                Animator.SetBool(VisibleField, false);
            }
        }

        protected void UpdateBackgroundVisibility(MenuManager manager)
        {
            if (manager.NumManagedMenus > 0)
            {
                IMenu menu = manager.PeekFromManagedStack();
                if(menu.ShowBackground == true)
                {
                    CurrentState = State.Visible;
                }
                else
                {
                    CurrentState = State.Hidden;
                }
            }
            else
            {
                CurrentState = State.Hidden;
            }
        }
    }
}