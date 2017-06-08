namespace OmiyaGames.Settings
{
    /// <summary>
    /// Adds audio settings to <see cref="GameSettings"/>.
    /// </summary>
    public class AddAudioSettings : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 0;

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
            return new IGenerator[]
            {
                new StoredFloatGenerator("Music Volume", 0.7f)
                {
                    IsValueRetainedOnClear = true,
                    Processor = Clamp<float>.Get(0, 1),
                    GetterScope = AccessModifier.Internal,
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "The stored music volume, between 0 and 1."
                    }
                },
                new StoredFloatGenerator("Sound Volume", 0.75f)
                {
                    IsValueRetainedOnClear = true,
                    Processor = Clamp<float>.Get(0, 1),
                    GetterScope = AccessModifier.Internal,
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "The stored sound volume, between 0 and 1."
                    }
                },
                new StoredBoolGenerator("Music Muted", false)
                {
                    PropertyName = "IsMusicMuted",
                    IsValueRetainedOnClear = true,
                    GetterScope = AccessModifier.Internal,
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "Whether the music is muted or not."
                    }
                },
                new StoredBoolGenerator("Sound Muted", false)
                {
                    PropertyName = "IsSoundMuted",
                    IsValueRetainedOnClear = true,
                    GetterScope = AccessModifier.Internal,
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "Whether the sound is muted or not."
                    }
                },
            };
        }
    }
}
