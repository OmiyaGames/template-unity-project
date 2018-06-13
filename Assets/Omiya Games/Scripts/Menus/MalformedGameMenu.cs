using UnityEngine;
using System;
using System.Text;
using System.Collections;
using OmiyaGames.Global;
using OmiyaGames.Translations;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MalformedGameMenu.cs" company="Omiya Games">
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
    /// <date>5/11/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Dialog indicating this game may not be genuine.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>5/11/2016</description>
    /// <description>Taro</description>
    /// <description>Initial verison.</description>
    /// 
    /// <description>6/5/2018</description>
    /// <description>Taro</description>
    /// <description>Moving the menu to the Main Menu scene.
    /// Scene transitionis removed.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    public class MalformedGameMenu : IMenu
    {
        public enum Reason
        {
            InProgress = -2,
            None = -1,
            IsNotGenuine = 0,
            CannotConfirmDomain,
            IsIncorrectDomain,
            JustTesting
        }

        [Serializable]
        public struct WebsiteInfo
        {
            [SerializeField]
            string labelTranslationKey;
            [SerializeField]
            string redirectTo;
        }

        static bool IsBuildVerified
        {
            get;
            set;
        } = false;

        [Header("UI")]
        [SerializeField]
        [Tooltip("Update the Website field to populate this label's website URL.")]
        UnityEngine.UI.Scrollbar defaultUi = null;
        [SerializeField]
        TranslatedTextMeshPro optionsMessage = null;
        [SerializeField]
        string websiteLinkId = "website";

        [Header("Error Messages")]
        [SerializeField]
        TranslatedTextMeshPro reasonMessage = null;
        [SerializeField]
        string gameIsNotGenuineMessageTranslationKey;
        [SerializeField]
        string cannotConfirmDomainMessageTranslationKey;
        [SerializeField]
        string domainDoesNotMatchMessageTranslationKey;

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
                return defaultUi.gameObject;
            }
        }

        public Reason BuildState
        {
            get;
            private set;
        } = Reason.None;

        WebLocationChecker WebChecker
        {
            get
            {
                return Singleton.Get<WebLocationChecker>();
            }
        }

        IEnumerator Start()
        {
            // Update build statue
            BuildState = Reason.None;

            // Check if we need to verify the build
            if (IsBuildVerified == false)
            {
                // Update state to in-progress
                BuildState = Reason.InProgress;

                // Wait until all start functions are run
                yield return null;
                yield return null;

                // Start varifying the build
                StartCoroutine(VerifyBuild());
            }
        }

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // Update options text
            optionsMessage.SetArguments(Singleton.Instance.WebsiteLinkShortened);
        }

        public void UpdateReason(Reason reason)
        {
            // Update the reason for this dialog to appear
            switch(reason)
            {
                case Reason.CannotConfirmDomain:
                    // Update translation key
                    reasonMessage.TranslationKey = cannotConfirmDomainMessageTranslationKey;
                    break;
                case Reason.IsIncorrectDomain:
                    if (WebChecker != null)
                    {
                        // Setup translation key, with proper population of fields
                        reasonMessage.SetTranslationKey(domainDoesNotMatchMessageTranslationKey, WebChecker.RetrievedHostName);
                    }
                    else
                    {
                        // Update translation key
                        reasonMessage.TranslationKey = gameIsNotGenuineMessageTranslationKey;
                    }
                    break;
                case Reason.JustTesting:
                    // Overwrite the text: it's a test
                    StringBuilder builder = new StringBuilder();
                    builder.Append("This menu is just a test. ");
                    Utility.BuildTestMessage(builder, WebChecker);
                    reasonMessage.CurrentText = builder.ToString();
                    break;
                default:
                    // Update translation key
                    reasonMessage.TranslationKey = gameIsNotGenuineMessageTranslationKey;
                    break;
            }
        }

        /// <summary>
        /// Event for when link is clicked.
        /// </summary>
        /// <param name="linkId"></param>
        /// <param name="linkText"></param>
        /// <param name="linkIndex"></param>
        public void OnWebsiteLinkClicked(string linkId, string linkText, int linkIndex)
        {
            if (linkId == websiteLinkId)
            {
                Application.OpenURL(Singleton.Instance.WebsiteLink);
            }
        }

        IEnumerator VerifyBuild()
        {
            BuildState = Reason.InProgress;
            if (Singleton.Instance.IsWebApp == true)
            {
                // Grab the web checker
                if (WebChecker != null)
                {
                    // Wait until the webchecker is done
                    while (WebChecker.CurrentState == WebLocationChecker.State.InProgress)
                    {
                        yield return null;
                    }

                    // Check the state
                    switch (WebChecker.CurrentState)
                    {
                        case WebLocationChecker.State.DomainMatched:
                        case WebLocationChecker.State.NotUsed:
                            BuildState = Reason.None;
                            break;
                        case WebLocationChecker.State.DomainDidntMatch:
                            BuildState = Reason.IsIncorrectDomain;
                            break;
                        case WebLocationChecker.State.EncounteredError:
                        default:
                            BuildState = Reason.CannotConfirmDomain;
                            break;
                    }
                }
            }
            else if ((Application.genuineCheckAvailable == true) && (Application.genuine == false))
            {
                BuildState = Reason.IsNotGenuine;
            }
            else
            {
                BuildState = Reason.None;
            }

            // Check if we're simulating failure
            if (Singleton.Instance.IsSimulatingMalformedGame == true)
            {
                // Indicate as such
                BuildState = Reason.JustTesting;
            }

            // Check if the build state is valid
            if (BuildState != Reason.None)
            {
                UpdateReason(BuildState);
                Show();
            }

            // Udpate flag
            IsBuildVerified = true;
        }
    }
}
