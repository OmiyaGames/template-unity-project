using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using OmiyaGames.Translations;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LanguageTextPairEditor.cs" company="Omiya Games">
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
    /// <date>9/21/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper editor class to display <code>TranslationDictionary.LanguageTextPair</code>.
    /// </summary>
    /// <seealso cref="TranslationDictionaryEditor"/>
    /// <seealso cref="OmiyaGames.Translations.TranslationDictionary.LanguageTextPair"/>
    public class LanguageTextPairEditor : TranslationPreviewEditor, System.IDisposable
    {
        const float KeyLength = 30f;

        SerializedProperty element;
        readonly AnimBool showHelpBox;
        readonly AnimBool expandToggle;
        float width;

        public LanguageTextPairEditor(Editor editor, SerializedProperty element, SupportedLanguages supportedLanguages) : base(supportedLanguages, editor, "Language", "Text")
        {
            // Setup member variables
            Element = element;

            // Setup the bools
            EditorUtility.CreateBool(editor, ref showHelpBox);
            EditorUtility.CreateBool(editor, ref expandToggle);
        }

        public LanguageTextPairEditor(LanguageTextPairEditor editor) : base(editor.SupportedLanguages, editor.editor, "Language", "Text")
        {
            // Setup member variables
            Element = editor.Element;

            // Setup the bools
            EditorUtility.CreateBool(editor.editor, ref showHelpBox);
            EditorUtility.CreateBool(editor.editor, ref expandToggle);

            // Transfer variables
            IsWordWrapEnabled = editor.IsWordWrapEnabled;
            IsExpanded = editor.IsExpanded;
        }

        #region Properties
        public AnimBool ShowHelpBox => showHelpBox;
        public AnimBool ExpandToggle => expandToggle;

        public override bool SetMininmumHeightToPreview
        {
            get
            {
                return false;
            }
        }

        public SerializedProperty LanguageIndexProperty
        {
            get;
            private set;
        }

        public SerializedProperty TextProperty
        {
            get;
            private set;
        }

        private string LastMessage
        {
            get;
            set;
        }

        public SerializedProperty Element
        {
            get
            {
                return element;
            }
            private set
            {
                // Setup properties
                element = value;
                LanguageIndexProperty = element.FindPropertyRelative("languageIndex");
                TextProperty = element.FindPropertyRelative("text");
            }
        }

        public override int LanguageIndex
        {
            get
            {
                int returnIndex = 0;
                if (LanguageIndexProperty != null)
                {
                    returnIndex = LanguageIndexProperty.intValue;
                }
                return returnIndex;
            }
            set
            {
                if ((LanguageIndexProperty != null) && (LanguageIndexProperty.intValue != value))
                {
                    LanguageIndexProperty.intValue = value;
                    IsLanguageIndexChanged = true;
                }
            }
        }

        public override string Text
        {
            get
            {
                string returnText = null;
                if (TextProperty != null)
                {
                    returnText = TextProperty.stringValue;
                }
                return returnText;
            }
            set
            {
                if ((TextProperty != null) && (TextProperty.stringValue != value))
                {
                    TextProperty.stringValue = value;
                    IsTextChanged = true;
                }
            }
        }

        public override float ExpandTranslationsLeft
        {
            get
            {
                return 15f;
            }
        }

        public override bool IsExpanded
        {
            get
            {
                return ExpandToggle.target;
            }
            protected set
            {
                ExpandToggle.target = value;
            }
        }
        #endregion

        public void Update(SerializedProperty element)
        {
            Element = element;
        }

        public override float CalculateHeight(Dictionary<int, int> frequencyInLanguageAppearance)
        {
            // Calculate the key height
            float height = EditorUtility.VerticalMargin;
            height += EditorGUIUtility.singleLineHeight;
            height += VerticalSpace;

            // Check if we're showing a warning
            if ((ShowHelpBox.target == true) || (ShowHelpBox.isAnimating == true))
            {
                // If so, calculate the height of this warning
                height += EditorUtility.GetHelpBoxHeight(LastMessage, Width) * ShowHelpBox.faded;
                height += VerticalSpace;
            }

            // Add one for the fold-out
            height += EditorGUIUtility.singleLineHeight;

            // If so, calculate the height of translations
            bool isExpandable;
            height += GetTextAreaHeight(TextProperty.stringValue, Width, ExpandToggle.faded, out isExpandable);
            height += EditorUtility.VerticalMargin;
            return height;
        }

        public override void DrawGui(Rect rect, Dictionary<int, int> frequencyInLanguageAppearance, bool indent = true)
        {
            // Draw the key field
            if (indent == true)
            {
                rect.y += EditorUtility.VerticalMargin;
            }
            DrawKeyField(ref rect, indent, frequencyInLanguageAppearance);

            // Draw the warning, if any
            DrawWarningMessage(ref rect, frequencyInLanguageAppearance);

            // Draw the translation list
            DrawText(ref rect);
        }

        public void Dispose()
        {
            //expandToggle.valueChanged.RemoveListener(editor.Repaint);
            //showHelpBox.valueChanged.RemoveListener(editor.Repaint);
        }

        #region Helper Methods
        private bool DrawWarningMessage(ref Rect rect, Dictionary<int, int> frequencyInLanguageAppearance)
        {
            // Adjust the bools
            LastMessage = GetWarning(frequencyInLanguageAppearance);
            ShowHelpBox.target = (string.IsNullOrEmpty(LastMessage) == false);

            bool isShown = ((ShowHelpBox.target == true) || (ShowHelpBox.isAnimating == true));
            if (isShown == true)
            {
                // Calculate range of warning
                float helpBoxHeight = EditorUtility.GetHelpBoxHeight(LastMessage, rect.width);
                rect.height = helpBoxHeight * ShowHelpBox.faded;

                // Show warning
                GUI.BeginGroup(rect);
                Rect helpBox = new Rect(0, 0, rect.width, helpBoxHeight);
                EditorGUI.HelpBox(helpBox, LastMessage, MessageType.Warning);
                GUI.EndGroup();

                // Adjust the rectangle
                rect.y += rect.height;
            }
            return isShown;
        }

        protected override void DrawText(ref Rect rect)
        {
            float originalX = rect.x;
            float originalWidth = rect.width;

            // Draw the label of the field
            rect.x = originalX - ExpandTranslationsLeft;
            rect.width = KeyLength;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, valueLabel);

            // Calculate the Expand toggle bound (to be used later)
            rect.x = (originalX + originalWidth) - ExpandLength;
            rect.width = ExpandLength;
            Rect expandToggleRect = new Rect(rect);

            // Calculate the Word Wrap toggle bound (to be used later)
            rect.x -= (WordWrapLength + VerticalSpace);
            rect.width = WordWrapLength;

            // Draw the word-wrap toggle
            IsWordWrapEnabled = EditorGUI.ToggleLeft(rect, "Word Wrap", IsWordWrapEnabled);

            // Offset the text area
            rect.x = originalX - ExpandTranslationsLeft;
            rect.width = originalWidth + ExpandTranslationsLeft;
            Width = rect.width;
            rect.y += rect.height;

            // Calculate range of warning
            string oldText = TextProperty.stringValue;
            bool isExpandable;
            rect.height = GetTextAreaHeight(oldText, Width, ExpandToggle.faded, out isExpandable);

            // Draw the translations list
            TextProperty.stringValue = EditorGUI.TextArea(rect, oldText, WrappedTextArea);

            // Draw the toggle, enabled only if the area is expandable
            GUI.enabled = isExpandable;
            ExpandToggle.target = EditorGUI.ToggleLeft(expandToggleRect, "Expand", ExpandToggle.target);
            GUI.enabled = true;

            // Adjust the rectangle
            rect.x = originalX;
            rect.width = originalWidth;
            rect.y += rect.height;
        }
        #endregion
    }
}
