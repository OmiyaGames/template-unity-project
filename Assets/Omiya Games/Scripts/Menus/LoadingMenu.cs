using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using OmiyaGames.Scenes;
using OmiyaGames.Translations;
using OmiyaGames.Global;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LoadingMenu.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu for the splash screen. You can retrieve this menu from the singleton
    /// script, <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    // FIXME: convert to Loading Screen.  Consider taking in the MalformedGameMenu as a way to verify whether game has loaded or not.
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class LoadingMenu : IMenu
    {
        private enum AnimatorState
        {
            Start = 0,
            ShowVerifying = 1,
            ShowLoading = 2,
            NextSceneFromStart = 3,
            NextSceneFromVerifying = 4,
            NextSceneFromLoading = 5
        }

        [Header("Progress Bar Timing")]
        [SerializeField]
        private float showLoadingBarAfterSeconds = 0.8f;
        [SerializeField]
        private float speedToCatchUpToTargetProgress = 10f;

        [Header("Required Components")]
        [SerializeField]
        private TranslatedTextMeshPro loadingLabel = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI progressPercentLabel = null;
        [SerializeField]
        private RectTransform progressBar = null;

        private float loadingProgress = 0f, startTime;
        private int currentlyDisplayedProgress = 0, lastDisplayedProgress = 0;
        private AnimatorState currentAnimationState = AnimatorState.Start;
        private Vector2 progressBarDimensions = new Vector2(0, 1);
        private AsyncOperation sceneLoadingInfo = null;
        private MalformedGameMenu checkBuildStatusMenu = null;
        private readonly StringBuilder builder = new StringBuilder();

        #region Properties
        public static SceneInfo NextScene
        {
            get;
            set;
        }

        public override Type MenuType
        {
            get => Type.UnmanagedMenu;
        }

        public override UnityEngine.UI.Selectable DefaultUi
        {
            get => null;
        }

        public float LoadingProgress
        {
            get => loadingProgress;
            set
            {
                // Update loading progress
                loadingProgress = Mathf.Clamp01(value);

                // Check to see if progress bar exists
                if (progressBar != null)
                {
                    // Update progress bar
                    progressBarDimensions.x = loadingProgress / SceneTransitionManager.SceneLoadingProgressComplete;
                    progressBar.anchorMax = progressBarDimensions;
                }

                // Check what's displayed, and see if it's different from the last displayed value
                currentlyDisplayedProgress = GetDisplayedLoadingPercentage(loadingProgress);
                if (currentlyDisplayedProgress != lastDisplayedProgress)
                {
                    // Check to see if label exists
                    if (progressPercentLabel != null)
                    {
                        // Setup the string to percentage
                        builder.Clear();
                        builder.Append(currentlyDisplayedProgress);
                        builder.Append('%');

                        // Display the percentage
                        progressPercentLabel.SetText(builder.ToString());
                    }

                    // Update the last displayed progress
                    lastDisplayedProgress = currentlyDisplayedProgress;
                }
            }
        }

        private AnimatorState CurrentState
        {
            get => currentAnimationState;
            set
            {
                if (currentAnimationState != value)
                {
                    currentAnimationState = value;
                    Animator.SetInteger(StateField, ((int)currentAnimationState));
                }
            }
        }

        private bool IsNextSceneReady
        {
            get => Mathf.Approximately(sceneLoadingInfo.progress, SceneTransitionManager.SceneLoadingProgressComplete);
        }

        private bool IsVerificationFinished
        {
            get
            {
                // By default, return true
                bool returnFlag = true;

                // Check if we're running the verification process at all
                if (checkBuildStatusMenu != null)
                {
                    // Switch to returning false
                    returnFlag = false;

                    // Check the build state
                    if (checkBuildStatusMenu.BuildState == MalformedGameMenu.Reason.None)
                    {
                        // If this build is genuine, return true
                        returnFlag = true;
                    }
                    else if (checkBuildStatusMenu.BuildState != MalformedGameMenu.Reason.InProgress)
                    {
                        // If verification is finished, and build is not found to be genuine,
                        // wait until the user prompts that they would like to continue playing
                        returnFlag = checkBuildStatusMenu.IsUserAcceptingRisk;
                    }
                }
                return returnFlag;
            }
        }
        #endregion

        public static int GetDisplayedLoadingPercentage(float loadingProgress)
        {
            return Mathf.RoundToInt((loadingProgress * 100f) / SceneTransitionManager.SceneLoadingProgressComplete);
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Do nothing
        }

        #region Unity Events
        // Use this for initialization
        private void Start()
        {
            // Setup initial member variables
            startTime = Time.time;

            // If next scene is null, this is the first scene to load
            if (NextScene == null)
            {
                // Indicate the next scene to load is the main menu
                NextScene = Singleton.Get<SceneTransitionManager>().MainMenu;

                // Indicate we're working on verifying the build
                checkBuildStatusMenu = Singleton.Get<MenuManager>().GetMenu<MalformedGameMenu>();
                CurrentState = AnimatorState.ShowVerifying;
            }

            // Start asynchronously loading the next scene
            sceneLoadingInfo = SceneManager.LoadSceneAsync(NextScene.ScenePath);

            // Prevent the scene from loading automatically
            sceneLoadingInfo.allowSceneActivation = false;
        }

        private void Update()
        {
            UpdateCurrentState();
            AnimateProgressBar();
        }
        #endregion

        #region Animation Events
        private void OnVerificationDismissed()
        {
            OnLoadingDismissed();
        }

        private void OnLoadingDismissed()
        {
            switch (CurrentState)
            {
                case AnimatorState.NextSceneFromStart:
                case AnimatorState.NextSceneFromVerifying:
                case AnimatorState.NextSceneFromLoading:
                    LoadNextScene();
                    break;
            }
        }
        #endregion

        #region Helper Methods
        private void UpdateCurrentState()
        {
            // Check if we're NOT loading the next scene
            switch (CurrentState)
            {
                case AnimatorState.Start:
                    // Check if the progress is complete
                    if (IsNextSceneReady == true)
                    {
                        // Indicate we're loading the next scene
                        CurrentState = AnimatorState.NextSceneFromStart;
                        LoadNextScene();
                    }
                    else if ((Time.time - startTime) > showLoadingBarAfterSeconds)
                    {
                        // If progress bar isn't visible, check how much time has passed
                        // Make the progress bar visible
                        CurrentState = AnimatorState.ShowLoading;
                    }
                    break;
                case AnimatorState.ShowVerifying:
                    // Check if verification has progressed
                    if (IsVerificationFinished == true)
                    {
                        // If no problems were found, check if we want to load immediately, or switch to the loading screen
                        if (IsNextSceneReady == true)
                        {
                            // Indicate we're loading the next scene
                            CurrentState = AnimatorState.NextSceneFromVerifying;
                        }
                        else
                        {
                            // Indicate we're showing the progress bar
                            CurrentState = AnimatorState.ShowLoading;
                        }
                    }
                    break;
                case AnimatorState.ShowLoading:
                    // Check if the progress is complete
                    if (IsNextSceneReady == true)
                    {
                        // Indicate we're loading the next scene
                        CurrentState = AnimatorState.NextSceneFromLoading;
                    }
                    break;
            }
        }

        private void AnimateProgressBar()
        {
            // Check if the progress bar is visible
            switch (CurrentState)
            {
                case AnimatorState.ShowLoading:
                case AnimatorState.NextSceneFromLoading:
                    // Animate the progress increasing
                    LoadingProgress = Mathf.Lerp(LoadingProgress, sceneLoadingInfo.progress, (Time.deltaTime * speedToCatchUpToTargetProgress));
                    break;
            }
        }

        private void LoadNextScene()
        {
            // Allow the next scene to activate
            sceneLoadingInfo.allowSceneActivation = true;
        }
        #endregion
    }
}
