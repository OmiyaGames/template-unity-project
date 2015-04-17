using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : ISingletonScript
{
    public enum ClickedAction
    {
        Paused,
        Continue,
        Restart,
        ReturnToMenu
    }

    [SerializeField]
    GameObject pausePanel;
    [SerializeField]
    CursorLockMode lockModeOnResume = CursorLockMode.None;
    [SerializeField]
    Button[] allButtons = null;
    [SerializeField]
    Text returnToMenuLabel = null;
    [SerializeField]
    string returnToMenuText = "Return to {0}";

    /// <summary>
    /// The action to take when the visibility of the dialog changes
    /// </summary>
    System.Action<ClickedAction> onVisibleChanged;
    GameSettings settings = null;

    public override void SingletonStart(Singleton instance)
    {
        OnContinueClicked();
    }
    
    public override void SceneStart(Singleton instance)
    {
        // Check if we've already retrieve the settings
        if(settings != null)
        {
            // If so, don't do anything
            return;
        }

        // Retrieve settings
        settings = Singleton.Get<GameSettings>();

        // Check if we need to update the menu label
        if((returnToMenuLabel != null) && (string.IsNullOrEmpty(returnToMenuText) == false))
        {
            // Update the menu label
            returnToMenuLabel.text = string.Format(returnToMenuText, settings.MenuLevel.DisplayName);
        }
    }

    void OnApplicationPause(bool isPaused)
    {
        if(isPaused == true)
        {
            Singleton.Get<GameSettings>().SaveSettings();
            Show();
        }
    }

    public void Show(System.Action<ClickedAction> visibleChanged = null)
    {
        // Store function pointer
        onVisibleChanged = visibleChanged;

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;

        // Make the game object active
        pausePanel.SetActive(true);

        // Stop time
        Time.timeScale = 0;

        // Indicate change
        if (onVisibleChanged != null)
        {
            onVisibleChanged(ClickedAction.Paused);
            onVisibleChanged = null;
        }
    }

    public void Hide()
    {
        OnContinueClicked();
    }

    public void OnOptionsClicked()
    {
        // Disable all buttons
        for(int index = 0; index < allButtons.Length; ++index)
        {
            allButtons[index].interactable = false;
        }

        // FIXME: open the options dialog
    }

    public void OnContinueClicked()
    {
        OnContinueClicked(ClickedAction.Continue);
    }

    public void OnRestartClicked()
    {
        OnContinueClicked(ClickedAction.Restart);

        // Transition to the current level
        GameSettings settings = Singleton.Get<GameSettings>();
        SceneTransition transition = Singleton.Get<SceneTransition>();
        transition.LoadLevel(settings.CurrentLevel);
    }

    public void OnReturnToMenuClicked()
    {
        OnContinueClicked(ClickedAction.ReturnToMenu);

        // Transition to the menu
        GameSettings settings = Singleton.Get<GameSettings>();
        SceneTransition transition = Singleton.Get<SceneTransition>();
        transition.LoadLevel(settings.MenuLevel);
    }

    void OnContinueClicked(ClickedAction action)
    {
        // Make time flow again
        Time.timeScale = 1;

        // Lock the cursor
        Cursor.lockState = lockModeOnResume;

        // Hide the panel
        pausePanel.SetActive(false);

        // Indicate change
        if (onVisibleChanged != null)
        {
            onVisibleChanged(action);
            onVisibleChanged = null;
        }
    }

    void EnableAllButtons()
    {
        for (int index = 0; index < allButtons.Length; ++index)
        {
            allButtons[index].interactable = true;
        }
    }
}
