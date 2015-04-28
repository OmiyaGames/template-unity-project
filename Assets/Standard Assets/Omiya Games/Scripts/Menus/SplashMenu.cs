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
            // Splash should be visible in the beginning
            CurrentState = State.Visible;

            // Check if we need to fade out
            if (tranitionType == Transition.FadeOut)
            {
                // Start the fadeout
                StartCoroutine(DelayedFadeOut());
            }
            else
            {
                // Load the next level
                Application.LoadLevel(Singleton.Get<SceneManager>().MainMenu.SceneName);
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
