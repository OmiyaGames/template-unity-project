using System.Collections.Generic;
using UnityEngine;
using System;

namespace OmiyaGames.Translations
{
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

            public Language(string languageName, SystemLanguage systemDefault)
            {
                this.languageName = languageName;
                isSystemDefault = true;
                mapTo = systemDefault;
            }

            public Language(string languageName)
            {
                this.languageName = languageName;
                isSystemDefault = false;
                mapTo = DefaultLanguageSystem;
            }

            public string LanguageName
            {
                get
                {
                    return languageName;
                }
            }

            public bool IsSystemDefault
            {
                get
                {
                    return isSystemDefault;
                }
            }

            public SystemLanguage LanguageMappedTo
            {
                get
                {
                    return mapTo;
                }
            }
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
    }
}
