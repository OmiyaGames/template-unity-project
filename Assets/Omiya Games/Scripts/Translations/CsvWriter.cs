// Implementation based on http://www.snowcrest.net/donnelly/piglatin.html
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using OmiyaGames.Translations;

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
    /// <date>10/12/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Reads a CSV file to generate a dictionary.
    /// </summary>
    /// <seealso cref="TranslatedTextMeshPro"/>
    public static class CsvWriter
    {
        public const char Divider = ',';
        public const char Literal = '"';
        public const string Newline = "\r\n";
        public const string KeyColumnName = "Keys";

        public static void WriteFile(string filePath, TranslationDictionary data)
        {
            SupportedLanguages languages = data.SupportedLanguages;
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write the key column
                writer.Write(KeyColumnName);

                // Go through all the languages
                for (int index = 0; index < languages.Count; ++index)
                {
                    // Write the divider
                    writer.Write(Divider);

                    // Write the language
                    WriteString(writer, languages[index]);
                }

                // Go through all the translations
                foreach (KeyValuePair<string, TranslationDictionary.LanguageTextMap> translation in data.AllTranslations)
                {
                    // Delimit the header
                    writer.Write(Newline);

                    // Write the key value
                    WriteString(writer, translation.Key);

                    // Go through all the languages
                    for (int index = 0; index < languages.Count; ++index)
                    {
                        if (translation.Value.SupportedLanguages.Contains(index) == true)
                        {
                            // Write the divider
                            writer.Write(Divider);

                            // Write the text
                            WriteString(writer, translation.Value[index]);
                        }
                    }
                }
            }
        }

        private static void WriteString(StreamWriter writer, string data)
        {
            // Make sure the string is available
            if (string.IsNullOrEmpty(data) == false)
            {
                // Check if we need to turn the string into a Literal
                bool delimiterNeeded = false;
                foreach (char letter in data)
                {
                    if (IsDelimiterNeeded(letter) == true)
                    {
                        delimiterNeeded = true;
                        break;
                    }
                }

                // If so, start and end with a double-quote
                if (delimiterNeeded == true)
                {
                    writer.Write(Literal);
                }

                // Write the string into the file
                foreach (char letter in data)
                {
                    WriteString(writer, letter);
                }
                if (delimiterNeeded == true)
                {
                    writer.Write(Literal);
                }
            }
        }

        private static bool IsDelimiterNeeded(char letter)
        {
            switch (letter)
            {
                case Divider:
                case Literal:
                case '\n':
                    return true;
                default:
                    return false;
            }
        }

        private static void WriteString(StreamWriter writer, char letter)
        {
            switch (letter)
            {
                // Ignore these characters
                case '\r':
                case '\0':
                    break;

                // If double-quote encountered, plop it twice
                case Literal:
                    writer.Write("\"\"");
                    break;

                // If newline encountered, set to CRLF
                case '\n':
                    writer.Write(Newline);
                    break;

                // Otherwise, just draw the letter itself
                default:
                    writer.Write(letter);
                    break;
            }
        }
    }
}
