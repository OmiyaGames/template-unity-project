using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    public class PauseMenu : IMenu
    {
        public enum ClickedAction
        {
            Paused,
            Continue,
            Restart,
            ReturnToMenu
        }

        [SerializeField]
        Button resumeButton = null;
        [SerializeField]
        Text restartLabel = null;
        [SerializeField]
        Text returnToMenuLabel = null;

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
                return resumeButton.gameObject;
            }
        }

        protected virtual void Start()
        {
            // Update the labels on each button
            SceneManager manager = Singleton.Get<SceneManager>();
            restartLabel.text = string.Format(manager.RestartCurrentSceneText, manager.CurrentScene.DisplayName);
            returnToMenuLabel.text = string.Format(manager.ReturnToMenuText, manager.MainMenu.DisplayName);
        }

        void OnApplicationPause(bool isPaused)
        {
            if (isPaused == true)
            {
                Show();
            }
        }

        public override void Show(System.Action<IMenu> stateChanged = null)
        {
            // Call base function
            base.Show(stateChanged);

            // Unlock the cursor
            Cursor.lockState = CursorLockMode.None;

            // Stop time
            Singleton.Get<TimeManager>().IsManuallyPaused = true;
        }

        public override void Hide()
        {
            // Call base function
            base.Hide();

            // Lock the cursor to what the scene is set to
            Cursor.lockState = Singleton.Get<SceneManager>().CurrentScene.LockMode;

            // Resume the time
            Singleton.Get<TimeManager>().IsManuallyPaused = false;
        }

        public void OnOptionsClicked()
        {
            // Open the options dialog
            Singleton.Get<OptionsMenu>().Show();
        }

        public void OnResumeClicked()
        {
            Hide();
        }

        public void OnRestartClicked()
        {
            Hide();

            // Transition to the current level
            Singleton.Get<SceneManager>().ReloadCurrentScene();
        }

        public void OnReturnToMenuClicked()
        {
            Hide();

            // Transition to the menu
            Singleton.Get<SceneManager>().LoadMainMenu();
        }
    }
}
