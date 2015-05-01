using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    public class LevelIntroMenu : IMenu
    {
        [Header("Components")]
        [SerializeField]
        Button defaultButton = null;
        [SerializeField]
        Text levelNameLabel = null;

        [Header("Behavior")]
        [SerializeField]
        bool pauseOnStart = false;

        System.Action<float> checkInput = null;

        public override Type MenuType
        {
            get
            {
                return Type.DefaultManagedMenu;
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
            // Setup all labels, if available
            if (levelNameLabel != null)
            {
                levelNameLabel.text = Singleton.Get<SceneManager>().CurrentScene.DisplayName;
            }
        }

        public override void Show(System.Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Unlock the cursor
            Cursor.lockState = CursorLockMode.None;

            // Check if we should stop time
            if (pauseOnStart == true)
            {
                // Stop time
                Singleton.Get<TimeManager>().IsManuallyPaused = true;
            }

            // Check if we've previously binded to the singleton's update function
            if (checkInput != null)
            {
                Singleton.Instance.OnUpdate -= checkInput;
                checkInput = null;
            }

            // Bind to Singleton's update function
            checkInput = new System.Action<float>(CheckForAnyKey);
            Singleton.Instance.OnUpdate += checkInput;
        }

        public override void Hide()
        {
            // Call base function
            base.Hide();

            // Lock the cursor to what the scene is set to
            Cursor.lockState = Singleton.Get<SceneManager>().CurrentScene.LockMode;

            // Check if we should stop time
            if (pauseOnStart == true)
            {
                // Resume the time
                Singleton.Get<TimeManager>().IsManuallyPaused = false;
            }

            // Unbind to Singleton's update function
            if (checkInput != null)
            {
                Singleton.Instance.OnUpdate -= checkInput;
                checkInput = null;
            }
        }

        void CheckForAnyKey(float deltaTime)
        {
            if(Input.anyKeyDown == true)
            {
                Hide();
            }
        }
    }
}
