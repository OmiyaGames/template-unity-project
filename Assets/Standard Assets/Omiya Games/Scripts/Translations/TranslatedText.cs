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
        static CSVLanguageParser parser = null;

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

        private static CSVLanguageParser Parser
        {
            get
            {
                if (parser == null)
                {
                    parser = Singleton.Get<CSVLanguageParser>();
                }
                return parser;
            }
        }

        /// <summary>
        /// The key to the CSVLanguageParser.
        /// </summary>
        [SerializeField]
        [Tooltip("The key to the CSVLanguageParser.")]
        string translationKey = "";
        [SerializeField]
        [Tooltip("(Optional) Any extra formatting one might want to add to a translated text (e.g. \"<b>{0}</b>\" will create a bolded text. Leave it blank for no formatting.")]
        string extraFormatting = "";
        [SerializeField]
        [Tooltip("(Optional) Any extra formatting one might want to add to a translated text (e.g. \"<b>{0}</b>\" will create a bolded text. Leave it blank for no formatting.")]
        LetterFormatting letterFormatting = LetterFormatting.None;

        /// <summary>
        /// The attached label.
        /// </summary>
        Text label = null;
        object[] formatArgs = null;
        string displayString = null;

        public bool IsTranslating
        {
            get
            {
                return (string.IsNullOrEmpty(translationKey) == false);
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
        /// Gets or sets the translation key.
        /// </summary>
        /// <value>The translation key.</value>
        public string TranslationKey
        {
            get
            {
                return translationKey;
            }
            set
            {
                translationKey = value;
                if (IsTranslating == true)
                {
                    // Add this script to the dictionary
                    if (allTranslationScripts.Contains(this) == false)
                    {
                        allTranslationScripts.Add(this);
                    }

                    // Update the label
                    UpdateLabel();
                }
                else if (allTranslationScripts.Contains(this) == true)
                {
                    // Remove this script from the dictionary
                    allTranslationScripts.Remove(this);
                }
            }
        }

        void Start()
        {
            if (IsTranslating == true)
            {
                // Add this script to the dictionary
                allTranslationScripts.Add(this);

                // Update the label
                UpdateLabel();
            }
        }

        void OnDestroy()
        {
            if (IsTranslating == true)
            {
                // Remove this script from the dictionary
                allTranslationScripts.Remove(this);
            }
        }

        public void UpdateLabel()
        {
            // Check if there's a CSV parser
            if ((enabled == true) && (Parser != null) && (Parser.ContainsKey(TranslationKey) == true))
            {
                // By default, find the key's translation
                displayString = Parser[TranslationKey];

                // Check if there's any formatting involved
                if ((formatArgs != null) && (formatArgs.Length > 0))
                {
                    // Format the string based on the translation and arguments
                    displayString = string.Format(displayString, formatArgs);
                }
                if(string.IsNullOrEmpty(extraFormatting) == false)
                {
                    // Format the string based on extra formatting
                    displayString = string.Format(extraFormatting, displayString);
                }
                switch(letterFormatting)
                {
                    // Format the string based on extra formatting
                    case LetterFormatting.UpperCase:
                        displayString = displayString.ToUpper();
                        break;
                    case LetterFormatting.LowerCase:
                        displayString = displayString.ToLower();
                        break;
                }

                // Set the label
                Label.text = displayString;
            }
        }

        public void SetLabelFormat(params object[] args)
        {
            // Update the member variable
            formatArgs = args;

            // Update the label
            UpdateLabel();
        }
    }
}
