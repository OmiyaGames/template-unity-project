using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LanguageTextPairEditor.cs" company="Omiya Games">
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
    /// Assets holding a giant list of translations.
    /// </summary>
    public partial class TranslationDictionary : ScriptableObject
    {
        public enum KeyNotFoundDefaults
        {
            Null,
            EmptyString,
            PresetMessage
        }

        public enum TranslationNotFoundDefaults
        {
            Null,
            EmptyString,
            PresetMessage,
            DefaultLanguageOrNull,
            DefaultLanguageOrEmptyString,
            DefaultLanguageOrPresetMessage
        }

        [Header("Required Components")]
        [SerializeField]
        SupportedLanguages supportedLanguages = null;
        [SerializeField]
        KeyNotFoundDefaults defaultToWhenKeyNotFound = KeyNotFoundDefaults.PresetMessage;
        [SerializeField]
        string presetMessageWhenKeyNotFound = "<Key Not Found>";
        [SerializeField]
        TranslationNotFoundDefaults defaultToWhenTranslationNotFound = TranslationNotFoundDefaults.PresetMessage;
        [SerializeField]
        int defaultLanguageWhenTranslationNotFound = 0;
        [SerializeField]
        string presetMessageWhenTranslationNotFound = "<Translation Not Found>";
        [SerializeField]
        bool replaceEmptyStringWithDefaultText = true;

        [Header("Translations")]
        [SerializeField]
        List<TranslationCollection> translations = null;

        /// <summary>
        /// A dictionary embedded in the dictionary. The top-most keys are keys to translations;
        /// next tier keys are languages; and values of the embedded tier are the text itself.
        /// </summary>
        KeyLanguageTextMap allTranslations = null;

        #region Properties
        public string this[string key, int languageIndex, bool allowRetrievingDefaultText]
        {
            get
            {
                return GetTranslation(key, languageIndex, allowRetrievingDefaultText);
            }
        }

        public string this[string key, string language, bool allowRetrievingDefaultText]
        {
            get
            {
                return this[key, SupportedLanguages[language], allowRetrievingDefaultText];
            }
        }

        public string this[string key, int languageIndex]
        {
            get
            {
                return this[key, languageIndex, true];
            }
            set
            {
                AddOrSetTranslation(key, languageIndex, value);
            }
        }

        public string this[string key, string language]
        {
            get
            {
                return this[key, language, true];
            }
            set
            {
                this[key, SupportedLanguages[language]] = value;
            }
        }

        public SupportedLanguages SupportedLanguages
        {
            get
            {
                return supportedLanguages;
            }
            set
            {
                supportedLanguages = value;
            }
        }

        public KeyNotFoundDefaults DefaultToWhenKeyNotFound
        {
            get
            {
                return defaultToWhenKeyNotFound;
            }
            set
            {
                defaultToWhenKeyNotFound = value;
            }
        }

        public string PresetMessageWhenKeyNotFound
        {
            get
            {
                return presetMessageWhenKeyNotFound;
            }
            set
            {
                presetMessageWhenKeyNotFound = value;
            }
        }

        public TranslationNotFoundDefaults DefaultToWhenTranslationNotFound
        {
            get
            {
                return defaultToWhenTranslationNotFound;
            }
            set
            {
                defaultToWhenTranslationNotFound = value;
            }
        }

        public string PresetMessageWhenTranslationNotFound
        {
            get
            {
                return presetMessageWhenTranslationNotFound;
            }
            set
            {
                presetMessageWhenTranslationNotFound = value;
            }
        }

        public int DefaultLanguageWhenTranslationNotFound
        {
            get
            {
                return defaultLanguageWhenTranslationNotFound;
            }
            set
            {
                defaultLanguageWhenTranslationNotFound = value;
            }
        }

        public bool IsEmptyStringReplacedWithDefaultText
        {
            get
            {
                return replaceEmptyStringWithDefaultText;
            }
            set
            {
                replaceEmptyStringWithDefaultText = value;
            }
        }

        /// <summary>
        /// A map of all the translations. The top-most keys are keys to translations;
        /// second-tier keys are languages; and values of the embedded tier are the text itself.
        /// </summary>
        /// <remarks>
        /// It's worth noting that any changes made to this dictionary will NOT be synced with
        /// the actual serialization until <code>UpdateSerializedTranslations()</code> is called.
        /// </remarks>
        /// <seealso cref="UpdateSerializedTranslations"/>
        public KeyLanguageTextMap AllTranslations
        {
            get
            {
                // Check if the dictionary is initialized
                if (allTranslations == null)
                {
                    RepopulateAllTranslations();
                }
                return allTranslations;
            }
        }

        public bool IsAllTranslationsSerialized
        {
            get;
            private set;
        }
        #endregion

        public List<TranslationCollection> UpdateSerializedTranslations(ProgressReport report = null)
        {
            // Check if we need to report our progress
            if (report != null)
            {
                // Set the number of steps involved in serialization
                report.SetTotalSteps(AllTranslations.Count);
            }

            // Grab a soft-copy of all translations
            KeyLanguageTextMap translationCopy = AllTranslations;

            // Clear the translations list (this needs to happen AFTER calling AllTranslations' getter)
            translations.Clear();

            // Go through all the keys
            TranslationCollection collectionToAdd;
            LanguageTextPair pairToAdd;
            foreach (KeyValuePair<string, LanguageTextMap> collection in translationCopy)
            {
                // Create a new collection of translations
                collectionToAdd = new TranslationCollection(collection.Key);

                // Go through all translations
                foreach (KeyValuePair<int, string> pair in collection.Value)
                {
                    // Create a new pair
                    pairToAdd = new LanguageTextPair(pair.Key, pair.Value);

                    // Add the pair to the collection
                    collectionToAdd.AllTranslations.Add(pairToAdd);
                }

                // Add new collection to the list
                translations.Add(collectionToAdd);

                // Check if we need to report our progress
                if (report != null)
                {
                    // Increment
                    report.IncrementCurrentStep();
                }
            }

            // Indicate the dictionary matches the serialization
            IsAllTranslationsSerialized = true;

            // Return the updated translations list
            return translations;
        }

        public KeyLanguageTextMap RepopulateAllTranslations()
        {
            // Setup or clear the dictionary
            if (allTranslations == null)
            {
                allTranslations = new KeyLanguageTextMap(this);
            }
            allTranslations.Clear();

            // Populate the dictionary
            LanguageTextMap toAdd = null;
            foreach (TranslationCollection collection in translations)
            {
                // Create a new dictionary
                toAdd = allTranslations.Add(collection.Key);
                if (toAdd != null)
                {
                    // Add all the pairs
                    foreach (LanguageTextPair pair in collection.AllTranslations)
                    {
                        // Make sure the language is supported before adding to the map
                        if (SupportedLanguages.Contains(pair.LanguageIndex) == true)
                        {
                            toAdd[pair.LanguageIndex] = pair.Text;
                        }
                    }
                }
            }

            // Indicate the dictionary matches the serialization
            IsAllTranslationsSerialized = true;
            return allTranslations;
        }

        public bool HasKey(string key)
        {
            return AllTranslations.ContainsKey(key);
        }

        public bool HasTranslation(string key, int languageIndex)
        {
            return ((HasKey(key) == true) && (SupportedLanguages.Contains(languageIndex) == true) && (AllTranslations[key][languageIndex] != null));
        }

        public bool HasTranslation(string key, string language)
        {
            return HasTranslation(key, SupportedLanguages[language]);
        }

        #region Helper Methods
        private string GetTranslation(string key, int languageIndex, bool allowRetrievingDefaultText)
        {
            // Check if key or translation is available
            string returnText = null;
            if (HasKey(key) == false)
            {
                if (allowRetrievingDefaultText == true)
                {
                    // Key is not available, return a default
                    returnText = GetDefaultKeyNotFoundText();
                }
            }
            else if (HasTranslation(key, languageIndex) == false)
            {
                if (allowRetrievingDefaultText == true)
                {
                    // Translation is not available, return a default
                    returnText = GetDefaultTranslationNotFoundText(key);
                }
            }
            else
            {
                // Grab the actual text
                returnText = AllTranslations[key][languageIndex];
                if ((allowRetrievingDefaultText == true) && (IsEmptyStringReplacedWithDefaultText == true) && (string.IsNullOrEmpty(returnText) == true))
                {
                    // Translation is not available, return a default
                    returnText = GetDefaultTranslationNotFoundText(key);
                }
            }
            return returnText;
        }

        private string GetDefaultKeyNotFoundText()
        {
            // By default, return null
            string returnText = null;
            switch (DefaultToWhenKeyNotFound)
            {
                case KeyNotFoundDefaults.PresetMessage:
                    returnText = PresetMessageWhenKeyNotFound;
                    break;

                case KeyNotFoundDefaults.EmptyString:
                    returnText = string.Empty;
                    break;
            }

            return returnText;
        }

        private string GetDefaultTranslationNotFoundText(string key)
        {
            // By default, return null
            string returnText = null;

            // Keep track of what we need to default to
            TranslationNotFoundDefaults defaultTo = DefaultToWhenTranslationNotFound;
            switch (defaultTo)
            {
                case TranslationNotFoundDefaults.DefaultLanguageOrPresetMessage:
                    // Pretend to be a preset message before analyzing whether a default translation is available or not
                    defaultTo = TranslationNotFoundDefaults.PresetMessage;
                    goto case TranslationNotFoundDefaults.DefaultLanguageOrNull;

                case TranslationNotFoundDefaults.DefaultLanguageOrEmptyString:
                    // Pretend to be an empty string before analyzing whether a default translation is available or not
                    defaultTo = TranslationNotFoundDefaults.EmptyString;
                    goto case TranslationNotFoundDefaults.DefaultLanguageOrNull;

                case TranslationNotFoundDefaults.DefaultLanguageOrNull:
                    // Check if the default language has a translation
                    if (HasTranslation(key, DefaultLanguageWhenTranslationNotFound) == true)
                    {
                        // If so, return that text
                        returnText = AllTranslations[key][DefaultLanguageWhenTranslationNotFound];

                        // Flag the variable so it skips the next check
                        defaultTo = TranslationNotFoundDefaults.DefaultLanguageOrNull;
                    }
                    break;
            }

            // Check the second time whether there's anything to default to.
            switch (defaultTo)
            {
                case TranslationNotFoundDefaults.PresetMessage:
                    returnText = PresetMessageWhenTranslationNotFound;
                    break;

                case TranslationNotFoundDefaults.EmptyString:
                    returnText = string.Empty;
                    break;
            }

            return returnText;
        }

        private void AddOrSetTranslation(string key, int languageIndex, string text)
        {
            // Grab a dictionary of translations from the argument, "key"
            LanguageTextMap translationsForKey;
            if (AllTranslations.TryGetValue(key, out translationsForKey) == false)
            {
                // Add a new dictionary for this key
                translationsForKey = AllTranslations.Add(key);
            }

            // Set the text
            translationsForKey[languageIndex] = text;
        }
        #endregion
    }
}
