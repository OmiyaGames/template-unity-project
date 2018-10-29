using OmiyaGames.Settings;

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
    /// Adds high score settings to <see cref="GameSettings"/>.
    /// </summary>
    public class AddHighScores : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 3;
        public const int MaxListSize = 20;
        const string HighScoresPropertyName = "HighScores";

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
            new SortedRecordSettingGenerator<int>("Local High Scores", new SortedIntRecords(MaxListSize, true))
            {
                PropertyName = HighScoresPropertyName,
                TooltipDocumentation = new string[]
                {
                    "List of highest scores"
                },
            },
            new PropertyGenerator("TopScore", typeof(IRecord<int>))
            {
                GetterCode = GeneratorDecorator.CreatePropertyWriter(HighScoresPropertyName, "TopRecord"),
                TooltipDocumentation = new string[]
                {
                    "Gets the top score from <seealso cref=\"" + HighScoresPropertyName + "\"/>"
                },
            },
            new StoredStringGenerator("Last Entered Name", string.Empty)
            {
                IsValueRetainedOnClear = true,
                TooltipDocumentation = new string[]
                {
                    "The name the player entered last time they got a new high score.",
                    "Used as a convenience feature for players to enter their name",
                    "more quickly on repeated playthroughs."
                },
            }
            };
        }
    }
}
