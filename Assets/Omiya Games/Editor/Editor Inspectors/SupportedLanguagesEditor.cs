using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;
using OmiyaGames.Translations;

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
        const float VerticalMargin = 2;

        // Member variables
        SerializedProperty previewLanguageInIndex;
        SerializedProperty supportedLanguages;
        ReorderableList supportedLanguagesList;

        [MenuItem("Tools/Omiya Games/Create Supported Languages...", priority = 801)]
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
            if(returnIndex >= target.Count)
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
            supportedLanguagesList.elementHeight = EditorUtility.GetHeight(null, 2, VerticalMargin) + VerticalMargin;
        }

        #region Helper Methods
        void DrawSupportedLanguagesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Supported Languages", EditorStyles.boldLabel);
        }

        void DrawSupportedLangaugesListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Grab the relevant element
            SerializedProperty element = supportedLanguagesList.serializedProperty.GetArrayElementAtIndex(index);

            // Adjust rect to the first line
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;

            // Display language name
            SerializedProperty property = element.FindPropertyRelative("languageName");
            EditorGUI.PropertyField(rect, property);

            // Adjust rect to the second line
            rect.y += VerticalMargin;
            rect.y += rect.height;
            rect.width /= 2f;

            // Draw checkbox
            property = element.FindPropertyRelative("isSystemDefault");
            property.boolValue = EditorGUI.Toggle(rect, "Map To System Language", property.boolValue);
            if (property.boolValue == true)
            {
                // Adjust rect to the second line
                rect.x += rect.width;

                // Draw enum
                property = element.FindPropertyRelative("mapTo");
                System.Enum selectedLanguage= EditorGUI.EnumPopup(rect, (SystemLanguage)property.enumValueIndex);
                property.enumValueIndex = System.Convert.ToInt32(selectedLanguage);
            }
        }

        static string[] GetAllLanguageNames(SupportedLanguages target, bool includeAddLanguage)
        {
            string[] returnNames;
            if(includeAddLanguage == true)
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
        #endregion
    }
}
