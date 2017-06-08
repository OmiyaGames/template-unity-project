namespace OmiyaGames.Settings
{
    /// <summary>
    /// Adds local analytics (like how long the player played,
    /// how many levels they have unlocked, etc.) to
    /// <see cref="GameSettings"/>.
    /// </summary>
    public class AddLocalAnalytics : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 1;
        public const int DefaultNumLevelsUnlocked = 1;

        public override ushort Version
        {
            get
            {
                return AppVersion;
            }
        }

        protected override string[] GetKeysToRemove()
        {
            // Do nothing!
            return null;
        }

        protected override IGenerator[] GetNewGenerators()
        {
            // Return an array
            return new IGenerator[]
            {
                new StoredIntGenerator("Number of Unlocked Levels", DefaultNumLevelsUnlocked)
                {
                    Processor = MinCap<int>.Get(DefaultNumLevelsUnlocked),
                    PropertyName = "NumLevelsUnlocked",
                    TooltipDocumentation = new string[]
                    {
                        "The number of levels unlocked."
                    },
                },
                new PropertyGenerator("DefaultNumLevelsUnlocked", typeof(int))
                {
                    GetterCode = "return AddLocalAnalytics.DefaultNumLevelsUnlocked;",
                    TooltipDocumentation = new string[]
                    {
                        "Default number of levels unlocked."
                    },
                },
                new StoredIntGenerator("Number of Times App Open", 0)
                {
                    IsValueRetainedOnClear = true,
                    Processor = MinCap<int>.Get(0),
                    PropertyName = "NumberOfTimesAppOpened",
                    TooltipDocumentation = new string[]
                    {
                        "The number of times the player opened this game."
                    },
                },
                new StoredPlayTimeGenerator("Total Play Time", "TotalPlayTime")
                {
                    TooltipDocumentation = new string[]
                    {
                        "How long the player played this game."
                    },
                },
             };
        }
    }
}
