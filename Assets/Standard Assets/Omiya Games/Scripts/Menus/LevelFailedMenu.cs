using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    public class LevelFailedMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject levelFailedPanel;
        [SerializeField]
        UnityEngine.UI.Text returnToMenuLabel = null;
        [SerializeField]
        UnityEngine.UI.Text completeLabel = null;
        [SerializeField]
        string displayString = "{0} Failed!";

        SceneManager settings = null;

        public bool IsVisible
        {
            get
            {
                return levelFailedPanel.activeSelf;
            }
        }

        void Setup()
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

                // Setup complete label
                if ((completeLabel != null) && (string.IsNullOrEmpty(displayString) == false))
                {
                    completeLabel.text = string.Format(displayString, settings.MainMenu.DisplayName);
                }
            }
        }

        public void Show()
        {
            Setup();
            if (IsVisible == false)
            {
                // Make the game object active
                levelFailedPanel.SetActive(true);
            }
        }

        public void Hide()
        {
            // Make the game object inactive
            Setup();
            levelFailedPanel.SetActive(false);
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
            transition.LoadLevel(settings.CurrentScene);
        }
    }
}
