using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    using Settings;

    ///-----------------------------------------------------------------------
    /// <copyright file="LevelSelectMenu.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2017 Omiya Games
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
        [SerializeField]
        RectTransform levelContent;
        [SerializeField]
        ListButtonScript levelButtonToDuplicate;
        [SerializeField]
        Button backButton;

        bool isButtonLocked = false;
        ListButtonScript[] allLevelButtons = null;
        GameObject lastUnlockedButton = null;

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
                return lastUnlockedButton;
            }
        }

        void Start()
        {
            // Setup all buttons
            allLevelButtons = SetupLevelButtons(levelButtonToDuplicate);

            // Update button states
            lastUnlockedButton = SetButtonsEnabled(true);
        }

        #region Button Events
        public void OnLevelClicked(SceneInfo level)
        {
            if (isButtonLocked == false)
            {
                Singleton.Get<SceneTransitionManager>().LoadScene(level);
                isButtonLocked = true;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnBackClicked()
        {
            if (isButtonLocked == false)
            {
                CurrentState = State.Hidden;
                isButtonLocked = true;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion

        public GameObject SetButtonsEnabled(bool enabled)
        {
            // Set all buttons
            GameObject returnButton = backButton.gameObject;
            GameSettings gameSettings = Singleton.Get<GameSettings>();
            for (int index = 0; index < allLevelButtons.Length; ++index)
            {
                // Make the button interactable if it's unlocked
                if ((enabled == true) && (index < gameSettings.NumLevelsUnlocked))
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

        protected override void OnStateChanged(IMenu.State from, IMenu.State to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            // If this menu is visible again, release the button lock
            if (to == State.Visible)
            {
                isButtonLocked = false;
            }
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
                    clone.transform.SetAsLastSibling();
                    clone.transform.localScale = Vector3.one;
                    clone.transform.localPosition = Vector3.one;
                    clone.transform.localRotation = Quaternion.identity;

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
            foreach (TranslatedText label in newButton.Labels)
            {
                label.SetTranslationKey(scene.DisplayName.TranslationKey, (newButton.Index + 1));
            }
            newButton.name = scene.DisplayName.TranslationKey;
        }

        // void SetupButtonNavigation(Button newButton, Button lastButton)
        // {
        //     // Update the new button to navigate up to the last button
        //     Navigation buttonNavigation = newButton.navigation;
        //     buttonNavigation.selectOnUp = lastButton;
        //     newButton.navigation = buttonNavigation;

        //     // Update the last button to navigate down to the new button
        //     buttonNavigation = lastButton.navigation;
        //     buttonNavigation.selectOnDown = newButton;
        //     lastButton.navigation = buttonNavigation;
        // }
        #endregion
    }
}
