using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Global;
using OmiyaGames.Translations;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SocialMediaMenu.cs" company="Omiya Games">
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
    /// <date>2/11/2019</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that hides itself if a full-screen dialog shows up.
    /// </summary>
    public class SocialMediaMenu : IMenu
    {
        public override Type MenuType => Type.UnmanagedMenu;

        /// <summary>
        /// The default UI to highlight.
        /// </summary>
        public override Selectable DefaultUi => null;

        protected override void OnSetup()
        {
            base.OnSetup();

            // Bind to the managed stack event
            Manager.OnManagedMenusStackChanged += UpdateVisibility;

            // Make the menu visible
            CurrentVisibility = VisibilityState.Visible;
        }

        private void Start()
        {
            Setup();
        }

        private void OnDestroy()
        {
            if (Manager != null)
            {
                Manager.OnManagedMenusStackChanged -= UpdateVisibility;
            }
        }

        private void UpdateVisibility(MenuManager obj)
        {
            if (obj != null)
            {
                // Check to see what the background is on the top of the menu stack
                IMenu topMenu = obj.LastManagedMenu;
                if ((topMenu != null) && (topMenu.Background == BackgroundMenu.BackgroundType.SolidColor))
                {
                    // If solid color, hide this menu
                    CurrentVisibility = VisibilityState.Hidden;
                }
                else
                {
                    CurrentVisibility = VisibilityState.Visible;
                }
            }
        }
    }
}
