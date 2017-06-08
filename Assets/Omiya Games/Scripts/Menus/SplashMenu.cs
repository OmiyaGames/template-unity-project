using UnityEngine;
using System.Collections;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SplashMenu.cs" company="Omiya Games">
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu for the splash screen. You can retrieve this menu from the singleton
    /// script, <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    public class SplashMenu : IMenu
    {
        public class LoadingInfo
        {
            bool isFinished = false;
            float ratio = 0;

            public bool IsFinished
            {
                get
                {
                    return isFinished;
                }
                set
                {
                    isFinished = value;
                    if(isFinished == true)
                    {
                        Ratio = 1f;
                    }
                }
            }

            public float Ratio
            {
                get
                {
                    return ratio;
                }
                set
                {
                    ratio = Mathf.Clamp01(value);
                }
            }
        }

        [Header("Logo Only settings")]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("logoDisplayDuration")]
        float minimumLogoDisplayDuration = 3f;
        [SerializeField]
        GameObject logoOnlySet = null;

        [Header("Loading Bar Included settings")]
        [SerializeField]
        RectTransform loadingBar = null;
        [SerializeField]
        GameObject loadingBarIncludedSet = null;

        MalformedGameMenu.Reason buildState = MalformedGameMenu.Reason.None;

        public override Type MenuType
        {
            get
            {
                return Type.UnmanagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Override this property to display the loading bar in the splash menu
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return (LoadingStatus != null);
            }
        }

        /// <summary>
        /// Override this property to let the splash menu know
        /// whether it needs to display the loading bar (don't
        /// return null), and if so, how much the game loaded.
        /// </summary>
        public virtual LoadingInfo LoadingStatus
        {
            get
            {
                return null;
            }
        }

        // Use this for initialization
        void Start()
        {
            if(IsLoading == false)
            {
                // Start the fadeout
                logoOnlySet.SetActive(true);
                loadingBarIncludedSet.SetActive(false);
                StartCoroutine(DelayedFadeOut());
            }
            else
            {
                // Start the loading screen
                loadingBarIncludedSet.SetActive(true);
                logoOnlySet.SetActive(false);
                StartCoroutine(ShowLoadingScreen());
            }
        }

        protected override void OnStateChanged(State from, State to)
        {
            // Do nothing
        }

        IEnumerator DelayedFadeOut()
        {
            float startTime = Time.realtimeSinceStartup;
            buildState = MalformedGameMenu.Reason.None;

            // Show the Malformed game menu if there's any problems
            yield return StartCoroutine(VerifyBuild());

            // Check how much time has passed
            float logoDisplayDuration = minimumLogoDisplayDuration - (Time.realtimeSinceStartup - startTime);
            if(logoDisplayDuration > 0)
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
            if (Singleton.Instance.IsWebplayer == true)
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
    }
}
