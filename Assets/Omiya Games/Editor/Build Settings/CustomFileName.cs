using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="BuildPlayersResult.cs" company="Omiya Games">
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
    /// <date>10/31/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Property used to define a flexible file name
    /// </summary>
    [System.Serializable]
    public struct CustomFileName
    {
        /// <summary>
        /// Regular expression to detect /, \, :, *, ?, ", <, >, and |.
        /// </summary>
        public static readonly Regex InvalidFileNameCharacters = new Regex("[.\\\\/:*?\"<>|]");
        /// <summary>
        /// The maximum WebGL build name
        /// </summary>
        public const int MaxSlugLength = 45;

        // Since colons are NOT allowed to be used in folder names, using it here!
        public const string appName = ":App:";
        public const string settingName = ":Setting:";

        [SerializeField]
        private string[] names;
        [SerializeField]
        bool asSlug;

        public CustomFileName(bool asSlug = false, params string[] names)
        {
            this.names = names;
            this.asSlug = asSlug;
        }

        // FIXME: somehow create a list of settings, some pre-made, to insert into file
        public string ToString(IChildBuildSetting setting)
        {
            StringBuilder builder = new StringBuilder();
            foreach(string name in names)
            {
                switch(name)
                {
                    case settingName:
                        builder.Append(setting.name);
                        break;
                    case appName:
                        builder.Append(AppName);
                        break;
                    default:
                        builder.Append(name);
                        break;
                }
            }

            // Check if this needs to be a slug
            if(asSlug == true)
            {
                return GenerateSlug(builder.ToString());
            }
            else
            {
                return builder.ToString();
            }
        }

        public static string AppName
        {
            get
            {
                return InvalidFileNameCharacters.Replace(RemoveDiacritics(PlayerSettings.productName), "");
            }
        }

        /// <summary>
        /// Taken from http://predicatet.blogspot.com/2009/04/improved-c-slug-generator-or-how-to.html
        /// </summary>
        public static string GenerateSlug(string originalString)
        {
            // Remove invalid chars
            string returnSlug = Regex.Replace(originalString.ToLower(), @"[^a-z0-9\s-]", "");

            // Convert multiple spaces into one space
            returnSlug = Regex.Replace(returnSlug, @"\s+", " ").Trim();

            // Trim the length of the slug down to MaxSlugLength characters
            if (returnSlug.Length > MaxSlugLength)
            {
                returnSlug = returnSlug.Substring(0, MaxSlugLength).Trim();
            }

            // Replace spaces with hyphens
            returnSlug = Regex.Replace(returnSlug, @"\s", "-");

            return returnSlug;
        }

        /// <summary>
        /// Taken from http://archives.miloush.net/michkap/archive/2007/05/14/2629747.html
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int index = 0; index < normalizedString.Length; ++index)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(normalizedString[index]);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(normalizedString[index]);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
