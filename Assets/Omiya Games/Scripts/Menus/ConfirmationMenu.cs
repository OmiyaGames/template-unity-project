using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ConfirmationMenu.cs" company="Omiya Games">
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
    /// <date>9/2/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that retains information about which scene to switch to.
    /// Where possible, it'll animate the <code>SceneTransitionMenu</code> before
    /// switching scenes.
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
    /// <description>9/2/2015</description>
    /// <description>Taro</description>
    /// <description>Initial verison.</description>
    /// 
    /// <description>6/6/2015</description>
    /// <description>Taro</description>
    /// <description>Adding copyright comments and
    /// other simple refactors.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    public class ConfirmationMenu : IMenu
    {
        [SerializeField]
        Button yesButton;
        [SerializeField]
        Button noButton;

        #region Properties
        public bool IsYesSelected
        {
            get;
            private set;
        } = false;

        public bool DefaultToYes
        {
            private get;
            set;
        } = false;

        public override GameObject DefaultUi
        {
            get
            {
                if (DefaultToYes == true)
                {
                    return yesButton.gameObject;
                }
                else
                {
                    return noButton.gameObject;
                }
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }
        #endregion

        public void OnYesClicked()
        {
            // Indicate Yes was selected
            IsYesSelected = true;

            // Hide the dialog
            Hide();
        }

        public void OnNoClicked()
        {
            // Indicate No was selected
            IsYesSelected = false;

            // Hide the dialog
            Hide();
        }
    }
}
