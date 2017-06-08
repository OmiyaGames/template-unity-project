using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames
{
    using Settings;

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
    [DisallowMultipleComponent]
    public class TranslationManager : ISingletonScript
    {
        public delegate void LanguageChanged(TranslationManager source, string lastLanguage, string currentLanguage);
        public event LanguageChanged OnAfterLanguageChanged;

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

        [System.Serializable]
        public struct FontMapDetails
        {
            [SerializeField]
            string name;
            [SerializeField]
            FontStyle style;
            [SerializeField]
            Font font;

            public string Name
            {
                get
                {
                    return name;
                }
            }

            public FontStyle Style
            {
                get
                {
                    return style;
                }
            }

            public Font Font
            {
                get
                {
                    return font;
                }
            }
        }

        public struct FontMapKey
        {
            public FontMapKey(string name, FontStyle style)
            {
                Name = name;
                Style = style;
            }

            public string Name { get; set; }
            public FontStyle Style { get; set; }
        }

        [System.Serializable]
        public class FontMap
        {
            [SerializeField]
            string header;
            [SerializeField]
            Font defaultFont;
            [SerializeField]
            FontMapDetails[] otherFonts;

            readonly Dictionary<FontMapKey, FontMapDetails> otherFontMap = new Dictionary<FontMapKey, FontMapDetails>();
            FontMapKey fontSearchCache = new FontMapKey();

            public string Header
            {
                get
                {
                    return header;
                }
            }

            public Font DefaultFont
            {
                get
                {
                    return defaultFont;
                }
            }

            public Dictionary<FontMapKey, FontMapDetails> OtherFonts
            {
                get
                {
                    if(otherFontMap.Count != otherFonts.Length)
                    {
                        otherFontMap.Clear();
                        foreach(FontMapDetails details in otherFonts)
                        {
                            fontSearchCache.Name = details.Name;
                            fontSearchCache.Style = details.Style;
                            otherFontMap.Add(fontSearchCache, details);
                        }
                    }
                    return otherFontMap;
                }
            }

            public Font GetFont(string name, FontStyle style = FontStyle.Normal)
            {
                Font returnFont = DefaultFont;

                fontSearchCache.Name = name;
                fontSearchCache.Style = style;
                if(OtherFonts.ContainsKey(fontSearchCache) == true)
                {
                    returnFont = OtherFonts[fontSearchCache].Font;
                }
                return returnFont;
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
        /// <summary>
        /// Default Language in case needed for whatever reason. If null or an empty string, defaults to first language encountered in the file.
        /// </summary>
        [Tooltip("The language the game defaults to if the system's language is not available on the Language Map. If left blank, it uses the left-most language column in the CSV file.")]
        [SerializeField]
        string defaultLanguage = "";

        [Header("Language Support")]
        [Tooltip("Maps a language to a header.")]
        [SerializeField]
        LanguageMap[] languageMap = null;
        [Tooltip("Maps a language to a header.")]
        [SerializeField]
        FontMap[] fontMap = null;

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
        /// Currently selected langauge. Defaults to the first language encountered in the file.
        /// </summary>
        string currentLanguage = "";
        readonly Dictionary<SystemLanguage, LanguageMap> headerDictionary = new Dictionary<SystemLanguage, LanguageMap>();
        readonly Dictionary<string, FontMap> fontDictionary = new Dictionary<string, FontMap>();

        /// <summary>
        /// Called when the first scene is loaded.
        /// </summary>
        public override void SingletonAwake(Singleton globalGameObject)
        {
            // Do nothing
        }

        /// <summary>
        /// Called when any scene after the first one is loaded.
        /// </summary>
        public override void SceneAwake(Singleton globalGameObject)
        {
            // Check if we've populated any translations
            if (translationDictionary.Count <= 0)
            {
                // Check the system's language
                SetupDefaultLanguage();

                // Retrieve the current language settings from GameSettings
                SetupCurrentLanguage();

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
                    Debug.LogWarning("No file found for TranslationManager");
                }

                // Indicate final results
                if(Debug.isDebugBuild == true)
                {
                    Debug.Log("Language settings, current: " + currentLanguage + ", and default: " + defaultLanguage);
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
                    throw new System.ArgumentException("The key, \"" + key + "\" was not present in the CSV file.");
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
                        string lastLanguage = currentLanguage;
                        currentLanguage = value;

                        // Parse the file
                        ParseFile();

                        // Update settings
                        Singleton.Get<GameSettings>().Language = currentLanguage;

                        // Call the event that the langauge changed
                        if(OnAfterLanguageChanged != null)
                        {
                            OnAfterLanguageChanged(this, lastLanguage, currentLanguage);
                        }
                    }
                    else
                    {
                        Debug.LogError(value + " is not a supported langague.");
                    }
                }
            }
        }

        public FontMap CurrentLanguageFont
        {
            get
            {
                FontMap returnFont = null;
                if(FontDictionary.ContainsKey(CurrentLanguage) == true)
                {
                    returnFont = FontDictionary[CurrentLanguage];
                }
                else if(FontDictionary.ContainsKey(DefaultLanguage) == true)
                {
                    returnFont = FontDictionary[DefaultLanguage];
                }
                return returnFont;
            }
        }
        
        Dictionary<SystemLanguage, LanguageMap> HeaderDictionary
        {
            get
            {
                // Check if the language map has a different size of our cached dictionary
                if((languageMap != null) && (headerDictionary.Count != languageMap.Length))
                {
                    // Setup the header dictionary
                    headerDictionary.Clear();
                    for(int i = 0; i < languageMap.Length; ++i)
                    {
                        headerDictionary.Add(languageMap[i].Language, languageMap[i]);
                    }
                }
                return headerDictionary;
            }
        }

        internal Dictionary<string, FontMap> FontDictionary
        {
            get
            {
                // Check if the language map has a different size of our cached dictionary
                if((fontMap != null) && (fontDictionary.Count != fontMap.Length))
                {
                    // Setup the header dictionary
                    fontDictionary.Clear();
                    for(int i = 0; i < fontMap.Length; ++i)
                    {
                        fontDictionary.Add(fontMap[i].Header, fontMap[i]);
                    }
                }
                return fontDictionary;
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
            ParseHeaders(data);
            ParseColumn(data);

            /* Update any Text labels */
            foreach (TranslatedText label in TranslatedText.AllTranslationScripts)
            {
                if (label != null)
                {
                    label.UpdateLabel();
                }
            }
            foreach (TranslatedTextMesh label in TranslatedTextMesh.AllTranslationScripts)
            {
                if (label != null)
                {
                    label.UpdateLabel();
                }
            }
        }

        void ParseHeaders(List<Dictionary<string, string>> data)
        {
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
                if ((string.IsNullOrEmpty(key) == false) && (key != keyHeader))
                {
                    supportedLanguages.Add(key);
                }
            }

            // Make sure there's more than one language
            if(supportedLanguages.Count > 0)
            {
                // Check if the default language is set
                if ((string.IsNullOrEmpty(defaultLanguage) == true) || (supportedLanguages.Contains(defaultLanguage) == false))
                {
                    // If not, grab the first language in the headers
                    defaultLanguage = supportedLanguages[0];
                }
                
                // Check if the current langauge is set
                if ((string.IsNullOrEmpty(currentLanguage) == true) || (supportedLanguages.Contains(currentLanguage) == false))
                {
                    // If not, use the default language instead
                    currentLanguage = defaultLanguage;

                    // Update the settings
                    Singleton.Get<GameSettings>().Language = currentLanguage;
                }
            }
        }

        void ParseColumn(List<Dictionary<string, string>> data)
        {
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
        }

        void SetupDefaultLanguage()
        {
            if(HeaderDictionary.ContainsKey(Application.systemLanguage) == true)
            {
                // Set the default language
                defaultLanguage = HeaderDictionary[Application.systemLanguage].Header;
                if(Debug.isDebugBuild == true)
                {
                    Debug.Log("Retrieved default language from system: " + defaultLanguage);
                }
            }
        }

        void SetupCurrentLanguage()
        {
            currentLanguage = Singleton.Get<GameSettings>().Language;
            if ((string.IsNullOrEmpty(currentLanguage) == false) && (Debug.isDebugBuild == true))
            {
                Debug.Log("Retrieved language from settings: " + currentLanguage);
            }
        }
        #endregion
    }
}
