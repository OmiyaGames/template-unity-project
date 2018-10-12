// Implementation based on script found at http://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/ 
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="CsvReader.cs" company="Omiya Games">
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
    /// <date>6/1/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Reads a CSV file to generate a dictionary.
    /// </summary>
    /// <seealso cref="TranslatedTextMeshPro"/>
    public static class CsvReader
    {
        const string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static readonly char[] TRIM_CHARS = { '\"' };

        public class ReadStatus
        {
            public enum State
            {
                ReadingFileIntoRows,
                ReadingRowsIntoCells,
                NumberOfStates
            }

            ThreadSafe<State> currentStatus = new ThreadSafe<State>();
            ProgressReport progressReport = new ProgressReport();

            public State CurrentState
            {
                get
                {
                    return currentStatus.Value;
                }
                internal set
                {
                    currentStatus.Value = value;
                }
            }

            public long CurrentStep
            {
                get
                {
                    return progressReport.CurrentStep;
                }
                internal set
                {
                    progressReport.CurrentStep = value;
                }
            }

            public long TotalSteps
            {
                get
                {
                    return progressReport.TotalSteps;
                }
            }

            public float ProgressPercent
            {
                get
                {
                    return progressReport.ProgressPercent;
                }
            }

            internal void IncrementStep()
            {
                progressReport.IncrementCurrentStep();
            }

            internal void SetTotalSteps(long newTotalSteps)
            {
                progressReport.SetTotalSteps(newTotalSteps);
            }
        }

        public static List<Dictionary<string, string>> ReadFile(TextAsset data, ReadStatus status = null)
        {
            return ReadText(data.text, status);
        }

        public static List<Dictionary<string, string>> ReadText(string csvText, ReadStatus status = null)
        {
            List<string> lines = null;
            using (StreamReader stringReader = new StreamReader(new FileStream(csvText, FileMode.Open)))
            {
                lines = SplitNewLines(stringReader, status);
            }
            if ((lines == null) || (lines.Count <= 1))
            {
                return GenerateEmptyList(status);
            }
            else
            {
                return GenerateRows(lines, status);
            }
        }

        public static List<Dictionary<string, string>> ReadFile(string filePath, ReadStatus status = null)
        {
            List<string> lines = null;
            using (StreamReader fileReader = new StreamReader(filePath))
            {
                lines = SplitNewLines(fileReader, status);
            }
            if ((lines == null) || (lines.Count <= 1))
            {
                return GenerateEmptyList(status);
            }
            else
            {
                return GenerateRows(lines, status);
            }
        }

        private static List<Dictionary<string, string>> GenerateEmptyList(ReadStatus status)
        {
            // Check if we need to report status
            if (status != null)
            {
                // Move the status to the next phase, and indicate completion
                status.CurrentState = ReadStatus.State.ReadingRowsIntoCells;
                status.SetTotalSteps(1);
                status.CurrentStep = 1;
            }

            // Return empty list
            return new List<Dictionary<string, string>>(0);
        }

        private static List<Dictionary<string, string>> GenerateRows(List<string> lines, ReadStatus status)
        {
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>(lines.Count - 1);
            string[] header = Regex.Split(lines[0], SPLIT_RE);

            // Check if we need to report status
            if (status != null)
            {
                // Reset status
                status.CurrentState = ReadStatus.State.ReadingRowsIntoCells;
                status.SetTotalSteps(lines.Count - 1);
            }

            for (int i = 1; i < lines.Count; ++i)
            {
                string[] values = Regex.Split(lines[i], SPLIT_RE);
                if ((values.Length == 0) || (string.IsNullOrEmpty(values[0]) == true))
                {
                    continue;
                }

                Dictionary<string, string> entry = new Dictionary<string, string>();
                for (int j = 0; (j < header.Length) && (j < values.Length); ++j)
                {
                    string value = values[j];

                    // Remove the first and last quotes
                    value = value.TrimStart(TRIM_CHARS);
                    value = value.TrimEnd(TRIM_CHARS);

                    // Replacing double double-quote with single double-quote
                    value = value.Replace("\"\"", "\"");

                    // Update entry
                    entry[header[j]] = value;
                }
                returnList.Add(entry);

                // Check if we need to report status
                if (status != null)
                {
                    // If so, indicate progress made
                    status.IncrementStep();
                }
            }
            return returnList;
        }

        private static List<string> SplitNewLines(StreamReader reader, ReadStatus status)
        {
            // Setup each variable
            StringBuilder eachLine = new StringBuilder();
            List<string> lines = new List<string>();
            bool isEscapedByQuote = false;
            char readChar;

            // Check if we need to report status
            if (status != null)
            {
                // Reset status
                status.CurrentState = ReadStatus.State.ReadingFileIntoRows;
                status.SetTotalSteps(1);
            }

            // Read the first letter
            int readInt = reader.Read();
            while (readInt >= 0)
            {
                // Convert the byte to a character
                readChar = (char)readInt;

                // Check to see if we hit a newline, not escaped by quotes
                if ((isEscapedByQuote == false) && (readChar == '\n'))
                {
                    // Add a line to the lines list
                    lines.Add(eachLine.ToString());

                    // Reset the string builder
                    eachLine.Length = 0;
                }
                else if (readChar != '\r')
                {
                    // Otherwise, add the character to the the string builder (except '\r')
                    eachLine.Append(readChar);

                    // Check to see if this character is a quote
                    if (readChar == '"')
                    {
                        // Toggle whether the next newline needs to be escaped or not
                        isEscapedByQuote = !isEscapedByQuote;
                    }
                }

                // Read the next byte
                readInt = reader.Read();
            }

            // Check if there's any text left (this assumes document doesn't end in a newline)
            if (eachLine.Length > 0)
            {
                // Add the last line into all lines
                lines.Add(eachLine.ToString());
            }

            // Check if we need to report status
            if (status != null)
            {
                // If so, indicate progress made
                status.IncrementStep();
            }
            return lines;
        }
    }
}
