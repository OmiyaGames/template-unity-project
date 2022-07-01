using UnityEngine;
using System.Collections.Generic;
using TMPro;
using OmiyaGames.Saves;
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
    /// <description>Initial version</description>
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
        public delegate void LanguageChanged(TranslationManager source, string lastLanguage, string currentLanguage);
        public event LanguageChanged OnBeforeLanguageChanged;
        public event LanguageChanged OnAfterLanguageChanged;

        /// <summary>
        /// A queue of setup methods that needs to run when setup of the translation manager is finished.
        /// </summary>
        private static event System.Action<TranslationManager> OnReady = null;

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

        [Tooltip("Maps a font to a header in the CSV file.")]
        [SerializeField]
        SupportedLanguages languages = null;
        [Tooltip("Maps a font to a header in the CSV file.")]
        [SerializeField]
        FontMap[] fontMap = null;

        bool isReady = false;

        readonly Dictionary<string, FontMap> fontDictionary = new Dictionary<string, FontMap>();

        #region Overrides
        /// <summary>
        /// Called when the first scene is loaded.
        /// </summary>
        public override void SingletonAwake()
        {
            // Do nothing
        }

        /// <summary>
        /// Called when any scene after the first one is loaded.
        /// </summary>
        public override void SceneAwake()
        {
            // Check if we've populated any translations
            if (IsReady == false)
            {
                // Update languages
                SetupDefaultLanguage();
                SetupCurrentLanguage();

                // Indicate final results
                IsReady = true;
            }
        }
        #endregion

        #region Properties
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
                return Singleton.Get<GameSettings>().Language;
            }
            set
            {
                // Check to see if this is a new language
                if (CurrentLanguage != value)
                {
                    // Check if this language is supported
                    if (languages.Contains(value) == true)
                    {
                        // Call event
                        OnBeforeLanguageChanged?.Invoke(this, CurrentLanguage, value);

                        // Update settings
                        string lastLanguage = CurrentLanguage;
                        Singleton.Get<GameSettings>().Language = value;

                        // Call the event that the langauge changed
                        OnAfterLanguageChanged?.Invoke(this, lastLanguage, CurrentLanguage);
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
        #endregion

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
        void SetupDefaultLanguage()
        {
            if (string.IsNullOrEmpty(DefaultLanguage) == true)
            {
                string language;
                int index;
                languages.GetDefaultLanguage(out language, out index);
                DefaultLanguage = language;
            }
        }

        void SetupCurrentLanguage()
        {
            // Check if the current langauge is set
            if ((string.IsNullOrEmpty(CurrentLanguage) == true) || (languages.Contains(CurrentLanguage) == false))
            {
                // If not, use the default language instead
                CurrentLanguage = DefaultLanguage;

                // Update the settings
                Singleton.Get<GameSettings>().Language = CurrentLanguage;
            }
        }
        #endregion
    }
}
