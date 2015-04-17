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
    Button optionsButton;
    [SerializeField]
    Button quitButton;

    bool isClicked = false;
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

        // Setup all buttons
        allLevelButtons = SetupLevelButtons(levelButton.transform.parent);

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

        // Update button states
        UpdateButtonEnabled(transition.State == SceneTransition.Transition.NotTransitioning);
        lastTransitionState = transition.State;
    }

    void Update()
    {
        // Check if we need to update the button states
        if (transition.State != lastTransitionState)
        {
            UpdateButtonEnabled(transition.State == SceneTransition.Transition.NotTransitioning);
            lastTransitionState = transition.State;
        }
    }

    public void OnLevelClicked(GameSettings.LevelInfo level)
    {
        if (isClicked == false)
        {
            transition.LoadLevel(level);
            isClicked = true;
        }
    }

    public void OnOptionsClicked()
    {
        // FIXME: open the options menu, and disable every button
        // FIXME: also bind an action that would reset the button enabled
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
        GameObject clone = null;
        if (settings.NumLevels > 1)
        {
            // Add the button into the button list
            allButtons = new Button[settings.NumLevels - 1];

            // Setup the first level button behavior
            allButtons[0] = SetupButton(levelButton.gameObject, 1);

            // Setup the rest of the buttons
            for (int index = 2; index < settings.NumLevels; ++index)
            {
                // Setup the level button
                clone = (GameObject)Instantiate(levelButton.gameObject);
                clone.transform.SetParent(buttonParent);
                clone.transform.localScale = Vector3.one;

                // Add the button into the button list
                allButtons[index - 1] = SetupButton(clone, index);
            }
        }
        return allButtons;
    }

    Button SetupButton(GameObject buttonObject, int levelOrdinal)
    {
        // Add an event to the button
        Button newButton = buttonObject.GetComponent<Button>();
        newButton.onClick.AddListener(() => { OnLevelClicked(settings.Levels[levelOrdinal]); });

        // Setup the level button labels
        Text buttonLabel = newButton.GetComponentInChildren<Text>();
        buttonLabel.text = settings.Levels[levelOrdinal].DisplayName;
        buttonObject.name = buttonLabel.text;
        return newButton;
    }

    void UpdateButtonEnabled(bool enabled)
    {
        // Set all buttons
        for (int index = 0; index < AllLevelButtons.Length; ++index)
        {
            // Make the button interactable if it's unlocked
            if ((enabled == true) && (index <= settings.NumLevelsUnlocked))
            {
                AllLevelButtons[index].interactable = true;
            }
            else
            {
                AllLevelButtons[index].interactable = false;
            }
        }
        quitButton.interactable = enabled;
        optionsButton.interactable = enabled;
    }

    void EnableAllButtons()
    {
        UpdateButtonEnabled(true);
    }
    #endregion
}
