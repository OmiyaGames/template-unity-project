using UnityEngine;
using System.Collections.Generic;
using TMPro;
using OmiyaGames.Settings;
using OmiyaGames.Global;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslationManager.cs" company="Omiya Games">
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
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>3/23/2015</description>
    /// <description>Dylan</description>
    /// <description>Initial verison</description>
    /// 
    /// <description>3/24/2015</description>
    /// <description>Taro</description>
    /// <description>Refactoring Documentation, replacing methods with properties and turning the script to a Singleton</description>
    /// 
    /// <description>3/25/2015</description>
    /// <description>Taro</description>
    /// <description>Adding variables for debugging</description>
    /// 
    /// <description>7/3/2015</description>
    /// <description>Taro</description>
    /// <description>Retrieving language from GameSettings</description>
    /// 
    /// <description>6/4/2018</description>
    /// <description>Taro</description>
    /// <description>Working on updating for bi-directional communication with components.</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="Singleton"/>
    [DisallowMultipleComponent]
    public class TranslationManager : ISingletonScript
    {
        // FIXME: add a static delegate stacking setup methods.
        // Also add a static function where one passes in a setup method.
        // If the translation manager is already setup, run that method directly.
        // Otherwise, queue it to the static delegate until we have verification it's setup.

        public delegate void LanguageChanged(TranslationManager source, string lastLanguage, string currentLanguage);
        public event LanguageChanged OnBeforeLanguageChanged;
        public event LanguageChanged OnAfterLanguageChanged;

        /// <summary>
        /// A queue of setup methods that needs to run when setup of the translation manager is finished.
        /// </summary>
        private static event System.Action<TranslationManager> OnReady = null;

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

        [System.Serializable]
        public struct FontAssetDetails
        {
            [SerializeField]
            string name;
            [SerializeField]
            TMP_FontAsset font;

            public string Name
            {
                get
                {
                    return name;
                }
            }

            public TMP_FontAsset Font
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

            [Header("Text Properties")]
            [SerializeField]
            [UnityEngine.Serialization.FormerlySerializedAs("defaultFont")]
            Font defaultUguiTextFont;
            [SerializeField]
            [UnityEngine.Serialization.FormerlySerializedAs("otherFonts")]
            FontMapDetails[] otherUguiTextFonts;

            [Header("TextMeshPro Properties")]
            [SerializeField]
            [UnityEngine.Serialization.FormerlySerializedAs("defaultFontAsset")]
            TMP_FontAsset defaultTextMeshProFont;
            [SerializeField]
            [UnityEngine.Serialization.FormerlySerializedAs("otherFontAssets")]
            FontAssetDetails[] otherTextMeshProFonts;

            readonly Dictionary<FontMapKey, FontMapDetails> otherFontMap = new Dictionary<FontMapKey, FontMapDetails>();
            readonly Dictionary<string, FontAssetDetails> otherFontAssetMap = new Dictionary<string, FontAssetDetails>();
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
                    return defaultUguiTextFont;
                }
            }

            public TMP_FontAsset DefaultFontAsset
            {
                get
                {
                    return defaultTextMeshProFont;
                }
            }

            public Dictionary<FontMapKey, FontMapDetails> OtherFonts
            {
                get
                {
                    if(otherFontMap.Count != otherUguiTextFonts.Length)
                    {
                        otherFontMap.Clear();
                        foreach(FontMapDetails details in otherUguiTextFonts)
                        {
                            fontSearchCache.Name = details.Name;
                            fontSearchCache.Style = details.Style;
                            otherFontMap.Add(fontSearchCache, details);
                        }
                    }
                    return otherFontMap;
                }
            }

            public Dictionary<string, FontAssetDetails> OtherTextMeshProFonts
            {
                get
                {
                    if (otherFontAssetMap.Count != otherTextMeshProFonts.Length)
                    {
                        otherFontAssetMap.Clear();
                        foreach (FontAssetDetails details in otherTextMeshProFonts)
                        {
                            otherFontAssetMap.Add(details.Name, details);
                        }
                    }
                    return otherFontAssetMap;
                }
            }

            public Font GetFontUgui(string name, FontStyle style = FontStyle.Normal)
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

            public TMP_FontAsset GetFontTextMeshPro(string name)
            {
                TMP_FontAsset returnFont = DefaultFontAsset;
                if (OtherTextMeshProFonts.ContainsKey(name) == true)
                {
                    returnFont = OtherTextMeshProFonts[name].Font;
                }
                return returnFont;
            }
        }

        [Header("Runtime Behavior")]
        [Tooltip("If true, keys that has missing values (an empty cell) will be replaced by their counterpart from the default language, or the left-most text if even that is missing.")]
        [SerializeField]
        bool retrieveDefaultsAsBackup = true;

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
        [Tooltip("Maps a system language setting to a header in the CSV file. Also defines the starting language, based on system's settings.")]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("langaugeMap")]
        LanguageMap[] systemLanguageToHeaderMap = null;
        [Tooltip("Maps a font to a header in the CSV file.")]
        [SerializeField]
        FontMap[] fontMap = null;

        /// <summary>
        /// Currently selected langauge. Defaults to the first language encountered in the file.
        /// </summary>
        string currentLanguage = "";
        bool isReady = false;

        readonly Dictionary<SystemLanguage, string> headerDictionary = new Dictionary<SystemLanguage, string>();
        readonly Dictionary<string, FontMap> fontDictionary = new Dictionary<string, FontMap>();

        #region Overrides
        /// <summary>
        /// Called when the first scene is loaded.
        /// </summary>
        internal override void SingletonAwake()
        {
            // Do nothing
        }

        /// <summary>
        /// Called when any scene after the first one is loaded.
        /// </summary>
        internal override void SceneAwake()
        {
            // Check if we've populated any translations
            if (IsReady == false)
            {
                // Parse the file
                ParseFile();

                // Indicate final results
                IsReady = true;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Retrieves a translated string.
        /// </summary>
        /// <param name="translationKey">A string corresponding to a key on the left-most column in the CSV file</param>
        /// <returns>A translated string parsed from a CSV file, based on the <seealso cref="CurrentLanguage"/>.</returns>
        public string this[string translationKey]
        {
            get
            {
                // Attempt to retrieve from the current translation
                string returnString = GetTranslatedStringForCurrentLanguage(translationKey);

                // If retrieving from defaults is enabled, and the current language didn't contain the proper string..
                if ((retrieveDefaultsAsBackup == true) && (string.IsNullOrEmpty(returnString) == true))
                {
                    // Grab the default language
                    returnString = GetTranslatedStringForDefaultanguage(translationKey);
                }
                return returnString;
            }
        }

        /// <summary>
        /// Indicates whether the translation manager is setup or not.
        /// </summary>
        public bool IsReady
        {
            get
            {
                return isReady;
            }
            private set
            {
                if(isReady != value)
                {
                    isReady = value;

                    //  Check if setup is done
                    if((isReady == true) && (OnReady != null))
                    {
                        // Run event
                        OnReady(this);

                        // Empty the method stack
                        OnReady = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the list of langauges identified in the most recent parse.
        /// </summary>
        /// <returns>The supported languages.</returns>
        public List<string> SupportedLanguages
        {
            get;
        } = new List<string>();

        /// <summary>
        /// Gets the default language.
        /// </summary>
        /// <value>The default language.</value>
        public string DefaultLanguage
        {
            get;
            private set;
        } = null;

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
                    if (SupportedLanguages.Contains(value) == true)
                    {
                        // Call event
                        OnBeforeLanguageChanged?.Invoke(this, currentLanguage, value);

                        // Set the language
                        string lastLanguage = currentLanguage;
                        currentLanguage = value;

                        // Parse the file
                        ParseFile();

                        // Update settings
                        Singleton.Get<GameSettings>().Language = currentLanguage;

                        // Call the event that the langauge changed
                        OnAfterLanguageChanged?.Invoke(this, lastLanguage, currentLanguage);
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

        Dictionary<SystemLanguage, string> HeaderDictionary
        {
            get
            {
                // Check if the language map has a different size of our cached dictionary
                if ((systemLanguageToHeaderMap != null) && (headerDictionary.Count != systemLanguageToHeaderMap.Length))
                {
                    // Setup the header dictionary
                    headerDictionary.Clear();
                    for (int i = 0; i < systemLanguageToHeaderMap.Length; ++i)
                    {
                        headerDictionary.Add(systemLanguageToHeaderMap[i].Language, systemLanguageToHeaderMap[i].Header);
                    }
                }
                return headerDictionary;
            }
        }

        /// <summary>
        /// A dictionary of the keys, current-language-values.
        /// </summary>
        Dictionary<string, string> CurrentTranslationDictionary
        {
            get;
        } = new Dictionary<string, string>();

        /// <summary>
        /// A dictionary of the keys, default-language-values.
        /// </summary>
        Dictionary<string, string> DefaultTranslationDictionary
        {
            get;
        } = new Dictionary<string, string>();
        #endregion

        public bool ContainsKey(string key)
        {
            return CurrentTranslationDictionary.ContainsKey(key);
        }

        public HashSet<string> GetAllKeys()
        {
            HashSet<string> keys = new HashSet<string>();
            foreach (KeyValuePair<string, string> pair in CurrentTranslationDictionary)
            {
                keys.Add(pair.Key);
            }
            return keys;
        }

        public string GetTranslatedStringForCurrentLanguage(string translationKey)
        {
            return GetTranslatedString(CurrentTranslationDictionary, translationKey);
        }

        public string GetTranslatedStringForDefaultanguage(string translationKey)
        {
            return GetTranslatedString(DefaultTranslationDictionary, translationKey);
        }

        public static void RunWhenReady(System.Action<TranslationManager> setupMethod)
        {
            // Make sure there's an actual method to push to the event list
            if(setupMethod != null)
            {
                // First verify that an instance of TranslationManager exists,
                // and it's ready for use.
                TranslationManager singleton = Singleton.Get<TranslationManager>();
                if((singleton != null) && (singleton.IsReady == true))
                {
                    // If the translation manager is ready,
                    // run the setup method immediately
                    setupMethod(singleton);
                }
                else
                {
                    // If not, push the method to the events list
                    OnReady += setupMethod;
                }
            }
        }

        #region Helper Methods
        static void SetupTranslationDictionary(Dictionary<string, string> dictionaryToPopulate, List<Dictionary<string, string>> data, string keyHeader, string firstLanguageHeader, params string[] backupLanguageHeaders)
        {
            // Setup loop variables
            string val;
            string key;

            // Reset the dictionary
            dictionaryToPopulate.Clear();

            // Loop through each row in the file. Grab the langauge-independet, and
            // the value based on the current langage. Put them in the dictionary.
            for (int i = 0; i < data.Count; i++)
            {
                // Default the value
                val = "";

                // Grab the value
                if (data[i].TryGetValue(firstLanguageHeader, out val) == false)
                {
                    // Default the value
                    val = "";
                    foreach (string header in backupLanguageHeaders)
                    {
                        // Retrieve the value from data
                        if ((data[i].TryGetValue(header, out val) == true) && (string.IsNullOrEmpty(val) == false))
                        {
                            break;
                        }
                    }
                }

                // Grab the key
                key = data[i][keyHeader];
                if (dictionaryToPopulate.ContainsKey(key) == false)
                {
                    // Add to the dictionary
                    dictionaryToPopulate.Add(key, val);
                }
                else
                {
                    Debug.LogError("Translation CSV file contains duplicate key: " + key);
                }
            }
        }

        static string GetTranslatedString(Dictionary<string, string> translations, string translationKey)
        {
            if (translations.ContainsKey(translationKey) == true)
            {
                return translations[translationKey];
            }
            else
            {
                throw new System.ArgumentException("The key, \"" + translationKey + "\" was not present in the CSV file.");
            }
        }

        void ParseFile()
        {
            // Check if we need to load a file from resources
            bool isFileLoaded = false;
            if ((loadFileAsset == null) && (string.IsNullOrEmpty(loadFileName) == false))
            {
                // Load from resources
                loadFileAsset = Resources.Load<TextAsset>(loadFileName);
                isFileLoaded = true;
            }

            // Make sure the file can be parsed
            if (loadFileAsset != null)
            {
                // Analyze the file, and update the member variables
                AnalyzeFile(loadFileAsset);

                // Unload the file, if we need to
                if (isFileLoaded == true)
                {
                    Resources.UnloadAsset(loadFileAsset);
                    loadFileAsset = null;
                }
            }
            else
            {
                Debug.LogWarning("No file found for TranslationManager");
            }
        }

        void AnalyzeFile(TextAsset loadFileAsset)
        {
            // Read the input file. Data is expected to be returned in a list of
            // Dictionary objects in which the position in the list corolates to the
            // row-number from the CSV file (element 0 is the second row where the
            // first row is headers), the keys in the dictionary reflect the column
            // names from the header, and the values in the dictionary reflect the
            // values for a given row/column.
            List<Dictionary<string, string>> data = CSVReader.ReadFile(loadFileAsset);

            // Check if this manager is properly setup
            if (IsReady == false)
            {
                // If not, setup the defaults
                SetupDefaults(data);
            }

            // Get the current translations
            SetupTranslationDictionary(CurrentTranslationDictionary, data, keyHeader, currentLanguage);
        }

        void SetupDefaults(List<Dictionary<string, string>> data)
        {
            // Populate the supported languages so the defaults are correct
            SupportedLanguages.Clear();
            foreach (string key in data[0].Keys)
            {
                if ((string.IsNullOrEmpty(key) == false) && (key != keyHeader))
                {
                    SupportedLanguages.Add(key);
                }
            }

            // Make sure the list is populated
            if (SupportedLanguages.Count > 0)
            {
                SetupDefaultLanguage();
                SetupCurrentLanguage();

                // Setup the default translation dictionary, taking in both default language and left-most column as backup
                SetupTranslationDictionary(DefaultTranslationDictionary, data, keyHeader, DefaultLanguage, SupportedLanguages[0]);
            }
            else
            {
                // Setup the default translation dictionary, taking in only the default language
                SetupTranslationDictionary(DefaultTranslationDictionary, data, keyHeader, DefaultLanguage);
            }
        }

        void SetupDefaultLanguage()
        {
            // Check to see if default language isn't set yet
            if (HeaderDictionary.ContainsKey(Application.systemLanguage) == true)
            {
                // Grab the default language from the system settings
                DefaultLanguage = HeaderDictionary[Application.systemLanguage];
            }

            // Check if the default language is set
            if ((string.IsNullOrEmpty(DefaultLanguage) == true) || (SupportedLanguages.Contains(DefaultLanguage) == false))
            {
                // If not, grab the first language in the headers
                DefaultLanguage = SupportedLanguages[0];
            }
        }

        void SetupCurrentLanguage()
        {
            // Retrieve the current language settings from GameSettings
            currentLanguage = Singleton.Get<GameSettings>().Language;

            // Check if the current langauge is set
            if ((string.IsNullOrEmpty(currentLanguage) == true) || (SupportedLanguages.Contains(currentLanguage) == false))
            {
                // If not, use the default language instead
                currentLanguage = DefaultLanguage;

                // Update the settings
                Singleton.Get<GameSettings>().Language = currentLanguage;
            }
        }
        #endregion
    }
}
