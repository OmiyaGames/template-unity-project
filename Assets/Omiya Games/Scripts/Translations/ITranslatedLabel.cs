using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ITranslatedLabel.cs" company="Omiya Games">
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
    /// <date>9/11/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Set the translation on a graphical label component.
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
    /// <description>9/11/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    public abstract class ITranslatedLabel<LABEL, STYLE> : MonoBehaviour where LABEL : ILayoutElement
    {
        public enum State
        {
            NeedSetup,
            WaitingForSetup,
            NeedUpdate,
            WaitingForUpdate,
            Ready
        }

        protected static TranslationManager Parser
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
        private LABEL label = default(LABEL);
        private object[] arguments = null;
        private string originalString = null;
        private TranslationManager.LanguageChanged langaugeChangedEvent = null;

        #region Properties
        public bool IsTranslating
        {
            get
            {
                return (string.IsNullOrEmpty(TranslationKey) == false) && (Parser != null) && Parser.IsReady && Parser.ContainsKey(TranslationKey);
            }
        }

        /// <summary>
        /// Gets the <c>Text</c> component.
        /// </summary>
        /// <value>The label.</value>
        public LABEL Label
        {
            get
            {
                if (label == null)
                {
                    // Grab the label component
                    label = GetComponent<LABEL>();
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

                // Update the label
                UpdateLabel();
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
                return LabelText;
            }
            set
            {
                LabelText = value;
                TranslationKey = null;
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
                UpdateFont(Parser);
            }
        }

        public State CurrentState
        {
            get;
            private set;
        } = State.NeedSetup;

        /// <summary>
        /// Gets or sets the style of the label's font directly.
        /// </summary>
        public STYLE CurrentStyle
        {
            get
            {
                return LabelFontStyle;
            }
            set
            {
                LabelFontStyle = value;
                UpdateFont(Parser);
            }
        }
        #endregion

        #region Abstract Properties and Methods
        /// <summary>
        /// Gets or sets the style of the label's font directly.
        /// Override to adjust the behavior of this script.
        /// </summary>
        public abstract STYLE LabelFontStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text of the label directly.
        /// Override to adjust the behavior of this script.
        /// </summary>
        protected abstract string LabelText
        {
            get;
            set;
        }

        protected abstract void UpdateFont(TranslationManager.FontMap fontMap, string fontKey);
        #endregion

        #region Unity Events
        public void OnEnable()
        {
            if (CurrentState == State.NeedSetup)
            {
                CurrentState = State.WaitingForSetup;
                TranslationManager.RunWhenReady(SetupLabelNow);
            }
            else if (CurrentState == State.NeedUpdate)
            {
                CurrentState = State.WaitingForUpdate;
                UpdateLabelNow(Parser);
            }
        }

        public void OnDestroy()
        {
            // Check if we've binded to an event before, and if so, the source is still available
            if ((Parser != null) && (langaugeChangedEvent != null))
            {
                // Unbind to the event
                Parser.OnAfterLanguageChanged -= langaugeChangedEvent;
                langaugeChangedEvent = null;
            }
        }
        #endregion

        /// <summary>
        /// Sets the arguments, if the text contains any "{0}" or similar values embedded in it.
        /// </summary>
        public void SetTranslationKey(TranslatedString translation)
        {
            if (translation != null)
            {
                SetTranslationKey(translation.TranslationKey, translation.Values);
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
            CurrentState = State.NeedUpdate;
            if (isActiveAndEnabled == true)
            {
                UpdateLabelNow(Parser);
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
            UpdateLabel();
        }

        #region Helper Methods
        private void SetupLabelNow(TranslationManager parser)
        {
            // Confirm the parser is ready
            if ((parser != null) && (parser.IsReady == true))
            {
                // Unbind to the last event
                OnDestroy();

                // Bind to the parser's event
                langaugeChangedEvent = new TranslationManager.LanguageChanged(AfterLanguageChanged);
                parser.OnAfterLanguageChanged += langaugeChangedEvent;
            }

            // Update the label
            UpdateLabelNow(parser);
        }

        private void UpdateLabelNow(TranslationManager parser)
        {
            // Confirm the parser is ready
            if ((parser != null) && (parser.IsReady == true) && (string.IsNullOrEmpty(translationKey) == false))
            {
                // Check if the original string needs to be updated
                if (string.IsNullOrEmpty(originalString) == true)
                {
                    // Update the original string
                    originalString = CurrentText;
                    if (IsTranslating == true)
                    {
                        originalString = parser[TranslationKey];
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
                LabelText = displayString;

                // Set the label's font
                UpdateFont(parser);

                // Indicate the label has been updated
                CurrentState = State.Ready;
            }
        }

        private void AfterLanguageChanged(TranslationManager source, string lastLanguage, string currentLanguage)
        {
            // Remove the original string, as it's no longer the correct language.
            originalString = null;

            // Update the label
            UpdateLabel();
        }

        private void UpdateFont(TranslationManager parser)
        {
            if (parser != null)
            {
                TranslationManager.FontMap map = parser.CurrentLanguageFont;
                if (map != null)
                {
                    UpdateFont(map, fontKey);
                }
            }
        }
        #endregion
    }
}
