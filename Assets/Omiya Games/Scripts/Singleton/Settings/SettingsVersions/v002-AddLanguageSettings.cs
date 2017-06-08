namespace OmiyaGames.Settings
{
    /// <summary>
    /// Adds language settings to <see cref="GameSettings"/>.
    /// </summary>
    public class AddLanguageSettings : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 2;

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
                new StoredStringGenerator("Language", string.Empty)
                {
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "The user-chosen language"
                    },
                },
            };
        }
    }
}
