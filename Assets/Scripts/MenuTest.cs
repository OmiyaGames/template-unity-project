using UnityEngine;
using System.Collections;
using OmiyaGames;

public class MenuTest : MonoBehaviour
{
    public void OnPauseClicked()
    {
        Singleton.Get<MenuManager>().GetMenu<PauseMenu>().Show();
    }
    public void OnFailedClicked()
    {
        Singleton.Get<MenuManager>().GetMenu<LevelFailedMenu>().Show();
    }
    public void OnCompleteClicked()
    {
        Singleton.Get<MenuManager>().GetMenu<LevelCompleteMenu>().Show();
    }
}
