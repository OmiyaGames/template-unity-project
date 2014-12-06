using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public Button levelButton;

    bool isClicked = false;
    
    void Awake()
    {
        // Grab the parent of the level button
        Transform buttonParent = levelButton.transform.parent;

        // Check how many levels there are
        Text buttonLabel = null;
        GameObject clone = null;
        Button newButton = null;
        if(GameSettings.NumLevels >= 1)
        {
            // Grab the settings
            SceneTransition transition = Singleton.Get<SceneTransition>();
            
            // Setup the first level button behavior
            levelButton.onClick.AddListener(() => { OnLevelClicked(1); });

            // Setup the first level button label
            buttonLabel = levelButton.GetComponentInChildren<Text>();
            buttonLabel.text = transition.GetLevelName(1);
            levelButton.name = buttonLabel.text;

            // Setup the rest of the buttons
            for(int i = 2; i <= GameSettings.NumLevels; ++i)
            {
                // Setup the level button
                clone = (GameObject)Instantiate(levelButton.gameObject);
                clone.transform.SetParent(buttonParent);

                // Setup the level button behavior
                newButton = clone.GetComponent<Button>();
                int levelIndex = i;
                newButton.onClick.AddListener(() => { OnLevelClicked(levelIndex); });

                // Setup the level button labels
                buttonLabel = newButton.GetComponentInChildren<Text>();
                buttonLabel.text = transition.GetLevelName(i);
                clone.name = buttonLabel.text;
            }
        }
    }

    public void OnLevelClicked(int buttonNumber)
    {
        Debug.Log("clicked " + buttonNumber);
        //if ((isClicked == false) && (Singleton.Get<SceneTransition>().LoadLevel(buttonNumber) == true))
        {
            //isClicked = true;
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
}
