using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace OmiyaGames
{
    /// <summary>
    /// Set translation text.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class TranslatedText : MonoBehaviour
    {
        static readonly HashSet<TranslatedText> allTranslationScripts = new HashSet<TranslatedText>();

        public enum LetterFormatting
        {
            None,
            UpperCase,
            LowerCase
        }

        public static IEnumerable<TranslatedText> AllTranslationScripts
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

        [Header("Optional Formatting")]
        [SerializeField]
        [Tooltip("(Optional) Any extra formatting one might want to add to a translated text (e.g. \"<b>{0}</b>\" will create a bolded text. Leave it blank for no formatting.")]
        string extraFormatting = "";
        [SerializeField]
        [Tooltip("(Optional) Any extra formatting one might want to add to a translated text (e.g. \"<b>{0}</b>\" will create a bolded text). Leave it blank for no formatting.")]
        LetterFormatting letterFormatting = LetterFormatting.None;

        [Header("Optional Font Adjustments")]
        [SerializeField]
        [Tooltip("(Optional) Name of the font key, set in the Translation Manager.")]
        string fontKey = "";
        [SerializeField]
        [Tooltip("(Optional) If checked, sets the font based on the label's style.")]
        bool changeFontOnStyle = false;

        /// <summary>
        /// The attached label.
        /// </summary>
        Text label = null;
        object[] arguments = null;
        string originalString = null;
        bool bindedToSingleton = false;

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
        public Text Label
        {
            get
            {
                if (label == null)
                {
                    // Grab the label component
                    label = GetComponent<Text>();
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
        public FontStyle CurrentStyle
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
        /// Gets or sets the letter styling (e.g. all-uppercase, all-lowercase, or custom).
        /// </summary>
        public LetterFormatting LetterStyle
        {
            get
            {
                return letterFormatting;
            }
            set
            {
                letterFormatting = value;
                UpdateLabelOnNextFrame();
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

        /// <summary>
        /// Gets or sets the extra text formatting
        /// (e.g. "<b>{0}</b>" will create a bolded text).
        /// Leave it blank for no formatting.
        /// </summary>
        public string ExtraFormatting
        {
            get
            {
                return extraFormatting;
            }
            set
            {
                extraFormatting = value;
                UpdateLabelOnNextFrame();  
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
            if(bindedToSingleton == true)
            {
                // Unbind to OnUpdate
                bindedToSingleton = false;
                Singleton.Instance.OnUpdate -= OnEveryFrame;
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
            TranslationManager.FontMap map = Parser.CurrentLanguageFont;
            if(map != null)
            {
                if(changeFontOnStyle == true)
                {
                    Label.font = map.GetFont(fontKey, Label.fontStyle);
                }
                else
                {
                    Label.font = map.GetFont(fontKey);
                }
            }
        }

        void UpdateLabelOnNextFrame()
        {
            if(bindedToSingleton == false)
            {
                Singleton.Instance.OnUpdate += OnEveryFrame;
                bindedToSingleton = true;
            }
        }

        void UpdateLabelNow()
        {
            // Check if the original string needs to be updated
            if (string.IsNullOrEmpty(originalString) == true)
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
            if (string.IsNullOrEmpty(extraFormatting) == false)
            {
                // Format the string based on extra formatting
                displayString = string.Format(extraFormatting, displayString);
            }
            switch (letterFormatting)
            {
                // Format the string based on extra formatting
                case LetterFormatting.UpperCase:
                    displayString = displayString.ToUpper();
                    break;
                case LetterFormatting.LowerCase:
                    displayString = displayString.ToLower();
                    break;
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
                bindedToSingleton = false;
                Singleton.Instance.OnUpdate -= OnEveryFrame;
            }
        }
        #endregion
    }
}
