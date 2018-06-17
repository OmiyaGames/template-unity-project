using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AudioFinder.cs" company="Omiya Games">
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
    /// <date>6/1/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Set the translation on a <code>TextMeshProUGUI</code> component.
    /// </summary>
    /// <seealso cref="TextMeshProUGUI"/>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [DisallowMultipleComponent]
    public class TranslatedTextMeshPro : MonoBehaviour
    {
        static readonly HashSet<TranslatedTextMeshPro> allTranslationScripts = new HashSet<TranslatedTextMeshPro>();

        public static IEnumerable<TranslatedTextMeshPro> AllTranslationScripts
        {
            get
            {
                return allTranslationScripts;
            }
        }

        private static TranslationManager Parser
        {
            get
            {
                return Singleton.Get<TranslationManager>();
            }
        }

        /// <summary>
        /// The key to the CSVLanguageParser.
        /// </summary>
        [SerializeField]
        [Tooltip("The key to the CSVLanguageParser.")]
        string translationKey = "";

        [Header("Optional Font Adjustments")]
        [SerializeField]
        [Tooltip("(Optional) Name of the font key, set in the Translation Manager.")]
        string fontKey = "";

        /// <summary>
        /// The attached label.
        /// </summary>
        TextMeshProUGUI label = null;
        object[] arguments = null;
        string originalString = null;
        System.Action<float> bindedToSingleton = null;

        #region Properties
        public bool IsTranslating
        {
            get
            {
                return (string.IsNullOrEmpty(TranslationKey) == false) && (Parser != null) && (Parser.ContainsKey(TranslationKey) == true);
            }
        }

        /// <summary>
        /// Gets the <c>Text</c> component.
        /// </summary>
        /// <value>The label.</value>
        public TextMeshProUGUI Label
        {
            get
            {
                if (label == null)
                {
                    // Grab the label component
                    label = GetComponent<TextMeshProUGUI>();
                }
                return label;
            }
        }

        /// <summary>
        /// Gets or sets the translation key, defined on the left-most column of the translation CSV file under <code>TranslationManager</code>.
        /// This property will overwrite <code>CurrentText</code> property.
        /// </summary>
        public string TranslationKey
        {
            get
            {
                return translationKey;
            }
            set
            {
                // Update variables
                translationKey = value;
                originalString = null;

                // Update dictionary
                if (IsTranslating == true)
                {
                    // Add this script to the dictionary
                    if (allTranslationScripts.Contains(this) == false)
                    {
                        allTranslationScripts.Add(this);
                    }
                }
                else if (allTranslationScripts.Contains(this) == true)
                {
                    // Remove this script from the dictionary
                    allTranslationScripts.Remove(this);
                }

                // Update the label
                UpdateLabelOnNextFrame();
            }
        }

        /// <summary>
        /// Get or sets the arguments, if the text contains any "{0}" or similar values embedded in it.
        /// </summary>
        public object[] Arguments
        {
            get
            {
                return arguments;
            }
            set
            {
                SetArguments(value);
            }
        }

        /// <summary>
        /// Gets or sets the label's text directly.
        /// This property will set <code>TranslationKey</code> property to null.
        /// </summary>
        public string CurrentText
        {
            get
            {
                return Label.text;
            }
            set
            {
                Label.text = value;
                TranslationKey = null;
            }
        }

        /// <summary>
        /// Gets or sets the style of the label's font directly.
        /// </summary>
        public FontStyles CurrentStyle
        {
            get
            {
                return Label.fontStyle;
            }
            set
            {
                Label.fontStyle = value;
                UpdateFont();
            }
        }

        /// <summary>
        /// Gets or sets the font key, described under the <code>TranslationManager</code> for each language's font settings.
        /// </summary>
        public string FontKey
        {
            get
            {
                return fontKey;
            }
            set
            {
                fontKey = value;
                UpdateFont();
            }
        }
        #endregion

        void Start()
        {
            if (IsTranslating == true)
            {
                // Add this script to the dictionary
                allTranslationScripts.Add(this);

                // Update the label
                UpdateLabelOnNextFrame();
            }
        }

        void OnDestroy()
        {
            if (IsTranslating == true)
            {
                // Remove this script from the dictionary
                allTranslationScripts.Remove(this);
            }
            if(bindedToSingleton != null)
            {
                // Unbind to OnUpdate
                Singleton.Instance.OnUpdate -= bindedToSingleton;
                bindedToSingleton = null;
            }
        }

        /// <summary>
        /// Sets the arguments, if the text contains any "{0}" or similar values embedded in it.
        /// </summary>
        public void SetTranslationKey(string translationKey, params object[] args)
        {
            // Update the member variable
            arguments = args;
            TranslationKey = translationKey;
        }

        /// <summary>
        /// Forces the label's text to be updated based on the new arguments
        /// </summary>
        public void UpdateLabel()
        {
            if(enabled == true)
            {
                UpdateLabelNow();
            }
            else
            {
                UpdateLabelOnNextFrame();
            }
        }

        /// <summary>
        /// Sets the arguments, if the text contains any "{0}" or similar values embedded in it.
        /// </summary>
        public void SetArguments(params object[] args)
        {
            // Update the member variable
            arguments = args;

            // Update the label
            UpdateLabelOnNextFrame();
        }

        #region Helper Methods
        void UpdateFont()
        {
            if (Parser != null)
            {
                TranslationManager.FontMap map = Parser.CurrentLanguageFont;
                if (map != null)
                {
                    Label.font = map.GetFontAsset(fontKey);
                }
            }
        }

        void UpdateLabelOnNextFrame()
        {
            if((Singleton.Instance != null) && (bindedToSingleton == null))
            {
                bindedToSingleton = new System.Action<float>(OnEveryFrame);
                Singleton.Instance.OnUpdate += bindedToSingleton;
            }
        }

        void UpdateLabelNow()
        {
            // Check if the original string needs to be updated
            if ((Parser != null) && (string.IsNullOrEmpty(originalString) == true))
            {
                originalString = CurrentText;
                if (IsTranslating == true)
                {
                    originalString = Parser[TranslationKey];
                }
            }

            // Check if there's any formatting involved
            string displayString = originalString;
            if ((arguments != null) && (arguments.Length > 0))
            {
                // Format the string based on the translation and arguments
                displayString = string.Format(displayString, arguments);
            }

            // Set the label's text
            Label.text = displayString;

            // Set the label's font
            UpdateFont();
        }

        /// <summary>
        /// Event called every frame
        /// </summary>
        void OnEveryFrame(float deltaTime)
        {
            if(enabled == true)
            {
                // Update label
                UpdateLabelNow();

                // Unbind to OnUpdate
                if (bindedToSingleton != null)
                {
                    Singleton.Instance.OnUpdate -= bindedToSingleton;
                    bindedToSingleton = null;
                }
            }
        }
        #endregion
    }
}
