using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    public class PauseMenu : ISceneChangingMenu
    {
        public override bool PauseOnShow
        {
            get
            {
                return true;
            }
        }

        void OnApplicationPause(bool isPaused)
        {
            if ((isPaused == true) && (Singleton.Get<TimeManager>().IsManuallyPaused == false))
            {
                Show();
            }
        }

        public void OnOptionsClicked()
        {
            // Open the options dialog
            Singleton.Get<MenuManager>().GetMenu<OptionsMenu>().Show();
        }

        public void OnResumeClicked()
        {
            Hide();
        }
    }
}
