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
		public const float ConfirmationDuration = 20f;

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
		readonly List<string> allWindowModeOptions = new List<string>(4);
		WindowModeOptions? supportedWindowOption = null;
		Selectable currentDefaultUi = null;
		int lastSelectedResolution = 0, lastSelectedMode = 0, lastSelectedDisplay = 0;

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

			// Setup screen resolution drop-down
			SetupScreenResolutionDropdown();

			// Setup window mode drop-down
			SetupWindowModeDropdown();

			// Setup display drop-down
			SetupDisplayDropdown();
		}

		protected override void OnStateChanged(VisibilityState from, VisibilityState to)
		{
			base.OnStateChanged(from, to);

			// Check if this 
			if((from == VisibilityState.Hidden) && (to == VisibilityState.Visible))
			{
				SetupDisplayDropdown();
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
				Resolution selectedResolution = Screen.resolutions[index];

				// Apply said resolution
				Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode, selectedResolution.refreshRate);

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
					selectedResolution = Screen.resolutions[lastSelectedResolution];
					Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode, selectedResolution.refreshRate);

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
					// Update the last selected resolution
					lastSelectedMode = index;

					// Enable or disable the dropdown
					displayControls.Dropdown.interactable = (selectedMode != FullScreenMode.Windowed);
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
		#endregion

		#region Helper Methods
		private void SetupScreenResolutionDropdown()
		{
			// Setup screen resolution drop-down
			screenResolutionControls.Setup();

			// Verify if this feature is enabled
			if(screenResolutionControls.IsEnabled == true)
			{
				// Go through all supported screen resolutions
				lastSelectedResolution = 0;
				var screenResolutions = new List<string>();
				foreach(var resolution in Screen.resolutions)
				{
					// Check if this resolution is the current resolution being set
					if(Screen.currentResolution.Equals(resolution) == true)
					{
						// Grab the index
						lastSelectedResolution = screenResolutions.Count;
					}

					// Add a new resolution option
					screenResolutions.Add($"{resolution.width,4} x{resolution.height,5}, {resolution.refreshRate,3}Hz");
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
						lastSelectedMode = 1;
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
				TranslationManager translationManager = Singleton.Get<TranslationManager>();
				translationManager.OnAfterLanguageChanged += TranslationManager_OnAfterLanguageChanged;
				void TranslationManager_OnAfterLanguageChanged(TranslationManager source, string lastLanguage, string currentLanguage)
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
				}
			}
		}

		private void SetupDisplayDropdown()
		{
			// Setup screen resolution drop-down
			bool isEnabled = (displayControls.IsEnabled && (Display.displays.Length > 1));
			displayControls.SetActive(isEnabled);

			// Verify if this feature is enabled
			if(isEnabled == true)
			{
				// Go through all supported screen resolutions
				int selectedIndex = 0;
				var displayIndexes = new List<string>(Display.displays.Length);
				for(int i = 0; i < Display.displays.Length; ++i)
				{
					// Check if this resolution is the current resolution being set
					if(Display.displays[i].active == true)
					{
						// Grab the index
						selectedIndex = i;
					}

					// Add a new resolution option
					displayIndexes.Add((i + 1).ToString());
				}

				// Add all the options
				IsListeningToEvents = false;
				displayControls.Dropdown.ClearOptions();
				displayControls.Dropdown.AddOptions(displayIndexes);

				// Select the current display's dropdown item
				displayControls.Dropdown.value = selectedIndex;
				lastSelectedDisplay = selectedIndex;
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
		#endregion
	}
}
