using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using OmiyaGames.Scenes;
using OmiyaGames.Translations;
using OmiyaGames.Global;
using OmiyaGames.Audio;

namespace OmiyaGames.Menus
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="LoadingMenu.cs" company="Omiya Games">
	/// The MIT License (MIT)
	/// 
	/// Copyright (c) 2015-2022 Omiya Games
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
	/// <list type="table">
	/// <listheader>
	/// <term>Revision</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>
	/// <strong>Date:</strong> 5/18/2015<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial verison.</description>
	/// </item><item>
	/// <term>
	/// <strong>Date:</strong> 6/29/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Updated to support <seealso cref="AudioManager"/>
	/// and <seealso cref="TimeManager"/>.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Menu for the splash screen. You can retrieve this menu from the singleton
	/// script, <seealso cref="MenuManager"/>.
	/// </summary>
	// FIXME: convert to Loading Screen.  Consider taking in the MalformedGameMenu as a way to verify whether game has loaded or not.
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Animator))]
	public class LoadingMenu : IMenu
	{
		enum AnimatorState
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
		float showLoadingBarAfterSeconds = 0.8f;
		[SerializeField]
		float speedToCatchUpToTargetProgress = 10f;

		[Header("Required Components")]
		[SerializeField]
		TranslatedTextMeshPro loadingLabel = null;
		[SerializeField]
		TMPro.TextMeshProUGUI progressPercentLabel = null;
		[SerializeField]
		RectTransform progressBar = null;

		float loadingProgress = 0f, startTime;
		int currentlyDisplayedProgress = 0, lastDisplayedProgress = 0;
		AnimatorState currentAnimationState = AnimatorState.Start;
		Vector2 progressBarDimensions = new Vector2(0, 1);
		AsyncOperation sceneLoadingInfo = null;
		MalformedGameMenu checkBuildStatusMenu = null;
		readonly StringBuilder builder = new StringBuilder();

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

		AnimatorState CurrentState
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

		bool IsNextSceneReady
		{
			get => Mathf.Approximately(sceneLoadingInfo.progress, SceneTransitionManager.SceneLoadingProgressComplete);
		}

		bool IsVerificationFinished
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
		IEnumerator Start()
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

				// Setup the audio manager
				yield return AudioManager.Setup();
			}

			// Start asynchronously loading the next scene
			sceneLoadingInfo = SceneManager.LoadSceneAsync(NextScene.ScenePath);

			// Prevent the scene from loading automatically
			sceneLoadingInfo.allowSceneActivation = false;
		}

		void Update()
		{
			if (sceneLoadingInfo != null)
			{
				UpdateCurrentState();
				AnimateProgressBar();
			}
		}
		#endregion

		#region Animation Events
		void OnVerificationDismissed()
		{
			OnLoadingDismissed();
		}

		void OnLoadingDismissed()
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
		void UpdateCurrentState()
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

		void AnimateProgressBar()
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

		void LoadNextScene()
		{
			// Allow the next scene to activate
			sceneLoadingInfo.allowSceneActivation = true;
		}
		#endregion
	}
}
