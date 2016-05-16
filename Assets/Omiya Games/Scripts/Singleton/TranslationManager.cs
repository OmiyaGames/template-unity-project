using UnityEngine;
using System;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslationManager.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
    /// <date>4/21/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Calls another script to parse a CSV in a specific format. Parses out different
    /// languages from that file, and key/value pairs for a single language. Also provides
    /// interface for retrieving those values based on a key, as well as changing the
    /// the current langage.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <Date>      <Name> <Description>
    /// 2015/03/23  Dylan  Initial verison
    /// 2015/03/24  Taro   Refactoring Documentation,
    ///                    replacing methods with properties
    ///                    And turning the script to a Singleton
    /// 2015/03/25  Taro   Adding variables for debugging
    /// 2015/07/03  Taro   Retrieving language from GameSettings
    /// </remarks>
    /// <seealso cref="Singleton"/>
    public class TranslationManager : ISingletonScript
    {
        [System.Serializable]
        public struct LanguageMap
        {
            [SerializeField]
            SystemLanguage language;
            [SerializeField]
            string header;

            public SystemLanguage Language
            {
                get
                {
                    return language;
                }
            }

            public string Header
            {
                get
                {
                    return header;
                }
            }
        }

        [Header("CSV File")]
        [Tooltip("Set this variable if the CSV file is a text asset that isn't in the Resources folder.")]
        [SerializeField]
        TextAsset loadFileAsset = null;
        [Tooltip("Set this variable if the CSV file is going to be loaded from the Resources folder.")]
        [SerializeField]
        string loadFileName = "";

        [Header("Content")]
        [Tooltip("The header containing the keys, referencing each string.")]
        [SerializeField]
        string keyHeader = "Keys";
        [Tooltip("The language to test on game loading. Leave blank to use the default language.")]
        [SerializeField]
        string testLanguage = "";

        [Header("Language Support")]
        [Tooltip("Maps a language to a header.")]
        [SerializeField]
        LanguageMap[] languageMap = null;

        /// <summary>
        /// The loaded file.
        /// </summary>
        TextAsset inputFile = null;
        /// <summary>
        /// List of supported languages available after the file has been parsed.
        /// </summary>
        List<string> supportedLanguages = new List<string>();
        /// <summary>
        /// A dictionary of the keys, current-language-values.
        /// </summary>
        Dictionary<string, string> translationDictionary = new Dictionary<string, string>();
        /// <summary>
        /// Default Language in case needed for whatever reason. Defaults to first language encountered in the file.
        /// </summary>
        string defaultLanguage = "";
        /// <summary>
        /// Currently selected langauge. Defaults to the first language encountered in the file.
        /// </summary>
        string currentLanguage = "";
        Dictionary<SystemLanguage, string> headerDictionary = new Dictionary<SystemLanguage, string>();

        /// <summary>
        /// Called when the first scene is loaded.
        /// </summary>
        public override void SingletonAwake(Singleton globalGameObject)
        {
            // Setup the header dictionary
            headerDictionary.Clear();
            for(int i = 0; i < languageMap.Length; ++i)
            {
                headerDictionary.Add(languageMap[i].Language, languageMap[i].Header);
            }
        }

        /// <summary>
        /// Called when any scene after the first one is loaded.
        /// </summary>
        public override void SceneAwake(Singleton globalGameObject)
        {
            // Check if we've populated any translations
            if (translationDictionary.Count <= 0)
            {
                // Check if we're testing a language
                if (string.IsNullOrEmpty(testLanguage) == true)
                {
                    // Retrieve the default language settings from GameSettings
                    GameSettings settings = Singleton.Get<GameSettings>();
                    currentLanguage = settings.Language;
                    if(Debug.isDebugBuild == true)
                    {
                        Debug.Log("Retrieved language from settings: " + currentLanguage);
                    }

                    // Check to see if we support the system language
                    if(headerDictionary.ContainsKey(Application.systemLanguage) == true)
                    {
                        // Set the default language
                        defaultLanguage = headerDictionary[Application.systemLanguage];
                        if(Debug.isDebugBuild == true)
                        {
                            Debug.Log("Retrieved default language from language map: " + currentLanguage);
                        }

                        // Check to see if the current language is an empty string
                        if (string.IsNullOrEmpty(currentLanguage) == true)
                        {
                            // If it is, use the default language instead
                            currentLanguage = defaultLanguage;

                            // Update the settings
                            settings.Language = currentLanguage;
                        }
                    }
                }
                else
                {
                    // If so, set both the current and default language to the test language
                    defaultLanguage = testLanguage;
                    currentLanguage = testLanguage;
                    if(Debug.isDebugBuild == true)
                    {
                        Debug.Log("Retrieved language from testLanguage: " + testLanguage);
                    }
                }
                if(Debug.isDebugBuild == true)
                {
                    Debug.Log("Language settings, current: " + currentLanguage + ", and default: " + defaultLanguage);
                }

                // Check which parameter to use to load the next file
                if (loadFileAsset != null)
                {
                    ParseFile(loadFileAsset);
                }
                else if (string.IsNullOrEmpty(loadFileName) == false)
                {
                    ParseFile(loadFileName);
                }
                else
                {
                    Debug.LogWarning("No file found for CSVLanguageParser");
                }
            }
        }

        #region Properties
        public string this[string key]
        {
            get
            {
                string returnString = null;
                if(translationDictionary.TryGetValue(key, out returnString) == false)
                {
                    throw new ArgumentException("The key, \"" + key + "\" was not present in the CSV file.");
                }
                return returnString;
            }
        }

        /// <summary>
        /// Gets the list of langauges identified in the most recent parse.
        /// </summary>
        /// <returns>The supported languages.</returns>
        public List<string> SupportedLanguages
        {
            get
            {
                return supportedLanguages;
            }
        }

        /// <summary>
        /// Gets the default language.
        /// </summary>
        /// <value>The default language.</value>
        public string DefaultLanguage
        {
            get
            {
                return defaultLanguage;
            }
        }

        /// <summary>
        /// Gets or sets the current language.
        /// </summary>
        /// <value>The current language.</value>
        /// <remarks>
        /// When setting a supported language, this property will
        /// reparse the file to reload the dictionary.
        /// If a language is not supported, an error will be raised.
        /// </remarks>
        public string CurrentLanguage
        {
            get
            {
                return currentLanguage;
            }
            set
            {
                // Check to see if this is a new language
                if (currentLanguage.Equals(value) == false)
                {
                    // Check if this language is supported
                    if (supportedLanguages.Contains(value) == true)
                    {
                        // Set the language
                        currentLanguage = value;

                        // Parse the file
                        ParseFile();

                        // Update settings
                        Singleton.Get<GameSettings>().Language = currentLanguage;
                    }
                    else
                    {
                        Debug.LogError(value + " is not a supported langague.");
                    }
                }
            }
        }
        #endregion

        public void ParseFile(string csvFilePath)
        {
            inputFile = Resources.Load<TextAsset>(csvFilePath);
            ParseFile();
        }

        public void ParseFile(TextAsset textAsset)
        {
            inputFile = textAsset;
            ParseFile();
        }

        public void UnloadFile()
        {
            if (inputFile != null)
            {
                Resources.UnloadAsset(inputFile);
            }
        }

        public bool ContainsKey(string key)
        {
            return translationDictionary.ContainsKey(key);
        }

        public List<string> GetAllKeys()
        {
            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, string> pair in translationDictionary)
            {
                keys.Add(pair.Key);
            }
            return keys;
        }

        public void ResetToDefaultLanguage()
        {
            currentLanguage = defaultLanguage;
            ParseFile();
        }

        #region Helper Methods
        /// <summary>
        /// Helper method to parse file based on inputFile.
        /// </summary>
        void ParseFile()
        {
            /* Read the input file. Data is expected to be returned in a list of
             * Dictionary objects in which the position in the list corolates to the
             * row-number from the CSV file (element 0 is the second row where the
             * first row is headers), the keys in the dictionary reflect the column
             * names from the header, and the values in the dictionary reflect the
             * values for a given row/column.
             */
            List<Dictionary<string, string>> data = CSVReader.Read(inputFile);

            /* Read the first row pulled back from the csv file. Parse the
             * dictionary to get a list of all the keys that were found. This
             * will be the list of languages in the file. When building the list,
             * ignore the column that has the language-independent keys. Also
             * while building, if a defualt or current language has not yet been
             * set, then set them to the first language encountered.
             */
            supportedLanguages.Clear();
            foreach (string key in data[0].Keys)
            {
                if (key != keyHeader)
                {
                    supportedLanguages.Add(key);
                    if (string.IsNullOrEmpty(defaultLanguage) == true)
                    {
                        defaultLanguage = key;
                    }
                    if (string.IsNullOrEmpty(currentLanguage) == true)
                    {
                        currentLanguage = key;
                    }
                }
            }

            /* Loop through each row in the file. Grab the langauge-independet, and
             * the value based on the current langage. Put them in the dictionary.
             */
            translationDictionary.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                string key = data[i][keyHeader];
                string val = data[i][currentLanguage];
                if(translationDictionary.ContainsKey(key) == false)
                {
                    translationDictionary.Add(key, val);
                }
                else
                {
                    Debug.LogError("Translation CSV file contains duplicate key: " + key);
                }
            }

            /* Update any Text labels */
            foreach (TranslatedText label in TranslatedText.AllTranslationScripts)
            {
                label.UpdateLabel();
            }
            foreach (TranslatedTextMesh label in TranslatedTextMesh.AllTranslationScripts)
            {
                label.UpdateLabel();
            }
        }
        #endregion
    }
}
