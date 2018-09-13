using System.Collections.Generic;
using UnityEngine;
using System;

namespace OmiyaGames.Translations
{
    public class TranslationDictionary : ScriptableObject
    {
        public const byte DefaultNumberOfLanguages = 3;

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

        [Serializable]
        public struct LanguageTextPair
        {
            [SerializeField]
            int languageIndex;
            [SerializeField]
            [TextArea]
            string text;

            public LanguageTextPair(int languageIndex, string text)
            {
                this.languageIndex = languageIndex;
                this.text = text;
            }

            public int LanguageIndex
            {
                get
                {
                    return languageIndex;
                }
                set
                {
                    languageIndex = value;
                }
            }

            public string Text
            {
                get
                {
                    return text;
                }
                set
                {
                    text = value;
                }
            }
        }

        [Serializable]
        public struct SystemToLanguagePair
        {
            [SerializeField]
            SystemLanguage system;
            [SerializeField]
            string language;

            public SystemToLanguagePair(SystemLanguage system, string language)
            {
                this.system = system;
                this.language = language;
            }

            public SystemLanguage System
            {
                get
                {
                    return system;
                }
            }

            public string Language
            {
                get
                {
                    return language;
                }
            }
        }

        [Serializable]
        public struct TranslationCollection
        {
            [SerializeField]
            string key;
            [SerializeField]
            List<LanguageTextPair> allTranslations;

            public TranslationCollection(string key)
            {
                this.key = key;
                this.allTranslations = new List<LanguageTextPair>(DefaultNumberOfLanguages);
            }

            public string Key
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

            public List<LanguageTextPair> AllTranslations
            {
                get
                {
                    if (allTranslations == null)
                    {
                        allTranslations = new List<LanguageTextPair>(DefaultNumberOfLanguages);
                    }
                    return allTranslations;
                }
            }
        }

        [Header("Required Components")]
        [SerializeField]
        SupportedLanguages supportedLanguages = null;

        [Header("Behavior When Key Is Not Found")]
        [SerializeField]
        KeyNotFoundDefaults defaultToWhenKeyNotFound = KeyNotFoundDefaults.PresetMessage;
        [SerializeField]
        string presetMessageWhenKeyNotFound = "<Key Not Found>";

        [Header("Behavior When Translation Is Not Found")]
        [SerializeField]
        TranslationNotFoundDefaults defaultToWhenTranslationNotFound = TranslationNotFoundDefaults.PresetMessage;
        [SerializeField]
        string presetMessageWhenTranslationNotFound = "<Translation Not Found>";
        [SerializeField]
        int defaultLanguageWhenTranslationNotFound = 0;

        [Header("Translations")]
        [SerializeField]
        List<TranslationCollection> translations = null;

        /// <summary>
        /// A dictionary embedded in the dictionary. The top-most keys are keys to translations;
        /// next tier keys are languages; and values of the embedded tier are the text itself.
        /// </summary>
        Dictionary<string, Dictionary<int, string>> allTranslations = null;

        #region Properties
        public string this[string key, int languageIndex]
        {
            get
            {
                return GetTranslation(key, languageIndex);
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
                return this[key, SupportedLanguages[language]];
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

        /// <summary>
        /// A dictionary embedded in the dictionary. The top-most keys are keys to translations;
        /// next tier keys are languages; and values of the embedded tier are the text itself.
        /// </summary>
        private Dictionary<string, Dictionary<int, string>> AllTranslations
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
        #endregion

        public List<TranslationCollection> UpdateSerializedTranslations()
        {
            // Grab a soft-copy of all translations
            Dictionary<string, Dictionary<int, string>> translationCopy = AllTranslations;

            // Clear the translations list (this needs to happen AFTER calling AllTranslations' getter)
            translations.Clear();

            // Go through all the keys
            TranslationCollection collectionToAdd;
            LanguageTextPair pairToAdd;
            foreach (KeyValuePair<string, Dictionary<int, string>> collection in translationCopy)
            {
                // Create a new collection of translations
                collectionToAdd = new TranslationCollection(collection.Key);

                // Go through all translations
                foreach(KeyValuePair<int, string> pair in collection.Value)
                {
                    // Create a new pair
                    pairToAdd = new LanguageTextPair(pair.Key, pair.Value);

                    // Add the pair to the collection
                    collectionToAdd.AllTranslations.Add(pairToAdd);
                }

                // Add new collection to the list
                translations.Add(collectionToAdd);
            }

            // Return the updated translations list
            return translations;
        }

        public Dictionary<string, Dictionary<int, string>> RepopulateAllTranslations()
        {
            // Setup or clear the dictionary
            if (allTranslations == null)
            {
                allTranslations = new Dictionary<string, Dictionary<int, string>>(translations.Count);
            }
            allTranslations.Clear();

            // Populate the dictionary
            Dictionary<int, string> toAdd = null;
            foreach (TranslationCollection collection in translations)
            {
                if (allTranslations.ContainsKey(collection.Key) == false)
                {
                    // Create a new dictionary
                    toAdd = new Dictionary<int, string>(collection.AllTranslations.Count);

                    // Add all the pairs
                    foreach (LanguageTextPair pair in collection.AllTranslations)
                    {
                        toAdd.Add(pair.LanguageIndex, pair.Text);
                    }

                    // Add this dictionary to the final collection
                    allTranslations.Add(collection.Key, toAdd);
                }
            }
            return allTranslations;
        }

        public bool HasKey(string key)
        {
            return AllTranslations.ContainsKey(key);
        }

        public bool HasTranslation(string key, int languageIndex)
        {
            bool returnFlag = HasKey(key);
            if(returnFlag == true)
            {
                returnFlag = AllTranslations[key].ContainsKey(languageIndex);
            }
            return returnFlag;
        }

        public bool HasTranslation(string key, string language)
        {
            return HasTranslation(key, SupportedLanguages[language]);
        }

        #region Helper Methods
        private string GetTranslation(string key, int languageIndex)
        {
            // Check if key or translation is available
            string returnText = null;
            if (HasKey(key) == false)
            {
                // Key is not available, return a default
                returnText = GetDefaultKeyNotFoundText();
            }
            else if (HasTranslation(key, languageIndex) == false)
            {
                // Translation is not available, return a default
                returnText = GetDefaultTranslationNotFoundText(key);
            }
            else
            {
                // Grab the actual text
                returnText = AllTranslations[key][languageIndex];
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
            Dictionary<int, string> translationsForKey;
            if(AllTranslations.TryGetValue(key, out translationsForKey) == false)
            {
                // Add a new dictionary for this key
                translationsForKey = new Dictionary<int, string>(SupportedLanguages.NumberOfLanguages);
                AllTranslations.Add(key, translationsForKey);
            }

            // Check if the language is already added into the dictionary of translations
            if(translationsForKey.ContainsKey(languageIndex) == true)
            {
                // If so, update the text
                translationsForKey[languageIndex] = text;
            }
            else
            {
                // If not, add the new language and text
                translationsForKey.Add(languageIndex, text);
            }
        }
        #endregion
    }
}
