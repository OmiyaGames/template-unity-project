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

    /// <summary>
    /// The action to take when the visibility of the dialog changes
    /// </summary>
    System.Action<ClickedAction> onVisibleChanged;

    public override void SingletonStart(Singleton instance)
    {
        OnContinueClicked();
    }
    
    public override void SceneStart(Singleton instance)
    {
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

    public void OnContinueClicked()
    {
        OnContinueClicked(ClickedAction.Continue);
    }

    public void OnRestartClicked()
    {
        OnContinueClicked(ClickedAction.Restart);
        Singleton.Get<SceneTransition>().LoadLevel(Application.loadedLevel);
    }

    public void OnReturnToMenuClicked()
    {
        OnContinueClicked(ClickedAction.ReturnToMenu);
        Singleton.Get<SceneTransition>().LoadLevel(GameSettings.MenuLevel);
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
}
