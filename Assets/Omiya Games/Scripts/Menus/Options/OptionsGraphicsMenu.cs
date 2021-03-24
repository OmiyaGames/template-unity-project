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
			TranslatedString name;

			public FullScreenMode Mode => mode;
			public TranslatedString Name => name;
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
		DropdownSet screenResolutionControls;
		[SerializeField]
		DropdownSet displayControls;
		[SerializeField]
		DropdownSet windowModeControls;
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

        #region Properties
        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override Selectable DefaultUi
        {
            get
            {
                Selectable returnUi = null;
				if(screenResolutionControls.IsEnabled == true)
				{
					returnUi = screenResolutionControls.Dropdown;
				}
				else if(windowModeControls.IsEnabled == true)
				{
					returnUi = windowModeControls.Dropdown;
				}
				else
				{
					foreach(ToggleSet controls in AllControls)
					{
						if(controls.IsEnabled == true)
						{
							returnUi = controls.Checkbox;
							break;
						}
					}
				}
				return returnUi;
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return BackgroundMenu.BackgroundType.SolidColor;
            }
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
				// Look for the first supported platform
				foreach(var options in windowModeOptions)
				{
					if(options.Platforms.IsSupported() == true)
					{
						return options;
					}
				}
				return windowModeOptions[0];
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

		#region UI Events
		public void OnCameraShakeClicked(bool isChecked)
        {
            if(IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsCameraShakesEnabled = isChecked;

                // Update bobbing head interactable state
                bobbingCameraControls.Checkbox.interactable = isChecked;
            }
        }

        public void OnHeadBobbingClicked(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsHeadBobbingOptionEnabled = isChecked;
            }
        }

        public void OnMotionBlursClicked(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsMotionBlursEnabled = isChecked;
            }
        }

        public void OnScreenFlashesClicked(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsScreenFlashesEnabled = isChecked;
            }
        }

        public void OnBloomClicked(bool isChecked)
        {
            if (IsListeningToEvents == true)
            {
                // Update settings
                Settings.IsBloomEffectEnabled = isChecked;
            }
        }

		public void OnScreenResolutionOptionSelected(int index)
		{
			if(IsListeningToEvents == true)
			{
				// FIXME: DO SOMETHING!
				// FIXME: Also bring up the confirmation window, with a 10 second timeout
			}
		}

		public void OnWindowModeOptionSelected(int index)
		{
			if(IsListeningToEvents == true)
			{
				// FIXME: DO SOMETHING!
				// FIXME: Also bring up the confirmation window, with a 10 second timeout
				// FIXME: Also enable or disable the display index dropdown
			}
		}

		public void OnDisplayOptionSelected(int index)
		{
			if(IsListeningToEvents == true)
			{
				// FIXME: DO SOMETHING!
				// FIXME: Also bring up the confirmation window, with a 10 second timeout
				// FIXME: Also update the screen resolution dropdown
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
				int selectedIndex = 0;
				var screenResolutions = new List<string>();
				foreach(var resolution in Screen.resolutions)
				{
					// Check if this resolution is the current resolution being set
					if(Screen.currentResolution.Equals(resolution) == true)
					{
						// Grab the index
						selectedIndex = screenResolutions.Count;
					}

					// Add a new resolution option
					screenResolutions.Add($"{resolution.width,4} x{resolution.height,5}, {resolution.refreshRate,3}Hz");
				}

				// Add all the options
				IsListeningToEvents = false;
				screenResolutionControls.Dropdown.ClearOptions();
				screenResolutionControls.Dropdown.AddOptions(screenResolutions);

				// Select the current resolution's dropdown item
				screenResolutionControls.Dropdown.value = selectedIndex;
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
				for(int i = 0; i < supportedOptions.Options.Length; ++i)
				{
					if(supportedOptions.Options[i].Mode == Screen.fullScreenMode)
					{
						// Select said option
						windowModeControls.Dropdown.value = i;
						break;
					}
				}
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

				// Select the current resolution's dropdown item
				displayControls.Dropdown.value = selectedIndex;
				IsListeningToEvents = true;
			}
		}
		#endregion
	}
}
