using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    MainMenuLayoutGroup levelLayoutGroup;
    [SerializeField]
    Button levelButton;
    [SerializeField]
    Button quitButton;

    bool isClicked = false;
    int index = 0;
    SceneTransition.Transition lastTransitionState = SceneTransition.Transition.NotTransitioning;
    Button[] allLevelButtons = null;
    SceneTransition transition = null;
    GameSettings settings = null;

    #region Properties
    public Button[] AllLevelButtons
    {
        get
        {
            return allLevelButtons;
        }
    }

    public Button QuitButton
    {
        get
        {
            return quitButton;
        }
    }
    #endregion

    void Start()
    {
        // Grab the settings
        transition = Singleton.Get<SceneTransition>();

        // Grab the game settings
        settings = Singleton.Get<GameSettings>();

        // Grab the parent of the level button
        Transform buttonParent = levelButton.transform.parent;
        
        // Setup all buttons
        allLevelButtons = SetupLevelButtons(buttonParent);

        // Check if we should remove the quit button (you can't quite out of a webplayer)
        if(settings.IsWebplayer == true)
        {
            // Grab the level gird's minimum range
            Vector2 minAnchor = levelLayoutGroup.CachedRectTransform.anchorMin;
            Vector2 minOffset = levelLayoutGroup.CachedRectTransform.offsetMin;

            // Grab quite button's minimum range
            RectTransform buttonTransform = quitButton.GetComponent<RectTransform>();
            minAnchor.y = buttonTransform.anchorMin.y;
            minOffset.y = buttonTransform.offsetMin.y;

            // Expand the level gird to encompass
            levelLayoutGroup.CachedRectTransform.anchorMin = minAnchor;
            levelLayoutGroup.CachedRectTransform.offsetMin = minOffset;

            // Disable the quit button entirely
            quitButton.gameObject.SetActive(false);
        }

        // Check if we need to update the button states
        if (transition.State != SceneTransition.Transition.NotTransitioning)
        {
            UpdateButtonEnabled(transition.State, allLevelButtons);
            lastTransitionState = transition.State;
        }
    }

    void Update()
    {
        // Check if we need to update the button states
        if (transition.State != lastTransitionState)
        {
            UpdateButtonEnabled(transition.State, allLevelButtons);
            lastTransitionState = transition.State;
        }
    }

    // FIXME: add options clicked event

    public void OnLevelClicked(int buttonNumber)
    {
        if ((isClicked == false) && (transition.LoadLevel(buttonNumber) == true))
        {
            isClicked = true;
        }
    }

    public void OnQuitClicked()
    {
        if (isClicked == false)
        {
            isClicked = true;
            Application.Quit();
        }
    }

    #region Helper Methods
    Button[] SetupLevelButtons(Transform buttonParent)
    {
        // Check how many levels there are
        Button[] allButtons = null;
        Text buttonLabel = null;
        GameObject clone = null;
        Button newButton = null;
        if (settings.NumLevels >= 1)
        {
            // Setup the first level button behavior
            levelButton.onClick.AddListener(() => { OnLevelClicked(1); });

            // Setup the first level button label
            buttonLabel = levelButton.GetComponentInChildren<Text>();
            buttonLabel.text = settings.GetLevelName(1);
            levelButton.name = buttonLabel.text;

            // Add the button into the button list
            allButtons = new Button[settings.NumLevels];
            allButtons[0] = levelButton;

            // Setup the rest of the buttons
            for (index = 2; index <= settings.NumLevels; ++index)
            {
                // Setup the level button
                clone = (GameObject)Instantiate(levelButton.gameObject);
                clone.transform.SetParent(buttonParent);

                // Setup the level button behavior
                newButton = clone.GetComponent<Button>();
                int levelIndex = index;
                newButton.onClick.AddListener(() => { OnLevelClicked(levelIndex); });

                // Setup the level button labels
                buttonLabel = newButton.GetComponentInChildren<Text>();
                buttonLabel.text = settings.GetLevelName(index);
                clone.name = buttonLabel.text;

                // Add the button into the button list
                allButtons[index - 1] = newButton;
            }
        }
        return allButtons;
    }

    void UpdateButtonEnabled(SceneTransition.Transition transitionState, Button[] allButtons)
    {
        if(allButtons != null)
        {
            // Check whether we want to enable buttons or not
            if(transitionState == SceneTransition.Transition.NotTransitioning)
            {
                // If not transitioning, enable buttons
                for (index = 0; index < allButtons.Length; ++index)
                {
                    // Make the button interactable if it's unlocked
                    allButtons[index].interactable = (index < settings.NumLevelsUnlocked);
                }

                // Enable the quit button
                quitButton.interactable = true;
            }
            else
            {
                // If so, disable all buttons
                for (index = 0; index < allButtons.Length; ++index)
                {
                    allButtons[index].interactable = false;
                }

                // Disable the quit button
                quitButton.interactable = true;
            }
        }
    }
    #endregion
}
