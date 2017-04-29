using System.Text;
using System;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GameSettings.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
    /// <date>12/7/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Stores high score information
    /// </summary>
    /// <seealso cref="GameSettings"/>
    public class HighScore : IRecord<int>
    {
        private int mScore = 0;

        public HighScore(string pastRecord) : base(pastRecord)
        { }

        public HighScore(int newRecord, string newName) : base(newRecord, newName)
        { }

        public int score
        {
            get
            {
                return mScore;
            }
        }

        protected override void ParseRecord(string record, int appVersion)
        {
            // TODO: decrypt the rest of the information based on app version
            if (int.TryParse(record, out mScore) == false)
            {
                throw new ArgumentException("Could not parse the score from pastRecord: " + record);
            }
        }

        protected override void StoreRecord(int record)
        {
            mScore = record;
        }

        protected override string ConvertRecordToString()
        {
            return mScore.ToString();
        }
    }

    public class BestTime : IRecord<float>
    {
        float totalSeconds = 0;

        public BestTime(string pastRecord) : base(pastRecord)
        { }

        public BestTime(float newRecord, string newName) : base(newRecord, newName)
        { }

        public float TotalSeconds
        {
            get
            {
                return totalSeconds;
            }
            private set
            {
                totalSeconds = value;
            }
        }

        protected override void ParseRecord(string record, int appVersion)
        {
            // TODO: decrypt the rest of the information based on app version
            bool recordingSuccessful = false;
            switch(appVersion)
            {
                case 0:
                    TimeSpan spanOfTime;
                    if (TimeSpan.TryParse(record, out spanOfTime) == true)
                    {
                        recordingSuccessful = true;
                        TotalSeconds = (float)spanOfTime.TotalSeconds;
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                case 1:
                default:
                    recordingSuccessful = float.TryParse(record, out totalSeconds);
                    break;
            }
            if(recordingSuccessful == false)
            {
                throw new ArgumentException("Could not parse the score from pastRecord: " + record);
            }
        }

        protected override void StoreRecord(float record)
        {
            totalSeconds = record;
        }

        protected override string ConvertRecordToString()
        {
            return totalSeconds.ToString();
        }
    }

    public abstract class IRecord<T>
    {
        public const char Divider = '|';

        public readonly DateTime dateAchievedUtc;

        private string mName;
        private readonly StringBuilder builder = new StringBuilder();

        public IRecord(string pastRecord)
        {
            if (string.IsNullOrEmpty(pastRecord) == true)
            {
                throw new ArgumentNullException("pastRecord must be provided");
            }

            // Split the information
            string[] info = pastRecord.Split(Divider);
            if ((info == null) || (info.Length <= 0))
            {
                throw new ArgumentException("Could not parse pastRecord: " + pastRecord);
            }

            // Grab app version
            byte index = 0;
            int appVersion;
            if (int.TryParse(info[index], out appVersion) == false)
            {
                throw new ArgumentException("Could not parse the version number from pastRecord: " + pastRecord);
            }

            // Grab score
            ++index;
            ParseRecord(info[index], appVersion);

            // Grab name
            ++index;
            name = info[index];

            // Grab time
            long ticks;
            ++index;
            if (long.TryParse(info[index], out ticks) == true)
            {
                dateAchievedUtc = new DateTime(ticks);
            }
            else
            {
                throw new ArgumentException("Could not parse the date from pastRecord: " + pastRecord);
            }
        }

        public IRecord(T newRecord, string newName)
        {
            // Setup all the member variables
            dateAchievedUtc = DateTime.UtcNow;

            // Record name (don't allow null)
            if (string.IsNullOrEmpty(newName) == false)
            {
                name = newName;
            }
            else
            {
                name = string.Empty;
            }

            // Record record
            StoreRecord(newRecord);
        }

        public string name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        protected abstract void ParseRecord(string record, int appVersion);
        protected abstract void StoreRecord(T record);
        protected abstract string ConvertRecordToString();

        public override string ToString()
        {
            // Create a record out of this information
            builder.Length = 0;

            // Add app version
            builder.Append(GameSettings.AppVersion);
            builder.Append(Divider);

            // Add score
            builder.Append(ConvertRecordToString());
            builder.Append(Divider);

            // Add name
            builder.Append(name);
            builder.Append(Divider);

            // Add date of record in UTC
            builder.Append(dateAchievedUtc.Ticks);

            // Store this achievement as a string record
            return builder.ToString();
        }
    }
}
