using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OmiyaGames
{
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
        [SerializeField]
        float delayPlayingTestSound = 0.2f;

        GameSettings settings = null;
        BackgroundMusic musicSettings = null;
        SoundEffect audioCache;
        bool inSetupMode = false,
            isButtonLocked = false;

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
            musicSettings = Singleton.Get<BackgroundMusic>();

            // Setup controls
            inSetupMode = true;
            musicControls.Setup(musicSettings.Volume, musicSettings.IsMuted);
            soundEffectsControls.Setup(SoundEffect.GlobalVolume, SoundEffect.GlobalMute);
            inSetupMode = false;
        }

        protected override void OnStateChanged(IMenu.State from, IMenu.State to)
        {
            // Call the base method
            base.OnStateChanged(from, to);

            if (to == State.Visible)
            {
                // If this menu is visible again, release the button lock
                isButtonLocked = false;
            }
            else if ((from == State.Visible) && (to == State.Hidden))
            {
                // Run the last action
                if (hideAction != null)
                {
                    hideAction(this);
                    hideAction = null;
                }
            }
        }

        public void Show(System.Action<OptionsMenu> returnAction = null)
        {
            if (CurrentState == State.Hidden)
            {
                // Make the panel visible
                CurrentState = State.Visible;

                // Setup the next action
                hideAction = returnAction;
            }
        }

        public void Hide()
        {
            if ((CurrentState == State.Visible) && (isButtonLocked == false))
            {
                // Lock the buttons
                isButtonLocked = true;

                // Make the panel hidden
                CurrentState = State.Hidden;
            }
        }

        #region UI events
        public void OnMusicSliderChanged()
        {
            if (inSetupMode == false)
            {
                musicSettings.Volume = musicControls.volumeSlider.value;
                musicControls.volumePercentLabel.text = Percent(musicSettings.Volume);
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
                musicSettings.IsMuted = !musicSettings.IsMuted;

                // Change the check box
                musicControls.checkBoxMark.enabled = musicSettings.IsMuted;

                // disable the slider
                musicControls.volumeSlider.interactable = !musicSettings.IsMuted;
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

                // Play a test sound effect
                OnSoundEffectsSliderPointerUp();
            }
        }
        #endregion

        static string Percent(float val)
        {
            return val.ToString("0%");
        }
    }
}
