using System;
using OmiyaGames.Settings;

namespace LudumDare38
{
    /// <summary>
    /// Adds high score settings to <see cref="GameSettings"/>.
    /// </summary>
    public class AddHighScores : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 3;
        public const int MaxListSize = 10;

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
                    PropertyName = "HighScores",
                    TooltipDocumentation = new string[]
                    {
                        "List of highest scores"
                    },
                },
                new PropertyGenerator("TopScore", typeof(IRecord<int>))
                {
                    GetterCode = "return HighScores.TopRecord;",
                    TooltipDocumentation = new string[]
                    {
                        "Gets the top score from <seealso cref=\"HighScores\"/>"
                    },
                },
                new SortedRecordSettingGenerator<float>("Local Best Times", new SortedFloatRecords(MaxListSize, true, ParseDuration))
                {
                    PropertyName = "BestSurvivalTimes",
                    TooltipDocumentation = new string[]
                    {
                        "List of longest survival times"
                    },
                },
                new PropertyGenerator("TopSurvivalTime", typeof(IRecord<float>))
                {
                    GetterCode = "return BestSurvivalTimes.TopRecord;",
                    TooltipDocumentation = new string[]
                    {
                        "Gets the top time from <seealso cref=\"BestSurvivalTimes\"/>"
                    },
                },
                new StoredStringGenerator("Last Entered Name", string.Empty)
                {
                    TooltipDocumentation = new string[]
                    {
                        "The name the player entered last time they got a new high score.",
                        "Used as a convenience feature for players to enter their name",
                        "more quickly on repeated playthroughs."
                    },
                }
            };
        }

        private bool ParseDuration(string record, int appVersion, out float newRecord)
        {
            bool recordingSuccessful = false;
            newRecord = 0f;

            if (appVersion <= 0)
            {
                TimeSpan spanOfTime;
                if (TimeSpan.TryParse(record, out spanOfTime) == true)
                {
                    newRecord = (float)spanOfTime.TotalSeconds;
                    recordingSuccessful = true;
                }
            }
            return recordingSuccessful;
        }
    }
}
