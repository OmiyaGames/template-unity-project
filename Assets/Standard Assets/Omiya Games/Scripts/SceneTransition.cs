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
	
	public float fadeInDuration = 0.6f;
	public float fadeInSpeed = 1f;
	public float fadeOutDuration = 1f;
	public float fadeOutSpeed = 5f;
    public Image fullScreenImage;
    public Text fullScreenText;
    public string[] levelNames;
	
	private int mNextLevel = 1;
	private Transition mTransitionState = Transition.NotTransitioning;
	private float mTargetAlpha = 0;
	private float mCurrentAlpha = 0;
	private Color mTargetColor, mTargetTextColor;
	
	public Transition State
	{
		get
		{
			return mTransitionState;
		}
	}

	public int NextLevel
	{
		get
		{
			return mNextLevel;
		}
	}

	public override void SingletonStart(Singleton instance)
	{
        mTargetColor = fullScreenImage.color;
        mTargetTextColor = fullScreenText.color;

		mTargetAlpha = 0;
		mCurrentAlpha = 0;
		
        mTargetColor.a = mTargetAlpha;
        mTargetTextColor.a = mTargetAlpha;
		
        fullScreenImage.color = mTargetColor;
        fullScreenText.color = mTargetTextColor;

        fullScreenImage.gameObject.SetActive(false);
        fullScreenText.gameObject.SetActive(false);
    }
	
    public override void SceneStart(Singleton instance)
	{
		if(Application.loadedLevel == mNextLevel)
		{
			// Loaded the correct scene, display fade-out transition
			StartCoroutine(FadeOut());
		}
		else if(Application.loadedLevel == 0)
		{
			mTransitionState = Transition.NotTransitioning; 
		}
	}

    public string GetLevelName(int levelIndex)
    {
        string returnString = "Menu";
        if (levelIndex > 0)
        {
            if (levelIndex <= levelNames.Length)
            {
                returnString = levelNames[levelIndex - 1];
            }
            else
            {
                returnString = "Level " + levelIndex;
            }
        }
        return returnString;
    }
	
	public bool LoadLevel(int levelIndex)
	{
        bool returnFlag = false;
		if((levelIndex >= 0) && (levelIndex <= GameSettings.NumLevels))
		{
			// Play sound
			audio.Play();

			// Set the next level
			mNextLevel = levelIndex;
			
			// Start fading in
			StartCoroutine(FadeIn());

            // Check what level we're loading to
            fullScreenText.text = GetLevelName(levelIndex);
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
					mTargetColor = fullScreenImage.color;
                    mTargetTextColor = fullScreenText.color;

					mTargetAlpha = 1;
					mCurrentAlpha = 0;
					
                    mTargetColor.a = mTargetAlpha;
                    mTargetTextColor.a = mTargetAlpha;

                    fullScreenImage.color = mTargetColor;
                    fullScreenText.color = mTargetTextColor;

					fullScreenImage.gameObject.SetActive(true);
                    fullScreenText.gameObject.SetActive(true);
				}
				else
				{
					mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, (Time.deltaTime * fadeInSpeed));
					
                    mTargetColor.a = mCurrentAlpha;
                    mTargetTextColor.a = mCurrentAlpha;

					fullScreenImage.color = mTargetColor;
                    fullScreenText.color = mTargetTextColor;
				}
				break;
			}
			case Transition.FadingOut:
			{
				mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, (Time.deltaTime * fadeOutSpeed));
				
                mTargetColor.a = mCurrentAlpha;
                mTargetTextColor.a = mCurrentAlpha;

				fullScreenImage.color = mTargetColor;
                fullScreenText.color = mTargetTextColor;
				break;
			}
			case Transition.CompletelyFaded:
			{
				mTargetColor = fullScreenImage.color;
                mTargetTextColor = fullScreenText.color;

				mTargetAlpha = 0;
				mCurrentAlpha = 1;
				
                mTargetColor.a = mCurrentAlpha;
                mTargetTextColor.a = mCurrentAlpha;

                fullScreenImage.color = mTargetColor;
                fullScreenText.color = mTargetTextColor;

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
		mTransitionState = Transition.FadingIn;
		yield return new WaitForSeconds(fadeInDuration);
		mTransitionState = Transition.CompletelyFaded;

		// Check if we're in a webplayer
		GameSettings settings = Singleton.Get<GameSettings>();
		Application.LoadLevelAsync(mNextLevel);
	}
	
	IEnumerator FadeOut()
	{
		mTransitionState = Transition.FadingOut;
		yield return new WaitForSeconds(fadeOutDuration);
		mTransitionState = Transition.NotTransitioning;
	}
}
