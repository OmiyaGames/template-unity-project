using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SceneTransition : ISingletonScript
{
    public enum Transition
    {
        NotTransitioning,
        FadingOut,
        FadingIn,
        CompletelyFaded
    }

    [Header("Fade Duration and Speed")]
    [SerializeField]
    float fadeInDuration = 0.6f;
    [SerializeField]
    float fadeInSpeed = 1f;
    [SerializeField]
    float fadeOutDuration = 1f;
    [SerializeField]
    float fadeOutSpeed = 5f;
    [Header("Fade Components")]
    [SerializeField]
    Image fullScreenImage;
    [SerializeField]
    Text fullScreenText;

    string nextLevel = "";
    Transition transitionState = Transition.NotTransitioning;
    float targetAlpha = 0, currentAlpha = 0;
    Color targetColor, targetTextColor;
    AudioSource audioCache = null;

    public Transition State
    {
        get
        {
            return transitionState;
        }
    }

    public AudioSource Sound
    {
        get
        {
            if(audioCache == null)
            {
                audioCache = GetComponent<AudioSource>();
            }
            return audioCache;
        }
    }

    public override void SingletonStart(Singleton instance)
    {
        targetColor = fullScreenImage.color;
        targetTextColor = fullScreenText.color;

        targetAlpha = 0;
        currentAlpha = 0;
        
        targetColor.a = targetAlpha;
        targetTextColor.a = targetAlpha;
        
        fullScreenImage.color = targetColor;
        fullScreenText.color = targetTextColor;

        fullScreenImage.gameObject.SetActive(false);
        fullScreenText.gameObject.SetActive(false);
    }
    
    public override void SceneStart(Singleton instance)
    {
        if(string.Equals(Application.loadedLevelName, nextLevel) == true)
        {
            // Loaded the correct scene, display fade-out transition
            StartCoroutine(FadeOut());
        }
        else if(Application.loadedLevel == 0)
        {
            transitionState = Transition.NotTransitioning; 
        }
    }

    public void LoadLevel(GameSettings.LevelInfo level)
    {
        // Play sound
        Sound.Play();

        // Set the next level
        nextLevel = level.SceneName;
            
        // Start fading in
        StartCoroutine(FadeIn());

        // Check what level we're loading to
        fullScreenText.text = level.DisplayName;
    }
    
    void Update()
    {
        // Do the transitioning here
        switch(State)
        {
            case Transition.FadingIn:
            {
                if(fullScreenImage.enabled == false)
                {
                    targetColor = fullScreenImage.color;
                    targetTextColor = fullScreenText.color;

                    targetAlpha = 1;
                    currentAlpha = 0;
                    
                    targetColor.a = targetAlpha;
                    targetTextColor.a = targetAlpha;

                    fullScreenImage.color = targetColor;
                    fullScreenText.color = targetTextColor;

                    fullScreenImage.gameObject.SetActive(true);
                    fullScreenText.gameObject.SetActive(true);
                }
                else
                {
                    currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, (Time.deltaTime * fadeInSpeed));
                    
                    targetColor.a = currentAlpha;
                    targetTextColor.a = currentAlpha;

                    fullScreenImage.color = targetColor;
                    fullScreenText.color = targetTextColor;
                }
                break;
            }
            case Transition.FadingOut:
            {
                currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, (Time.deltaTime * fadeOutSpeed));
                
                targetColor.a = currentAlpha;
                targetTextColor.a = currentAlpha;

                fullScreenImage.color = targetColor;
                fullScreenText.color = targetTextColor;
                break;
            }
            case Transition.CompletelyFaded:
            {
                targetColor = fullScreenImage.color;
                targetTextColor = fullScreenText.color;

                targetAlpha = 0;
                currentAlpha = 1;
                
                targetColor.a = currentAlpha;
                targetTextColor.a = currentAlpha;

                fullScreenImage.color = targetColor;
                fullScreenText.color = targetTextColor;

                fullScreenImage.gameObject.SetActive(true);
                fullScreenText.gameObject.SetActive(true);
                break;
            }
            default:
            {
                if(fullScreenImage.enabled == true)
                {
                    fullScreenImage.gameObject.SetActive(false);
                }
                if(fullScreenText.enabled == true)
                {
                    fullScreenText.gameObject.SetActive(false);
                }
                break;
            }
        }
    }
    
    IEnumerator FadeIn()
    {
        transitionState = Transition.FadingIn;
        yield return new WaitForSeconds(fadeInDuration);
        transitionState = Transition.CompletelyFaded;

        // Load the next level
        Application.LoadLevelAsync(nextLevel);
    }
    
    IEnumerator FadeOut()
    {
        transitionState = Transition.FadingOut;
        yield return new WaitForSeconds(fadeOutDuration);
        transitionState = Transition.NotTransitioning;
    }
}
