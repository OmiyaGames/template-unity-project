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
    /// Adds local analytics (like how long the player played,
    /// how many levels they have unlocked, etc.) to
    /// <see cref="GameSettings"/>.
    /// </summary>
    public class AddLocalAnalytics : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 1;
        public const int DefaultNumLevelsUnlocked = 1;
        public const string DefaultNumLevelsUnlockedName = "DefaultNumLevelsUnlocked";

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
                new PropertyGenerator(DefaultNumLevelsUnlockedName, typeof(int))
                {
                    GetterCode = GeneratorDecorator.CreatePropertyWriter(typeof(AddLocalAnalytics).Name, DefaultNumLevelsUnlockedName),
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
