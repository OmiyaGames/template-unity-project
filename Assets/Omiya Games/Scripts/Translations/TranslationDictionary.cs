using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OmiyaGames.Translations
{
    public class TranslationDictionary : ScriptableObject
    {
        public const byte DefaultNumberOfLanguages = 3;

        [Serializable]
        public struct LanguageTextPair
        {
            [SerializeField]
            string language;
            [SerializeField]
            [TextArea]
            string text;

            public LanguageTextPair(string language, string text)
            {
                this.language = language;
                this.text = text;
            }

            public string Language
            {
                get
                {
                    return language;
                }
                set
                {
                    language = value;
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

        [SerializeField]
        List<TranslationCollection> translations = null;

        /// <summary>
        /// A dictionary embedded in the dictionary. The top-most keys are keys to translations;
        /// next tier keys are languages; and values of the embedded tier are the text itself.
        /// </summary>
        Dictionary<string, Dictionary<string, string>> allTranslations = null;

        #region Properties
        public string this[string key, string language]
        {
            get
            {
                string returnText = null;
                if (AllTranslations.ContainsKey(key) == true)
                {
                    returnText = GetDefaultText(key);
                    if (AllTranslations[key].ContainsKey(language) == true)
                    {
                        returnText = AllTranslations[key][language];
                    }
                }
                return returnText;
            }
            set
            {
                if ((AllTranslations.ContainsKey(key) == true) && (AllTranslations[key].ContainsKey(language) == true))
                {
                    AllTranslations[key][language] = value;
                }
            }
        }

        public string this[string key]
        {
            get
            {
                return this[key, Translator.CurrentLanguage];
            }
            set
            {
                this[key, Translator.CurrentLanguage] = value;
            }
        }

        static TranslationManager Translator
        {
            get
            {
                return Singleton.Get<TranslationManager>();
            }
        }

        /// <summary>
        /// A dictionary embedded in the dictionary. The top-most keys are keys to translations;
        /// next tier keys are languages; and values of the embedded tier are the text itself.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> AllTranslations
        {
            get
            {
                // Check if the dictionary is initialized
                if(allTranslations == null)
                {
                    // If not, setup the dictionary (we know the fianl capacity)
                    allTranslations = new Dictionary<string, Dictionary<string, string>>(translations.Count);

                    // Populate the dictionary
                    Dictionary<string, string> toAdd = null;
                    foreach (TranslationCollection collection in translations)
                    {
                        if(allTranslations.ContainsKey(collection.Key) == false)
                        {
                            // Create a new dictionary
                            toAdd = new Dictionary<string, string>(collection.AllTranslations.Count);

                            // Add all the pairs
                            foreach(LanguageTextPair pair in collection.AllTranslations)
                            {
                                toAdd.Add(pair.Language, pair.Text);
                            }

                            // Add this dictionary to the final collection
                            allTranslations.Add(collection.Key, toAdd);
                        }
                    }
                }
                return allTranslations;
            }
        }
        #endregion

        public string GetDefaultText(string key)
        {
            string returnText = null;
            if (AllTranslations.ContainsKey(key) == true)
            {
                foreach(string firstLanguage in AllTranslations[key].Keys)
                {
                    returnText = AllTranslations[key][firstLanguage];
                    break;
                }
                if (AllTranslations[key].ContainsKey(Translator.DefaultLanguage) == true)
                {
                    returnText = AllTranslations[key][Translator.DefaultLanguage];
                }
            }
            return returnText;
        }

        public List<TranslationCollection> UpdateSerializedTranslations()
        {
            // Grab a soft-copy of all translations
            Dictionary<string, Dictionary<string, string>> translationCopy = AllTranslations;

            // Clear the translations list (this needs to happen AFTER calling AllTranslations' getter)
            translations.Clear();

            // Go through all the keys
            TranslationCollection collectionToAdd;
            LanguageTextPair pairToAdd;
            foreach (KeyValuePair<string, Dictionary<string, string>> collection in translationCopy)
            {
                // Create a new collection of translations
                collectionToAdd = new TranslationCollection(collection.Key);

                // Go through all translations
                foreach(KeyValuePair<string, string> pair in collection.Value)
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
    }
}
