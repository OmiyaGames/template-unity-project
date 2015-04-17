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

    int nextLevel = 1;
    Transition transitionState = Transition.NotTransitioning;
    float targetAlpha = 0, currentAlpha = 0;
    Color targetColor, targetTextColor;
    
    public Transition State
    {
        get
        {
            return transitionState;
        }
    }

    public int NextLevel
    {
        get
        {
            return nextLevel;
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
        if(Application.loadedLevel == nextLevel)
        {
            // Loaded the correct scene, display fade-out transition
            StartCoroutine(FadeOut());
        }
        else if(Application.loadedLevel == 0)
        {
            transitionState = Transition.NotTransitioning; 
        }
    }

    public bool LoadLevel(int levelIndex)
    {
        bool returnFlag = false;
        GameSettings settings = Singleton.Get<GameSettings>();
        if ((levelIndex >= 0) && (levelIndex <= settings.NumLevels))
        {
            // Play sound
            GetComponent<AudioSource>().Play();

            // Set the next level
            nextLevel = levelIndex;
            
            // Start fading in
            StartCoroutine(FadeIn());

            // Check what level we're loading to
            fullScreenText.text = settings.GetLevelName(levelIndex);
            returnFlag = true;
        }
        return returnFlag;
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

        // Check if we're in a webplayer
        Application.LoadLevelAsync(nextLevel);
    }
    
    IEnumerator FadeOut()
    {
        transitionState = Transition.FadingOut;
        yield return new WaitForSeconds(fadeOutDuration);
        transitionState = Transition.NotTransitioning;
    }
}
