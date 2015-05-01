using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    [RequireComponent(typeof(Animator))]
    public class SplashMenu : BackgroundMenu
    {
        public enum Transition
        {
            NextScene,
            FadeOut
        }
        [SerializeField]
        Transition tranitionType = Transition.FadeOut;
        [SerializeField]
        float fadeoutDuration = 1f;

        // Use this for initialization
        protected override void Start()
        {
            // By default, show the splash screen
            CurrentState = State.Visible;

            // Check if the last scene was splash (or this is the first scene loaded)
            SceneManager manager = Singleton.Get<SceneManager>();
            if((manager.LastScene == null) || (manager.LastScene == manager.Splash))
            {
                // Check if we need to fade out
                if (manager.CurrentScene == manager.MainMenu)
                {
                    // Start the fadeout
                    StartCoroutine(DelayedFadeOut());
                }
                else
                {
                    // Load the next level
                    manager.LoadMainMenu();
                }
            }
            else
            {
                // Hide the splash
                CurrentState = State.Hidden;
                gameObject.SetActive(false);
            }
        }

        protected override void OnDestroy()
        {
            // Do nothing
        }

        IEnumerator DelayedFadeOut()
        {
            // Wait for the designated time
            yield return new WaitForSeconds(fadeoutDuration);

            // Indicate we're hidden
            CurrentState = State.Hidden;
        }
    }
}
