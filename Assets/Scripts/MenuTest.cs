using UnityEngine;
using System.Collections;
using OmiyaGames;

public class MenuTest : MonoBehaviour
{
    public void OnPauseClicked()
    {
        Singleton.Get<MenuManager>().Show<PauseMenu>();
    }
    public void OnFailedClicked()
    {
        Singleton.Get<MenuManager>().Show<LevelFailedMenu>();
    }
    public void OnCompleteClicked()
    {
        Singleton.Get<MenuManager>().Show<LevelCompleteMenu>();
    }
}
