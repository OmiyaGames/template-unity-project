using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    public abstract class ISceneChangingMenu : IMenu
    {
        [Header("Components")]
        [SerializeField]
        protected Button defaultButton = null;
        [SerializeField]
        protected Text completeLabel = null;
        [SerializeField]
        protected Text failedLabel = null;
        [SerializeField]
        protected Text nextSceneLabel = null;
        [SerializeField]
        protected Text restartLabel = null;
        [SerializeField]
        protected Text returnToMenuLabel = null;

        abstract public bool PauseOnShow
        {
            get;
        }

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
                return defaultButton.gameObject;
            }
        }

        protected virtual void Start()
        {
            // Update the labels on each button
            MenuManager manager = Singleton.Get<MenuManager>();

            // Setup all labels, if available
            if (completeLabel != null)
            {
                completeLabel.text = manager.CompletedCurrentSceneText;
            }
            if (failedLabel != null)
            {
                failedLabel.text = manager.FailedCurrentSceneText;
            }
            if (nextSceneLabel != null)
            {
                nextSceneLabel.text = manager.NextSceneText;
            }
            if (restartLabel != null)
            {
                restartLabel.text = manager.RestartCurrentSceneText;
            }
            if (returnToMenuLabel != null)
            {
                returnToMenuLabel.text = manager.ReturnToMenuText;
            }
        }

        public override void Show(System.Action<IMenu> stateChanged = null)
        {
            // Call base function
            base.Show(stateChanged);

            // Unlock the cursor
            Cursor.lockState = CursorLockMode.None;

            // Check if we should stop time
            if (PauseOnShow == true)
            {
                // Stop time
                Singleton.Get<TimeManager>().IsManuallyPaused = true;
            }
        }

        public override void Hide()
        {
            // Call base function
            base.Hide();

            // Lock the cursor to what the scene is set to
            Cursor.lockState = Singleton.Get<SceneManager>().CurrentScene.LockMode;

            // Check if we should stop time
            if (PauseOnShow == true)
            {
                // Resume the time
                Singleton.Get<TimeManager>().IsManuallyPaused = false;
            }
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