using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    public class CreditsMenu : IMenu
    {
        [Header("Components")]
        [SerializeField]
        Button defaultButton = null;
        [SerializeField]
        ScrollRect scrollable = null;
        [SerializeField]
        RectTransform content = null;

        [Header("Behavior")]
        [SerializeField]
        float startDelay = 1f;
        [SerializeField]
        float scrollSpeed = 20f;
        [SerializeField]
        float endDelay = 1f;

        float contentSize = 0, normalizedPosition = 0;
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
            // Adjust the size of the scroll panel content
            contentSize = content.sizeDelta.y;
            normalizedPosition = 1;

            // Start scrolling the contents
            StartCoroutine(ScrollCredits());
        }

        public override void Show(System.Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Unlock the cursor
            Cursor.lockState = CursorLockMode.None;

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
            bool wasVisible = (CurrentState == State.Visible);

            // Call base function
            base.Hide();

            if (wasVisible == true)
            {
                // Lock the cursor to what the scene is set to
                SceneManager manager = Singleton.Get<SceneManager>();
                Cursor.lockState = manager.CurrentScene.LockMode;

                // Unbind to Singleton's update function
                if (checkInput != null)
                {
                    Singleton.Instance.OnUpdate -= checkInput;
                    checkInput = null;
                }

                // Return to the menu
                manager.LoadMainMenu();
            }
        }

        IEnumerator ScrollCredits()
        {
            // Wait for a bit before starting the credits
            yield return new WaitForSeconds(startDelay);

            // Check what the scroll panel condition is so far
            while (Mathf.Approximately(normalizedPosition, 0) == false)
            {
                // Scroll the panel
                normalizedPosition -= (scrollSpeed * Time.deltaTime) / contentSize;
                normalizedPosition = Mathf.Clamp01(normalizedPosition);
                scrollable.verticalNormalizedPosition = normalizedPosition;

                // Wait for a frame
                yield return null;
            }

            // Wait for a bit before hiding the credits
            yield return new WaitForSeconds(endDelay);
            Hide();
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
