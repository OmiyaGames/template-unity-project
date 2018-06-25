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
        const BackgroundMenu.BackgroundType DefaultBackground = BackgroundMenu.BackgroundType.GradientRightToLeft;

        [Header("Confirmation Menu")]
        [SerializeField]
        Translations.TranslatedTextMeshPro messageLabel;
        [SerializeField]
        Button yesButton;
        [SerializeField]
        Button noButton;

        BackgroundSettings background = new BackgroundSettings();
        float selectDefaultAfterSeconds = -1f, timeDialogShown = -1f;
        int lastDisplayedSeconds = 0;
        System.Action<float> autoSelectAction = null;

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

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return background.BackgroundState;
            }
        }

        public override string TitleTranslationKey
        {
            get
            {
                return background.TitleTranslationKey;
            }
        }

        public override object[] TitleTranslationArgs
        {
            get
            {
                return background.TitleTranslationArgs;
            }
        }

        public int DisplayedTime
        {
            get
            {
                return Mathf.CeilToInt(selectDefaultAfterSeconds - (Time.realtimeSinceStartup - timeDialogShown));
            }
        }
        #endregion

        private void OnDestroy()
        {
            if(autoSelectAction != null)
            {
                Singleton.Instance.OnRealTimeUpdate -= autoSelectAction;
                autoSelectAction = null;
            }
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Call base method
            base.OnStateChanged(from, to);

            // Check if we're hiding
            if(to != VisibilityState.Visible)
            {
                // Reset all variables
                selectDefaultAfterSeconds = -1f;
                lastDisplayedSeconds = 0;

                // Unbind to event, if any
                OnDestroy();
            }
            else if (selectDefaultAfterSeconds > 0)
            {
                // Unbind to event, if any
                OnDestroy();

                // Grab the time in which this dialog became visible
                timeDialogShown = Time.realtimeSinceStartup;

                // Bind to update
                autoSelectAction = new System.Action<float>(UpdateLabelWithTime);
                Singleton.Instance.OnRealTimeUpdate += autoSelectAction;
            }
        }

        /// <summary>
        /// Sets up the dialog background based off of another menu.
        /// </summary>
        public void UpdateDialog(IMenu copyBackgroundSettings, string messageTranslatedKey = null, float automaticallySelectDefaultAfterSeconds = -1f)
        {
            // Check the parameter
            if(copyBackgroundSettings != null)
            {
                UpdateDialog(messageTranslatedKey, automaticallySelectDefaultAfterSeconds, copyBackgroundSettings.Background, copyBackgroundSettings.TitleTranslationKey, copyBackgroundSettings.TitleTranslationArgs);
            }
            else
            {
                UpdateDialog(messageTranslatedKey, automaticallySelectDefaultAfterSeconds);
            }
        }

        /// <summary>
        /// Sets up the dialog with the proper message and time on when to select the default dialog selection
        /// </summary>
        /// <param name="messageTranslatedKey"></param>
        /// <param name="automaticallySelectDefaultAfterSeconds"></param>
        public void UpdateDialog(string messageTranslatedKey = null, float automaticallySelectDefaultAfterSeconds = -1f, BackgroundMenu.BackgroundType backgroundType = DefaultBackground, string titleTranslationKey = null, params object[] titleTranslationArgs)
        {
            // Setup the timer
            selectDefaultAfterSeconds = -1;
            if (automaticallySelectDefaultAfterSeconds > 0)
            {
                selectDefaultAfterSeconds = automaticallySelectDefaultAfterSeconds;
            }

            // Setup the label
            if (string.IsNullOrEmpty(messageTranslatedKey) == true)
            {
                // Simply remove the message label entirely
                messageLabel.gameObject.SetActive(false);
            }
            else
            {
                // Activate the message label
                messageLabel.gameObject.SetActive(true);
                if(selectDefaultAfterSeconds > 0)
                {
                    // Update the label with time
                    lastDisplayedSeconds = DisplayedTime;
                    messageLabel.SetTranslationKey(messageTranslatedKey, lastDisplayedSeconds);
                }
                else
                {
                    // Update label, no time
                    messageLabel.TranslationKey = messageTranslatedKey;
                }
            }

            // Update background
            background.Update(backgroundType, titleTranslationKey, titleTranslationArgs);
        }

        public void OnYesClicked()
        {
            if(IsListeningToEvents == true)
            {
                // Indicate Yes was selected
                IsYesSelected = true;

                // Hide the dialog
                Hide();
            }
        }

        public void OnNoClicked()
        {
            if (IsListeningToEvents == true)
            {
                // Indicate No was selected
                IsYesSelected = false;

                // Hide the dialog
                Hide();
            }
        }

        void UpdateLabelWithTime(float deltaRealTIme)
        {
            // Check if we still have time left
            if ((Time.realtimeSinceStartup - timeDialogShown) < selectDefaultAfterSeconds)
            {
                // Get the ceiling of select-default duration, subtracted by how much time has passed.
                int currentDisplaySeconds = DisplayedTime;

                // Check if this is different from what's displayed
                if (currentDisplaySeconds != lastDisplayedSeconds)
                {
                    // Update the message label with the new unit
                    messageLabel.SetArguments(currentDisplaySeconds);

                    // Update current display
                    lastDisplayedSeconds = currentDisplaySeconds;
                }
            }
            else if (DefaultToYes == true)
            {
                // If time's up, "click" the default button
                OnYesClicked();
            }
            else
            {
                // If time's up, "click" the default button
                OnNoClicked();
            }
        }
    }
}
