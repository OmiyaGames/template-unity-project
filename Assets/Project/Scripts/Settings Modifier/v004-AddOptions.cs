using OmiyaGames.Settings;

namespace Project.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsGraphicsMenu.cs" company="Omiya Games">
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
    /// <date>6/15/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Adds Options settings to <see cref="GameSettings"/>.
    /// </summary>
    public class AddOptions : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 4;
        public const float DefaultCameraSmoothFactor = 0.25f;
        public const float DefaultScale = 1f;
        public const float DefaultSensitivity = 0.5f;

        const string CameraSmoothFactorPropertyName = "CameraSmoothFactor";
        const string IsHeadingBobbingEnabledPropertyName = "IsHeadBobbingEnabled";
        const string CustomTimeScalePropertyName = "CustomTimeScale";

        /////////////////////////////////////////////////////
        // Mouse Stuff
        /////////////////////////////////////////////////////
        static readonly StoredFloatGenerator SmoothCameraFactorOptionProperty = new StoredFloatGenerator("Smooth Camera Factor Option", DefaultCameraSmoothFactor)
        {
            Processor = Clamp<float>.Get(0, 1),
            GetterScope = AccessModifier.Internal,
            SetterScope = AccessModifier.Internal,
            TooltipDocumentation = new string[]
                {
                    "The smoothing factor for making the camera follow the mouse movement.",
                    "A value between 0 and 1.",
                    "The lower the value, the more tightly it tracks the mouse movement."
                }
        };
        static readonly StoredBoolGenerator IsSmoothCameraEnabledProperty = new StoredBoolGenerator("Is Smooth Camera Enabled", false)
        {
            SetterScope = AccessModifier.Internal,
            TooltipDocumentation = new string[]
                {
                    "If true, enables smooth camera controls."
                }
        };

        /////////////////////////////////////////////////////
        // Graphics Stuff
        /////////////////////////////////////////////////////
        static readonly StoredBoolGenerator IsCameraShakesEnabledProperty = new StoredBoolGenerator("Is Camera Shakes Enabled", true)
        {
            SetterScope = AccessModifier.Internal,
            TooltipDocumentation = new string[]
                {
                    "If true, enables bloom graphic effects."
                }
        };
        static readonly StoredBoolGenerator IsHeadBobbingOptionEnabledProperty = new StoredBoolGenerator("Is Head Bobbing Option Enabled", false)
        {
            SetterScope = AccessModifier.Internal,
            GetterScope = AccessModifier.Internal,
            TooltipDocumentation = new string[]
                {
                    "The stored value for the head bobbing checkbox in the Graphics options menu."
                }
        };

        /////////////////////////////////////////////////////
        // Accessibility Stuff
        /////////////////////////////////////////////////////
        static readonly StoredFloatGenerator CustomTimeScaleOptionProperty = new StoredFloatGenerator("Custom Time Scale Option", DefaultScale)
        {
            Processor = Clamp<float>.Get(0, 2),
            GetterScope = AccessModifier.Internal,
            SetterScope = AccessModifier.Internal,
            TooltipDocumentation = new string[]
                {
                    "The smoothing factor for making the camera follow the mouse movement.",
                    "A value between 0 and 1.",
                    "The lower the value, the more tightly it tracks the mouse movement."
                }
        };
        static readonly StoredBoolGenerator IsCustomTimeScaleEnabledProperty = new StoredBoolGenerator("Is Custom Time Scale Enabled", false)
        {
            SetterScope = AccessModifier.Internal,
            TooltipDocumentation = new string[]
                {
                    "If true, enables smooth camera controls."
                }
        };

        #region Properties
        public override ushort Version
        {
            get
            {
                return AppVersion;
            }
        }

        /// <summary>
        /// Provides Camera Smooth Factor.  Returns a negative value if not set
        /// </summary>
        public static float CameraSmoothFactor
        {
            get
            {
                float returnSmoothFactor = -1f;
                if (IsSmoothCameraEnabledProperty.Value == true)
                {
                    returnSmoothFactor = SmoothCameraFactorOptionProperty.Value;
                }
                return returnSmoothFactor;
            }
        }

        public static bool IsHeadBobbingEnabled
        {
            get
            {
                return IsCameraShakesEnabledProperty.Value && IsHeadBobbingOptionEnabledProperty.Value;
            }
        }

        public static float CustomTimeScale
        {
            get
            {
                float returnScale = DefaultScale;
                if (IsCustomTimeScaleEnabledProperty.Value == true)
                {
                    returnScale = CustomTimeScaleOptionProperty.Value;
                }
                return returnScale;
            }
        }

        static string ClassName
        {
            get
            {
                return typeof(AddOptions).FullName;
            }
        }
        #endregion

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
            new StoredFloatGenerator("Keyboard X-Axis Sensitivity", DefaultSensitivity)
            {
                Processor = Clamp<float>.Get(0, 1),
                SetterScope = AccessModifier.Internal,
                TooltipDocumentation = new string[]
                {
                    "The keyboard's X-axis' sensitivity.",
                    "A value between 0 and 1."
                }
            },
            new StoredFloatGenerator("Keyboard Y-Axis Sensitivity", DefaultSensitivity)
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
            new StoredFloatGenerator("Mouse X-Axis Sensitivity", DefaultSensitivity)
            {
                Processor = Clamp<float>.Get(0, 1),
                SetterScope = AccessModifier.Internal,
                TooltipDocumentation = new string[]
                {
                    "The mouse's X-axis' sensitivity.",
                    "A value between 0 and 1."
                }
            },
            new StoredFloatGenerator("Mouse Y-Axis Sensitivity", DefaultSensitivity)
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
            SmoothCameraFactorOptionProperty,
            IsSmoothCameraEnabledProperty,
            new PropertyGenerator(CameraSmoothFactorPropertyName, typeof(float))
            {
                GetterCode = GeneratorDecorator.CreatePropertyWriter(ClassName, CameraSmoothFactorPropertyName),
                TooltipDocumentation = new string[]
                {
                    "The amount to apply the camera smoothing. Zero indicates instant-snapping to mouse."
                },
            },
            /////////////////////////////////////////////////////
            // Scroll Wheel Stuff
            /////////////////////////////////////////////////////
            new StoredFloatGenerator("Scroll Wheel Sensitivity", DefaultSensitivity)
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
            IsCameraShakesEnabledProperty,
            IsHeadBobbingOptionEnabledProperty,
            new PropertyGenerator(IsHeadingBobbingEnabledPropertyName, typeof(bool))
            {
                GetterCode = GeneratorDecorator.CreatePropertyWriter(ClassName, IsHeadingBobbingEnabledPropertyName),
                TooltipDocumentation = new string[]
                {
                    "If true, enables head bobbing camera effect."
                },
            },
            new StoredBoolGenerator("Is Screen Flashes Enabled", true)
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
            new StoredBoolGenerator("Is Bloom Effect Enabled", true)
            {
                SetterScope = AccessModifier.Internal,
                TooltipDocumentation = new string[]
                {
                    "If true, enables bloom graphic effects."
                }
            },
            /////////////////////////////////////////////////////
            // Accessibility Stuff
            /////////////////////////////////////////////////////
            new StoredFloatGenerator("Text Size Multiplier", DefaultScale)
            {
                Processor = Clamp<float>.Get(0.5f, 1.5f),
                SetterScope = AccessModifier.Internal,
                TooltipDocumentation = new string[]
                {
                    "Multiplier on how much the font size of a text should change."
                }
            },
            CustomTimeScaleOptionProperty,
            IsCustomTimeScaleEnabledProperty,
            new PropertyGenerator(CustomTimeScalePropertyName, typeof(float))
            {
                GetterCode = GeneratorDecorator.CreatePropertyWriter(ClassName, CustomTimeScalePropertyName),
                TooltipDocumentation = new string[]
                {
                    "The default global time scale for the game."
                },
            },
            };
        }
    }
}
