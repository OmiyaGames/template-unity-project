namespace OmiyaGames.Settings
{
    /// <summary>
    /// This code is auto-generated. All changes will be overwritten!
    /// </summary>
    public partial class GameSettings : ISingletonScript
    {
        #region Private Arrays
        /// <summary>
        /// Array of all the <see cref="ISettingsVersion"/> detected in this project.
        /// Used as reference in the properties.
        /// </summary>
        private readonly ISettingsVersion[] AllSettingsVersions = new ISettingsVersion[]
        {
            new OmiyaGames.Settings.AddAudioSettings(),
            new OmiyaGames.Settings.AddLocalAnalytics(),
            new OmiyaGames.Settings.AddLanguageSettings(),
            new LudumDare38.AddHighScores(),
            new LudumDare38.AddOptions(),
        };

        /// <summary>
        /// Array cache used by <see cref="AllSingleSettings"/>.
        /// </summary>
        private IStoredSetting[] allSingleSettings = null;

        /// <summary>
        /// Array of <see cref="IStoredSetting"/> that has a Property in this class.
        /// Used for collective saving and retrieval of settings.
        /// </summary>
        private IStoredSetting[] AllSingleSettings
        {
            get
            {
                if(allSingleSettings == null)
                {
                    allSingleSettings = new IStoredSetting[]
                    {
                        #region ISingleSettings from version 0
                        AllSettingsVersions[0].GetSetting("Music Volume"),
                        AllSettingsVersions[0].GetSetting("Sound Volume"),
                        AllSettingsVersions[0].GetSetting("Music Muted"),
                        AllSettingsVersions[0].GetSetting("Sound Muted"),
                        #endregion

                        #region ISingleSettings from version 1
                        AllSettingsVersions[1].GetSetting("Number of Unlocked Levels"),
                        AllSettingsVersions[1].GetSetting("Number of Times App Open"),
                        AllSettingsVersions[1].GetSetting("Total Play Time"),
                        #endregion

                        #region ISingleSettings from version 2
                        AllSettingsVersions[2].GetSetting("Language"),
                        #endregion

                        #region ISingleSettings from version 3
                        AllSettingsVersions[3].GetSetting("Local High Scores"),
                        AllSettingsVersions[3].GetSetting("Local Best Times"),
                        AllSettingsVersions[3].GetSetting("Last Entered Name"),
                        #endregion

                        #region ISingleSettings from version 4
                        AllSettingsVersions[4].GetSetting("Split Keyboard Axis"),
                        AllSettingsVersions[4].GetSetting("Keyboard X-Axis Sensitivity"),
                        AllSettingsVersions[4].GetSetting("Keyboard Y-Axis Sensitivity"),
                        AllSettingsVersions[4].GetSetting("Keyboard X-Axis is Inverted"),
                        AllSettingsVersions[4].GetSetting("Keyboard Y-Axis is Inverted"),
                        AllSettingsVersions[4].GetSetting("Split Mouse Axis"),
                        AllSettingsVersions[4].GetSetting("Mouse X-Axis Sensitivity"),
                        AllSettingsVersions[4].GetSetting("Mouse Y-Axis Sensitivity"),
                        AllSettingsVersions[4].GetSetting("Mouse X-Axis is Inverted"),
                        AllSettingsVersions[4].GetSetting("Mouse Y-Axis is Inverted"),
                        AllSettingsVersions[4].GetSetting("Scroll Wheel Sensitivity"),
                        AllSettingsVersions[4].GetSetting("Scroll Wheel is Inverted"),
                        AllSettingsVersions[4].GetSetting("Is Smooth Camera Enabled"),
                        AllSettingsVersions[4].GetSetting("Is Bobbing Camera Enabled"),
                        AllSettingsVersions[4].GetSetting("Is Flashes Enabled"),
                        AllSettingsVersions[4].GetSetting("Is Motion Blurs Enabled"),
                        AllSettingsVersions[4].GetSetting("Is Bloom Enabled"),
                        #endregion
                    };
                }
                return allSingleSettings;
            }
        }
        #endregion

        /// <summary>
        /// The latest version number stored in settings.
        /// This is the size of <see cref="AllSettingsVersions"/>
        /// </summary>
        public int AppVersion
        {
            get
            {
                return AllSettingsVersions.Length;
            }
        }

        #region Properties from AppVersion 0
        /// <summary>
        /// The stored music volume, between 0 and 1.
        /// </summary>
        internal float MusicVolume
        {
            get
            {
                return AllSettingsVersions[0].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Music Volume").Value;
            }
            set
            {
                AllSettingsVersions[0].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Music Volume").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// The stored sound volume, between 0 and 1.
        /// </summary>
        internal float SoundVolume
        {
            get
            {
                return AllSettingsVersions[0].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Sound Volume").Value;
            }
            set
            {
                AllSettingsVersions[0].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Sound Volume").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// Whether the music is muted or not.
        /// </summary>
        internal bool IsMusicMuted
        {
            get
            {
                return AllSettingsVersions[0].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Music Muted").Value;
            }
            set
            {
                AllSettingsVersions[0].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Music Muted").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// Whether the sound is muted or not.
        /// </summary>
        internal bool IsSoundMuted
        {
            get
            {
                return AllSettingsVersions[0].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Sound Muted").Value;
            }
            set
            {
                AllSettingsVersions[0].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Sound Muted").SetValue(value, Settings, AppVersion);
            }
        }
        #endregion

        #region Properties from AppVersion 1
        /// <summary>
        /// The number of levels unlocked.
        /// </summary>
        public int NumLevelsUnlocked
        {
            get
            {
                return AllSettingsVersions[1].GetGenerator<OmiyaGames.Settings.StoredIntGenerator>("Number of Unlocked Levels").Value;
            }
            set
            {
                AllSettingsVersions[1].GetGenerator<OmiyaGames.Settings.StoredIntGenerator>("Number of Unlocked Levels").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// Default number of levels unlocked.
        /// </summary>
        public int DefaultNumLevelsUnlocked
        {
            get
            {
                return AddLocalAnalytics.DefaultNumLevelsUnlocked;
            }
        }

        /// <summary>
        /// The number of times the player opened this game.
        /// </summary>
        public int NumberOfTimesAppOpened
        {
            get
            {
                return AllSettingsVersions[1].GetGenerator<OmiyaGames.Settings.StoredIntGenerator>("Number of Times App Open").Value;
            }
            set
            {
                AllSettingsVersions[1].GetGenerator<OmiyaGames.Settings.StoredIntGenerator>("Number of Times App Open").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// How long the player played this game.
        /// </summary>
        public System.TimeSpan TotalPlayTime
        {
            get
            {
                return AllSettingsVersions[1].GetGenerator<OmiyaGames.Settings.StoredPlayTimeGenerator>("Total Play Time").TotalPlayTime;
            }
        }
        #endregion

        #region Properties from AppVersion 2
        /// <summary>
        /// The user-chosen language
        /// </summary>
        public string Language
        {
            get
            {
                return AllSettingsVersions[2].GetGenerator<OmiyaGames.Settings.StoredStringGenerator>("Language").Value;
            }
            internal set
            {
                AllSettingsVersions[2].GetGenerator<OmiyaGames.Settings.StoredStringGenerator>("Language").SetValue(value, Settings, AppVersion);
            }
        }
        #endregion

        #region Properties from AppVersion 3
        /// <summary>
        /// List of highest scores
        /// </summary>
        public OmiyaGames.Settings.ISortedRecords<int> HighScores
        {
            get
            {
                return AllSettingsVersions[3].GetGenerator<OmiyaGames.Settings.SortedRecordSettingGenerator<int>>("Local High Scores").Value;
            }
        }

        /// <summary>
        /// Gets the top score from <seealso cref="HighScores"/>
        /// </summary>
        public OmiyaGames.Settings.IRecord<int> TopScore
        {
            get
            {
                return HighScores.TopRecord;
            }
        }

        /// <summary>
        /// List of longest survival times
        /// </summary>
        public OmiyaGames.Settings.ISortedRecords<float> BestSurvivalTimes
        {
            get
            {
                return AllSettingsVersions[3].GetGenerator<OmiyaGames.Settings.SortedRecordSettingGenerator<float>>("Local Best Times").Value;
            }
        }

        /// <summary>
        /// Gets the top time from <seealso cref="BestSurvivalTimes"/>
        /// </summary>
        public OmiyaGames.Settings.IRecord<float> TopSurvivalTime
        {
            get
            {
                return BestSurvivalTimes.TopRecord;
            }
        }

        /// <summary>
        /// The name the player entered last time they got a new high score.
        /// Used as a convenience feature for players to enter their name
        /// more quickly on repeated playthroughs.
        /// </summary>
        public string LastEnteredName
        {
            get
            {
                return AllSettingsVersions[3].GetGenerator<OmiyaGames.Settings.StoredStringGenerator>("Last Entered Name").Value;
            }
            set
            {
                AllSettingsVersions[3].GetGenerator<OmiyaGames.Settings.StoredStringGenerator>("Last Entered Name").SetValue(value, Settings, AppVersion);
            }
        }
        #endregion

        #region Properties from AppVersion 4
        /// <summary>
        /// If true, splits the keyboard's X- and Y-axis' sensitivity
        /// </summary>
        public bool IsKeyboardAxisSensitivitySplit
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Split Keyboard Axis").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Split Keyboard Axis").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// The keyboard's X-axis' sensitivity.
        /// A value between 0 and 1.
        /// </summary>
        public float KeyboardXAxisSensitivity
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Keyboard X-Axis Sensitivity").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Keyboard X-Axis Sensitivity").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// The keyboard's Y-axis' sensitivity.
        /// A value between 0 and 1.
        /// This value isn't used if <see cref="IsKeyboardAxisSensitivitySplit"/> is false.
        /// </summary>
        public float KeyboardYAxisSensitivity
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Keyboard Y-Axis Sensitivity").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Keyboard Y-Axis Sensitivity").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, inverts the keyboard's X-axis.
        /// </summary>
        public bool IsKeyboardXAxisInverted
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Keyboard X-Axis is Inverted").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Keyboard X-Axis is Inverted").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, inverts the keyboard's Y-axis.
        /// </summary>
        public bool IsKeyboardYAxisInverted
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Keyboard Y-Axis is Inverted").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Keyboard Y-Axis is Inverted").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, splits the mouse's X- and Y-axis' sensitivity
        /// </summary>
        public bool IsMouseAxisSensitivitySplit
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Split Mouse Axis").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Split Mouse Axis").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// The mouse's X-axis' sensitivity.
        /// A value between 0 and 1.
        /// </summary>
        public float MouseXAxisSensitivity
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Mouse X-Axis Sensitivity").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Mouse X-Axis Sensitivity").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// The keyboard's Y-axis' sensitivity.
        /// A value between 0 and 1.
        /// This value isn't used if <see cref="IsKeyboardAxisSensitivitySplit"/> is false.
        /// </summary>
        public float MouseYAxisSensitivity
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Mouse Y-Axis Sensitivity").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Mouse Y-Axis Sensitivity").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, inverts the keyboard's X-axis.
        /// </summary>
        public bool IsMouseXAxisInverted
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Mouse X-Axis is Inverted").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Mouse X-Axis is Inverted").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, inverts the keyboard's Y-axis.
        /// </summary>
        public bool IsMouseYAxisInverted
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Mouse Y-Axis is Inverted").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Mouse Y-Axis is Inverted").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// The mouse' scroll wheel's sensitivity.
        /// A value between 0 and 1.
        /// </summary>
        public float ScrollWheelSensitivity
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Scroll Wheel Sensitivity").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredFloatGenerator>("Scroll Wheel Sensitivity").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, inverts the mouse' scroll wheel.
        /// </summary>
        public bool IsScrollWheelInverted
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Scroll Wheel is Inverted").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Scroll Wheel is Inverted").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, enables smooth camera controls.
        /// </summary>
        public bool IsSmoothCameraEnabled
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Smooth Camera Enabled").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Smooth Camera Enabled").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, enables bobbing camera effect.
        /// </summary>
        public bool IsBobbingCameraEnabled
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Bobbing Camera Enabled").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Bobbing Camera Enabled").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, enables flashing graphic effects.
        /// </summary>
        public bool IsFlashesEnabled
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Flashes Enabled").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Flashes Enabled").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, enables motion blur graphic effects.
        /// </summary>
        public bool IsMotionBlursEnabled
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Motion Blurs Enabled").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Motion Blurs Enabled").SetValue(value, Settings, AppVersion);
            }
        }

        /// <summary>
        /// If true, enables bloom graphic effects.
        /// </summary>
        public bool IsBloomEnabled
        {
            get
            {
                return AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Bloom Enabled").Value;
            }
            internal set
            {
                AllSettingsVersions[4].GetGenerator<OmiyaGames.Settings.StoredBoolGenerator>("Is Bloom Enabled").SetValue(value, Settings, AppVersion);
            }
        }
        #endregion
    }
}
