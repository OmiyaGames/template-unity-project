using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames
{
    public class SceneManager : ISingletonScript
    {
        [SerializeField]
        SceneInfo splash;
        [SerializeField]
        SceneInfo mainMenu;
        [SerializeField]
        SceneInfo credits;
        [SerializeField]
        SceneInfo[] levels;

        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating to return to a scene")]
        [SerializeField]
        string returnToTextTemplate = "Return to {0}";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating to restart a scene")]
        [SerializeField]
        string restartTextTemplate = "Restart {0}";

#if UNITY_EDITOR
        [SerializeField]
        string levelDisplayNameTemplate = "Level {0}";
#endif

        readonly Dictionary<string, SceneInfo> sceneNameToInfo = new Dictionary<string, SceneInfo>();
        string menuTextCache = null;

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

        public string ReturnToMenuText
        {
            get
            {
                if(menuTextCache == null)
                {
                    menuTextCache = mainMenu.DisplayName;
                    if (string.IsNullOrEmpty(returnToTextTemplate) == false)
                    {
                        menuTextCache = string.Format(returnToTextTemplate, mainMenu.DisplayName);
                    }
                }
                return menuTextCache;
            }
        }

        public string RestartCurrentSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo currentScene = CurrentScene;
                if(currentScene != null)
                {
                    returnText = currentScene.DisplayName;
                    if (string.IsNullOrEmpty(restartTextTemplate) == false)
                    {
                        returnText = string.Format(restartTextTemplate, currentScene.DisplayName);
                    }
                }
                return returnText;
            }
        }

        public override void SingletonAwake(Singleton instance)
        {
            // Update level information
            for (int index = 0; index < Levels.Length; ++index)
            {
                Levels[index].Ordinal = index;
            }
        }

        public override void SceneAwake(Singleton instance)
        {
        }

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
                    string displayName = GetDisplayName(index, sceneName, formatText);

                    // Create a new level
                    int ordinal = newLevels.Count;
                    newLevels.Add(new SceneInfo(sceneName, displayName, ordinal));
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
    }
}
