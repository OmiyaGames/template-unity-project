using UnityEngine;
using System;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslatedString.cs" company="Omiya Games">
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
    /// <date>10/3/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A struct whose <code>ToString()</code> method automatically translates
    /// based on settings.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>3/23/2017</description>
    /// <description>Taro Omiya</description>
    /// <description>Initial version</description>
    /// 
    /// <description>10/3/2018</description>
    /// <description>Taro</description>
    /// <description>Upgrading with TranslationDictionary support</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    public class TranslatedString
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private TranslationDictionary dictionary;

        public TranslatedString() : this(string.Empty) { }

        public TranslatedString(string key) : this(key, null) { }

        public TranslatedString(string key, params object[] values)
        {
            this.key = key;
            Arguments = values;
        }

        #region Properties
        public string TranslationKey
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        public TranslationDictionary Dictionary
        {
            get
            {
                return dictionary;
            }
            set
            {
                dictionary = value;
            }
        }

        public object[] Arguments
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Indicating whether this class is ready to translate.
        /// </summary>
        /// <seealso cref="ToString"/>
        /// <seealso cref="ToString(int)"/>
        /// <seealso cref="ToString(string)"/>
        public bool IsTranslating
        {
            get
            {
                return (string.IsNullOrEmpty(TranslationKey) == false) && (Dictionary != null) &&
                    (Dictionary.SupportedLanguages != null) &&
                    (Dictionary.AllTranslations.ContainsKey(TranslationKey) == true);
            }
        }
        #endregion

        /// <summary>
        /// Generates a translated text based on TranslationManager's language.
        /// </summary>
        /// <remarks>
        /// A new string will be generated each time,
        /// making this operation potentially slow.
        /// </remarks>
        /// <returns>A translated text, or null if not ready.</returns>
        /// <seealso cref="TranslationManager"/>
        public override string ToString()
        {
#if UNITY_EDITOR
            if(Application.isPlaying == true)
            {
                return GetTextFromTranslationManager();
            }
            else
            {
                return GetTextFromSupportedLanguage();
            }
#else
            return GetTextFromTranslationManager();
#endif
        }

        /// <summary>
        /// Generates a translated text based on input language.
        /// </summary>
        /// <remarks>
        /// A new string will be generated each time,
        /// making this operation potentially slow.
        /// </remarks>
        /// <returns>A translated text, or null if not ready.</returns>
        public string ToString(int languageIndex)
        {
            string returnString = null;
            if ((IsTranslating == true) && (Dictionary.SupportedLanguages.Contains(languageIndex) == true))
            {
                returnString = AddFormatting(Dictionary[TranslationKey, languageIndex]);
            }
            return returnString;
        }

        /// <summary>
        /// Generates a translated text based on input language.
        /// </summary>
        /// <remarks>
        /// A new string will be generated each time,
        /// making this operation potentially slow.
        /// </remarks>
        /// <returns>A translated text, or null if not ready.</returns>
        public string ToString(string language)
        {
            string returnString = null;
            if ((IsTranslating == true) && (Dictionary.SupportedLanguages.Contains(language) == true))
            {
                returnString = AddFormatting(Dictionary[TranslationKey, language]);
            }
            return returnString;
        }

        public void SetValues(params object[] values)
        {
            Arguments = values;
        }

        #region Helper Methods
        private string GetTextFromTranslationManager()
        {
            string returnString = null;

            // Check if the TranslationManager is ready
            TranslationManager manager = Singleton.Get<TranslationManager>();
            if ((manager != null) && (manager.IsReady == true))
            {
                returnString = ToString(manager.CurrentLanguage);
            }
            return returnString;
        }

        private string GetTextFromSupportedLanguage()
        {
            string returnString = null;

            // Check if the TranslationManager is ready
            if ((Dictionary != null) && (Dictionary.SupportedLanguages != null))
            {
                returnString = ToString(Dictionary.SupportedLanguages.PreviewIndex);
            }
            return returnString;
        }

        private string AddFormatting(string translatedText)
        {
            if ((Arguments != null) && (Arguments.Length > 0))
            {
                translatedText = string.Format(translatedText, Arguments);
            }
            return translatedText;
        }
        #endregion
    }
}
