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
    /// <description>Initial verison</description>
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
        string key;
        [SerializeField]
        TranslationDictionary dictionary;

        public TranslatedString(string key) : this(key, null) { }

        public TranslatedString(string key, params object[] values)
        {
            this.key = key;
            Values = values;
        }

        #region Properties
        private static TranslationManager Manager
        {
            get
            {
                return Singleton.Get<TranslationManager>();
            }
        }

        public string TranslationKey => key;
        public TranslationDictionary Dictionary => dictionary;

        public object[] Values
        {
            get;
            set;
        } = null;

        public bool IsTranslating
        {
            get
            {
                return (string.IsNullOrEmpty(TranslationKey) == false) && (dictionary != null) && (dictionary.AllTranslations.ContainsKey(TranslationKey) == true);
            }
        }
        #endregion

        /// <summary>
        /// Generates a translated text based on GameSettings' language.
        /// </summary>
        /// <remarks>
        /// A new string will be generated each time,
        /// making this operation potentially slow.
        /// </remarks>
        /// <returns>A translated text.</returns>
        public override string ToString()
        {
            // Check if we can translate
            string returnString = null;
            if (Manager != null)
            {
                returnString = ToString(Manager.CurrentLanguage);
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
        /// <returns>A translated text.</returns>
        public string ToString(int languageIndex)
        {
            string returnString = null;
            if ((IsTranslating == true) && (dictionary.SupportedLanguages.Contains(languageIndex) == true))
            {
                returnString = AddFormatting(dictionary[TranslationKey, languageIndex]);
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
        /// <returns>A translated text.</returns>
        public string ToString(string language)
        {
            string returnString = null;
            if ((IsTranslating == true) && (dictionary.SupportedLanguages.Contains(language) == true))
            {
                returnString = AddFormatting(dictionary[TranslationKey, language]);
            }
            return returnString;
        }

        public void SetValues(params object[] values)
        {
            Values = values;
        }

        #region Helper Methods
        private string AddFormatting(string translatedText)
        {
            if ((Values != null) && (Values.Length > 0))
            {
                translatedText = string.Format(translatedText, Values);
            }
            return translatedText;
        }
        #endregion
    }
}
