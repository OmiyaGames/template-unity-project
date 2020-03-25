using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using System.Collections.Generic;
using OmiyaGames.Translations;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslationCollectionEditor.cs" company="Omiya Games">
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
    /// <date>9/20/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper editor class to display <code>TranslationDictionary.TranslationCollection</code>.
    /// </summary>
    /// <seealso cref="TranslationDictionaryEditor"/>
    /// <seealso cref="OmiyaGames.Translations.TranslationDictionary.TranslationCollection"/>
    public class TranslationCollectionEditor : System.IDisposable
    {
        const float VerticalMargin = 2;
        const float VerticalSpace = 4;
        const float KeyLength = 30f;
        const float ExpandTranslationsLeft = 14f;

        readonly UnityEditor.Editor editor;
        SerializedProperty element;
        readonly AnimBool showHelpBox;
        readonly AnimBool showAllTranslationsList;
        SerializedProperty allTranslationsProperty;
        ReorderableList translationsList;
        readonly Dictionary<int, int> languageFrequency = new Dictionary<int, int>();
        readonly List<LanguageTextPairEditor> allTranslationsEditors = new List<LanguageTextPairEditor>();
        SupportedLanguages supportedLanguages;

        public TranslationCollectionEditor(UnityEditor.Editor editor, SerializedProperty element, SupportedLanguages supportedLanguages)
        {
            // Setup member variables
            this.editor = editor;
            Element = element;
            this.supportedLanguages = supportedLanguages;

            // Setup the bools
            EditorUiUtility.CreateBool(editor, ref showHelpBox);
            EditorUiUtility.CreateBool(editor, ref showAllTranslationsList);
        }

        #region Properties
        public AnimBool ShowHelpBox => showHelpBox;

        public AnimBool ShowAllTranslationsList => showAllTranslationsList;

        public SerializedProperty KeyProperty
        {
            get;
            private set;
        }

        public SerializedProperty AllTranslationsProperty
        {
            get
            {
                return allTranslationsProperty;
            }
            private set
            {
                allTranslationsProperty = value;
                translationsList = new ReorderableList(Element.serializedObject, allTranslationsProperty, true, false, true, true);
                translationsList.drawElementCallback = DrawTranslationsListElement;
                translationsList.elementHeightCallback = CalculateTranslationsListElementHeight;
                translationsList.onAddCallback = OnAddTranslation;
                translationsList.onRemoveCallback = OnRemoveTranslation;
                //translationsList.onReorderCallbackWithDetails = OnReorderTranslationList;
                translationsList.headerHeight = VerticalSpace;
            }
        }

        /// <summary>
        /// This is a hack variable.  Can't think of a better way to retrieve the width of the inside of the reorderablelist
        /// </summary>
        private float Width
        {
            get;
            set;
        }

        private string LastMessage
        {
            get;
            set;
        }

        public SupportedLanguages SupportedLanguages
        {
            get
            {
                return supportedLanguages;
            }
            set
            {
                supportedLanguages = value;
                foreach (LanguageTextPairEditor editor in allTranslationsEditors)
                {
                    editor.SupportedLanguages = supportedLanguages;
                }
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
                KeyProperty = element.FindPropertyRelative("key");
                AllTranslationsProperty = element.FindPropertyRelative("allTranslations");
                allTranslationsEditors.Clear();
            }
        }
        #endregion

        public void Update(SerializedProperty element)
        {
            Element = element;
        }

        public float CalculateHeight(Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Calculate the key height
            float height = VerticalMargin;
            height += EditorGUIUtility.singleLineHeight;
            height += VerticalSpace;

            // Check if we're showing a warning
            if ((ShowHelpBox.target == true) || (ShowHelpBox.isAnimating == true))
            {
                // If so, calculate the height of this warning
                height += EditorUiUtility.GetHelpBoxHeight(LastMessage, Width) * ShowHelpBox.faded;
                height += VerticalSpace;
            }

            // Add one for the fold-out
            height += EditorGUIUtility.singleLineHeight;

            // Check if we're showing the translations
            if ((ShowAllTranslationsList.target == true) || (ShowAllTranslationsList.isAnimating == true))
            {
                // If so, calculate the height of translations
                height += (translationsList.GetHeight() + VerticalMargin) * ShowAllTranslationsList.faded;
            }
            height += VerticalMargin;
            return height;
        }

        public void DrawGui(Rect rect, Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Update the width variable
            Width = rect.width;

            // Draw the key field
            rect.y += VerticalMargin;
            DrawKeyField(ref rect, frequencyInKeyAppearance);

            // Draw the warning, if any
            rect.y += VerticalSpace;
            if (DrawWarningMessage(ref rect, frequencyInKeyAppearance) == true)
            {
                // If there are, add an extra margin
                rect.y += VerticalSpace;
            }

            // Draw the translation list
            DrawAllTranslations(ref rect);
        }

        public static void AddKeyToFrequencyDictionary(Dictionary<string, int> frequencyInKeyAppearance, string key)
        {
            // Make sure argument is correct
            if (frequencyInKeyAppearance != null)
            {
                if (string.IsNullOrEmpty(key) == false)
                {
                    // Add this key to the dictionary
                    if (frequencyInKeyAppearance.ContainsKey(key) == false)
                    {
                        frequencyInKeyAppearance.Add(key, 1);
                    }
                    else
                    {
                        frequencyInKeyAppearance[key] += 1;
                    }
                }
            }
        }

        public static void RemoveKeyFromFrequencyDictionary(Dictionary<string, int> frequencyInKeyAppearance, string key)
        {
            // Make sure argument is correct
            if (frequencyInKeyAppearance != null)
            {
                if ((string.IsNullOrEmpty(key) == true) && (frequencyInKeyAppearance.ContainsKey(key) == true))
                {
                    // Remove this key from the dictionary
                    frequencyInKeyAppearance[key] -= 1;
                    if (frequencyInKeyAppearance[key] <= 0)
                    {
                        // Remove the key if the value is below 0
                        frequencyInKeyAppearance.Remove(key);
                    }
                }
            }
        }

        public void Dispose()
        {
            //showHelpBox.valueChanged.RemoveListener(editor.Repaint);
            //showPreview.valueChanged.RemoveListener(editor.Repaint);
        }

        #region Helper Methods
        private void DrawKeyField(ref Rect rect, Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Hold onto the original rect position
            float originalX = rect.x;
            float originalWidth = rect.width;

            // Adjust the values 
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = KeyLength;

            // Draw the key label
            EditorGUI.LabelField(rect, "Key");

            // Draw the key text field
            rect.x += rect.width + VerticalSpace;
            rect.width = originalWidth - (KeyLength + VerticalSpace);
            string oldKey = KeyProperty.stringValue;
            KeyProperty.stringValue = EditorGUI.DelayedTextField(rect, oldKey);

            // Check if there's a difference
            if (KeyProperty.stringValue != oldKey)
            {
                // Update dictionary
                RemoveKeyFromFrequencyDictionary(frequencyInKeyAppearance, oldKey);
                AddKeyToFrequencyDictionary(frequencyInKeyAppearance, KeyProperty.stringValue);

                // Indicate this dictionary needs to be updated
                if (Element == null)
                {
                    // Do something
                }
                else if(Element.serializedObject == null)
                {
                    // Do something
                }
                else if(Element.serializedObject.context == null)
                {
                    // Do something
                }
                else
                {
                    UnityEditor.EditorUtility.SetDirty(Element.serializedObject.context);
                }
            }

            // Re-adjust the rectangle, full-width for the next part
            rect.x = originalX;
            rect.y += rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = originalWidth;
        }

        private bool DrawWarningMessage(ref Rect rect, Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Adjust the bools
            LastMessage = GetWarning(frequencyInKeyAppearance);
            ShowHelpBox.target = (string.IsNullOrEmpty(LastMessage) == false);

            bool isShown = ((ShowHelpBox.target == true) || (ShowHelpBox.isAnimating == true));
            if (isShown == true)
            {
                // Calculate range of warning
                float helpBoxHeight = EditorUiUtility.GetHelpBoxHeight(LastMessage, rect.width);
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

        private bool DrawAllTranslations(ref Rect rect)
        {
            bool isFoldedOut = false;

            // Draw the fold out
            rect.height = EditorGUIUtility.singleLineHeight;
            ShowAllTranslationsList.target = EditorGUI.Foldout(rect, ShowAllTranslationsList.target, "Translations");
            rect.y += rect.height;

            // Check if we want to draw the list
            isFoldedOut = ((ShowAllTranslationsList.target == true) || (ShowAllTranslationsList.isAnimating == true));
            if (isFoldedOut == true)
            {
                // Expand the list to the left
                rect.width += ExpandTranslationsLeft;
                rect.x -= ExpandTranslationsLeft;

                // Calculate range of list
                float previewHeight = translationsList.GetHeight();
                rect.height = previewHeight * ShowAllTranslationsList.faded;

                // Draw the translations list
                GUI.BeginGroup(rect);
                Rect previewBox = new Rect(0, 0, rect.width, previewHeight);
                UpdateAllTranslationsEditors();
                translationsList.DoList(previewBox);
                GUI.EndGroup();

                // Adjust the rectangle
                rect.width -= ExpandTranslationsLeft;
                rect.x += ExpandTranslationsLeft;
                rect.y += rect.height;
            }
            return isFoldedOut;
        }

        private string GetWarning(Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Check what warning to display
            string message = null;
            if (string.IsNullOrEmpty(KeyProperty.stringValue) == true)
            {
                message = "Key cannot be an empty string.";
            }
            else if (frequencyInKeyAppearance[KeyProperty.stringValue] > 1)
            {
                message = "Multiple keys with the same name exists in this set.";
            }

            return message;
        }
        #endregion

        #region ReorderableList
        //private void OnReorderTranslationList(ReorderableList list, int oldIndex, int newIndex)
        //{
        //    throw new NotImplementedException();
        //}

        private void OnRemoveTranslation(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            UpdateAllTranslationsEditors();
        }

        private void OnAddTranslation(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            UpdateAllTranslationsEditors();
        }

        private float CalculateTranslationsListElementHeight(int index)
        {
            if (translationsList.count != allTranslationsEditors.Count)
            {
                UpdateAllTranslationsEditors();
            }
            float returnHeight = allTranslationsEditors[index].CalculateHeight(languageFrequency);
            //Debug.Log("( " + index + "->" + returnHeight + " )");
            return returnHeight;
        }

        private void DrawTranslationsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (translationsList.count != allTranslationsEditors.Count)
            {
                UpdateAllTranslationsEditors();
            }
            allTranslationsEditors[index].DrawGui(rect, languageFrequency);
        }
        #endregion

        void UpdateAllTranslationsEditors()
        {
            SerializedProperty element;
            while (translationsList.count > allTranslationsEditors.Count)
            {
                element = translationsList.serializedProperty.GetArrayElementAtIndex(allTranslationsEditors.Count);
                allTranslationsEditors.Add(new LanguageTextPairEditor(editor, element, SupportedLanguages));
            }
            while (translationsList.count < allTranslationsEditors.Count)
            {
                allTranslationsEditors[allTranslationsEditors.Count - 1].Dispose();
                allTranslationsEditors.RemoveAt(allTranslationsEditors.Count - 1);
            }

            languageFrequency.Clear();
            foreach (LanguageTextPairEditor editor in allTranslationsEditors)
            {
                LanguageTextPairEditor.AddLanguageToFrequencyDictionary(languageFrequency, editor.LanguageIndexProperty.intValue);
            }
        }
    }
}
