using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

namespace OmiyaGames
{
    using Menu;

    ///-----------------------------------------------------------------------
    /// <copyright file="SceneTransitionManager.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <author>Taro Omiya</author>
    /// <date>4/5/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that retains information about which scene to switch to.
    /// Where possible, it'll animate the <code>SceneTransitionMenu</code> before
    /// switching scenes.
    /// </summary>
    /// <seealso cref="SceneTransitionMenu"/>
    /// <seealso cref="Singleton"/>
    public class SceneTransitionManager : ISingletonScript
    {
        public event Action<IMenu> OnSceneTransitionInStart;
        public event Action<IMenu> OnSceneTransitionInEnd;
        public event Action<IMenu> OnSceneTransitionOutStart;
        public event Action<IMenu> OnSceneTransitionOutEnd;

        // TODO: Add a loading scene to transition asynchronously to, so that we can show a loading bar
        [Header("Scene Transition")]
        [SerializeField]
        bool loadLevelAsynchronously = true;
        [SerializeField]
        SoundEffect soundEffect = null;

        [Header("Scene Information")]
        [SerializeField]
        SceneInfo splash;
        [SerializeField]
        SceneInfo mainMenu;
        [SerializeField]
        SceneInfo credits;
        [SerializeField]
        SceneInfo[] levels;

        [Header("Debugging")]
        [SerializeField]
        CursorLockMode defaultLockMode = CursorLockMode.Locked;

        SceneInfo lastScene = null;
        SceneInfo sceneToLoad = null;
        readonly Dictionary<string, SceneInfo> sceneNameToInfo = new Dictionary<string, SceneInfo>();

        #region Properties
        public static CursorLockMode CursorMode
        {
            get
            {
                return Cursor.lockState;
            }
            set
            {
                Cursor.lockState = value;
                Cursor.visible = (value != CursorLockMode.Locked);
            }
        }

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
                if (sceneNameToInfo.TryGetValue(SceneManager.GetActiveScene().path, out returnScene) == false)
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
            sceneNameToInfo.Add(MainMenu.ScenePath, MainMenu);
            sceneNameToInfo.Add(Credits.ScenePath, Credits);
            sceneNameToInfo.Add(Splash.ScenePath, Splash);

            // Update level information
            for (int index = 0; index < Levels.Length; ++index)
            {
                // Update the ordinal of the level
                Levels[index].Ordinal = index;

                // Add the level to the dictionary
                sceneNameToInfo.Add(Levels[index].ScenePath, Levels[index]);
            }
        }

        public override void SceneAwake(Singleton instance)
        {
            if(CurrentScene == null)
            {
                Debug.LogWarning("Current scene is not added to the Build Settings");
                return;
            }

            // Update the cursor locking
            RevertCursorLockMode(true);

            // Revert the time scale
            if (CurrentScene.RevertTimeScale == true)
            {
                Singleton.Get<TimeManager>().RevertToOriginalTime();
            }
        }

        public void RevertCursorLockMode(bool allowWebplayerSettings)
        {
            CursorLockMode mode = defaultLockMode;
            if (CurrentScene != null)
            {
                mode = CurrentScene.LockMode;
                if ((allowWebplayerSettings == true) && (Singleton.Instance.IsWebplayer == true))
                {
                    mode = CurrentScene.LockModeWeb;
                }
            }
            CursorMode = mode;
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
                throw new ArgumentNullException("scene");
            }
            else if(string.IsNullOrEmpty(scene.ScenePath) == true)
            {
                throw new ArgumentException("No scene name is set", "scene");
            }

            // Check if we need to update the last scene
            if (CurrentScene != scene)
            {
                lastScene = CurrentScene;
            }

            // Update which scene to load
            sceneToLoad = scene;

            // Make sure we have a level to load
            if (sceneToLoad != null)
            {
                // Show the level transition menu
                SceneTransitionMenu transitionMenu = Singleton.Get<MenuManager>().Show<SceneTransitionMenu>(TransitionOut);

                // Check if there's a transition menu
                if(transitionMenu == null)
                {
                    // Just load the scene without the menu
                    TransitionOut(null);
                }
            }
        }

        internal void TransitionIn(IMenu menu)
        {
            // Check to see if the argument for the next menu is provided
            SceneTransitionMenu transitionMenu = menu as SceneTransitionMenu;
            if(transitionMenu == null)
            {
                // If not, we're not transitioning, so run both transition-out events at the same time
                if(OnSceneTransitionInStart != null)
                {
                    OnSceneTransitionInStart(menu);
                }
                if(OnSceneTransitionInEnd != null)
                {
                    OnSceneTransitionInEnd(menu);
                }
            }
            else
            {
                // If so, check to see the current menu state
                if((transitionMenu.CurrentTransition == SceneTransitionMenu.Transition.SceneTransitionInStart) && (OnSceneTransitionInStart != null))
                {
                    // If just transitioning in, run the transition-out start event
                    OnSceneTransitionInStart(menu);
                }
                else if((transitionMenu.CurrentTransition == SceneTransitionMenu.Transition.SceneTransitionInEnd) && (OnSceneTransitionInEnd != null))
                {
                    // If transitioning ended, run the transition-out end event
                    OnSceneTransitionInEnd(menu);
                }
            }
        }

        void TransitionOut(IMenu menu)
        {
            // Check to see if the next scene name is provided
            if (sceneToLoad != null)
            {
                // Check to see if the argument for the next menu is provided
                SceneTransitionMenu transitionMenu = menu as SceneTransitionMenu;
                if(transitionMenu == null)
                {
                    // If not, we're not transitioning, so run both transition-out events at the same time
                    if(OnSceneTransitionOutStart != null)
                    {
                        OnSceneTransitionOutStart(menu);
                    }
                    if(OnSceneTransitionOutEnd != null)
                    {
                        OnSceneTransitionOutEnd(menu);
                    }

                    // Transition to the next scene
                    TransitionToScene(loadLevelAsynchronously, sceneToLoad);
                }
                else
                {
                    // If so, check to see the current menu state
                    if(transitionMenu.CurrentTransition == SceneTransitionMenu.Transition.SceneTransitionOutStart)
                    {
                        // If just transitioning in, run the transition-out start event
                        if(OnSceneTransitionOutStart != null)
                        {
                            OnSceneTransitionOutStart(menu);
                        }

                        // Play the sound effect
                        if(soundEffect != null)
                        {
                            soundEffect.Play();
                        }
                    }
                    else if(transitionMenu.CurrentTransition == SceneTransitionMenu.Transition.SceneTransitionOutEnd)
                    {
                        // If transitioning ended, run the transition-out end event
                        if(OnSceneTransitionOutEnd != null)
                        {
                            OnSceneTransitionOutEnd(menu);
                        }

                        // Transition to the next scene
                        TransitionToScene(loadLevelAsynchronously, sceneToLoad);
                    }
                }
            }
        }

        static void TransitionToScene(bool loadLevelAsynchronously, SceneInfo sceneToLoad)
        {
            // Indicate the next scene was loaded
            Singleton.Get<PoolingManager>().DestroyAll();

            // Check the async flag
            if (loadLevelAsynchronously == true)
            {
                // Load asynchronously
                SceneManager.LoadSceneAsync(sceneToLoad.SceneFileName);
            }
            else
            {
                // Load synchronously
                SceneManager.LoadScene(sceneToLoad.SceneFileName);
            }

            // Indicate this level is already in progress of loading
            sceneToLoad = null;
        }

        #region Editor Methods
#if UNITY_EDITOR
        public void SetupLevels(string displayNameTemplate, bool fillInSceneName, bool defaultRevertTimeScale, CursorLockMode defaultCursorLockMode, bool appendLevels)
        {
            // To prevent conflicts, collect all the scenes we already have listed
            HashSet<string> usedPaths = new HashSet<string>();
            usedPaths.Add(splash.ScenePath);
            usedPaths.Add(mainMenu.ScenePath);
            usedPaths.Add(credits.ScenePath);

            // Create a new list
            int index = 0;
            List<SceneInfo> newLevels = new List<SceneInfo>();

            // If we're appending, add all the scene info from before
            if (appendLevels == true)
            {
                for (index = 0; index < levels.Length; ++index)
                {
                    newLevels.Add(levels[index]);
                    usedPaths.Add(levels[index].ScenePath);
                }
            }

            // Go through each level
            int numScenes = UnityEditor.EditorBuildSettings.scenes.Length;
            for (index = 0; index < numScenes; ++index)
            {
                // Grab the scene
                UnityEditor.EditorBuildSettingsScene scene = UnityEditor.EditorBuildSettings.scenes[index];

                // Make sure the scene doesn't have the same name as the splash, main menu or the credits
                if ((scene.enabled == true) && (usedPaths.Contains(scene.path) == false))
                {
                    // Get the display name
                    int ordinal = newLevels.Count;
                    string displayName = displayNameTemplate;
                    if (fillInSceneName == true)
                    {
                        displayName = string.Format(displayNameTemplate, System.IO.Path.GetFileNameWithoutExtension(scene.path));
                    }
                    else
                    {
                        displayName = string.Format(displayNameTemplate, (ordinal + 1));
                    }

                    // Create a new level
                    newLevels.Add(new SceneInfo(scene.path, displayName, defaultRevertTimeScale, defaultCursorLockMode, ordinal));

                    // Add this path to avoid adding this scene twice
                    usedPaths.Contains(scene.path);
                }
            }

            // Setup the levels array
            levels = newLevels.ToArray();
        }

        static string GetDisplayName(int index, string formatText)
        {
            return string.Format(formatText, index);
        }
#endif
        #endregion
    }
}
