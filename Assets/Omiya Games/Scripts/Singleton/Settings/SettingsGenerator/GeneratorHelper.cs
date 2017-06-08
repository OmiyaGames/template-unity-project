using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GeneratorHelper.cs" company="Omiya Games">
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
    /// <date>5/29/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper class containing a series of methods to help generate code.
    /// </summary>
    public static class GeneratorHelper
    {
        /// <summary>
        /// Matches any variable/property name: a non-empty string
        /// that starts with a letter, and the rest contains
        /// letters and/or numbers.
        /// </summary>
        private static readonly Regex propertyNameTester = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$");
        /// <summary>
        /// Matches any variable/property name: a non-empty string
        /// that starts with a letter, and the rest contains
        /// letters, numbers, and/or period.
        /// </summary>
        private static readonly Regex typeNameTester = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*(\.[a-zA-Z][a-zA-Z0-9]*)*$");
        /// <summary>
        /// Matches any settings keys: a non-empty string that
        /// starts with a letter, and the rest contains
        /// letters, spaces, and/or numbers.
        /// </summary>
        private static readonly Regex settingsKeyTester = new Regex(@"^[a-zA-Z][a-zA-Z0-9 \\/-]*$");
        /// <summary>
        /// Matches any text embedded between "`1[" and "]"
        /// (where "1" can be any number).
        /// </summary>
        private static readonly Regex genericTypeTester = new Regex(@"`\d+\[([^\]]+)\]");

        private static readonly HashSet<string> settingsToExclude = new HashSet<string>()
        {
            "OnBeforeRetrieveSettings",
            "OnAfterRetrieveSettings",
            "OnBeforeSaveSettings",
            "OnAfterSaveSettings",
            "OnBeforeClearSettings",
            "OnAfterClearSettings",
            "VersionKey",
            "DefaultSettings",
            "status",
            "DefaultLeaderboardUserScope",
            "Status",
            "Settings",
            "SingletonAwake",
            "SceneAwake",
            "OnApplicationQuit",
            "RetrieveFromSettings",
            "SaveSettings",
            "ClearSettings",
            "AppVersion",
            "AllSettingsVersions",
            "allSingleSettings",
            "AllSingleSettings",
            "isSettingsRetrieved",
        };

        public static string ConvertKeyToPropertyName(string key, out string errorMessage)
        {
            // Setup return variables
            string propertyName = null;
            errorMessage = null;

            // Test the namespace
            if (settingsKeyTester.IsMatch(key) == false)
            {
                errorMessage = "Illegal characters in key \"" + key + "\".";
            }
            else
            {
                // Convert settings key to new key
                propertyName = key.Replace(" ", null);
                propertyName = propertyName.Replace("/", null);
                propertyName = propertyName.Replace("-", null);

                // Make sure this property name isn't excluded
                if (settingsToExclude.Contains(propertyName) == true)
                {
                    propertyName = null;
                    errorMessage = "Property/Function \"" + propertyName + "\" (generated from key \"" + key + "\") is reserved, and cannot be added to the arguments.";
                }
            }
            return propertyName;
        }

        public static bool IsPropertyNameValid(string propertyName, out string errorMessage)
        {
            // Setup return variables
            bool returnFlag = false;
            errorMessage = null;

            // Test the namespace
            if (propertyNameTester.IsMatch(propertyName) == false)
            {
                errorMessage = "Illegal characters in Property/Function \"" + propertyName + "\".";
            }
            else if (settingsToExclude.Contains(propertyName) == true)
            {
                errorMessage = "Property/Function \"" + propertyName + "\" is reserved, and cannot be added to the arguments.";
            }
            else
            {
                returnFlag = true;
            }
            return returnFlag;
        }

        public static bool IsTypeNameValid(string typeName, out string errorMessage)
        {
            // Setup return variables
            bool returnFlag = false;
            errorMessage = null;

            // Test the namespace
            if (typeNameTester.IsMatch(typeName) == false)
            {
                errorMessage = "Illegal characters in Type \"" + typeName + "\".";
            }
            else
            {
                returnFlag = true;
            }
            return returnFlag;
        }

        #region WriteCodeToInstance
        public static void WriteCodeToInstance(IStoredSettingGenerator generator, StreamWriter writer, int versionArrayIndex, bool writeGenerator)
        {
            // Write accessor to an array
            WriteCodeToInstance(writer, versionArrayIndex);

            // Check if we need to include a generic
            if (writeGenerator == true)
            {
                writer.Write(".GetGenerator");
                writer.Write('<');
                writer.Write(CorrectedTypeName(generator.GetType()));
                writer.Write('>');
            }
            else
            {
                writer.Write(".GetSetting");
            }

            // Write parameter
            writer.Write("(\"");
            writer.Write(generator.Key);
            writer.Write("\")");
        }

        public static void WriteCodeToInstance(StreamWriter writer, int versionArrayIndex, Type settingsVersionType)
        {
            // Write type-cast for the array
            writer.Write("((");
            writer.Write(settingsVersionType);
            writer.Write(')');

            // Write array
            WriteCodeToInstance(writer, versionArrayIndex);
            writer.Write(')');
        }

        private static void WriteCodeToInstance(StreamWriter writer, int versionArrayIndex)
        {
            // Write accessor to an array
            writer.Write("AllSettingsVersions[");
            writer.Write(versionArrayIndex);
            writer.Write("]");
        }

        public static void WriteCodeToInstance(IStoredSettingGenerator generator, StringBuilder writer, int versionArrayIndex, bool writeGenerator)
        {
            // Write accessor to an array
            WriteCodeToInstance(writer, versionArrayIndex);

            // Check if we need to include a generic
            if (writeGenerator == true)
            {
                writer.Append(".GetGenerator");
                writer.Append('<');
                writer.Append(CorrectedTypeName(generator.GetType()));
                writer.Append('>');
            }
            else
            {
                writer.Append(".GetSetting");
            }

            // Write parameter
            writer.Append("(\"");
            writer.Append(generator.Key);
            writer.Append("\")");
        }

        public static void WriteCodeToInstance(StringBuilder writer, int versionArrayIndex, Type settingsVersionType)
        {
            // Write type-cast for the array
            writer.Append("((");
            writer.Append(settingsVersionType);
            writer.Append(')');

            // Write array
            WriteCodeToInstance(writer, versionArrayIndex);
            writer.Append(')');
        }

        private static void WriteCodeToInstance(StringBuilder writer, int versionArrayIndex)
        {
            // Write accessor to an array
            writer.Append("AllSettingsVersions[");
            writer.Append(versionArrayIndex);
            writer.Append("]");
        }
        #endregion

        public static string CorrectedTypeName(Type type)
        {
            // First, replace the "`1[" and "]" with "<" and ">" respectively
            string correctedType = type.ToString();
            while(genericTypeTester.IsMatch(correctedType) == true)
            {
                correctedType = genericTypeTester.Replace(correctedType, @"<$1>");
            }

            // Replace any common types to simplified text
            correctedType = correctedType.Replace("System.Int32", "int");
            correctedType = correctedType.Replace("System.Single", "float");
            correctedType = correctedType.Replace("System.String", "string");
            correctedType = correctedType.Replace("System.Boolean", "bool");
            return correctedType;
        }
    }
}
