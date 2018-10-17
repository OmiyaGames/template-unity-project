namespace OmiyaGames.Settings
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
