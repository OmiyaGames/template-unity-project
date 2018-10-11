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
    /// 
    /// <description>10/5/2018</description>
    /// <description>Taro</description>
    /// <description>Adding new <code>TranslatedString</code>
    /// as a serialized member variable</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="TranslatedString"/>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public abstract class ITranslatedLabel<LABEL, STYLE> : MonoBehaviour, System.IDisposable where LABEL : Component
    {
        public enum LetterFormatting
        {
            None,
            UpperCase,
            LowerCase
        }

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
        /// Translations to plop onto the label.
        /// </summary>
        [SerializeField]
        protected TranslatedString translation = new TranslatedString();

        [Header("Optional Font Adjustments")]
        [SerializeField]
        [Tooltip("(Optional) Name of the font key, set in the Translation Manager.")]
        protected string fontKey = "";

        /// <summary>
        /// The attached label.
        /// </summary>
        private LABEL label = default(LABEL);
        /// <summary>
        /// If null, means this label is dirty
        /// </summary>
        private string cacheString = null;
        private TranslationManager.LanguageChanged languageChangedEvent = null;

        #region Properties
        public bool IsTranslating
        {
            get
            {
                return (translation.IsTranslating == true) && (Parser != null) && Parser.IsReady;
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
                return translation.TranslationKey;
            }
            set
            {
                // Check if different
                if (translation.TranslationKey != value)
                {
                    // Update variables
                    translation.TranslationKey = value;

                    // Repaint the label
                    RepaintLabel();
                }
            }
        }

        public TranslationDictionary Dictionary
        {
            get
            {
                return translation.Dictionary;
            }
            set
            {
                // Check if different
                if (translation.Dictionary != value)
                {
                    // Update variables
                    translation.Dictionary = value;

                    // Repaint the label
                    RepaintLabel();
                }
            }
        }

        /// <summary>
        /// Get or sets the arguments, if the text contains any "{0}" or similar values embedded in it.
        /// </summary>
        public object[] Arguments
        {
            get
            {
                return translation.Arguments;
            }
            set
            {
                // Update variables
                translation.Arguments = value;

                // Repaint the label
                RepaintLabel();
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

        public virtual string DisplayString
        {
            get
            {
                return translation.ToString();
            }
        }
        #endregion

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

        #region Unity Events
        public void OnEnable()
        {
            if (Application.isPlaying == true)
            {
                Setup();
            }
        }

        public void OnDestroy()
        {
            if (Application.isPlaying == true)
            {
                Dispose();
            }
        }

#if UNITY_EDITOR
        // Don't allow binding to the update event outside of the editor
        public void Update()
        {
            // ONLY update the label's text if the application is NOT playing
            // (i.e. in editing mode), and the text can be translated
            if ((Application.isPlaying == false) && (translation.IsTranslating == true))
            {
                LabelText = translation.ToString();
            }
        }
#endif
        #endregion

        /// <summary>
        /// Sets the arguments, if the text contains any "{0}" or similar values embedded in it.
        /// </summary>
        public void SetTranslationKey(TranslatedString translation)
        {
            if (translation != null)
            {
                // Set the dictionary
                this.translation.Dictionary = translation.Dictionary;

                // Set the rest of the member variables
                SetTranslationKey(translation.TranslationKey, translation.Arguments);
            }
        }

        /// <summary>
        /// Sets the arguments, if the text contains any "{0}" or similar values embedded in it.
        /// </summary>
        public void SetTranslationKey(string translationKey, params object[] args)
        {
            // Update the member variable
            translation.TranslationKey = translationKey;
            translation.Arguments = args;

            // Repaint the label
            RepaintLabel();
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
            Arguments = args;
        }

        public void Dispose()
        {
            // Check if we've binded to an event before, and if so, the source is still available
            if ((Parser != null) && (languageChangedEvent != null))
            {
                // Unbind to the event
                Parser.OnAfterLanguageChanged -= languageChangedEvent;
                languageChangedEvent = null;
            }
        }

        #region Helper Methods
        private void Setup()
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

        protected void RepaintLabel()
        {
            // Mark label as dirty and regenerate new string
            cacheString = null;

            // Update label
            UpdateLabel();
        }

        private void SetupLabelNow(TranslationManager parser)
        {
            // Confirm the parser is ready
            if ((parser != null) && (parser.IsReady == true))
            {
                // Unbind to the last event
                OnDestroy();

                // Bind to the parser's event
                languageChangedEvent = new TranslationManager.LanguageChanged(AfterLanguageChanged);
                parser.OnAfterLanguageChanged += languageChangedEvent;
            }

            // Update the label
            UpdateLabelNow(parser);
        }

        private void UpdateLabelNow(TranslationManager parser)
        {
            // Confirm the parser is ready
            if (IsTranslating == true)
            {
                // Check if the original string needs to be updated
                if (string.IsNullOrEmpty(cacheString) == true)
                {
                    // Update the original string
                    cacheString = DisplayString;
                }

                // Set the label's text
                LabelText = cacheString;

                // Set the label's font
                UpdateFont(parser);

                // Indicate the label has been updated
                CurrentState = State.Ready;
            }
        }

        private void AfterLanguageChanged(TranslationManager source, string lastLanguage, string currentLanguage)
        {
            // Repaint the label
            RepaintLabel();
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
