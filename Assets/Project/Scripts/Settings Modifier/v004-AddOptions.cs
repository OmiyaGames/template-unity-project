using OmiyaGames.Settings;

namespace LudumDare38
{
    /// <summary>
    /// Adds Options settings to <see cref="GameSettings"/>.
    /// </summary>
    public class AddOptions : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 4;

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
                /////////////////////////////////////////////////////
                // Keyboard Stuff
                /////////////////////////////////////////////////////
                new StoredBoolGenerator("Split Keyboard Axis", false)
                {
                    PropertyName = "IsKeyboardAxisSensitivitySplit",
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, splits the keyboard's X- and Y-axis' sensitivity"
                    }
                },
                new StoredFloatGenerator("Keyboard X-Axis Sensitivity", 0.5f)
                {
                    Processor = Clamp<float>.Get(0, 1),
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "The keyboard's X-axis' sensitivity.",
                        "A value between 0 and 1."
                    }
                },
                new StoredFloatGenerator("Keyboard Y-Axis Sensitivity", 0.5f)
                {
                    Processor = Clamp<float>.Get(0, 1),
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "The keyboard's Y-axis' sensitivity.",
                        "A value between 0 and 1.",
                        "This value isn't used if <see cref=\"IsKeyboardAxisSensitivitySplit\"/> is false."
                    }
                },
                new StoredBoolGenerator("Keyboard X-Axis is Inverted", false)
                {
                    PropertyName = "IsKeyboardXAxisInverted",
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, inverts the keyboard's X-axis."
                    }
                },
                new StoredBoolGenerator("Keyboard Y-Axis is Inverted", false)
                {
                    PropertyName = "IsKeyboardYAxisInverted",
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, inverts the keyboard's Y-axis."
                    }
                },
                /////////////////////////////////////////////////////
                // Mouse Stuff
                /////////////////////////////////////////////////////
                new StoredBoolGenerator("Split Mouse Axis", false)
                {
                    PropertyName = "IsMouseAxisSensitivitySplit",
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, splits the mouse's X- and Y-axis' sensitivity"
                    }
                },
                new StoredFloatGenerator("Mouse X-Axis Sensitivity", 0.5f)
                {
                    Processor = Clamp<float>.Get(0, 1),
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "The mouse's X-axis' sensitivity.",
                        "A value between 0 and 1."
                    }
                },
                new StoredFloatGenerator("Mouse Y-Axis Sensitivity", 0.5f)
                {
                    Processor = Clamp<float>.Get(0, 1),
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "The keyboard's Y-axis' sensitivity.",
                        "A value between 0 and 1.",
                        "This value isn't used if <see cref=\"IsKeyboardAxisSensitivitySplit\"/> is false."
                    }
                },
                new StoredBoolGenerator("Mouse X-Axis is Inverted", false)
                {
                    PropertyName = "IsMouseXAxisInverted",
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, inverts the keyboard's X-axis."
                    }
                },
                new StoredBoolGenerator("Mouse Y-Axis is Inverted", false)
                {
                    PropertyName = "IsMouseYAxisInverted",
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, inverts the keyboard's Y-axis."
                    }
                },
                /////////////////////////////////////////////////////
                // Scroll Wheel Stuff
                /////////////////////////////////////////////////////
                new StoredFloatGenerator("Scroll Wheel Sensitivity", 0.5f)
                {
                    Processor = Clamp<float>.Get(0, 1),
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "The mouse' scroll wheel's sensitivity.",
                        "A value between 0 and 1."
                    }
                },
                new StoredBoolGenerator("Scroll Wheel is Inverted", false)
                {
                    PropertyName = "IsScrollWheelInverted",
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, inverts the mouse' scroll wheel."
                    }
                },
                /////////////////////////////////////////////////////
                // Graphics Stuff
                /////////////////////////////////////////////////////
                new StoredBoolGenerator("Is Smooth Camera Enabled", false)
                {
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, enables smooth camera controls."
                    }
                },
                new StoredBoolGenerator("Is Bobbing Camera Enabled", false)
                {
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, enables bobbing camera effect."
                    }
                },
                new StoredBoolGenerator("Is Flashes Enabled", true)
                {
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, enables flashing graphic effects."
                    }
                },
                new StoredBoolGenerator("Is Motion Blurs Enabled", true)
                {
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, enables motion blur graphic effects."
                    }
                },
                new StoredBoolGenerator("Is Bloom Enabled", true)
                {
                    SetterScope = AccessModifier.Internal,
                    TooltipDocumentation = new string[]
                    {
                        "If true, enables bloom graphic effects."
                    }
                },
            };
        }
    }
}
