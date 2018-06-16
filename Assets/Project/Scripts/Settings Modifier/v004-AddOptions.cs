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
        const string CameraShakePropertyName = "IsCameraShakesEnabled";
        const string HeadBobbingOptionPropertyName = "IsHeadBobbingOptionEnabled";

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
            new StoredFloatGenerator("Smooth Camera Factor", 0.25f)
            {
                Processor = Clamp<float>.Get(0, 1),
                SetterScope = AccessModifier.Internal,
                TooltipDocumentation = new string[]
                {
                    "The smoothing factor for making the camera follow the mouse movement.",
                    "A value between 0 and 1.",
                    "The lower the value, the more tightly it tracks the mouse movement."
                }
            },
            new StoredBoolGenerator("Is Smooth Camera Enabled", false)
            {
                SetterScope = AccessModifier.Internal,
                TooltipDocumentation = new string[]
                {
                    "If true, enables smooth camera controls."
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
            new StoredBoolGenerator("Is Camera Shakes Enabled", true)
            {
                PropertyName = CameraShakePropertyName,
                SetterScope = AccessModifier.Internal,
                TooltipDocumentation = new string[]
                {
                    "If true, enables bloom graphic effects."
                }
            },
            new StoredBoolGenerator("Is Head Bobbing Option Enabled", false)
            {
                PropertyName = HeadBobbingOptionPropertyName,
                SetterScope = AccessModifier.Internal,
                TooltipDocumentation = new string[]
                {
                    "The stored value for the head bobbing checkbox in the Graphics options menu."
                }
            },
            new PropertyGenerator("IsHeadBobbingEnabled", typeof(bool))
            {
                GetterCode = WriteHeadBobbingGetter,
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
            };
        }

        private void WriteHeadBobbingGetter(GeneratorDecorator source, GeneratePropertyEventArgs args)
        {
            if (args != null)
            {
                args.WriteTabs();
                args.writer.Write("return ");
                args.writer.Write(CameraShakePropertyName);
                args.writer.Write(" && ");
                args.writer.Write(HeadBobbingOptionPropertyName);
                args.writer.Write(';');
                args.writer.WriteLine();
            }
        }
    }
}
