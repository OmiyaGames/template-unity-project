using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames
{
    public class SceneManager : ISingletonScript
    {
        [Header("Scene Transition")]
        [SerializeField]
        bool loadLevelAsynchronously = true;

        [Header("Scene Information")]
        [SerializeField]
        SceneInfo splash;
        [SerializeField]
        SceneInfo mainMenu;
        [SerializeField]
        SceneInfo credits;
        [SerializeField]
        SceneInfo[] levels;

#if UNITY_EDITOR
        [Header("Default Levels Information")]
        [SerializeField]
        string levelDisplayNameTemplate = "Level {0}";
        [SerializeField]
        bool levelRevertsTimeScaleTemplate = true;
        [SerializeField]
        CursorLockMode levelLockModeTemplate = CursorLockMode.None;
#endif

        SceneInfo lastScene = null;
        readonly Dictionary<string, SceneInfo> sceneNameToInfo = new Dictionary<string, SceneInfo>();
        string sceneToLoad = null;

        #region Properties
        public SceneInfo Splash
        {
            get
            {
                return splash;
            }
        }

        public SceneInfo MainMenu
        {
            get
            {
                return mainMenu;
            }
        }

        public SceneInfo Credits
        {
            get
            {
                return credits;
            }
        }

        public SceneInfo[] Levels
        {
            get
            {
                return levels;
            }
        }

        public int NumLevels
        {
            get
            {
                return Levels.Length;
            }
        }

        public SceneInfo CurrentScene
        {
            get
            {
                SceneInfo returnScene = null;
                if (sceneNameToInfo.TryGetValue(Application.loadedLevelName, out returnScene) == false)
                {
                    returnScene = null;
                }
                return returnScene;
            }
        }

        public SceneInfo LastScene
        {
            get
            {
                return lastScene;
            }
            private set
            {
                lastScene = value;
            }
        }

        public SceneInfo NextScene
        {
            get
            {
                // Get the current scene
                SceneInfo returnLevel = null, currenLevel = CurrentScene;

                // Check which scene this is
                if (currenLevel != null)
                {
                    if ((currenLevel == splash) || (currenLevel == credits))
                    {
                        // If we're on the splash or credits scene, the next level is the main menu
                        returnLevel = mainMenu;
                    }
                    else if(levels.Length > 0)
                    {
                        // By default, go straight to the credits
                        returnLevel = credits;

                        // If there's more than one level, check if we're on the main menu or a level scene
                        if (currenLevel == mainMenu)
                        {
                            // If we're the main scene, just return the first level
                            returnLevel = levels[0];
                        }
                        else if ((currenLevel.Ordinal >= 0) && (currenLevel.Ordinal < (NumLevels - 1)))
                        {
                            // If this is NOT the last level, return the next level
                            returnLevel = levels[currenLevel.Ordinal + 1];
                        }
                    }
                }
                return returnLevel;
            }
        }
        #endregion

        public override void SingletonAwake(Singleton instance)
        {
            // Reset scene name to info dictionary
            sceneNameToInfo.Clear();

            // Add the main menu, credits, and splash scene
            sceneNameToInfo.Add(MainMenu.SceneName, MainMenu);
            sceneNameToInfo.Add(Credits.SceneName, Credits);
            sceneNameToInfo.Add(Splash.SceneName, Splash);

            // Update level information
            for (int index = 0; index < Levels.Length; ++index)
            {
                // Update the ordinal of the level
                Levels[index].Ordinal = index;

                // Add the level to the dictionary
                sceneNameToInfo.Add(Levels[index].SceneName, Levels[index]);
            }
        }

        public override void SceneAwake(Singleton instance)
        {
            // Update the cursor locking
            Cursor.lockState = CurrentScene.LockMode;

            // Revert the time scale
            if(CurrentScene.RevertTimeScale == true)
            {
                Singleton.Get<TimeManager>().RevertToOriginalTime();
            }
        }

        /// <summary>
        /// Reloads the current scene, effectively resetting it.
        /// </summary>
        public void ReloadCurrentScene()
        {
            LoadScene(CurrentScene);
        }

        /// <summary>
        /// Loads the Main Menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            LoadScene(MainMenu);
        }
        
        /// <summary>
        /// Loads the Credits scene.
        /// </summary>
        public void LoadCredits()
        {
            LoadScene(Credits);
        }

        /// <summary>
        /// Loads the next scene.
        /// </summary>
        public void LoadNextLevel()
        {
            LoadScene(NextScene);
        }

        public void LoadScene(SceneInfo scene)
        {
            // Make sure the parameter is correct
            if(scene == null)
            {
                throw new System.ArgumentNullException("scene");
            }
            else if(string.IsNullOrEmpty(scene.SceneName) == true)
            {
                throw new System.ArgumentException("No scene name is set", "scene");
            }

            // Check if we need to update the last scene
            if (CurrentScene != scene)
            {
                lastScene = CurrentScene;
            }

            // Update which scene to load
            sceneToLoad = scene.SceneName;

            // FIXME: Grab the level transition menu here
            //SceneTransitionMenu transitionMenu = Singleton.Get<MenuManager>().GetMenu<SceneTransitionMenu>();
            SceneTransitionMenu transitionMenu = null;

            // Check if there's a transition menu
            if(transitionMenu != null)
            {
                // FIXME: run the transition menu's display function
                //transitionMenu.Show(TransitionMenuFullyOpaque);
            }
            else
            {
                // Just load the scene without the menu
                TransitionMenuFullyOpaque(null);
            }
        }

        void TransitionMenuFullyOpaque(SceneTransitionMenu menu)
        {
            // Make sure we have a level to load
            if (string.IsNullOrEmpty(sceneToLoad) == false)
            {
                // Check the async flag
                if (loadLevelAsynchronously == true)
                {
                    // Load asynchronously
                    Application.LoadLevelAsync(sceneToLoad);
                }
                else
                {
                    // Load synchronously
                    Application.LoadLevel(sceneToLoad);
                }

                // Indicate this level is already in progress of loading
                sceneToLoad = null;
            }
        }

        #region Editor Methods
#if UNITY_EDITOR
        [ContextMenu("Setup Levels (using Level Numbers)")]
        public void SetupLevelsUsingLevelNumber()
        {
            SetupLevels(levelDisplayNameTemplate);
        }

        [ContextMenu("Setup Levels (using Scene Names)")]
        public void SetupLevelsUsingSceneName()
        {
            SetupLevels();
        }

        [ContextMenu("Setup Levels")]
        void SetupLevels(string formatText = null)
        {
            // Create a new list
            int numScenes = UnityEditor.EditorBuildSettings.scenes.Length;
            List<SceneInfo> newLevels = new List<SceneInfo>();

            // Go through each level
            for (int index = 0; index < numScenes; ++index)
            {
                // Grab the scene
                UnityEditor.EditorBuildSettingsScene scene = UnityEditor.EditorBuildSettings.scenes[index];

                // Get the scene name
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

                // Make sure the scene doesn't have the same name as the splash, main menu or the credits
                if ((string.Equals(sceneName, splash.SceneName) == false) &&
                    (string.Equals(sceneName, mainMenu.SceneName) == false) &&
                    (string.Equals(sceneName, credits.SceneName) == false))
                {
                    // Get the display name
                    string displayName = GetDisplayName((newLevels.Count + 1), sceneName, formatText);

                    // Create a new level
                    int ordinal = newLevels.Count;
                    newLevels.Add(new SceneInfo(sceneName, displayName, levelRevertsTimeScaleTemplate, levelLockModeTemplate, ordinal));
                }
            }

            // Setup the levels array
            levels = newLevels.ToArray();
        }

        static string GetDisplayName(int index, string sceneName, string formatText)
        {
            string displayName;
            if (string.IsNullOrEmpty(formatText) == true)
            {
                displayName = sceneName;
            }
            else
            {
                displayName = string.Format(formatText, index);
            }
            return displayName;
        }
#endif
        #endregion
    }
}
