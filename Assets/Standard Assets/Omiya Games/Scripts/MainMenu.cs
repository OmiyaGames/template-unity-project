using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    bool isClicked = false;
    
    public void OnLevelClicked(int buttonNumber)
    {
        if ((isClicked == false) && (Singleton.Get<SceneTransition>().LoadLevel(buttonNumber) == true))
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
}
