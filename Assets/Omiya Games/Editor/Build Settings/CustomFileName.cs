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
        public enum PrefillType
        {
            Literal,
            AppName,
            BuildSettingName,
            DateTime
        }

        [System.Serializable]
        public struct Prefill
        {
            [SerializeField]
            PrefillType type;
            [SerializeField]
            string text;

            public Prefill(PrefillType type, string text)
            {
                this.type = type;
                this.text = text;
            }

            public PrefillType Type => type;
            public string Text => text;
        }

        public delegate string GetText(string text, IChildBuildSetting setting);

        /// <summary>
        /// The maximum WebGL build name
        /// </summary>
        public const int MaxSlugLength = 45;
        /// <summary>
        /// Set of invalid folder chars: "/, \, :, *, ?, ", <, >, and |."
        /// </summary>
        public static readonly HashSet<char> InvalidFileNameCharactersSet = new HashSet<char>()
        {
            '.',
            '\\',
            '/',
            ':',
            '*',
            '?',
            '"',
            '<',
            '>',
            '|'
        };
        /// <summary>
        /// Map from PrefillType to method
        /// </summary>
        public static readonly Dictionary<PrefillType, GetText> TextMapper = new Dictionary<PrefillType, GetText>()
        {
            {
                PrefillType.Literal,
                (string text, IChildBuildSetting setting) =>
                {
                    return text;
                }
            }, {
                PrefillType.AppName,
                (string text, IChildBuildSetting setting) =>
                {
                    return PlayerSettings.productName;
                }
            }, {
                PrefillType.BuildSettingName,
                (string text, IChildBuildSetting setting) =>
                {
                    return setting.name;
                }
            }, {
                PrefillType.DateTime,
                (string text, IChildBuildSetting setting) =>
                {
                    return System.DateTime.Now.ToString(text);
                }
            }
        };

        [SerializeField]
        private Prefill[] names;
        [SerializeField]
        bool asSlug;

        public CustomFileName(bool asSlug = false, params Prefill[] names)
        {
            this.names = names;
            this.asSlug = asSlug;
        }

        public string ToString(IChildBuildSetting setting)
        {
            // Append all the text into one
            StringBuilder builder = new StringBuilder();
            GetText method;
            foreach (Prefill name in names)
            {
                if(TextMapper.TryGetValue(name.Type, out method) == true)
                {
                    builder.Append(method(name.Text, setting));
                }
            }

            // Remove invalid characters
            string returnString = builder.ToString();
            returnString = RemoveDiacritics(builder, returnString);

            // Check if this needs to be a slug
            if (asSlug == true)
            {
                returnString = GenerateSlug(returnString);
            }
            return returnString;
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
        public static string RemoveDiacritics(StringBuilder stringBuilder, string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            stringBuilder.Clear();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if ((unicodeCategory != UnicodeCategory.NonSpacingMark) && (InvalidFileNameCharactersSet.Contains(c) == false))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
