using System;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="VersionGeneratorArgs.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2017 Omiya Games
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
    /// Arguments providing a collection of <code>ISettingsVersionGenerator</code>,
    /// sorted by version number in ascending order.
    /// </summary>
    /// <seealso cref="GameSettings"/>
    /// <seealso cref="ISettingsVersionGenerator"/>
    public class VersionGeneratorArgs : EventArgs, IEnumerable<ISettingsVersionGenerator>
    {
        readonly SortedDictionary<int, ISettingsVersionGenerator> allVersions = new SortedDictionary<int, ISettingsVersionGenerator>();

        public bool AddVersion(ISettingsVersionGenerator version, out string errorMessage)
        {
            // Setup default return variables
            bool returnFlag = true;
            errorMessage = null;

            // Check if the version is in the dictionary
            if(version == null)
            {
                // If so, provide an error message
                returnFlag = false;
                errorMessage = "No version provided.";
            }
            else if (allVersions.ContainsKey(version.Version) == false)
            {
                // If not, add this version in the dictionary
                allVersions.Add(version.Version, version);
            }
            else
            {
                // If so, provide an error message
                returnFlag = false;
                errorMessage = "Version \"" + version.Version + "\" is already in the arguments.";
            }
            return returnFlag;
        }

        public IEnumerator<ISettingsVersionGenerator> GetEnumerator()
        {
            return allVersions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return allVersions.Values.GetEnumerator();
        }
    }
}
