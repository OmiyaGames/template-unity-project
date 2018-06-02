// Implementation based on script found at http://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/ 
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AudioFinder.cs" company="Omiya Games">
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
    public static class CSVReader
    {
        const string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static readonly char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, string>> Read(TextAsset data)
        {
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
            List<string> lines = SplitNewLines(data.text);

            if (lines.Count <= 1)
            {
                return returnList;
            }

            string[] header = Regex.Split(lines[0], SPLIT_RE);
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
            }
            return returnList;
        }

        private static List<string> SplitNewLines(string allText)
        {
            // Setup each variable
            StringBuilder eachLine = new StringBuilder();
            List<string> lines = new List<string>();
            int scanTo = allText.Length;
            bool isEscapedByQuote = false;

            for (int i = 0; i < scanTo; ++i)
            {
                // Check to see if we hit a newline, not escaped by quotes
                if ((isEscapedByQuote == false) && (allText[i] == '\n'))
                {
                    // Add a line to the lines list
                    lines.Add(eachLine.ToString());

                    // Reset the string builder
                    eachLine.Length = 0;
                }
                else if (allText[i] != '\r')
                {
                    // Otherwise, add the character to the the string builder (except '\r')
                    eachLine.Append(allText[i]);

                    // Check to see if this character is a quote
                    if (allText[i] == '"')
                    {
                        // Toggle whether the next newline needs to be escaped or not
                        isEscapedByQuote = !isEscapedByQuote;
                    }
                }
            }

            // Check if there's any text left (this assumes document doesn't end in a newline
            if(eachLine.Length > 0)
            {
                // Add the last line into all lines
                lines.Add(eachLine.ToString());
            }
            return lines;
        }
    }
}
