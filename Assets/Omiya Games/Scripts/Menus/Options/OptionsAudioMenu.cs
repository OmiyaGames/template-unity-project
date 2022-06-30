using System.Collections;
using UnityEngine;
using OmiyaGames.Audio;

namespace OmiyaGames.Menus
{
	///-----------------------------------------------------------------------
	/// <remarks>
	/// <copyright file="OptionsAudioMenu.cs" company="Omiya Games">
	/// The MIT License (MIT)
	/// 
	/// Copyright (c) 2018-2022 Omiya Games
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
	/// <strong>Date:</strong> 6/11/2018<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>Initial verison.</description>
	/// </item><item>
	/// <term>
	/// <strong>Date:</strong> 6/29/2022<br/>
	/// <strong>Author:</strong> Taro Omiya
	/// </term>
	/// <description>
	/// Updated to support <seealso cref="AudioManager"/>.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	///-----------------------------------------------------------------------
	/// <summary>
	/// Menu that provides audio options.
	/// You can retrieve this menu from the singleton script,
	/// <seealso cref="MenuManager"/>.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	[DisallowMultipleComponent]
	public class OptionsAudioMenu : IOptionsMenu
	{
		[System.Serializable]
		public struct LayerControls
		{
			[SerializeField]
			SupportedPlatforms enableFor;
			[SerializeField]
			AudioVolumeControls volumeControls;
			[SerializeField]
			GameObject[] volumeSection;
			[SerializeField]
			SoundEffect testAudio;

			public SupportedPlatforms EnableFor => enableFor;
			public UnityEngine.UI.Slider VolumeSlider => volumeControls.Slider;

			public void SetupControls(AudioLayer layer)
			{
				// Setup enabling the music controls
				bool enableControl = EnableFor.IsSupported();
				foreach (GameObject controls in volumeSection)
				{
					controls.SetActive(enableControl);
				}

				if (enableControl == true)
				{
					// Setup controls
					volumeControls.Setup(layer.VolumePercent, layer.IsMuted);

					// Bind to the control events
					volumeControls.OnCheckboxUpdated += enableMute => layer.IsMuted = enableMute;
					volumeControls.OnSliderValueUpdated += volume => layer.VolumePercent = volume;
					volumeControls.OnSliderReleaseUpdated += volume => layer.VolumePercent = volume;

					// Check if test audio exists
					if (testAudio != null)
					{
						// Play the sound effect
						SoundEffect testClip = testAudio;
						volumeControls.OnCheckboxUpdated += enableMute =>
						{
							if (enableMute == false)
							{
								testClip.Play();
							}
						};
						volumeControls.OnSliderReleaseUpdated += volume => testClip.Play();
					}
				}
			}
		}

		[Header("Audio Controls")]
		[SerializeField]
		LayerControls main;
		[SerializeField]
		LayerControls music;
		[SerializeField]
		LayerControls soundEffects;
		[SerializeField]
		LayerControls voices;
		[SerializeField]
		LayerControls ambience;

		[Header("Other")]
		[SerializeField]
		GameObject[] allDividers;

		#region Properties
		public override Type MenuType => Type.ManagedMenu;

		public override UnityEngine.UI.Selectable DefaultUi
		{
			get
			{
				if (main.EnableFor.IsSupported())
				{
					return main.VolumeSlider;
				}
				else if (music.EnableFor.IsSupported())
				{
					return music.VolumeSlider;
				}
				else if (soundEffects.EnableFor.IsSupported())
				{
					return soundEffects.VolumeSlider;
				}
				else if (voices.EnableFor.IsSupported())
				{
					return voices.VolumeSlider;
				}
				else if (ambience.EnableFor.IsSupported())
				{
					return ambience.VolumeSlider;
				}
				return null;
			}
		}

		public override BackgroundMenu.BackgroundType Background => BackgroundMenu.BackgroundType.SolidColor;
		#endregion

		protected override void OnSetup()
		{
			// Call base method
			base.OnSetup();

			StartCoroutine(SetupCoroutine());

			IEnumerator SetupCoroutine()
			{
				// Make sure to setup the AudioManager first
				yield return StartCoroutine(AudioManager.Setup());

				// Setup enabling controls
				main.SetupControls(AudioManager.Main);
				music.SetupControls(AudioManager.Music);
				soundEffects.SetupControls(AudioManager.SoundEffects);
				voices.SetupControls(AudioManager.Voices);
				ambience.SetupControls(AudioManager.Ambience);

				// Update how dividers appear
				SetupDividers(allDividers, main.EnableFor, music.EnableFor, soundEffects.EnableFor, voices.EnableFor, ambience.EnableFor);
			}
		}
	}
}
