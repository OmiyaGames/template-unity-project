using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MalformedGameMenu.cs" company="Omiya Games">
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

            public void UpdateButton(WebsiteButton button)
            {
                button.LabelComponent.TranslationKey = labelTranslationKey;
                button.RedirectTo = redirectTo;
            }
        }

        [SerializeField]
        bool showDebugInformation = false;

        [Header("First Option")]
        [SerializeField]
        WebsiteButton websiteButton = null;
        [SerializeField]
        WebsiteInfo websiteInfo;

        [Header("Second Option")]
        [SerializeField]
        WebsiteButton otherSitesButton = null;
        [SerializeField]
        WebsiteInfo[] otherSites = null;
        [SerializeField]
        TranslatedText[] secondOptionSet = null;

        [Header("Error Messages")]
        [SerializeField]
        TranslatedText reasonMessage = null;
        [SerializeField]
        string gameIsNotGenuineMessageTranslationKey;
        [SerializeField]
        string cannotConfirmDomainMessageTranslationKey;
        [SerializeField]
        string domainDoesNotMatchMessageTranslationKey;

        readonly List<WebsiteButton> allSecondOptionButtons = new List<WebsiteButton>();

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
                return websiteButton.gameObject;
            }
        }

        public override void Show(Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Setup the dialog
            websiteInfo.UpdateButton(websiteButton);
            if((otherSites != null) && (otherSites.Length > 0))
            {
                // Setup the second options
                SetupSecondOptions();
            }
            else
            {
                // Turn off everything related to the second options
                for(int index = 0; index < secondOptionSet.Length; ++index)
                {
                    secondOptionSet[index].gameObject.SetActive(false);
                }
                otherSitesButton.gameObject.SetActive(false);
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

            if(showDebugInformation == true)
            {
                reason = Reason.JustTesting;
            }

            // Update the reason for this dialog to appear
            switch(reason)
            {
                case Reason.CannotConfirmDomain:
                    // Update translation key
                    reasonMessage.TranslationKey = cannotConfirmDomainMessageTranslationKey;
                    break;
                case Reason.IsIncorrectDomain:
                    if (webChecker != null)
                    {
                        // Setup translation key, with proper population of fields
                        reasonMessage.SetTranslationKey(domainDoesNotMatchMessageTranslationKey, webChecker.RetrievedHostName);
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
                    Utility.BuildTestMessage(builder, webChecker);
                    reasonMessage.CurrentText = builder.ToString();
                    break;
                default:
                    // Update translation key
                    reasonMessage.TranslationKey = gameIsNotGenuineMessageTranslationKey;
                    break;
            }
        }

        #region Helper Methods
        void SetupSecondOptions()
        {
            // Populate the list of buttons with at least one button
            if (allSecondOptionButtons.Count <= 0)
            {
                allSecondOptionButtons.Add(otherSitesButton);
            }

            // Go through all the sites
            int index = 0;
            GameObject clone;
            for (; index < otherSites.Length; ++index)
            {
                // Check if we have enough buttons
                if (allSecondOptionButtons.Count <= index)
                {
                    // If not, create a new one
                    clone = Instantiate<GameObject>(otherSitesButton.gameObject);
                    allSecondOptionButtons.Add(clone.GetComponent<WebsiteButton>());

                    // Position this button properly
                    clone.transform.SetParent(otherSitesButton.transform.parent);
                    clone.transform.localScale = Vector3.one;
                    clone.transform.localRotation = Quaternion.identity;
                    clone.transform.SetSiblingIndex(otherSitesButton.transform.GetSiblingIndex() + index);
                }

                // Setup this button
                allSecondOptionButtons[index].gameObject.SetActive(true);
                otherSites[index].UpdateButton(allSecondOptionButtons[index]);
                
            }

            // Turn off the rest of the buttons
            for (; index < allSecondOptionButtons.Count; ++index)
            {
                allSecondOptionButtons[index].gameObject.SetActive(false);
            }
        }
        #endregion
    }
}
