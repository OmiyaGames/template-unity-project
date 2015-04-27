using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    public class LevelCompleteMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject levelCompletePanel;
        [SerializeField]
        UnityEngine.UI.Text returnToMenuLabel = null;
        [SerializeField]
        UnityEngine.UI.Button nextLevelButton = null;
        [SerializeField]
        UnityEngine.UI.Text completeLabel = null;
        [SerializeField]
        string displayString = "{0} complete!";
        [SerializeField]
        bool unlockNextLevel = true;

        SceneManager settings = null;

        public bool IsVisible
        {
            get
            {
                return levelCompletePanel.activeSelf;
            }
        }

        public void Show()
        {
            Setup();
            if (IsVisible == false)
            {
                // Check if we need to unlock the next level
                if (unlockNextLevel == true)
                {
                    // Retrieve settings, and check if this level ISN'T unlocked
                    GameSettings gameSettings = Singleton.Get<GameSettings>();
                    if((gameSettings != null) && (gameSettings.NumLevelsUnlocked <= settings.CurrentScene.Ordinal))
                    {
                        // Indicate the next scene is unlocked
                        gameSettings.NumLevelsUnlocked = (settings.CurrentScene.Ordinal + 1);
                    }
                }

                // Make the game object active
                levelCompletePanel.SetActive(true);
            }
        }

        public void Hide()
        {
            // Make the game object inactive
            levelCompletePanel.SetActive(false);
        }

        #region Button Events
        public void OnNextLevelClicked()
        {
            // Hide the panel
            Hide();

            // Transition to the current level
            SceneTransition transition = Singleton.Get<SceneTransition>();
            transition.LoadLevel(settings.NextScene);
        }

        public void OnRestartClicked()
        {
            // Hide the panel
            Hide();

            // Transition to the current level
            SceneTransition transition = Singleton.Get<SceneTransition>();
            transition.LoadLevel(settings.CurrentScene);
        }

        public void OnReturnToMenuClicked()
        {
            // Hide the panel
            Hide();

            // Transition to the menu
            SceneTransition transition = Singleton.Get<SceneTransition>();
            transition.LoadLevel(settings.MainMenu);
        }
        #endregion

        protected virtual void Setup()
        {
            if (settings == null)
            {
                // Retrieve settings
                settings = Singleton.Get<SceneManager>();

                // Check if we need to update the menu label
                if (returnToMenuLabel != null)
                {
                    // Update the menu label
                    returnToMenuLabel.text = settings.ReturnToMenuText;
                }

                // Check if we need to disable the next level button
                if ((nextLevelButton != null) && (settings.NextScene == null))
                {
                    nextLevelButton.interactable = false;
                }

                // Setup complete label
                if ((completeLabel != null) && (string.IsNullOrEmpty(displayString) == false))
                {
                    completeLabel.text = string.Format(displayString, settings.CurrentScene.DisplayName);
                }
            }
        }
    }
}
