using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using OmiyaGames.Translations;

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

        List<TranslationDictionary.LanguageTextPair> previewTranslations;
        ReorderableList previewTranslationList;

        #region Properties
        float Width
        {
            get;
            set;
        } = 0;

        bool IsExpanded
        {
            get;
            set;
        } = true;

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

        bool ShowButton
        {
            get;
            set;
        } = false;
        #endregion

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (IsExpanded == true)
            {
                // Grab every field
                SerializedProperty key = property.FindPropertyRelative("key");
                SerializedProperty dictionary = property.FindPropertyRelative("dictionary");

                // Allocate key field
                height += EditorUtility.GetHeight(2);

                // Update status
                TranslationDictionary translationDictionary = dictionary.objectReferenceValue as TranslationDictionary;
                string translationKey = key.stringValue;
                UpdateMessageStatus(translationDictionary, translationKey);

                // Check status
                if (string.IsNullOrEmpty(Message) == false)
                {
                    // Allocate help box
                    height += EditorUtility.VerticalMargin;
                    height += EditorUtility.GetHelpBoxHeight(Message, Width);
                }

                // Check button
                if(ShowButton == true)
                {
                    // Allocate button
                    height += EditorUtility.VerticalMargin;
                    height += ButtonHeight;
                }
            }
            return height;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Grab every field
            SerializedProperty key = property.FindPropertyRelative("key");
            SerializedProperty dictionary = property.FindPropertyRelative("dictionary");

            // Calculate height
            float previewHeight = rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            previewHeight -= rect.height;
            Width = rect.width;

            // Draw label of object
            IsExpanded = EditorGUI.Foldout(rect, IsExpanded, label);
            if (IsExpanded == true)
            {
                // Indent
                using (EditorGUI.IndentLevelScope indent = new EditorGUI.IndentLevelScope())
                {
                    // Draw the properties regularly
                    rect = DrawFields(rect, key, dictionary);

                    // Update status
                    TranslationDictionary translationDictionary = dictionary.objectReferenceValue as TranslationDictionary;
                    string translationKey = key.stringValue;
                    UpdateMessageStatus(translationDictionary, translationKey);

                    // Add indentation
                    rect.x += IndentLeft;
                    rect.width -= IndentLeft;

                    // Draw HelpBox
                    rect = DrawHelpBox(rect);

                    if (string.IsNullOrEmpty(Message) == true)
                    {
                        // Construct a list for the clip variations
                        //rect.y += EditorUtility.VerticalMargin + rect.height;
                        //rect.height = EditorGUIUtility.singleLineHeight;
                        //previewTranslationList = new ReorderableList(previewTranslations, typeof(TranslationDictionary.LanguageTextPair), true, true, true, true);
                        //previewTranslationList.drawHeaderCallback = DrawTranslationsListHeader;
                        //previewTranslationList.drawElementCallback = DrawTranslationsListElement;
                    }

                    // Show button to add a new translation key
                    rect = DrawButton(rect, translationDictionary, translationKey, dictionary);

                    // Remove indentation
                    rect.x -= IndentLeft;
                    rect.width += IndentLeft;
                }
            }
        }

        #region Helper Methods
        private static Rect DrawFields(Rect rect, SerializedProperty key, SerializedProperty dictionary)
        {
            rect.y += EditorUtility.VerticalMargin + rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.DelayedTextField(rect, key);
            rect.y += EditorUtility.VerticalMargin + rect.height;
            EditorGUI.PropertyField(rect, dictionary);
            return rect;
        }

        private Rect DrawButton(Rect rect, TranslationDictionary translationDictionary, string translationKey, SerializedProperty dictionary)
        {
            if (ShowButton == true)
            {
                rect.y += EditorUtility.VerticalMargin + rect.height;
                rect.height = ButtonHeight;
                if ((MessageType == MessageType.Error) && (GUI.Button(rect, "Create New Dictionary") == true))
                {
                    // Add the key into the translations dictionary
                    dictionary.objectReferenceValue = TranslationDictionaryEditor.CreateTranslationDictionary();
                }
                else if ((MessageType == MessageType.Warning) && (GUI.Button(rect, "Create New Key") == true))
                {
                    // Add the key into the translations dictionary
                    translationDictionary.AllTranslations.Add(translationKey, new Dictionary<int, string>());
                    translationDictionary.UpdateSerializedTranslations();
                }
                else if ((MessageType == MessageType.None) && (GUI.Button(rect, "Update Dictionary") == true))
                {
                    // Apply changes to the dictionary
                    translationDictionary.UpdateSerializedTranslations();
                }
            }
            return rect;
        }

        private Rect DrawHelpBox(Rect rect)
        {
            // Check whether to show the help box
            if (string.IsNullOrEmpty(Message) == false)
            {
                // Draw a header message
                rect.y += EditorUtility.VerticalMargin + rect.height;
                rect.height = EditorUtility.GetHelpBoxHeight(Message, rect.width);
                EditorGUI.HelpBox(rect, Message, MessageType);
            }

            return rect;
        }

        private void UpdateMessageStatus(TranslationDictionary translationDictionary, string translationKey)
        {
            // Update message
            ShowButton = true;
            Message = null;
            MessageType = MessageType.None;
            if (translationDictionary == null)
            {
                Message = "Field, \"Dictionary\" must be set!";
                MessageType = MessageType.Error;
            }
            else
            {
                if (string.IsNullOrEmpty(translationKey) == true)
                {
                    Message = "Field, \"Key\" needs to be set for the label to be translated.";
                    MessageType = MessageType.Warning;
                    ShowButton = false;
                }
                else if (translationDictionary.AllTranslations.ContainsKey(translationKey) == false)
                {
                    Message = "The key, \"" + translationKey + "\" isn't in the dictionary.";
                    MessageType = MessageType.Warning;
                }
            }
        }
        #endregion
    }
}
