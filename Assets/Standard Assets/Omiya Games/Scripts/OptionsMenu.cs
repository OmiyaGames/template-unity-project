using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenu : ISingletonScript
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
    GameObject optionsPanel;
    [SerializeField]
    AudioSource testSoundEffects;
    [SerializeField]
    AudioControls musicControls;
    [SerializeField]
    AudioControls soundEffectsControls;

    GameSettings settings = null;
    BackgroundMusic musicSettings = null;
    bool inSetupMode = false;

    System.Action<OptionsMenu> hideAction = null;

    public bool IsVisible
    {
        get
        {
            return optionsPanel.activeSelf;
        }
    }

    public override void SingletonStart(Singleton instance)
    {
        // Do nothing
    }

    public override void SceneStart(Singleton instance)
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

    public void Show(System.Action<OptionsMenu> returnAction = null)
    {
        if (IsVisible == false)
        {
            // Make the game object active
            optionsPanel.SetActive(true);

            // Setup the next action
            hideAction = returnAction;
        }
    }

    public void Hide()
    {
        if (IsVisible == true)
        {
            // Make the game object inactive
            optionsPanel.SetActive(false);

            // Run the last action
            if(hideAction != null)
            {
                hideAction(this);
            }
        }
    }

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
            musicControls.volumePercentLabel.text = Percent(SoundEffect.GlobalVolume);

            testSoundEffects.volume = SoundEffect.GlobalVolume;
            testSoundEffects.Play();
        }
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

            // Play sound effect if not muted
            if (SoundEffect.GlobalMute == false)
            {
                testSoundEffects.Play();
            }
        }
    }

    static string Percent(float val)
    {
        //float volumePercent = (val * 100);
        //return volumePercent.ToString("%");
        return val.ToString("%");
    }
}
