using UnityEngine;
using System.Collections;
using System.Text;
using OmiyaGames.Web;
using OmiyaGames.Scenes;

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
        private const float MaxLoadingPercentage = 0.9f;

        [SerializeField]
        private float showLoadingBarAfterSeconds = 0.8f;
        [SerializeField]
        private float speedToCatchUpToTargetProgress = 10f;

        // TODO: take out this variable
        [SerializeField]
        [Range(0f, MaxLoadingPercentage)]
        private float testProgress = 0f;

        [Header("Required Components")]
        [SerializeField]
        private Translations.TranslatedTextMeshPro loadingLabel = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI progressPercentLabel = null;
        [SerializeField]
        private RectTransform progressBar = null;
        [SerializeField]
        private Translations.TranslatedString verifyingWebBuildText = null;

        [Header("Animation Info")]
        [SerializeField]
        private string fieldVisible = "Visible";

        private bool isProgressVisible = false, isLoadingNextScene = false;
        private float loadingProgress = 0f, startTime;
        private int currentlyDisplayedProgress = 0, lastDisplayedProgress = 0;
        private Vector2 progressBarDimensions = new Vector2(0, 1);
        private MalformedGameMenu.Reason buildState = MalformedGameMenu.Reason.None;
        private readonly StringBuilder builder = new StringBuilder();

        #region Properties
        public static SceneInfo NextScene
        {
            get;
            private set;
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
                    progressBarDimensions.x = loadingProgress / MaxLoadingPercentage;
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

        private bool IsProgressVisible
        {
            get => isProgressVisible;
            set
            {
                if (isProgressVisible != value)
                {
                    isProgressVisible = value;
                    Animator.SetBool(fieldVisible, isProgressVisible);
                }
            }
        }
        #endregion

        public static int GetDisplayedLoadingPercentage(float loadingProgress)
        {
            return Mathf.RoundToInt((loadingProgress * 100f) / MaxLoadingPercentage);
        }

        // Use this for initialization
        void Start()
        {
            startTime = Time.time;
            //if (IsLoading == false)
            //{
            //    // Start the fadeout
            //    logoOnlySet.SetActive(true);
            //    loadingBarIncludedSet.SetActive(false);
            //    StartCoroutine(DelayedFadeOut());
            //}
            //else
            //{
            //    // Start the loading screen
            //    loadingBarIncludedSet.SetActive(true);
            //    logoOnlySet.SetActive(false);
            //    StartCoroutine(ShowLoadingScreen());
            //}
        }

        private void Update()
        {
            // Check if the progress bar is visible
            if (IsProgressVisible == true)
            {
                // Animate the progress increasing
                // FIXME: remove "testProgress," and insert proper scene loading progress
                LoadingProgress = Mathf.Lerp(LoadingProgress, testProgress, (Time.deltaTime * speedToCatchUpToTargetProgress));
            }

            // Check if we're NOT loading the next scene
            if (isLoadingNextScene == false)
            {
                // Check if the progress is complete
                // FIXME: remove "testProgress," and insert proper scene loading progress
                if (Mathf.Approximately(testProgress, MaxLoadingPercentage) == true)
                {
                    // If next scene is null, load the main menu scene
                    if (NextScene == null)
                    {
                        NextScene = Singleton.Get<SceneTransitionManager>().MainMenu;
                    }

                    // Load the proper scene
                    Singleton.Get<SceneTransitionManager>().LoadScene(NextScene);

                    // Indicate we're loading the next scene
                    isLoadingNextScene = true;
                }
                else if ((IsProgressVisible == false) && ((Time.time - startTime) > showLoadingBarAfterSeconds))
                {
                    // If progress bar isn't visible, check how much time has passed
                    // If enough time has passed, check if this is a web-build, and we're loading the first scene (NextScene would be null, in that case)
                    if ((NextScene == null) && (Singleton.Instance.IsWebApp == true))
                    {
                        // Update the loading text to verifying, instead
                        loadingLabel.SetTranslationKey(verifyingWebBuildText);
                    }

                    // Make the progress bar visible
                    IsProgressVisible = true;
                }
            }
        }

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Do nothing
        }

        /*
        IEnumerator DelayedFadeOut()
        {
            float startTime = Time.realtimeSinceStartup;
            buildState = MalformedGameMenu.Reason.None;

            // Show the Malformed game menu if there's any problems
            yield return StartCoroutine(VerifyBuild());

            // Check how much time has passed
            float logoDisplayDuration = minimumLogoDisplayDuration - (Time.realtimeSinceStartup - startTime);
            if (logoDisplayDuration > 0)
            {
                // Wait for the designated time
                yield return new WaitForSeconds(logoDisplayDuration);
            }

            // Check the build state
            LoadNextMenu(buildState);
        }

        IEnumerator ShowLoadingScreen()
        {
            // Reset the loading bar
            Vector2 max = loadingBar.anchorMax;
            max.x = 0;
            loadingBar.anchorMax = max;
            buildState = MalformedGameMenu.Reason.None;

            // Show the Malformed game menu if there's any problems
            yield return StartCoroutine(VerifyBuild());

            // Wait until the loading status is finished
            while (LoadingStatus.IsFinished == false)
            {
                // Update the loading bar
                max.x = LoadingStatus.Ratio;
                loadingBar.anchorMax = max;

                // Wait for a frame
                yield return null;
            }

            // Wait for a frame
            yield return null;

            // Make the loading bar full
            max.x = 1;
            loadingBar.anchorMax = max;

            // Wait for a frame
            yield return null;

            // Get the scene manager to change scenes
            LoadNextMenu(buildState);
        }

        IEnumerator VerifyBuild()
        {
            if (Singleton.Instance.IsWebApp == true)
            {
                // Grab the web checker
                WebLocationChecker webChecker = Singleton.Get<WebLocationChecker>();
                if (webChecker != null)
                {
                    // Wait until the webchecker is done
                    while (webChecker.CurrentState == WebLocationChecker.State.InProgress)
                    {
                        yield return null;
                    }

                    // Check the state
                    switch (webChecker.CurrentState)
                    {
                        case WebLocationChecker.State.EncounteredError:
                            buildState = MalformedGameMenu.Reason.CannotConfirmDomain;
                            break;
                        case WebLocationChecker.State.DomainDidntMatch:
                            buildState = MalformedGameMenu.Reason.IsIncorrectDomain;
                            break;
                    }
                }
            }
            else if ((Application.genuineCheckAvailable == true) && (Application.genuine == false))
            {
                buildState = MalformedGameMenu.Reason.IsNotGenuine;
            }

            // Check if we're simulating failure
            if (Singleton.Instance.IsSimulatingMalformedGame == true)
            {
                // Indicate as such
                buildState = MalformedGameMenu.Reason.JustTesting;
            }
        }

        static void LoadNextMenu(MalformedGameMenu.Reason state)
        {
            // Check the build state
            if (state == MalformedGameMenu.Reason.None)
            {
                // Get the scene manager to change scenes
                Singleton.Get<SceneTransitionManager>().LoadMainMenu();
            }
            else
            {
                // Show the malformed menu
                MalformedGameMenu menu = Singleton.Get<MenuManager>().Show<MalformedGameMenu>();

                // Update the reasoning
                menu.UpdateReason(state);
            }
        }
        */
    }
}
