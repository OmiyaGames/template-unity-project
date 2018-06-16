using System;
using System.IO;
using System.Text;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GeneratorDecorator.cs" company="Omiya Games">
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
    /// Helper class that has a couple of built-in stuff to implement a <see cref="IGenerator"/> more easily.
    /// </summary>
    public class GeneratePropertyEventArgs : EventArgs
    {
        public readonly TextWriter writer;
        public readonly string instanceName;
        public readonly int versionArrayIndex;

        public int numTabs
        {
            get;
            set;
        }

        public GeneratePropertyEventArgs(TextWriter writer, int numTabs, string instanceName, int versionArrayIndex)
        {
            this.writer = writer;
            this.numTabs = numTabs;
            this.instanceName = instanceName;
            this.versionArrayIndex = versionArrayIndex;
        }

        public void WriteTabs()
        {
            // Write tabs
            GameSettingsGenerator.WriteTabs(writer, numTabs);
        }

        public void WriteSingleLine(bool withReturn, string variableOrFunction)
        {
            // Write return
            if(withReturn == true)
            {
                writer.Write("return ");
            }
            writer.Write(instanceName);
            writer.Write('.');
            writer.Write(variableOrFunction);
            writer.WriteLine(';');
        }
    }
}
