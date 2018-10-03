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
    public class LanguageTextPairEditor : System.IDisposable
    {
        const float MinHelpBoxHeight = 30f;
        const float VerticalMargin = 2;
        const float VerticalSpace = 4;
        const float KeyLength = 30f;
        const float ExpandLength = 60f;
        const float WordWrapLength = 80f;
        const float ExpandTranslationsLeft = 14f;
        const bool WordWrapEnabledDefault = false;
        static GUIStyle wrappedTextArea = null;

        readonly Editor editor;
        SerializedProperty element;
        readonly AnimBool showHelpBox;
        readonly AnimBool expandToggle;
        float width;

        public LanguageTextPairEditor(Editor editor, SerializedProperty element, SupportedLanguages supportedLanguages)
        {
            // Setup member variables
            this.editor = editor;
            this.SupportedLanguages = supportedLanguages;
            Element = element;

            // Setup the bools
            EditorUtility.CreateBool(editor, ref showHelpBox);
            EditorUtility.CreateBool(editor, ref expandToggle);
        }

        #region Properties
        public GUIStyle WrappedTextArea
        {
            get
            {
                if (wrappedTextArea == null)
                {
                    wrappedTextArea = new GUIStyle(EditorStyles.textArea);
                }
                wrappedTextArea.wordWrap = IsWordWrapEnabled;
                return wrappedTextArea;
            }
        }

        public static float PreviewHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight * 2 + VerticalSpace;
            }
        }

        public AnimBool ShowHelpBox => showHelpBox;
        public AnimBool ExpandToggle => expandToggle;

        public bool IsWordWrapEnabled
        {
            get;
            private set;
        } = WordWrapEnabledDefault;

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

        private float Width
        {
            get
            {
                return width;
            }
            set
            {
                // Not sure why we need this, but it fixes a lot of problems.
                if(value > 0)
                {
                    width = value;
                    //Debug.Log("Width: " + width + ", vs Utility: " + EditorGUIUtility.currentViewWidth);
                }
                //else
                //{
                //    Debug.Log("Trying to set width to negative");
                //}
            }
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

        public SupportedLanguages SupportedLanguages
        {
            get;
            set;
        }
        #endregion

        public void Update(SerializedProperty element)
        {
            Element = element;
        }

        public float CalculateHeight(Dictionary<int, int> frequencyInLanguageAppearance)
        {
            // Calculate the key height
            float height = VerticalMargin;
            height += EditorGUIUtility.singleLineHeight;
            height += VerticalSpace;

            // Check if we're showing a warning
            if ((ShowHelpBox.target == true) || (ShowHelpBox.isAnimating == true))
            {
                // If so, calculate the height of this warning
                height += EditorUtility.GetHelpBoxHeight(LastMessage, Width, MinHelpBoxHeight) * ShowHelpBox.faded;
                height += VerticalSpace;
            }

            // Add one for the fold-out
            height += EditorGUIUtility.singleLineHeight;

            // If so, calculate the height of translations
            bool isExpandable;
            height += GetTextAreaHeight(TextProperty.stringValue, Width, ExpandToggle.faded, out isExpandable);
            //height += VerticalMargin;
            height += VerticalMargin;
            return height;
        }

        public void DrawGui(Rect rect, Dictionary<int, int> frequencyInLanguageAppearance)
        {
            // Draw the key field
            rect.y += VerticalMargin;
            DrawKeyField(ref rect, frequencyInLanguageAppearance);

            // Draw the warning, if any
            //rect.y += VerticalSpace;
            if (DrawWarningMessage(ref rect, frequencyInLanguageAppearance) == true)
            {
                // If there are, add an extra margin
                //rect.y += VerticalSpace;
            }

            // Draw the translation list
            DrawText(ref rect);
        }

        public static void AddLanguageToFrequencyDictionary(Dictionary<int, int> frequencyInLanguageAppearance, int key)
        {
            // Make sure argument is correct
            if (frequencyInLanguageAppearance != null)
            {
                // Add this key to the dictionary
                if (frequencyInLanguageAppearance.ContainsKey(key) == false)
                {
                    frequencyInLanguageAppearance.Add(key, 1);
                }
                else
                {
                    frequencyInLanguageAppearance[key] += 1;
                }
            }
        }

        public static void RemoveLanguageFromFrequencyDictionary(Dictionary<int, int> frequencyInLanguageAppearance, int key)
        {
            // Make sure argument is correct
            if ((frequencyInLanguageAppearance != null) && (frequencyInLanguageAppearance.ContainsKey(key) == true))
            {
                // Remove this key from the dictionary
                frequencyInLanguageAppearance[key] -= 1;
                if (frequencyInLanguageAppearance[key] <= 0)
                {
                    // Remove the key if the value is below 0
                    frequencyInLanguageAppearance.Remove(key);
                }
            }
        }

        public void Dispose()
        {
            //expandToggle.valueChanged.RemoveListener(editor.Repaint);
            //showHelpBox.valueChanged.RemoveListener(editor.Repaint);
        }

        #region Helper Methods
        private void DrawKeyField(ref Rect rect, Dictionary<int, int> frequencyInLanguageAppearance)
        {
            // Hold onto the original rect position
            float originalX = rect.x;
            float originalWidth = rect.width;

            // Adjust the values 
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = KeyLength;

            // Draw the key label
            EditorGUI.LabelField(rect, "Language");

            // Draw the key text field
            rect.x += rect.width + VerticalSpace;
            rect.width = originalWidth - (KeyLength + VerticalSpace);
            EditorGUI.BeginChangeCheck();
            int oldLanguageIndex = LanguageIndexProperty.intValue;
            LanguageIndexProperty.intValue = SupportedLanguagesEditor.DrawSupportedLanguages(rect, LanguageIndexProperty, SupportedLanguages);

            // Check if there's a difference
            if (LanguageIndexProperty.intValue != oldLanguageIndex)
            {
                // Update dictionary
                RemoveLanguageFromFrequencyDictionary(frequencyInLanguageAppearance, oldLanguageIndex);
                AddLanguageToFrequencyDictionary(frequencyInLanguageAppearance, LanguageIndexProperty.intValue);

                // Testing...
                editor.serializedObject.ApplyModifiedProperties();
            }

            // Re-adjust the rectangle, full-width for the next part
            rect.x = originalX;
            rect.y += rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = originalWidth;
        }

        private bool DrawWarningMessage(ref Rect rect, Dictionary<int, int> frequencyInLanguageAppearance)
        {
            // Adjust the bools
            LastMessage = GetWarning(frequencyInLanguageAppearance);
            ShowHelpBox.target = (string.IsNullOrEmpty(LastMessage) == false);

            bool isShown = ((ShowHelpBox.target == true) || (ShowHelpBox.isAnimating == true));
            if (isShown == true)
            {
                // Calculate range of warning
                float helpBoxHeight = EditorUtility.GetHelpBoxHeight(LastMessage, rect.width, MinHelpBoxHeight);
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

        private void DrawText(ref Rect rect)
        {
            float originalX = rect.x;
            float originalWidth = rect.width;

            // Draw the label of the field
            rect.x = originalX - ExpandTranslationsLeft;
            rect.width = KeyLength;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, "Text");

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

        private string GetWarning(Dictionary<int, int> frequencyInLanguageAppearance)
        {
            // Check what warning to display
            string message = null;
            int langaugeIndex = LanguageIndexProperty.intValue;
            if ((langaugeIndex < 0) || (langaugeIndex >= SupportedLanguages.NumberOfLanguages))
            {
                message = "Language is not set to a valid value.";
            }
            else if (frequencyInLanguageAppearance[langaugeIndex] > 1)
            {
                message = "Multiple texts for the same language exists in this set.";
            }

            return message;
        }

        private float GetTextAreaHeight(string text, float viewWidth, float fadeValue, out bool isExpandable)
        {
            var content = new GUIContent(text);

            // Get the minimum and maximum measurement
            float min = PreviewHeight;
            float max = WrappedTextArea.CalcHeight(content, viewWidth);
            if (max < min)
            {
                isExpandable = false;
                return max;
            }
            else
            {
                isExpandable = true;
                return Mathf.Lerp(min, max, fadeValue);
            }
        }
        #endregion
    }
}
