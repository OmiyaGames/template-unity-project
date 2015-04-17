using UnityEngine;
using System.Collections;

public class LevelCompleteMenu : ISingletonScript
{
    [SerializeField]
    GameObject levelCompletePanel;
    [SerializeField]
    UnityEngine.UI.Text returnToMenuLabel = null;

    GameSettings settings = null;

    public bool IsVisible
    {
        get
        {
            return levelCompletePanel.activeSelf;
        }
    }

	public override void SingletonStart(Singleton instance)
    {
        // Do nothing
    }

    public override void SceneStart(Singleton instance)
    {
        // Check if we've already retrieve the settings
        if (settings != null)
        {
            // If so, don't do anything
            return;
        }

        // Retrieve settings
        settings = Singleton.Get<GameSettings>();

        // Check if we need to update the menu label
        if ((returnToMenuLabel != null) && (string.IsNullOrEmpty(settings.ReturnToMenuText) == false))
        {
            // Update the menu label
            returnToMenuLabel.text = string.Format(settings.ReturnToMenuText, settings.MenuLevel.DisplayName);
        }
    }

    public void Show()
    {
        if (IsVisible == false)
        {
            // Make the game object active
            levelCompletePanel.SetActive(true);
        }
    }

    public void Hide()
    {
        // Make the game object inactive
        levelCompletePanel.SetActive(false);
    }

    public void OnNextLevelClicked()
    {
        // Hide the panel
        Hide();

        // Transition to the current level
        GameSettings settings = Singleton.Get<GameSettings>();
        SceneTransition transition = Singleton.Get<SceneTransition>();
        transition.LoadLevel(settings.NextLevel);
    }

    public void OnRestartClicked()
    {
        // Hide the panel
        Hide();

        // Transition to the current level
        GameSettings settings = Singleton.Get<GameSettings>();
        SceneTransition transition = Singleton.Get<SceneTransition>();
        transition.LoadLevel(settings.CurrentLevel);
    }

    public void OnReturnToMenuClicked()
    {
        // Hide the panel
        Hide();

        // Transition to the menu
        GameSettings settings = Singleton.Get<GameSettings>();
        SceneTransition transition = Singleton.Get<SceneTransition>();
        transition.LoadLevel(settings.MenuLevel);
    }
}
