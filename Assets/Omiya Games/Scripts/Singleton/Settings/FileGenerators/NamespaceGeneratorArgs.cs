using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="NamespaceGeneratorArgs.cs" company="Omiya Games">
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
    /// Arguments providing a collection of Namespaces.
    /// </summary>
    /// <seealso cref="GameSettings"/>
    public class NamespaceGeneratorArgs : EventArgs, IEnumerable<string>
    {
        static readonly Regex namespaceTester = new Regex("^[a-zA-Z][a-zA-Z0-9]*(\\.[a-zA-Z][a-zA-Z0-9]*)*$");

        static readonly HashSet<string> namespacesToExclude = new HashSet<string>()
        {
            "OmiyaGames",
            "OmiyaGames.Settings"
        };

        readonly HashSet<string> allNamespaces = new HashSet<string>();

        public bool IsNamespaceValid(string namespaceName, out string errorMessage)
        {
            // Setup return variables
            bool returnFlag = true;
            errorMessage = null;

            // Test the namespace
            if(namespaceTester.IsMatch(namespaceName) == false)
            {
                returnFlag = false;
                errorMessage = "Illegal characters in namespace \"" + namespaceName + "\".";
            }
            else if(namespacesToExclude.Contains(namespaceName) == true)
            {
                returnFlag = false;
                errorMessage = "Namespace \"" + namespaceName + "\" is reserved, and cannot be added to the arguments.";
            }
            return returnFlag;
        }

        public bool AddNamespace(string namespaceName, out string errorMessage)
        {
            bool returnFlag = false;
            if(IsNamespaceValid(namespaceName, out errorMessage) == true)
            {
                if (allNamespaces.Contains(namespaceName) == false)
                {
                    returnFlag = allNamespaces.Add(namespaceName);
                }
                else
                {
                    errorMessage = "Namespace \"" + namespaceName + "\" is already in the arguments.";
                }
            }
            return returnFlag;
        }

        public bool RemoveNamespace(string namespaceName, out string errorMessage)
        {
            bool returnFlag = false;
            if (IsNamespaceValid(namespaceName, out errorMessage) == true)
            {
                if (allNamespaces.Contains(namespaceName) == true)
                {
                    returnFlag = allNamespaces.Remove(namespaceName);
                }
                else
                {
                    errorMessage = "Namespace \"" + namespaceName + "\" is not in the arguments.";
                }
            }
            return returnFlag;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return allNamespaces.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return allNamespaces.GetEnumerator();
        }
    }
}