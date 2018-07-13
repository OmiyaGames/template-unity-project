using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Scenes;
using OmiyaGames.Translations;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LevelSelectMenu.cs" company="Omiya Games">
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
    /// Menu that let's you switch to a specific level scene. You can retrieve this
    /// menu from the singleton script, <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    public class LevelSelectMenu : IMenu
    {
        const BackgroundMenu.BackgroundType DefaultBackground = BackgroundMenu.BackgroundType.GradientRightToLeft;

        [Header("Level Select")]
        [SerializeField]
        MenuNavigator navigator;
        [SerializeField]
        RectTransform levelContent;
        [SerializeField]
        ListButtonScript levelButtonToDuplicate;
        [SerializeField]
        Button backButton;

        ListButtonScript[] allLevelButtons = null;
        BackgroundSettings background = new BackgroundSettings();

        #region Properties
        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override Selectable DefaultUi
        {
            get
            {
                Selectable returnObject = backButton;
                if((allLevelButtons != null) && (allLevelButtons.Length > 0))
                {
                    // Parse the level button array list in reverse direction
                    for(int index = (allLevelButtons.Length - 1); index >= 0; --index)
                    {
                        // Check if this button is interactable
                        if((allLevelButtons[index] != null) && (allLevelButtons[index].Button != null) && (allLevelButtons[index].Button.IsInteractable() == true))
                        {
                            returnObject = allLevelButtons[index].Button;
                            break;
                        }
                    }
                }
                return returnObject;
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

        public override MenuNavigator Navigator
        {
            get
            {
                return navigator;
            }
        }
        #endregion

        /// <summary>
        /// Sets up the dialog background based off of another menu.
        /// </summary>
        public void UpdateDialog(IMenu copyBackgroundSettings)
        {
            // Check the parameter
            if (copyBackgroundSettings != null)
            {
                background.CopySettings(copyBackgroundSettings);
            }
        }

        /// <summary>
        /// Sets up the dialog with the proper message and time on when to select the default dialog selection
        /// </summary>
        /// <param name="messageTranslatedKey"></param>
        /// <param name="automaticallySelectDefaultAfterSeconds"></param>
        public void UpdateDialog(BackgroundMenu.BackgroundType backgroundType = DefaultBackground, string titleTranslationKey = null, params object[] titleTranslationArgs)
        {
            // Update background
            background.Update(backgroundType, titleTranslationKey, titleTranslationArgs);
        }

        protected override void OnSetup()
        {
            // Setup all buttons
            allLevelButtons = SetupLevelButtons(levelButtonToDuplicate);

            // Update button states
            SetButtonsEnabled(true);

            // Setup Navigator

            // Call base method
            base.OnSetup();
        }

        #region Button Events
        public void OnLevelClicked(SceneInfo level)
        {
            if (IsListeningToEvents == true)
            {
                SceneChanger.LoadScene(level);
            }
        }
        #endregion

        public GameObject SetButtonsEnabled(bool enabled)
        {
            // Set all buttons
            GameObject returnButton = backButton.gameObject;
            for (int index = 0; index < allLevelButtons.Length; ++index)
            {
                // Make the button interactable if it's unlocked
                if ((enabled == true) && (index < Settings.NumLevelsUnlocked))
                {
                    allLevelButtons[index].Button.interactable = true;
                    returnButton = allLevelButtons[index].gameObject;
                }
                else
                {
                    allLevelButtons[index].Button.interactable = false;
                }
            }
            backButton.interactable = enabled;

            // Return the last interactable button
            return returnButton;
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Call the base method
            base.OnStateChanged(from, to);
        }

        #region Helper Methods
        ListButtonScript[] SetupLevelButtons(ListButtonScript buttonToDuplicate)
        {
            // Grab the Scene Manager
            SceneTransitionManager settings = Singleton.Get<SceneTransitionManager>();

            // Check how many levels there are
            ListButtonScript[] allButtons = null;
            GameObject clone = null;
            if (settings.NumLevels >= 1)
            {
                // Add the button into the button list
                allButtons = new ListButtonScript[settings.NumLevels];

                // Setup the first level button behavior
                int index = 0;
                SetupButtonEventAndName(settings, buttonToDuplicate);
                //SetupButtonNavigation(buttonToDuplicate, backButton);
                allButtons[index] = buttonToDuplicate;
                ++index;

                // Setup the rest of the buttons
                for (; index < allButtons.Length; ++index)
                {
                    // Setup the level button
                    clone = Instantiate<GameObject>(buttonToDuplicate.gameObject);
                    clone.transform.SetParent(levelContent);
                    clone.transform.localScale = Vector3.one;
                    clone.transform.localPosition = Vector3.one;
                    clone.transform.localRotation = Quaternion.identity;
                    clone.transform.SetAsLastSibling();

                    // Push the back button at the bottom
                    backButton.transform.SetAsLastSibling();

                    // Add the button into the button list
                    allButtons[index] = SetupButtonEventAndName(settings, clone);
                    //SetupButtonNavigation(allButtons[index], allButtons[index - 1]);
                }

                // Setup the last button
                //SetupButtonNavigation(backButton, allButtons[allButtons.Length - 1]);
            }
            return allButtons;
        }

        ListButtonScript SetupButtonEventAndName(SceneTransitionManager settings, GameObject buttonObject)
        {
            // Add an event to the button
            ListButtonScript newButton = buttonObject.GetComponent<ListButtonScript>();
            SetupButtonEventAndName(settings, newButton);
            return newButton;
        }

        void SetupButtonEventAndName(SceneTransitionManager settings, ListButtonScript newButton)
        {
            // Add an event to the button
            SceneInfo scene = settings.Levels[newButton.Index];
            newButton.OnClicked += ((button) =>
            {
                OnLevelClicked(scene);
            });

            // Setup the level button labels
            foreach (TranslatedTextMeshPro label in newButton.Labels)
            {
                if(label != null)
                {
                    label.SetTranslationKey(scene.DisplayName.TranslationKey, (newButton.Index + 1));
                }
            }
            newButton.name = scene.DisplayName.TranslationKey;
        }
        #endregion
    }
}
