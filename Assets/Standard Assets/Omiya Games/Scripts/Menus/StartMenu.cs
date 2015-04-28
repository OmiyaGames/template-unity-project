using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    [RequireComponent(typeof(Animator))]
    public class StartMenu : IMenu
    {
        [SerializeField]
        Button levelSelectButton;
        [SerializeField]
        Button optionsButton;
        [SerializeField]
        Button creditsButton;
        [SerializeField]
        Button quitButton;

        GameObject defaultButton = null;
        bool isButtonLocked = false;

        public override Type MenuType
        {
            get
            {
                return Type.DefaultManagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return defaultButton;
            }
        }

        void Start()
        {
            // Check if we should remove the quit button (you can't quit out of a webplayer)
            GameSettings gameSettings = Singleton.Get<GameSettings>();
            if (gameSettings.IsWebplayer == true)
            {
                // Disable the quit button entirely
                quitButton.gameObject.SetActive(false);
            }

            // Select the level select button by default
            defaultButton = levelSelectButton.gameObject;
            Singleton.Get<UnityEngine.EventSystems.EventSystem>().firstSelectedGameObject = defaultButton;
        }

        #region Button Events
        public void OnLevelSelectClicked()
        {
            if (isButtonLocked == false)
            {
                // Open the Level Select menu
                //Singleton.Get<MenuManager>().GetMenu<OptionsMenu>().CurrentState = IMenu.State.Visible;

                // Indicate we've clicked on a button
                defaultButton = levelSelectButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnOptionsClicked()
        {
            if (isButtonLocked == false)
            {
                // Open the options menu
                Singleton.Get<MenuManager>().GetMenu<OptionsMenu>().Show();

                // Indicate we've clicked on a button
                defaultButton = optionsButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnCreditsClicked()
        {
            if (isButtonLocked == false)
            {
                // Transition to the credits
                Singleton.Get<SceneTransition>().LoadLevel(Singleton.Get<SceneManager>().Credits);

                // Change the menu to stand by
                CurrentState = State.StandBy;

                // Indicate we've clicked on a button
                defaultButton = creditsButton.gameObject;
                isButtonLocked = true;
            }
        }

        public void OnQuitClicked()
        {
            if (isButtonLocked == false)
            {
                // Quit the application
                Application.Quit();

                // Change the menu to stand by
                CurrentState = State.StandBy;

                // Indicate we've clicked on a button
                defaultButton = quitButton.gameObject;
                isButtonLocked = true;
            }
        }
        #endregion

        protected override void OnStateChanged(IMenu.State from, IMenu.State to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            // If this menu is visible again, release the button lock
            if(to == State.Visible)
            {
                isButtonLocked = false;
            }
        }
    }
}
