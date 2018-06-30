using UnityEngine;
using OmiyaGames.Audio;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsAudioMenu.cs" company="Omiya Games">
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
    /// <date>6/11/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that provides audio options.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SoundEffect))]
    [DisallowMultipleComponent]
    public class OptionsAudioMenu : IOptionsMenu
    {
        #region Serialized Fields
        [Header("Features to Enable")]
        [SerializeField]
        SupportedPlatforms enableMusicVolumeControls;
        [SerializeField]
        SupportedPlatforms enableSoundEffectVolumeControls;
        [SerializeField]
        GameObject[] allDividers;

        [Header("Music Controls")]
        [SerializeField]
        AudioVolumeControls musicVolumeControls;
        [SerializeField]
        GameObject[] musicVolumeSection;

        [Header("Sound Effects Controls")]
        [SerializeField]
        AudioVolumeControls soundEffectsVolumeControls;
        [SerializeField]
        GameObject[] soundEffectsSection;
        #endregion

        SoundEffect audioCache;

        #region Properties
        public SoundEffect TestSoundEffect
        {
            get
            {
                if (audioCache == null)
                {
                    audioCache = GetComponent<SoundEffect>();
                }
                return audioCache;
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override UnityEngine.UI.Selectable DefaultUi
        {
            get
            {
                if(enableMusicVolumeControls.IsThisBuildSupported())
                {
                    return musicVolumeControls.Slider;
                }
                else
                {
                    return soundEffectsVolumeControls.Slider;
                }
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return BackgroundMenu.BackgroundType.SolidColor;
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // Setup enabling the music controls
            SetupMusicControls();

            // Setup enabling the sound effect controls
            SetupSoundEffectControls();

            // Update how dividers appear
            SetupDividers(allDividers,
                enableMusicVolumeControls,
                enableSoundEffectVolumeControls);
        }

        #region UI events
        private void OnSoundEffectsSliderReleaseUpdated(float volume)
        {
            // Adjust the volume
            OnSoundEffectsSliderValueUpdated(volume);

            // Play the sound effect
            TestSoundEffect.Play();
        }

        private void OnSoundEffectsSliderValueUpdated(float volume)
        {
            // Adjust the volume
            SoundEffect.GlobalVolume = volume;
        }

        private void OnSoundEffectsCheckboxUpdated(bool enableMute)
        {
            // Adjust mute setting
            SoundEffect.GlobalMute = enableMute;

            // Check if we're unmuted
            if(enableMute == false)
            {
                // Play the sound effect
                TestSoundEffect.Play();
            }
        }

        private void OnMusicSliderValueUpdated(float volume)
        {
            // Adjust the volume
            BackgroundMusic.GlobalVolume = volume;
        }

        private void OnMusicCheckboxUpdated(bool enableMute)
        {
            // Adjust mute setting
            BackgroundMusic.GlobalMute = enableMute;
        }
        #endregion

        #region Helper Methods
        private void SetupMusicControls()
        {
            // Setup enabling the music controls
            bool enableControl = enableMusicVolumeControls.IsThisBuildSupported();
            foreach (GameObject controls in musicVolumeSection)
            {
                controls.SetActive(enableControl);
            }

            if (enableControl == true)
            {
                // Setup controls
                musicVolumeControls.Setup(BackgroundMusic.GlobalVolume, BackgroundMusic.GlobalMute);

                // Bind to the control events
                musicVolumeControls.OnCheckboxUpdated += OnMusicCheckboxUpdated;
                musicVolumeControls.OnSliderValueUpdated += OnMusicSliderValueUpdated;
                musicVolumeControls.OnSliderReleaseUpdated += OnMusicSliderValueUpdated;
            }
        }

        private bool SetupSoundEffectControls()
        {
            bool enableControl = enableSoundEffectVolumeControls.IsThisBuildSupported();
            foreach (GameObject controls in soundEffectsSection)
            {
                controls.SetActive(enableControl);
            }

            if (enableControl == true)
            {
                // Setup controls
                soundEffectsVolumeControls.Setup(SoundEffect.GlobalVolume, SoundEffect.GlobalMute);

                // Bind to the control events
                soundEffectsVolumeControls.OnCheckboxUpdated += OnSoundEffectsCheckboxUpdated;
                soundEffectsVolumeControls.OnSliderValueUpdated += OnSoundEffectsSliderValueUpdated;
                soundEffectsVolumeControls.OnSliderReleaseUpdated += OnSoundEffectsSliderReleaseUpdated;
            }

            return enableControl;
        }
        #endregion
    }
}
