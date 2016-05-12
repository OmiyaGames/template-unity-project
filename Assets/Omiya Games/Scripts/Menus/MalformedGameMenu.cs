using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.ObjectModel;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MalformedGameMenu.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
    /// <date>5/11/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Dialog indicating this game may not be genuine.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    public class MalformedGameMenu : IMenu
    {
        public enum Reason
        {
            None = -1,
            CannotConfirmGenuine = 0,
            IsNotGenuine,
            CannotConfirmDomain,
            IsIncorrectDomain
        }

        [Header("Components")]
        [SerializeField]
        Button defaultButton = null;
        [SerializeField]
        ScrollRect scrollable = null;
        [SerializeField]
        RectTransform content = null;
        [SerializeField]
        Text message = null;

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return defaultButton.gameObject;
            }
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

                // Return to the menu
                manager.LoadMainMenu();
            }
        }

        public void UpdateReason(Reason reason)
        {
            // Grab the web checker
            WebLocationChecker webChecker = null;
            if (Singleton.Instance.IsWebplayer == true)
            {
                webChecker = Singleton.Get<WebLocationChecker>();
            }

            StringBuilder builder = new StringBuilder();

            // FIXME: do something!
            switch (reason)
            {
                case Reason.CannotConfirmGenuine:
                    builder.Append("Cannot confirm game is genuine");
                    break;
                case Reason.IsNotGenuine:
                    builder.Append("Game is not genuine");
                    break;
                case Reason.CannotConfirmDomain:
                    builder.Append("Error confirming the domain of this website");
                    break;
                case Reason.IsIncorrectDomain:
                    builder.AppendLine("Incorrect domain detected");
                    builder.Append("Detected domain: ");
                    builder.AppendLine(webChecker.RetrievedHostName);
                    builder.AppendLine("Accepted domains:");
                    if (webChecker != null)
                    {
                        ReadOnlyCollection<string> allDomains = webChecker.DomainList;
                        for(int index = 0; index < allDomains.Count; ++index)
                        {
                            builder.Append("* ");
                            builder.AppendLine(allDomains[index]);
                        }
                    }
                    break;
            }
            message.text = builder.ToString();
        }
    }
}
