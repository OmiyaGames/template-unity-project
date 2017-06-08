using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    public class HowToPlayMenu : IMenu
    {
        [SerializeField]
        Button backButton;

        public override GameObject DefaultUi
        {
            get
            {
                return backButton.gameObject;
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public void OnBackClicked()
        {
            Hide();
            Manager.ButtonClick.Play();
        }
    }
}
