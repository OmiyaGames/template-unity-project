using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    [RequireComponent(typeof(Animator))]
    public class SplashMenu : BackgroundMenu
    {
        [SerializeField]
        bool fadeoutSplashOnStart = true;
        [SerializeField]
        float fadeoutDuration = 1f;

        // Use this for initialization
        protected override void Start()
        {
            // Splash should be visible in the beginning
            CurrentState = State.Visible;

            // Check if we need to fade out
            if(fadeoutSplashOnStart == true)
            {
                // Start the fadeout
                StartCoroutine(DelayedFadeOut());
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
