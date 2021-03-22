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
				fullSet.SetActive(IsEnabled);
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

		List<string> AllWindowModeOptions
		{
			get
			{
				if(allWindowModeOptions.Count == 0)
				{
					// Look for the first supported platform
					WindowModeOptions supportedOptions = windowModeOptions[0];
					foreach(var options in windowModeOptions)
					{
						if(options.Platforms.IsSupported() == true)
						{
							supportedOptions = options;
							break;
						}
					}

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
			screenResolutionControls.Setup();
			windowModeControls.Setup();
            foreach (ToggleSet controls in AllControls)
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
			var screenResolutions = new List<string>();
			foreach(var resolution in Screen.resolutions)
			{
				screenResolutions.Add($"{resolution.width,4} x{resolution.height,5}, {resolution.refreshRate,3}Hz");
			}
			screenResolutionControls.Dropdown.ClearOptions();
			screenResolutionControls.Dropdown.AddOptions(screenResolutions);

			// Setup window mode drop-down
			windowModeControls.Dropdown.ClearOptions();
			windowModeControls.Dropdown.AddOptions(AllWindowModeOptions);

			// Bind to the event where the localization language changes
			TranslationManager translationManager = Singleton.Get<TranslationManager>();
			translationManager.OnAfterLanguageChanged += TranslationManager_OnAfterLanguageChanged;
		}

		private void TranslationManager_OnAfterLanguageChanged(TranslationManager source, string lastLanguage, string currentLanguage)
		{
			// Reset the list of window mode options (they need to be re-localized)
			AllWindowModeOptions.Clear();

			// Update the drop down again
			windowModeControls.Dropdown.ClearOptions();
			windowModeControls.Dropdown.AddOptions(AllWindowModeOptions);
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
			}
		}
		#endregion
	}
}
