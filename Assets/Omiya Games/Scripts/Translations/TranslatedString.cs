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
    public class TranslatedString : IDisposable
    {
        [SerializeField]
        string key;
        [SerializeField]
        TranslationDictionary dictionary;

        object[] values;
        TranslationManager.LanguageChanged onLanguageChanged = null;
        /// <summary>
        /// If null, this string is marked dirty.
        /// </summary>
        string translatedFormattedText = null;

        public TranslatedString(string key) : this(key, null) { }

        public TranslatedString(string key, params object[] values)
        {
            this.key = key;
            this.values = values;
        }

        #region Properties
        private static TranslationManager Manager
        {
            get
            {
                return Singleton.Get<TranslationManager>();
            }
        }

        public string TranslationKey
        {
            get
            {
                return key;
            }
            set
            {
                // Check if the value is actually different
                if(key != value)
                {
                    // Set key
                    key = value;

                    // Mark dirty
                    SetDirty();
                }
            }
        }

        public object[] Values
        {
            get
            {
                return values;
            }
            set
            {
                // Set the values directly; it's not performant to check whether all objects are different
                values = value;

                // Mark dirty
                SetDirty();
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
                // Check if the value is different
                if (dictionary != value)
                {
                    // Set dictionary
                    dictionary = value;

                    // Mark dirty
                    SetDirty();
                }
            }
        }

        public bool IsDirty
        {
            get
            {
                return (translatedFormattedText == null);
            }
        }

        public bool IsTranslating
        {
            get
            {
                return (string.IsNullOrEmpty(TranslationKey) == false) && (dictionary != null) && (dictionary.AllTranslations.ContainsKey(TranslationKey) == true);
            }
        }
        #endregion

        /// <summary>
        /// Generates a translated text based on GameSettings
        /// </summary>
        /// <returns>A translated text.</returns>
        public override string ToString()
        {
            // Check if we can translate
            if (IsTranslating == true)
            {
                // Check whether TranslationManager is available, and we need to generate a new string
                if ((IsDirty != true) && (Manager != null))
                {
                    // Check if we've binded to language change event
                    if (onLanguageChanged == null)
                    {
                        // If not, bind to the language manager, setting the flag dirty on language change
                        onLanguageChanged = new TranslationManager.LanguageChanged(SetDirty);
                        Manager.OnAfterLanguageChanged += onLanguageChanged;
                    }

                    // Retrieve the language
                    translatedFormattedText = ToString(Manager.CurrentLanguage);
                }
                else if (Manager == null)
                {
                    Dispose();
                }
            }
            return translatedFormattedText;
        }

        /// <summary>
        /// Generates a translated text based on input language
        /// </summary>
        /// <remarks>
        /// A new string will be generated each time,
        /// making this operation potentially slow.
        /// </remarks>
        /// <returns></returns>
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
        /// Generates a translated text based on input language
        /// </summary>
        /// <remarks>
        /// A new string will be generated each time,
        /// making this operation potentially slow.
        /// </remarks>
        /// <returns></returns>
        public string ToString(string language)
        {
            string returnString = null;
            if ((IsTranslating == true) && (dictionary.SupportedLanguages.Contains(language) == true))
            {
                returnString = AddFormatting(dictionary[TranslationKey, language]);
            }
            return returnString;
        }

        /// <summary>
        /// Cleans-up events and generated string.
        /// </summary>
        public void Dispose()
        {
            if (onLanguageChanged != null)
            {
                // Unbind to the language change event
                Manager.OnAfterLanguageChanged -= onLanguageChanged;
                onLanguageChanged = null;
            }

            // Dispose the generated string
            SetDirty();
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

        private void SetDirty()
        {
            translatedFormattedText = null;
        }

        private void SetDirty(TranslationManager source, string lastLanguage, string currentLanguage)
        {
            SetDirty();
        }
        #endregion
    }
}
