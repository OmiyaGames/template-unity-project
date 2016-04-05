using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames
{
    /// <summary>
    /// Set translation text.
    /// </summary>
    [RequireComponent(typeof(TextMesh))]
    public class TranslatedTextMesh : MonoBehaviour
    {
        static readonly HashSet<TranslatedTextMesh> allTranslationScripts = new HashSet<TranslatedTextMesh>();
        static TranslationManager parser = null;

        public static IEnumerable<TranslatedTextMesh> AllTranslationScripts
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
                if (parser == null)
                {
                    parser = Singleton.Get<TranslationManager>();
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
        TextMesh label = null;
        object[] formatArgs = null;

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
        public TextMesh Label
        {
            get
            {
                if (label == null)
                {
                    // Grab the label component
                    label = GetComponent<TextMesh>();
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
            if ((Parser != null) && (Parser.ContainsKey(TranslationKey) == true))
            {
                // check if there's any formatting involved
                if ((formatArgs != null) && (formatArgs.Length > 0))
                {
                    // Set the label to the text directly
                    Label.text = string.Format(Parser[TranslationKey], formatArgs);
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
