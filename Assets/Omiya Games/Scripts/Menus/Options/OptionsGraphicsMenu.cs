using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Global;
using OmiyaGames.Translations;

namespace OmiyaGames.Menus
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="OptionsGraphicsMenu.cs" company="Omiya Games">
	/// The MIT License (MIT)
	/// 
	/// Copyright (c) 2014-2021 Omiya Games
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
	/// <strong>Date:</strong> 6/15/2018<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Initial version.
	/// </description>
	/// </item>
	/// <item>
	/// <term>
	/// <strong>Date:</strong> 3/21/2021<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Adding window mode and screen resolution options.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Menu that provides graphics options.
	/// You can retrieve this menu from the singleton script,
	/// <seealso cref="MenuManager"/>.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class OptionsGraphicsMenu : IOptionsMenu
	{
		public const float ConfirmationDuration = 15f;

		[System.Serializable]
		public struct ToggleSet
		{
			[SerializeField]
			SupportedPlatforms enableFor;
			[SerializeField]
			Toggle checkbox;
			[SerializeField]
			GameObject[] parents;

			public SupportedPlatforms EnableFor => enableFor;
			public Toggle Checkbox => checkbox;
			public GameObject[] Parents => parents;
			public bool IsEnabled => EnableFor.IsSupported();

			public void Setup()
			{
				bool isActive = IsEnabled;
				if((Parents != null) && (Parents.Length > 0))
				{
					foreach(GameObject parent in Parents)
					{
						parent.SetActive(isActive);
					}
				}
				else
				{
					Checkbox.gameObject.SetActive(isActive);
				}
			}
		}

		[System.Serializable]
		public struct DropdownSet
		{
			[SerializeField]
			SupportedPlatforms enableFor;
			[SerializeField]
			TMPro.TMP_Dropdown dropdown;
			[SerializeField]
			GameObject fullSet;

			public SupportedPlatforms EnableFor => enableFor;
			public TMPro.TMP_Dropdown Dropdown => dropdown;
			public bool IsEnabled => EnableFor.IsSupported();
			public GameObject gameObject => fullSet;

			public void Setup()
			{
				SetActive(IsEnabled);
			}

			public void SetActive(bool active)
			{
				fullSet.SetActive(active);
			}
		}

		[System.Serializable]
		public struct WindowModeOption
		{
			[SerializeField]
			FullScreenMode mode;
			[SerializeField]
			bool isFullscreen;
			[SerializeField]
			TranslatedString name;

			public FullScreenMode Mode => mode;
			public TranslatedString Name => name;
			public bool IsFullscreen => isFullscreen;
		}

		[System.Serializable]
		public struct WindowModeOptions
		{
			[SerializeField]
			SupportedPlatforms platforms;
			[SerializeField]
			WindowModeOption[] options;

			public SupportedPlatforms Platforms => platforms;
			public WindowModeOption[] Options => options;
		}

		private class ScreenInfo
		{
			public Resolution Resolution
			{
				get;
				private set;
			}
			public float Dpi
			{
				get;
				private set;
			}
			public ScreenOrientation Orientation
			{
				get;
				private set;
			}

			/// <summary>
			/// Checks if the current monitor contains the same
			/// stats as the data held within this class.
			/// </summary>
			/// <returns></returns>
			public bool IsSameScreen()
			{
				return Resolution.Equals(Screen.currentResolution)
					|| Mathf.Approximately(Dpi, Screen.dpi)
					|| (Orientation == Screen.orientation);
			}

			/// <summary>
			/// Updates all properties to the latest info.
			/// </summary>
			public void UpdateInfo()
			{
				Resolution = Screen.currentResolution;
				Dpi = Screen.dpi;
				Orientation = Screen.orientation;
			}
		}

		private struct ResolutionOption : System.IEquatable<ResolutionOption>, System.IEquatable<Resolution>
		{
			public ResolutionOption(Resolution resolution)
			{
				Width = resolution.width;
				Height = resolution.height;
			}

			public int Width
			{
				get;
			}
			public int Height
			{
				get;
			}

			public bool Equals(ResolutionOption other)
			{
				return (Width == other.Width) && (Height == other.Height);
			}

			public bool Equals(Resolution other)
			{
				return (Width == other.width) && (Height == other.height);
			}

			public bool Equals(int width, int height)
			{
				return (Width == width) && (Height == height);
			}

			public override bool Equals(object obj)
			{
				if(obj is ResolutionOption)
				{
					return Equals((ResolutionOption)obj);
				}
				else if(obj is Resolution)
				{
					return Equals((Resolution)obj);
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return Width.GetHashCode() ^ Height.GetHashCode();
			}
		}

		#region Serialized Fields
		[Header("Screen Controls")]
		[SerializeField]
		TranslatedString confirmationTitle;
		[SerializeField]
		DropdownSet screenResolutionControls;
		[SerializeField]
		TranslatedString setScreenResolutionMessage;
		[SerializeField]
		DropdownSet displayControls;
		[SerializeField]
		TranslatedString setDisplayMessage;
		[SerializeField]
		DropdownSet windowModeControls;
		[SerializeField]
		TranslatedString setWindowModeMessage;
		[SerializeField]
		WindowModeOptions[] windowModeOptions;

		[Header("Special Effects Controls")]
		[SerializeField]
		ToggleSet cameraShakeControls;
		[SerializeField]
		ToggleSet bobbingCameraControls;
		[SerializeField]
		ToggleSet screenFlashesControls;
		[SerializeField]
		ToggleSet motionBlursControls;
		[SerializeField]
		ToggleSet bloomControls;
		#endregion

		ToggleSet[] allControls = null;
		WindowModeOptions? supportedWindowOption = null;
		Selectable currentDefaultUi = null;
		System.Action<float> updateAction = null;
		TranslationManager.LanguageChanged updateWindowModeDropDown = null;
		int lastSelectedResolution = 0, lastSelectedMode = 0, lastSelectedDisplay = 0;
		readonly List<string> allWindowModeOptions = new List<string>(4);
		readonly List<ResolutionOption> allResolutionOptions = new List<ResolutionOption>();
		readonly ScreenInfo lastFrameMonitorData = new ScreenInfo();

		#region Properties
		/// <inheritdoc/>
		public override Type MenuType
		{
			get
			{
				return Type.ManagedMenu;
			}
		}

		/// <inheritdoc/>
		public override Selectable DefaultUi
		{
			get => CurrentDefaultUi;
		}

		/// <inheritdoc/>
		public override BackgroundMenu.BackgroundType Background
		{
			get
			{
				return BackgroundMenu.BackgroundType.SolidColor;
			}
		}

		Selectable CurrentDefaultUi
		{
			get
			{
				if(currentDefaultUi == null)
				{
					if(screenResolutionControls.IsEnabled == true)
					{
						currentDefaultUi = screenResolutionControls.Dropdown;
					}
					else if(windowModeControls.IsEnabled == true)
					{
						currentDefaultUi = windowModeControls.Dropdown;
					}
					else
					{
						foreach(ToggleSet controls in AllControls)
						{
							if(controls.IsEnabled == true)
							{
								currentDefaultUi = controls.Checkbox;
								break;
							}
						}
					}
				}
				return currentDefaultUi;
			}
			set => currentDefaultUi = value;
		}

		ToggleSet[] AllControls
		{
			get
			{
				if(allControls == null)
				{
					allControls = new ToggleSet[]
					{
						cameraShakeControls,
						bobbingCameraControls,
						motionBlursControls,
						screenFlashesControls,
						bloomControls
					};
				}
				return allControls;
			}
		}

		WindowModeOptions SupportedOption
		{
			get
			{
				// Check if an option is chosen
				if(supportedWindowOption == null)
				{
					// Look for the first supported platform
					supportedWindowOption = windowModeOptions[0];
					foreach(var options in windowModeOptions)
					{
						if(options.Platforms.IsSupported() == true)
						{
							supportedWindowOption = options;
							break;
						}
					}
				}
				return supportedWindowOption.Value;
			}
		}

		List<string> AllWindowModeOptions
		{
			get
			{
				if(allWindowModeOptions.Count == 0)
				{
					// Look for the first supported platform
					WindowModeOptions supportedOptions = SupportedOption;

					// Populate the list
					allWindowModeOptions.Clear();
					foreach(var option in supportedOptions.Options)
					{
						allWindowModeOptions.Add(option.Name.ToString());
					}
				}
				return allWindowModeOptions;
			}
		}
		#endregion

		public virtual void OnDestroy()
		{
			// Make sure to clean up any events we're listening to
			if(updateWindowModeDropDown != null)
			{
				Singleton.Get<TranslationManager>().OnAfterLanguageChanged -= updateWindowModeDropDown;
				updateWindowModeDropDown = null;
			}
			if(updateAction != null)
			{
				Singleton.Instance.OnUpdate -= updateAction;
				updateAction = null;
			}
		}

		/// <inheritdoc/>
		protected override void OnSetup()
		{
			// Call base method
			base.OnSetup();

			// Call setup on all controls
			foreach(ToggleSet controls in AllControls)
			{
				controls.Setup();
			}

			// Setup checkbox isOn state
			cameraShakeControls.Checkbox.isOn = Settings.IsCameraShakesEnabled;
			bobbingCameraControls.Checkbox.isOn = Settings.IsHeadBobbingOptionEnabled;
			motionBlursControls.Checkbox.isOn = Settings.IsMotionBlursEnabled;
			screenFlashesControls.Checkbox.isOn = Settings.IsScreenFlashesEnabled;
			bloomControls.Checkbox.isOn = Settings.IsBloomEffectEnabled;

			// Setup checkbox interactable state
			bobbingCameraControls.Checkbox.interactable = cameraShakeControls.Checkbox.isOn;

			// Setup whether to disable bobbing camera or not
			bobbingCameraControls.Checkbox.interactable = cameraShakeControls.Checkbox.isOn;
		}

		/// <inheritdoc/>
		protected override void OnStateChanged(VisibilityState from, VisibilityState to)
		{
			base.OnStateChanged(from, to);

			// Check if this 
			if((from == VisibilityState.Hidden) && (to == VisibilityState.Visible))
			{
				// Setup screen resolution drop-down
				SetupScreenResolutionDropdown();

				// Setup window mode drop-down
				SetupWindowModeDropdown();

				// Setup display drop-down
				SetupDisplayDropdown();

				// Update dropdown being enabled
				UpdateDropdownEnabled();

				// Update this current frame's monitor data
				lastFrameMonitorData.UpdateInfo();

				// Cache delegate reference
				if(updateAction == null)
				{
					updateAction = new System.Action<float>(CheckDisplayChange);
				}

				// Bind to update function
				Singleton.Instance.OnUpdate += updateAction;
			}
			else if((from == VisibilityState.Visible) && (to == VisibilityState.Hidden) && (updateAction != null))
			{
				// Unbind to update function
				Singleton.Instance.OnUpdate -= updateAction;
			}
		}

		#region UI Events
		public void OnCameraShakeClicked(bool isChecked)
		{
			if(IsListeningToEvents == true)
			{
				// Update settings
				Settings.IsCameraShakesEnabled = isChecked;

				// Update bobbing head interactable state
				bobbingCameraControls.Checkbox.interactable = isChecked;
				CurrentDefaultUi = cameraShakeControls.Checkbox;
			}
		}

		public void OnHeadBobbingClicked(bool isChecked)
		{
			if(IsListeningToEvents == true)
			{
				// Update settings
				Settings.IsHeadBobbingOptionEnabled = isChecked;
				CurrentDefaultUi = bobbingCameraControls.Checkbox;
			}
		}

		public void OnMotionBlursClicked(bool isChecked)
		{
			if(IsListeningToEvents == true)
			{
				// Update settings
				Settings.IsMotionBlursEnabled = isChecked;
				CurrentDefaultUi = motionBlursControls.Checkbox;
			}
		}

		public void OnScreenFlashesClicked(bool isChecked)
		{
			if(IsListeningToEvents == true)
			{
				// Update settings
				Settings.IsScreenFlashesEnabled = isChecked;
				CurrentDefaultUi = screenFlashesControls.Checkbox;
			}
		}

		public void OnBloomClicked(bool isChecked)
		{
			if(IsListeningToEvents == true)
			{
				// Update settings
				Settings.IsBloomEffectEnabled = isChecked;
				CurrentDefaultUi = bloomControls.Checkbox;
			}
		}

		public void OnScreenResolutionOptionSelected(int index)
		{
			if(IsListeningToEvents == true)
			{
				// Indicate this dropdown was clicked
				CurrentDefaultUi = screenResolutionControls.Dropdown;

				// Get selected screen resolution
				var selectedResolution = allResolutionOptions[index];

				// Apply said resolution
				Screen.SetResolution(selectedResolution.Width, selectedResolution.Height, Screen.fullScreenMode);

				// Bring up the confirmation window with a timeout
				DisplayConfirmation(setScreenResolutionMessage, ApplyScreenResolution, ResetScreenResolution);
				void ApplyScreenResolution()
				{
					// Update the last selected resolution
					lastSelectedResolution = index;
				}
				void ResetScreenResolution()
				{
					// Reset the screen resolution back to the old value
					IsListeningToEvents = false;
					selectedResolution = allResolutionOptions[lastSelectedResolution];
					Screen.SetResolution(selectedResolution.Width, selectedResolution.Height, Screen.fullScreenMode);

					// Revert the drop down to the old value
					screenResolutionControls.Dropdown.value = lastSelectedResolution;
					IsListeningToEvents = true;
				}
			}
		}

		public void OnWindowModeOptionSelected(int index)
		{
			if(IsListeningToEvents == true)
			{
				// Indicate this dropdown was clicked
				CurrentDefaultUi = windowModeControls.Dropdown;

				// Set full screen mode
				FullScreenMode selectedMode = SupportedOption.Options[index].Mode;
				Screen.fullScreenMode = selectedMode;

				// Also bring up the confirmation window, with a 10 second timeout
				DisplayConfirmation(setWindowModeMessage, ApplyWindowMode, ResetWindowMode);
				void ApplyWindowMode()
				{
					// Update the last selected window mode
					lastSelectedMode = index;

					// Update all the other dropdowns
					UpdateDropdownEnabled();
					UpdateDropdownValue();
				}
				void ResetWindowMode()
				{
					// Reset the window mode back to the old value
					IsListeningToEvents = false;
					selectedMode = SupportedOption.Options[lastSelectedMode].Mode;
					Screen.fullScreenMode = selectedMode;

					// Revert the drop down to the old value
					windowModeControls.Dropdown.value = lastSelectedMode;
					IsListeningToEvents = true;
				}
			}
		}

		public void OnDisplayOptionSelected(int index)
		{
			if(IsListeningToEvents == true)
			{
				// Indicate this dropdown was clicked
				CurrentDefaultUi = displayControls.Dropdown;

				// Set full screen mode
				Display.displays[index].Activate();

				// Also bring up the confirmation window, with a 10 second timeout
				DisplayConfirmation(setWindowModeMessage, ApplyWindowMode, ResetWindowMode);
				void ApplyWindowMode()
				{
					// Update the last selected resolution
					lastSelectedDisplay = index;

					// Also update the screen resolution dropdown
					SetupScreenResolutionDropdown();
				}
				void ResetWindowMode()
				{
					// Reset the display back to the old value
					IsListeningToEvents = false;
					Display.displays[lastSelectedDisplay].Activate();

					// Revert the drop down to the old value
					displayControls.Dropdown.value = lastSelectedDisplay;
					IsListeningToEvents = true;
				}
			}
		}

		public void UpdateScreenResolutionControls()
		{
			if((IsListeningToEvents == true) && (screenResolutionControls.Dropdown.IsExpanded == true))
			{
				ScrollDropDown(screenResolutionControls.gameObject, screenResolutionControls.Dropdown);
			}
		}

		public void UpdateWindowModeControls()
		{
			if((IsListeningToEvents == true) && (windowModeControls.Dropdown.IsExpanded == true))
			{
				ScrollDropDown(windowModeControls.gameObject, windowModeControls.Dropdown);
			}
		}

		public void UpdateDisplayControls()
		{
			if((IsListeningToEvents == true) && (displayControls.Dropdown.IsExpanded == true))
			{
				ScrollDropDown(displayControls.gameObject, displayControls.Dropdown);
			}
		}
		#endregion

		#region Helper Methods
		private void SetupScreenResolutionDropdown()
		{
			// Setup screen resolution drop-down
			screenResolutionControls.Setup();

			// Verify if this feature is enabled
			if(screenResolutionControls.IsEnabled == true)
			{
				// Setup lists
				var uniqueOptions = new HashSet<ResolutionOption>();
				var screenResolutions = new List<string>();
				lastSelectedResolution = 0;
				allResolutionOptions.Clear();

				// Go through all supported screen resolutions
				foreach(var resolution in Screen.resolutions)
				{
					// Convert resolution to an option; check if it conflicts with another option
					var newOption = new ResolutionOption(resolution);
					if(uniqueOptions.Contains(newOption) == false)
					{
						// Check if this resolution is the current resolution being set
						if(newOption.Equals(Screen.width, Screen.height) == true)
						{
							// Grab the index
							lastSelectedResolution = allResolutionOptions.Count;
						}

						// Add a new resolution option
						allResolutionOptions.Add(newOption);
						screenResolutions.Add($"{newOption.Width, 4} x{newOption.Height,5}");
						uniqueOptions.Add(newOption);
					}
				}

				// Add all the options
				IsListeningToEvents = false;
				screenResolutionControls.Dropdown.ClearOptions();
				screenResolutionControls.Dropdown.AddOptions(screenResolutions);

				// Select the current resolution's dropdown item
				screenResolutionControls.Dropdown.value = lastSelectedResolution;
				IsListeningToEvents = true;
			}
		}

		private void SetupWindowModeDropdown()
		{
			// Setup window mode drop-down
			windowModeControls.Setup();

			// Verify if this feature is enabled
			if(windowModeControls.IsEnabled == true)
			{
				// Add all the options
				IsListeningToEvents = false;
				windowModeControls.Dropdown.ClearOptions();
				windowModeControls.Dropdown.AddOptions(AllWindowModeOptions);

				// Check what the current window mode is
				WindowModeOptions supportedOptions = SupportedOption;
				lastSelectedMode = -1;
				for(int i = 0; i < supportedOptions.Options.Length; ++i)
				{
					if(supportedOptions.Options[i].Mode == Screen.fullScreenMode)
					{
						// Select said option
						lastSelectedMode = i;
						break;
					}
				}

				// Check if we actually got the correct index from just fullscreen mode
				if(lastSelectedMode < 0)
				{
					// Default to the first mode
					lastSelectedMode = 0;

					// Search via boolean flags instead
					for(int i = 0; i < supportedOptions.Options.Length; ++i)
					{
						if(supportedOptions.Options[i].IsFullscreen == Screen.fullScreen)
						{
							// Select said option
							lastSelectedMode = 1;
							break;
						}
					}
				}

				// Select the default dropdown item
				windowModeControls.Dropdown.value = lastSelectedMode;
				IsListeningToEvents = true;

				// Bind to the event where the localization language changes
				if(updateWindowModeDropDown == null)
				{
					// Setup the translation changed event
					updateWindowModeDropDown = (TranslationManager source, string lastLanguage, string currentLanguage) =>
					{
						// Reset the list of window mode options (they need to be re-localized)
						AllWindowModeOptions.Clear();

						// Grab the old selected option
						int lastValue = windowModeControls.Dropdown.value;

						// Update the drop down again
						IsListeningToEvents = false;
						windowModeControls.Dropdown.ClearOptions();
						windowModeControls.Dropdown.AddOptions(AllWindowModeOptions);
						windowModeControls.Dropdown.value = lastValue;
						IsListeningToEvents = true;
					};

					// Bind to the translation changed event
					Singleton.Get<TranslationManager>().OnAfterLanguageChanged += updateWindowModeDropDown;
				}
			}
		}

		private void SetupDisplayDropdown()
		{
			// Setup screen resolution drop-down
			displayControls.Setup();

			// Verify if this feature is enabled
			if(displayControls.IsEnabled == true)
			{
				// Go through all supported screen resolutions
				lastSelectedDisplay = 0;
				var displayIndexes = new List<string>(Display.displays.Length);
				for(int i = 0; i < Display.displays.Length; ++i)
				{
					// Check if this resolution is the current resolution being set
					if(Display.displays[i].active == true)
					{
						// Grab the index
						lastSelectedDisplay = i;
					}

					// Add a new resolution option
					displayIndexes.Add((i + 1).ToString());
				}

				// Add all the options
				IsListeningToEvents = false;
				displayControls.Dropdown.ClearOptions();
				displayControls.Dropdown.AddOptions(displayIndexes);

				// Select the current display's dropdown item
				displayControls.Dropdown.value = lastSelectedDisplay;
				IsListeningToEvents = true;
			}
		}

		private void DisplayConfirmation(TranslatedString message, System.Action confirmAction, System.Action denyAction)
		{
			// Grab the dialog
			ConfirmationMenu menu = Manager.GetMenu<ConfirmationMenu>();
			if(menu != null)
			{
				// Update the confirmation dialog
				menu.DefaultToYes = false;
				menu.UpdateDialog(this, message.TranslationKey, ConfirmationDuration);

				// Display confirmation dialog
				menu.Show(ConfirmationAction);
				void ConfirmationAction(IMenu source, VisibilityState from, VisibilityState to)
				{
					if((to == VisibilityState.Hidden) && (source is ConfirmationMenu))
					{
						if(((ConfirmationMenu)source).IsYesSelected == true)
						{
							confirmAction?.Invoke();
						}
						else
						{
							denyAction?.Invoke();
						}
					}
				}
			}
		}

		private void ScrollDropDown(GameObject fullDropdownSet, TMPro.TMP_Dropdown dropdown)
		{
			// Attempt to grab a scrollbar from the gameobject
			Scrollbar scrollbar = null;
			Scrollbar[] scrollbars = fullDropdownSet.GetComponentsInChildren<Scrollbar>(true);
			foreach(var checkScrollbar in scrollbars)
			{
				// Check if this isn't the template's scrollbar
				if(dropdown.template != checkScrollbar.transform.parent)
				{
					// Cache this scrollbar
					scrollbar = checkScrollbar;
					break;
				}
			}

			// Check if a scrollbar is found
			if(scrollbar != null)
			{
				// Scroll to the selection
				float fraction = dropdown.value;
				fraction /= (dropdown.options.Count - 1);
				scrollbar.value = (1f - fraction);
			}
		}

		private void UpdateDropdownEnabled()
		{
			// Disable display controls if not in full screen
			displayControls.Dropdown.interactable = (Screen.fullScreenMode != FullScreenMode.Windowed);

			// Disable screen resolution control if in "windowed" fullscreen mode
			screenResolutionControls.Dropdown.interactable = (Screen.fullScreenMode != FullScreenMode.MaximizedWindow);
		}

		private void UpdateDropdownValue()
		{
			// Check which screen resolution is selected
			for(int i = 0; i < allResolutionOptions.Count; ++i)
			{
				// Check if this resolution is the current resolution being set
				if(allResolutionOptions[i].Equals(Screen.width, Screen.height) == true)
				{
					// Grab the index
					lastSelectedResolution = i;
					break;
				}
			}

			// Check which display is active
			for(int i = 0; i < Display.displays.Length; ++i)
			{
				// Check if this resolution is the current resolution being set
				if(Display.displays[i].active == true)
				{
					// Grab the index
					lastSelectedDisplay = i;
					break;
				}
			}

			// Update the drop down values
			IsListeningToEvents = false;
			screenResolutionControls.Dropdown.value = lastSelectedResolution;
			displayControls.Dropdown.value = lastSelectedDisplay;
			IsListeningToEvents = true;
		}

		private void CheckDisplayChange(float obj)
		{
			if(lastFrameMonitorData.IsSameScreen() == false)
			{
				// Update the screen resolution drop down with latest screen resolutions list
				SetupScreenResolutionDropdown();

				// Update the display drop down with the latest connected monitor info
				SetupDisplayDropdown();

				// Update all the other dropdowns
				UpdateDropdownEnabled();

				// Update the screen data
				lastFrameMonitorData.UpdateInfo();
			}
		}
		#endregion
	}
}
