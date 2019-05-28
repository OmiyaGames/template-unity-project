using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

// FIXME: look into having each language hold a unique ID,
// and make TranslatedDictionary.cs store that ID per translation.
// This way, if a user deletes a language, that ID remains unchanged.
namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SupportedLanguages.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2019 Omiya Games
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
    /// <date>10/10/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Assets holding a list of languages and their information.
    /// </summary>
    public class SupportedLanguages : ScriptableObject
    {
        public const string DefaultLanguageName = "English (USA)";
        public const SystemLanguage DefaultLanguageSystem = SystemLanguage.English;

        [Serializable]
        public struct Language
        {
            [SerializeField]
            string languageName;
            [SerializeField]
            bool isSystemDefault;
            [SerializeField]
            SystemLanguage mapTo;
            [SerializeField]
            TMP_FontAsset[] fonts;

            public Language(string languageName, SystemLanguage systemDefault)
            {
                this.languageName = languageName;
                isSystemDefault = true;
                mapTo = systemDefault;
                fonts = new TMP_FontAsset[0];
            }

            public Language(string languageName)
            {
                this.languageName = languageName;
                isSystemDefault = false;
                mapTo = SystemLanguage.Unknown;
                fonts = new TMP_FontAsset[0];
            }

            public string LanguageName => languageName;

            public bool IsSystemDefault => isSystemDefault;

            public SystemLanguage LanguageMappedTo => mapTo;

            public TMP_FontAsset[] Fonts => fonts;
        }

        [SerializeField]
        int previewLanguageInIndex = 0;
        [SerializeField]
        Language[] supportedLanguages = new Language[]
        {
            new Language(DefaultLanguageName, DefaultLanguageSystem)
        };

        Dictionary<string, int> languageToIndexMap = null;

        #region Properties
        /// <summary>
        /// Gets the name of the langauge mapped to this index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Name of the langauge mapped to <code>index</code>.</returns>
        public string this[int index]
        {
            get
            {
                return supportedLanguages[index].LanguageName;
            }
        }

        /// <summary>
        /// Gets the index this language is mapped to.
        /// </summary>
        /// <param name="language"></param>
        /// <returns>The index the <code>language</code>is mapped to.</returns>
        public int this[string language]
        {
            get
            {
                return LanguageToIndexMap[language];
            }
        }

        /// <summary>
        /// Returns the number of supported languages
        /// </summary>
        public int Count
        {
            get
            {
                return supportedLanguages.Length;
            }
        }

        /// <summary>
        /// Gets the number of languages that are supported.
        /// </summary>
        public int NumberOfLanguages
        {
            get
            {
                return supportedLanguages.Length;
            }
        }

        /// <summary>
        /// Language to preview in the editor.  On runtime, this variable will be ignored.
        /// </summary>
        public string PreviewLanguage
        {
            get
            {
                return this[PreviewIndex];
            }
        }

        /// <summary>
        /// Index of the language to preview in the editor.  On runtime, this variable will be ignored.
        /// </summary>
        public int PreviewIndex
        {
            get
            {
                return previewLanguageInIndex;
            }
            set
            {
                previewLanguageInIndex = value;
            }
        }

        private Dictionary<string, int> LanguageToIndexMap
        {
            get
            {
                if (languageToIndexMap == null)
                {
                    // Setup dictionary
                    languageToIndexMap = new Dictionary<string, int>(supportedLanguages.Length);

                    // Go through all the supported languages
                    for (int index = 0; index < supportedLanguages.Length; ++index)
                    {
                        languageToIndexMap.Add(supportedLanguages[index].LanguageName, index);
                    }
                }
                return languageToIndexMap;
            }
        }
        #endregion

        /// <summary>
        /// Gets the default supported language based on system's settings.
        /// Returns true if a language matching the <code>systemDefault</code> is outputted.
        /// Since this operation iterates through all supported languages, it may be slow.
        /// </summary>
        /// <param name="language">Name of the Language.</param>
        /// <param name="index">The index this language belongs to.</param>
        /// <returns>True if a language matching the <code>systemDefault</code> is outputted.</returns>
        public bool GetDefaultLanguage(out string language, out int index)
        {
            return GetLanguage(Application.systemLanguage, out language, out index);
        }

        /// <summary>
        /// Gets the supported language mapped to <code>systemDefault</code>.
        /// Returns true if a language matching the <code>systemDefault</code> is outputted.
        /// Since this operation iterates through all supported languages, it may be slow.
        /// </summary>
        /// <param name="systemDefault">The system language to find the corresponding language.</param>
        /// <param name="language">Name of the Language.</param>
        /// <param name="index">The index this language belongs to.</param>
        /// <returns>True if a language matching the <code>systemDefault</code> is outputted.</returns>
        public bool GetLanguage(SystemLanguage systemDefault, out string language, out int index)
        {
            // By default, return null and the first language in the list
            bool returnIsFound = false;
            index = 0;
            language = supportedLanguages[index].LanguageName;

            // Go through the rest of the languages
            for (int i = 0; i < supportedLanguages.Length; ++i)
            {
                // Make sure the language has a language it supports
                if ((supportedLanguages[i].IsSystemDefault == true) && (supportedLanguages[i].LanguageMappedTo == systemDefault))
                {
                    // If so, return this language
                    index = i;
                    language = supportedLanguages[i].LanguageName;
                    returnIsFound = true;
                    break;
                }
            }
            return returnIsFound;
        }

        /// <summary>
        /// Checks if the index is supported.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Contains(int index)
        {
            return ((index >= 0) && (index < Count));
        }

        /// <summary>
        /// Checks if the language is supported
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool Contains(string language)
        {
            return LanguageToIndexMap.ContainsKey(language);
        }

        public TMP_FontAsset[] GetLanguageFonts(int index)
        {
            return supportedLanguages[index].Fonts;
        }

        public TMP_FontAsset[] GetLanguageFonts(string language)
        {
            return GetLanguageFonts(this[language]);
        }

        public Language GetLanguageMetaData(int index)
        {
            return supportedLanguages[index];
        }

        public Language GetLanguageMetaData(string language)
        {
            return GetLanguageMetaData(this[language]);
        }
    }
}
