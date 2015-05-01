using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    public class LevelSelectMenu : IMenu
    {
        [SerializeField]
        GridLayoutGroup levelContent;
        [SerializeField]
        Button levelButtonToDuplicate;
        [SerializeField]
        Button backButton;

        bool isButtonLocked = false;
        Button[] allLevelButtons = null;
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
            
            // Make sure there are more than one button
            if(allLevelButtons.Length > 0)
            {
                // Get the grid layout size
                RectTransform contentTransform = levelContent.GetComponent<RectTransform>();

                // Calculate the height of the content
                float height = (levelContent.cellSize.y * allLevelButtons.Length);
                height += (levelContent.spacing.y * (allLevelButtons.Length - 1));
                contentTransform.sizeDelta = new Vector2(levelContent.cellSize.x, height);
            }
        }

        #region Button Events
        public void OnLevelClicked(SceneInfo level)
        {
            if (isButtonLocked == false)
            {
                Singleton.Get<SceneTransition>().LoadLevel(level);
                isButtonLocked = true;
            }
        }

        public void OnBackClicked()
        {
            if (isButtonLocked == false)
            {
                CurrentState = State.Hidden;
                isButtonLocked = true;
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
                    allLevelButtons[index].interactable = true;
                    returnButton = allLevelButtons[index].gameObject;
                }
                else
                {
                    allLevelButtons[index].interactable = false;
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
        Button[] SetupLevelButtons(Button buttonToDuplicate)
        {
            // Grab the Scene Manager
            SceneManager settings = Singleton.Get<SceneManager>();

            // Grab the parent transform from the button
            Transform buttonParent = buttonToDuplicate.transform.parent;

            // Check how many levels there are
            Button[] allButtons = null;
            GameObject clone = null;
            if (settings.NumLevels > 1)
            {
                // Add the button into the button list
                allButtons = new Button[settings.NumLevels];

                // Setup the first level button behavior
                int index = 0;
                SetupButtonEventAndName(settings, buttonToDuplicate, index);
                SetupButtonNavigation(buttonToDuplicate, backButton);
                allButtons[index] = buttonToDuplicate;
                ++index;

                // Setup the rest of the buttons
                for (; index < allButtons.Length; ++index)
                {
                    // Setup the level button
                    clone = (GameObject)Instantiate(buttonToDuplicate.gameObject);
                    clone.transform.SetParent(buttonParent);
                    clone.transform.localScale = Vector3.one;

                    // Add the button into the button list
                    allButtons[index] = SetupButtonEventAndName(settings, clone, index);
                    SetupButtonNavigation(allButtons[index], allButtons[index - 1]);
                }

                // Setup the last button
                SetupButtonNavigation(backButton, allButtons[allButtons.Length - 1]);
            }
            return allButtons;
        }

        Button SetupButtonEventAndName(SceneManager settings, GameObject buttonObject, int levelOrdinal)
        {
            // Add an event to the button
            Button newButton = buttonObject.GetComponent<Button>();
            SetupButtonEventAndName(settings, newButton, levelOrdinal);
            return newButton;
        }

        void SetupButtonEventAndName(SceneManager settings, Button newButton, int levelOrdinal)
        {
            // Add an event to the button
            newButton.onClick.AddListener(() =>
            {
                OnLevelClicked(settings.Levels[levelOrdinal]);
            });

            // Setup the level button labels
            Text[] buttonLabels = newButton.GetComponentsInChildren<Text>(true);
            foreach (Text label in buttonLabels)
            {
                label.text = settings.Levels[levelOrdinal].DisplayName;
            }
            newButton.name = settings.Levels[levelOrdinal].DisplayName;
        }

        void SetupButtonNavigation(Button newButton, Button lastButton)
        {
            // Update the new button to navigate up to the last button
            Navigation buttonNavigation = newButton.navigation;
            buttonNavigation.selectOnUp = lastButton;
            newButton.navigation = buttonNavigation;

            // Update the last button to navigate down to the new button
            buttonNavigation = lastButton.navigation;
            buttonNavigation.selectOnDown = newButton;
            lastButton.navigation = buttonNavigation;
        }
        #endregion
    }
}
