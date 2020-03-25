using UnityEditor;
using UnityEngine;
using OmiyaGames.Translations;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslatedStringEditor.cs" company="Omiya Games">
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
    /// <date>10/5/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor to make it easier to edit <code>TranslatedString</code>s.
    /// </summary>
    /// <seealso cref="TranslatedString"/>
    [CustomPropertyDrawer(typeof(TranslatedString), true)]
    public class TranslatedStringEditor : PropertyDrawer
    {
        const float ButtonHeight = 18f;
        const float IndentLeft = 16f;

        public enum Status
        {
            OK = 0,
            DictionaryNotSet,
            SupportedLanguageNotSet,
            NoKey,
            UnknownKey,
            PreviewError
        }

        #region Properties
        float Width
        {
            get;
            set;
        } = 0;

        string Message
        {
            get;
            set;
        } = null;

        MessageType MessageType
        {
            get;
            set;
        } = MessageType.Info;

        TranslationPreviewEditor TextPreview
        {
            get;
        } = new TranslationPreviewEditor(null);
        #endregion

        public static bool IsTextPreviewDrawn(Status status)
        {
            switch (status)
            {
                case Status.OK:
                case Status.PreviewError:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsButtonDrawn(Status status)
        {
            switch (status)
            {
                case Status.DictionaryNotSet:
                case Status.UnknownKey:
                case Status.OK:
                    return true;
                default:
                    return false;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded == true)
            {
                // Grab every field
                SerializedProperty key = property.FindPropertyRelative("key");
                SerializedProperty dictionary = property.FindPropertyRelative("dictionary");

                // Allocate key field
                height += EditorUiUtility.GetHeight(2);

                // Update status
                TranslationDictionary translationDictionary = dictionary.objectReferenceValue as TranslationDictionary;
                string translationKey = key.stringValue;
                Status status = UpdateMessageStatus(translationDictionary, translationKey);

                // Check status
                if (string.IsNullOrEmpty(Message) == false)
                {
                    // Allocate help box
                    height += EditorUiUtility.VerticalMargin;
                    height += EditorUiUtility.GetHelpBoxHeight(Message, Width);
                }

                // Check button
                if (IsButtonDrawn(status) == true)
                {
                    // Allocate button
                    height += EditorUiUtility.VerticalMargin;
                    height += ButtonHeight;
                }

                // Check preview
                if (IsTextPreviewDrawn(status) == true)
                {
                    // Allocate preview
                    height += EditorUiUtility.VerticalSpace;
                    height += EditorGUIUtility.singleLineHeight;
                    height += TextPreview.CalculateHeight(null, !translationDictionary.IsAllTranslationsSerialized);
                }
            }
            return height;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Grab every field
            SerializedProperty key = property.FindPropertyRelative("key");
            SerializedProperty dictionary = property.FindPropertyRelative("dictionary");

            using (EditorGUI.PropertyScope scope = new EditorGUI.PropertyScope(rect, label, property))
            {
                // Calculate height
                float previewHeight = rect.height;
                rect.height = EditorGUIUtility.singleLineHeight;
                previewHeight -= rect.height;
                Width = rect.width;

                // Draw label of object
                property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, scope.content);
                if (property.isExpanded == true)
                {
                    // Indent
                    using (EditorGUI.IndentLevelScope indent = new EditorGUI.IndentLevelScope())
                    {
                        // Draw the properties regularly
                        rect = DrawFields(rect, key, dictionary);
                    }
                }
            }

            if (property.isExpanded == true)
            {
                // Indent
                using (EditorGUI.IndentLevelScope indent = new EditorGUI.IndentLevelScope())
                {
                    // Update status
                    TranslationDictionary translationDictionary = dictionary.objectReferenceValue as TranslationDictionary;
                    string translationKey = key.stringValue;
                    Status status = UpdateMessageStatus(translationDictionary, translationKey);

                    // Draw preview
                    rect = DrawPreviewLabel(rect, status);

                    // Draw HelpBox
                    rect = DrawHelpBox(rect);

                    // Draw preview
                    rect = DrawTextPreview(rect, status, translationKey, translationDictionary);

                    // Show button to add a new translation key
                    rect = DrawButton(rect, status, translationDictionary, translationKey, dictionary);
                }
            }
        }

        #region Helper Methods
        private static Rect DrawFields(Rect rect, SerializedProperty key, SerializedProperty dictionary)
        {
            rect.y += EditorUiUtility.VerticalMargin + rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.DelayedTextField(rect, key);
            rect.y += EditorUiUtility.VerticalMargin + rect.height;
            EditorGUI.PropertyField(rect, dictionary);
            return rect;
        }

        private Rect DrawButton(Rect rect, Status status, TranslationDictionary translationDictionary, string translationKey, SerializedProperty dictionary)
        {
            if (IsButtonDrawn(status) == true)
            {
                // Adjust height
                rect.y += EditorUiUtility.VerticalMargin + rect.height;
                rect.height = ButtonHeight;

                // Add indentation
                rect.x += IndentLeft;
                rect.width -= IndentLeft;

                // Draw buttons
                if ((status == Status.DictionaryNotSet) && (GUI.Button(rect, "Create New Dictionary") == true))
                {
                    // Add the key into the translations dictionary
                    dictionary.objectReferenceValue = TranslationDictionaryEditor.CreateTranslationDictionary();
                }
                else if ((status == Status.UnknownKey) && (GUI.Button(rect, "Create New Key") == true))
                {
                    // Add the key into the translations dictionary
                    translationDictionary.AllTranslations.Add(translationKey);
                    translationDictionary.UpdateSerializedTranslations();
                }
                else if (status == Status.OK)
                {
                    DrawDictionaryButton(rect, dictionary, translationDictionary);
                }

                // Remove indentation
                rect.x -= IndentLeft;
                rect.width += IndentLeft;
            }
            return rect;
        }

        private void DrawDictionaryButton(Rect rect, SerializedProperty dictionary, TranslationDictionary translationDictionary)
        {
            using (EditorGUI.DisabledGroupScope scope = new EditorGUI.DisabledGroupScope(translationDictionary.IsAllTranslationsSerialized))
            {
                // Draw update button
                rect.width += EditorUiUtility.VerticalMargin;
                rect.width /= 2f;
                GUI.SetNextControlName("Apply");
                if (GUI.Button(rect, "Apply Changes") == true)
                {
                    // Apply changes to the dictionary
                    translationDictionary.UpdateSerializedTranslations();

                    // Save these changes to the file
                    UnityEditor.EditorUtility.SetDirty(translationDictionary);
                    AssetDatabase.SaveAssets();

                    // Change focus
                    GUI.FocusControl("Apply");
                }

                // Draw revert button
                rect.x += EditorUiUtility.VerticalMargin;
                rect.x += rect.width;
                GUI.SetNextControlName("Revert");
                if (GUI.Button(rect, "Revert Changes") == true)
                {
                    // Revert the dictionary
                    translationDictionary.RepopulateAllTranslations();

                    // Change focus
                    GUI.FocusControl("Revert");
                }
            }
        }

        private Rect DrawHelpBox(Rect rect)
        {
            // Check whether to show the help box
            if (string.IsNullOrEmpty(Message) == false)
            {
                // Add indentation
                rect.x += IndentLeft;
                rect.width -= IndentLeft;

                // Draw a header message
                rect.y += EditorUiUtility.VerticalMargin + rect.height;
                rect.height = EditorUiUtility.GetHelpBoxHeight(Message, rect.width);
                EditorGUI.HelpBox(rect, Message, MessageType);

                // Remove indentation
                rect.x -= IndentLeft;
                rect.width += IndentLeft;
            }
            return rect;
        }

        private Rect DrawPreviewLabel(Rect rect, Status status)
        {
            if (IsTextPreviewDrawn(status) == true)
            {
                // Draw header
                rect.y += EditorUiUtility.VerticalSpace + rect.height;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(rect, "Preview", EditorStyles.boldLabel);
            }
            return rect;
        }

        private Rect DrawTextPreview(Rect rect, Status status, string key, TranslationDictionary translationDictionary)
        {
            if (IsTextPreviewDrawn(status) == true)
            {
                // Update text preview
                TextPreview.SupportedLanguages = translationDictionary.SupportedLanguages;
                TextPreview.LanguageIndex = translationDictionary.SupportedLanguages.PreviewIndex;
                TranslationDictionary.LanguageTextMap translations;
                if (translationDictionary.AllTranslations.TryGetValue(key, out translations) == true)
                {
                    TextPreview.Text = translations[TextPreview.LanguageIndex];
                }

                // Draw the preview
                rect.y += rect.height;
                rect.height = TextPreview.CalculateHeight(null, !translationDictionary.IsAllTranslationsSerialized);
                TextPreview.DrawGui(rect, !translationDictionary.IsAllTranslationsSerialized);

                // Check if we need to apply changes
                if (TextPreview.IsLanguageIndexChanged == true)
                {
                    translationDictionary.SupportedLanguages.PreviewIndex = TextPreview.LanguageIndex;
                }
                else if ((TextPreview.IsTextChanged == true) && (translationDictionary.AllTranslations.TryGetValue(key, out translations) == true))
                {
                    // Apply the changes to the dictionary
                    translations[TextPreview.LanguageIndex] = TextPreview.Text;
                }
            }
            return rect;
        }

        private Status UpdateMessageStatus(TranslationDictionary translationDictionary, string translationKey)
        {
            // Update message
            Status returnStatus = Status.OK;
            Message = null;
            MessageType = MessageType.None;
            if (translationDictionary == null)
            {
                returnStatus = Status.DictionaryNotSet;
                Message = "Field, \"Dictionary\" must be set!";
                MessageType = MessageType.Error;
            }
            else if (translationDictionary.SupportedLanguages == null)
            {
                returnStatus = Status.SupportedLanguageNotSet;
                Message = "The dictionary, \"" + translationDictionary.name + "\" must have its supported language field set!";
                MessageType = MessageType.Error;
            }
            else if (string.IsNullOrEmpty(translationKey) == true)
            {
                returnStatus = Status.NoKey;
                Message = "Field, \"Key\" needs to be set for the label to be translated.";
                MessageType = MessageType.Warning;
            }
            else if (translationDictionary.AllTranslations.ContainsKey(translationKey) == false)
            {
                returnStatus = Status.UnknownKey;
                Message = "The key, \"" + translationKey + "\" isn't in the dictionary.";
                MessageType = MessageType.Warning;
            }
            else
            {
                // Indicate the text preview should be drawn
                Message = TextPreview.GetWarning(null);
                if (string.IsNullOrEmpty(Message) == true)
                {
                    MessageType = MessageType.Error;
                }
            }
            return returnStatus;
        }
        #endregion
    }
}
