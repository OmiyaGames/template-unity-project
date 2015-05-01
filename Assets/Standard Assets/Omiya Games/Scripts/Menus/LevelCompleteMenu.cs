using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
    public class LevelCompleteMenu : ISceneChangingMenu
    {
        [Header("Behavior")]
        [SerializeField]
        bool pauseGameOnShow = false;
        [SerializeField]
        bool unlockNextLevel = true;

        public override bool PauseOnShow
        {
            get
            {
                return pauseGameOnShow;
            }
        }

        protected override void Start()
        {
            base.Start();

            // Check if we need to disable the next level button
            if ((defaultButton != null) && (Singleton.Get<SceneManager>().NextScene == null))
            {
                defaultButton.interactable = false;
            }
        }

        public override void Show(System.Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Check if we need to unlock the next level
            if (unlockNextLevel == true)
            {
                SceneManager manager = Singleton.Get<SceneManager>();
                GameSettings settings = Singleton.Get<GameSettings>();
                if (Singleton.Get<SceneManager>().NextScene != null)
                {
                    // Unlock the next level
                    settings.NumLevelsUnlocked = manager.CurrentScene.Ordinal + 1;
                }
                else
                {
                    // Unlock this level (last one)
                    settings.NumLevelsUnlocked = manager.CurrentScene.Ordinal;
                }
            }
        }

        public void OnNextLevelClicked()
        {
            Hide();

            // Transition to the current level
            Singleton.Get<SceneManager>().LoadNextLevel();
        }
    }
}
