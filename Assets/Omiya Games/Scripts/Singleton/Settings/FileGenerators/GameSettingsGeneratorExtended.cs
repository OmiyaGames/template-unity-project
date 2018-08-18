using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GameSettingsGenerator.cs" company="Omiya Games">
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
    /// <date>5/17/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A list of helper methods that writes all the
    /// <see cref="ISettingsVersion"/> and the latest
    /// <see cref="IStoredSetting"/> into a single,
    /// readable C# file.
    /// </summary>
    /// <seealso cref="GameSettings"/>
    /// <seealso cref="ISettingsVersion"/>
    public static partial class GameSettingsGenerator
    {
        public const string Tabs = "    ";

        public static void WriteLine(TextWriter writer, int numTabs, string line)
        {
            WriteTabs(writer, numTabs);
            writer.WriteLine(line);
        }

        public static void WriteLine(TextWriter writer, int numTabs, char letter)
        {
            WriteTabs(writer, numTabs);
            writer.WriteLine(letter);

        }

        public static void WriteStartOfLine(TextWriter writer, int numTabs, string line)
        {
            WriteTabs(writer, numTabs);
            writer.Write(line);
        }

        public static void WriteStartOfLine(TextWriter writer, int numTabs, char letter)
        {
            WriteTabs(writer, numTabs);
            writer.Write(letter);
        }

        public static void WriteTabs(TextWriter writer, int numTabs)
        {
            for (int index = 0; index < numTabs; ++index)
            {
                writer.Write(Tabs);
            }
        }
    }
}
