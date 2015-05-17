using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    public class LevelFailedMenu : ISceneChangingMenu
    {
        [Header("Behavior")]
        [SerializeField]
        bool pauseGameOnShow = false;

        public override bool PauseOnShow
        {
            get
            {
                return pauseGameOnShow;
            }
        }

        public void OnOptionsClicked()
        {
            // Open the options dialog
            Singleton.Get<MenuManager>().Show<OptionsMenu>();
        }
    }
}
