using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    public class ConfirmationMenu : IMenu
    {
        [SerializeField]
        Button yesButton;
        [SerializeField]
        Button noButton;

        bool isYesSelected = false, defaultToYes = false;

        #region Properties
        public bool IsYesSelected
        {
            get
            {
                return isYesSelected;
            }
        }

        public bool DefaultToYes
        {
            set
            {
                defaultToYes = value;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                if (defaultToYes == true)
                {
                    return yesButton.gameObject;
                }
                else
                {
                    return noButton.gameObject;
                }
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }
        #endregion

        public void OnYesClicked()
        {
            // Indicate Yes was selected
            isYesSelected = true;

            // Hide the dialog
            Hide();

            // Indicate button is clicked
            Manager.ButtonClick.Play();
        }

        public void OnNoClicked()
        {
            // Indicate No was selected
            isYesSelected = false;

            // Hide the dialog
            Hide();

            // Indicate button is clicked
            Manager.ButtonClick.Play();
        }
    }
}
