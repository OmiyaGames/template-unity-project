using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;
using OmiyaGames.Translations;
using System.Collections.Generic;
using System;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SupportedLanguagesEditor.cs" company="Omiya Games">
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
    /// <date>9/12/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor to allow editing <code>SupportedLanguages</code> scripts.
    /// </summary>
    /// <seealso cref="SupportedLanguages"/>
    [CustomEditor(typeof(SupportedLanguages), true)]
    public class SupportedLanguagesEditor : Editor
    {
        public const string DefaultFileName = "New Supported Languages" + Utility.FileExtensionScriptableObject;

        // Member variables
        SerializedProperty previewLanguageInIndex;
        SerializedProperty supportedLanguages;
        ReorderableList supportedLanguagesList;
        readonly Dictionary<SerializedProperty, ReorderableList> fonts = new Dictionary<SerializedProperty, ReorderableList>();

        [MenuItem("Assets/Create/Omiya Games/Supported Languages", priority = 20)]
        private static void CreateSupportedLanguages()
        {
            // Setup asset
            SupportedLanguages newAsset = ScriptableObject.CreateInstance<SupportedLanguages>();

            // Setup path to file
            string folderName = AssetUtility.GetSelectedFolder();
            string pathOfAsset = Path.Combine(folderName, DefaultFileName);
            pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(pathOfAsset);

            // Create the asset, and prompt the user to rename it
            ProjectWindowUtil.CreateAsset(newAsset, pathOfAsset);
        }

        /// <summary>
        /// Draws a popup mapping an int-property to a supported language.
        /// </summary>
        public static void DrawSupportedLanguages(string label, SerializedProperty property, SupportedLanguages target)
        {
            property.intValue = EditorGUILayout.Popup(label, property.intValue, GetAllLanguageNames(target, false));
        }

        /// <summary>
        /// Draws a popup mapping an int-property to a supported language.
        /// </summary>
        public static int DrawSupportedLanguages(Rect rect, string label, int index, SupportedLanguages target, bool includeAddLanguage = false)
        {
            int returnIndex = EditorGUI.Popup(rect, label, index, GetAllLanguageNames(target, includeAddLanguage));
            if (returnIndex >= target.Count)
            {
                returnIndex = index;
                Selection.activeObject = target;
            }
            return returnIndex;
        }

        /// <summary>
        /// Draws a popup mapping an int-property to a supported language.
        /// </summary>
        public static int DrawSupportedLanguages(Rect rect, SerializedProperty property, SupportedLanguages target)
        {
            return DrawSupportedLanguages(rect, property.intValue, target);
        }

        /// <summary>
        /// Draws a popup mapping an int-property to a supported language.
        /// </summary>
        public static int DrawSupportedLanguages(Rect rect, int index, SupportedLanguages target, bool includeAddLanguage = false)
        {
            int returnIndex = EditorGUI.Popup(rect, index, GetAllLanguageNames(target, includeAddLanguage));
            if (returnIndex >= target.Count)
            {
                returnIndex = index;
                Selection.activeObject = target;
            }
            return returnIndex;
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object
            serializedObject.Update();

            // Display the language to preview
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            DrawSupportedLanguages("Language", previewLanguageInIndex, (SupportedLanguages)target);

            // Display list of languages
            EditorGUILayout.Space();
            supportedLanguagesList.DoLayoutList();

            // Apply modifications
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            // Grab all properties
            previewLanguageInIndex = serializedObject.FindProperty("previewLanguageInIndex");
            supportedLanguages = serializedObject.FindProperty("supportedLanguages");

            // Setup a new list
            supportedLanguagesList = new ReorderableList(serializedObject, supportedLanguages, true, true, true, true);
            supportedLanguagesList.drawHeaderCallback = DrawSupportedLanguagesListHeader;
            supportedLanguagesList.drawElementCallback = DrawSupportedLangaugesListElement;
            supportedLanguagesList.elementHeightCallback = CalculateSupportedLangaugesListElementHeight;
            supportedLanguagesList.onAddCallback = AddSupportedLangaugesListElement;
            supportedLanguagesList.onRemoveCallback = RemoveSupportedLangaugesListElement;

            // Clean everything up
            fonts.Clear();
        }

        #region Helper Methods
        private void AddSupportedLangaugesListElement(ReorderableList list)
        {
            // Add an element to the list
            ReorderableList.defaultBehaviours.DoAddButton(list);

            // Setup this list
            GetFontsList(list.serializedProperty.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative("fonts"));
        }

        private ReorderableList GetFontsList(SerializedProperty property)
        {
            // Attempt to retrieve the list
            ReorderableList fontsList = null;
            if (fonts.TryGetValue(property, out fontsList) == false)
            {
                // If none is found, create a new one
                fontsList = new ReorderableList(serializedObject, property, true, true, true, true);
                fontsList.drawHeaderCallback = DrawFontsListHeader;
                fontsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    DrawFontsListElement(property.GetArrayElementAtIndex(index), rect);
                };
                fontsList.elementHeight = EditorUtility.SingleLineHeight(EditorUtility.VerticalMargin);
                fonts.Add(property, fontsList);
            }
            return fontsList;
        }

        private void RemoveSupportedLangaugesListElement(ReorderableList list)
        {
            // Remove the reorderable list
            SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if (fonts.ContainsKey(property) == true)
            {
                fonts.Remove(property);
            }
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        void DrawSupportedLanguagesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Supported Languages", EditorStyles.boldLabel);
        }

        void DrawSupportedLangaugesListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Grab the relevant element
            SerializedProperty element = supportedLanguagesList.serializedProperty.GetArrayElementAtIndex(index);

            // Adjust rect to the first line
            rect.y += EditorUtility.VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;

            // Display language name
            SerializedProperty property = element.FindPropertyRelative("languageName");
            EditorGUI.PropertyField(rect, property);

            // Adjust rect to the second line
            rect.y += EditorUtility.VerticalMargin;
            rect.y += rect.height;

            // Expand the left side of the indent
            rect.x -= EditorUtility.IndentSpace;
            rect.width += EditorUtility.IndentSpace;

            // Draw checkbox
            property = element.FindPropertyRelative("isSystemDefault");
            DrawSystemDefaultPopUp(ref rect, element, ref property);

            // Adjust rect to the last line
            rect.y += EditorUtility.VerticalMargin;
            rect.y += rect.height;

            // Draw fonts
            property = element.FindPropertyRelative("fonts");
            DrawFontsList(rect, property);
        }

        private float CalculateSupportedLangaugesListElementHeight(int index)
        {
            // Grab the relevant element
            SerializedProperty element = supportedLanguagesList.serializedProperty.GetArrayElementAtIndex(index);

            // Calculate base height
            float returnHeight = EditorUtility.GetHeight(null, 2, EditorUtility.VerticalMargin) + EditorUtility.VerticalMargin;

            // Grab the relevant list
            ReorderableList list = GetFontsList(element.FindPropertyRelative("fonts"));

            // Calculate list height
            returnHeight += list.GetHeight();
            returnHeight += EditorUtility.VerticalMargin;
            return returnHeight;
        }

        static string[] GetAllLanguageNames(SupportedLanguages target, bool includeAddLanguage)
        {
            string[] returnNames;
            if (includeAddLanguage == true)
            {
                returnNames = new string[target.NumberOfLanguages + 1];
            }
            else
            {
                returnNames = new string[target.NumberOfLanguages];
            }
            for (int index = 0; index < target.NumberOfLanguages; ++index)
            {
                returnNames[index] = target[index];
            }
            if (includeAddLanguage == true)
            {
                returnNames[target.NumberOfLanguages] = "Add Language...";
            }
            return returnNames;
        }

        private void DrawFontsList(Rect rect, SerializedProperty property)
        {
            // Create a font list
            ReorderableList fontList = GetFontsList(property);

            // Draw fonts
            rect.height = fontList.GetHeight();
            fontList.DoList(rect);
        }

        private static void DrawSystemDefaultPopUp(ref Rect rect, SerializedProperty element, ref SerializedProperty property)
        {
            float originalX = rect.x;
            float originalWidth = rect.width;

            rect.width /= 2f;

            property.boolValue = EditorGUI.Toggle(rect, "Map To System Language", property.boolValue);
            if (property.boolValue == true)
            {
                // Adjust rect to the second line
                rect.x += rect.width;

                // Draw enum
                property = element.FindPropertyRelative("mapTo");
                EditorGUI.PropertyField(rect, property, GUIContent.none);
            }

            // Revert the rectangle
            rect.x = originalX;
            rect.width = originalWidth;
        }

        private static void DrawFontsListElement(SerializedProperty element, Rect rect)
        {
            // Adjust rect to the first line
            rect.y += EditorUtility.VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;

            // Grab the relevant element
            element.objectReferenceValue = EditorGUI.ObjectField(rect, element.objectReferenceValue, typeof(TMPro.FontStyles), true);
        }

        private void DrawFontsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Associated Fonts");
        }
        #endregion
    }
}
