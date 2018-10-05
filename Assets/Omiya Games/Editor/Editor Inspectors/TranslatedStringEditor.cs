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
        const float VerticalMargin = 2;
        const string message = "Testing, testing...";

        float Width
        {
            get;
            set;
        } = 0;
        List<TranslationDictionary.LanguageTextPair> previewTranslations;
        ReorderableList previewTranslationList;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorUtility.GetHelpBoxHeight(message, Width, EditorGUIUtility.singleLineHeight);
            height += EditorGUIUtility.singleLineHeight * 3;
            height += VerticalMargin * 3;
            return height;
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Set width
            Width = rect.width;

            // Draw a header message
            rect.height = EditorUtility.GetHelpBoxHeight(message, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.HelpBox(rect, message, MessageType.Info);

            // Grab every field
            SerializedProperty key = property.FindPropertyRelative("key");
            SerializedProperty dictionary = property.FindPropertyRelative("dictionary");

            // Draw label of object
            rect.y += VerticalMargin + rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
            EditorGUI.indentLevel += 1;

            // Draw the properties regularly
            rect.y += VerticalMargin + rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.DelayedTextField(rect, key);
            rect.y += VerticalMargin + rect.height;
            EditorGUI.PropertyField(rect, dictionary);

            // Construct a list for the clip variations
            //previewTranslationList = new ReorderableList(previewTranslations, typeof(TranslationDictionary.LanguageTextPair), true, true, true, true);
            //previewTranslationList.drawHeaderCallback = DrawTranslationsListHeader;
            //previewTranslationList.drawElementCallback = DrawTranslationsListElement;
            EditorGUI.indentLevel -= 1;
        }
    }
}
