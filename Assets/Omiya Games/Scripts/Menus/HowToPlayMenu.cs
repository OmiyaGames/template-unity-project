using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    public class HowToPlayMenu : IMenu
    {
        [Header("How to Play Settings")]
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

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return BackgroundMenu.BackgroundType.SolidColor;
            }
        }

        public override string TitleTranslationKey
        {
            get
            {
                return null;
            }
        }
    }
}
