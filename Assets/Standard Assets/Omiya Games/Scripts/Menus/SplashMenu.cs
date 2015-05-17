using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    [RequireComponent(typeof(Animator))]
    public class SplashMenu : IMenu
    {
        public class LoadingInfo
        {
            bool isFinished = false;
            float ratio = 0;

            public bool IsFinished
            {
                get
                {
                    return isFinished;
                }
                set
                {
                    isFinished = value;
                    if(isFinished == true)
                    {
                        Ratio = 1f;
                    }
                }
            }

            public float Ratio
            {
                get
                {
                    return ratio;
                }
                set
                {
                    ratio = Mathf.Clamp01(value);
                }
            }
        }

        [Header("Logo Only settings")]
        [SerializeField]
        float logoDisplayDuration = 4f;
        [SerializeField]
        GameObject logoOnlySet = null;

        [Header("Loading Bar Included settings")]
        [SerializeField]
        RectTransform loadingBar = null;
        [SerializeField]
        GameObject loadingBarIncludedSet = null;

        public override Type MenuType
        {
            get
            {
                return Type.UnmanagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Override this property to display the loading bar in the splash menu
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return (LoadingStatus != null);
            }
        }

        /// <summary>
        /// Override this property to let the splash menu know
        /// whether it needs to display the loading bar (don't
        /// return null), and if so, how much the game loaded.
        /// </summary>
        public virtual LoadingInfo LoadingStatus
        {
            get
            {
                return null;
            }
        }

        // Use this for initialization
        void Start()
        {
            if(IsLoading == false)
            {
                // Start the fadeout
                logoOnlySet.SetActive(true);
                loadingBarIncludedSet.SetActive(false);
                StartCoroutine(DelayedFadeOut());
            }
            else
            {
                // Start the loading screen
                loadingBarIncludedSet.SetActive(true);
                logoOnlySet.SetActive(false);
                StartCoroutine(ShowLoadingScreen());
            }
        }

        protected override void OnStateChanged(State from, State to)
        {
            // Do nothing
        }

        IEnumerator DelayedFadeOut()
        {
            // Wait for the designated time
            yield return new WaitForSeconds(logoDisplayDuration);

            // Get the scene manager to change scenes
            Singleton.Get<SceneManager>().LoadMainMenu();
        }

        IEnumerator ShowLoadingScreen()
        {
            // Reset the loading bar
            Vector2 max = loadingBar.anchorMax;
            max.x = 0;
            loadingBar.anchorMax = max;

            // Wait until the loading status is finished
            while (LoadingStatus.IsFinished == false)
            {
                // Wait for a frame
                yield return null;

                // Update the loading bar
                max.x = LoadingStatus.Ratio;
                loadingBar.anchorMax = max;
            }

            // Wait for a frame
            yield return null;

            // Make the loading bar full
            max.x = 1;
            loadingBar.anchorMax = max;

            // Wait for a frame
            yield return null;

            // Get the scene manager to change scenes
            Singleton.Get<SceneManager>().LoadMainMenu();
        }
    }
}
