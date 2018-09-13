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
            SystemLanguage? systemDefault;

            public Language(string languageName, SystemLanguage systemDefault)
            {
                this.languageName = languageName;
                this.systemDefault = systemDefault;
            }

            public Language(string languageName)
            {
                this.languageName = languageName;
                this.systemDefault = null;
            }

            public string LanguageName
            {
                get
                {
                    return languageName;
                }
                set
                {
                    languageName = value;
                }
            }

            /// <summary>
            /// Sets to this languageName by default if <code>Application.systemLanguage</code> is set to this enum value.
            /// Can be null if not intended to be a default of any language, e.g. if there are multiple English variations (USA, UK, AUS), and only one of them is meant to be default.
            /// </summary>
            public SystemLanguage? SystemDefault
            {
                get
                {
                    return systemDefault;
                }
                set
                {
                    systemDefault = value;
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
        Language? defaultLangauge = null;

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
                return languageToIndexMap[language];
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
        }
        #endregion

        /// <summary>
        /// Gets the default supported language based on system's settings.  Since this operation iterates through all supported languages, it may be slow.
        /// </summary>
        /// <param name="language">Name of the Language.</param>
        /// <param name="index">The index this language belongs to.</param>
        /// <returns></returns>
        public bool GetDefaultLanguage(out string language, out int index)
        {
            return GetLanguage(Application.systemLanguage, out language, out index);
        }

        /// <summary>
        /// Gets the supported language mapped to <code>systemDefault</code>.  Since this operation iterates through all supported languages, it may be slow.
        /// </summary>
        /// <param name="systemDefault">The system language to find the corresponding language.</param>
        /// <param name="language">Name of the Language.</param>
        /// <param name="index">The index this language belongs to.</param>
        /// <returns></returns>
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
                if ((supportedLanguages[i].SystemDefault.HasValue == true) && (supportedLanguages[i].SystemDefault.Value == systemDefault))
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
    }
}
