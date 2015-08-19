using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

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
        string translationKey = "";

        /// <summary>
        /// The attached label.
        /// </summary>
        Text label = null;
        object[] formatArgs = null;
        StringBuilder formatBuilder = null;

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
                // check if there's any formatting involved
                if ((formatArgs != null) && (formatArgs.Length > 0))
                {
                    // Create a string builder if it hasn't already
                    if(formatBuilder == null)
                    {
                        formatBuilder = new StringBuilder();
                    }

                    // Use StringBuilder for formatting text
                    formatBuilder.Length = 0;
                    formatBuilder.AppendFormat(Parser[TranslationKey], formatArgs);

                    // Set the label to the text directly
                    Label.text = formatBuilder.ToString();
                }
                else
                {
                    // Set the label to the text directly
                    Label.text = Parser[TranslationKey];
                }
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
