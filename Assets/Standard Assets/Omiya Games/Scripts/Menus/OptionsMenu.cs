using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsMenu.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that provides options.  Currently only supports changing sound
    /// and music volume. You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SoundEffect))]
    public class OptionsMenu : IMenu
    {
        [System.Serializable]
        public struct AudioControls
        {
            public Slider volumeSlider;
            public Text volumePercentLabel;
            public Image checkBoxMark;

            public void Setup(float volume, bool mute)
            {
                volumeSlider.value = volume;
                volumePercentLabel.text = Percent(volume);
                volumeSlider.interactable = !mute;
                checkBoxMark.enabled = mute;
            }
        }

        [SerializeField]
        AudioControls musicControls;
        [SerializeField]
        AudioControls soundEffectsControls;

        GameSettings settings = null;
        SoundEffect audioCache;
        bool inSetupMode = false;

        System.Action<OptionsMenu> hideAction = null;

        public SoundEffect TestSoundEffect
        {
            get
            {
                if(audioCache == null)
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

        public override GameObject DefaultUi
        {
            get
            {
                return musicControls.volumeSlider.gameObject;
            }
        }

        void Start()
        {
            // Check if we've already retrieve the settings
            if (settings != null)
            {
                // If so, don't do anything
                return;
            }

            // Retrieve settings
            settings = Singleton.Get<GameSettings>();

            // Setup controls
            inSetupMode = true;
            musicControls.Setup(BackgroundMusic.GlobalVolume, BackgroundMusic.GlobalMute);
            soundEffectsControls.Setup(SoundEffect.GlobalVolume, SoundEffect.GlobalMute);
            inSetupMode = false;
        }

        protected override void OnStateChanged(IMenu.State from, IMenu.State to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            if ((from == State.Visible) && (to == State.Hidden))
            {
                // Run the last action
                if (hideAction != null)
                {
                    hideAction(this);
                    hideAction = null;
                }
            }
        }

        #region UI events
        public override void Hide()
        {
            base.Hide();

            // Indicate button is clicked
            Manager.ButtonClick.Play();
        }

        public void OnMusicSliderChanged()
        {
            if (inSetupMode == false)
            {
                BackgroundMusic.GlobalVolume = musicControls.volumeSlider.value;
                musicControls.volumePercentLabel.text = Percent(BackgroundMusic.GlobalVolume);
            }
        }

        public void OnSoundEffectsSliderChanged()
        {
            if (inSetupMode == false)
            {
                SoundEffect.GlobalVolume = soundEffectsControls.volumeSlider.value;
                soundEffectsControls.volumePercentLabel.text = Percent(SoundEffect.GlobalVolume);
            }
        }
        
        public void OnSoundEffectsSliderPointerUp()
        {
            TestSoundEffect.Play();
        }

        public void OnMusicMuteClicked()
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                BackgroundMusic.GlobalMute = !BackgroundMusic.GlobalMute;

                // Change the check box
                musicControls.checkBoxMark.enabled = BackgroundMusic.GlobalMute;

                // disable the slider
                musicControls.volumeSlider.interactable = !BackgroundMusic.GlobalMute;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }

        public void OnSoundEffectsMuteClicked()
        {
            if (inSetupMode == false)
            {
                // Toggle mute
                SoundEffect.GlobalMute = !SoundEffect.GlobalMute;

                // Change the check box
                soundEffectsControls.checkBoxMark.enabled = SoundEffect.GlobalMute;

                // disable the slider
                soundEffectsControls.volumeSlider.interactable = !SoundEffect.GlobalMute;

                // Indicate button is clicked
                Manager.ButtonClick.Play();
            }
        }
        #endregion

        static string Percent(float val)
        {
            return val.ToString("0%");
        }
    }
}
