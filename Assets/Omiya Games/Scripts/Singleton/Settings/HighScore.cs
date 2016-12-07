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
    public struct HighScore
    {
        static readonly StringBuilder builder = new StringBuilder();
        public const char Divider = ',';

        public readonly int score;
        public readonly int appVersion;
        public readonly string name;
        public readonly DateTime dateAchievedUtc;
        public readonly string record;

        public HighScore(string pastRecord)
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
            if (int.TryParse(info[index], out appVersion) == false)
            {
                throw new ArgumentException("Could not parse the version number from pastRecord: " + pastRecord);
            }

            // TODO: decrypt the rest of the information based on app version
            // Grab score
            ++index;
            if (int.TryParse(info[index], out score) == false)
            {
                throw new ArgumentException("Could not parse the score from pastRecord: " + pastRecord);
            }

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

            // Hold the record
            record = pastRecord;
        }

        public HighScore(int newRecord, string newName)
        {
            // Setup all the member variables
            score = newRecord;
            appVersion = GameSettings.AppVersion;
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

            // Create a record out of this information
            builder.Length = 0;

            // TODO: check the app version before recording everything else
            // Add app version
            builder.Append(appVersion);
            builder.Append(Divider);

            // Add score
            builder.Append(score);
            builder.Append(Divider);

            // Add name
            builder.Append(name);
            builder.Append(Divider);

            // Add date of record in UTC
            builder.Append(dateAchievedUtc.Ticks);

            // Store this achievement as a string record
            record = builder.ToString();
        }

        public override string ToString()
        {
            return record;
        }
    }
}
