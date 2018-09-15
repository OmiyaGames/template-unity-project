using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using System.Text;
using System.IO;
using OmiyaGames.Translations;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslationDictionaryEditor.cs" company="Omiya Games">
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
    /// An editor to allow editing <code>TranslationDictionary</code> scripts.
    /// </summary>
    /// <seealso cref="TranslationDictionary"/>
    [CustomEditor(typeof(TranslationDictionary), true)]
    public class TranslationDictionaryEditor : Editor
    {
        public const string DefaultFileName = "New Translation Dictionary" + Utility.FileExtensionScriptableObject;
        const float VerticalMargin = 2;
        static readonly GUIContent DefaultTextToLabel = new GUIContent("Default Text To");

        #region Member Variables
        // Serialized properties and graphical wrappers
        SerializedProperty supportedLanguages;
        SerializedProperty defaultToWhenKeyNotFound;
        SerializedProperty presetMessageWhenKeyNotFound;
        SerializedProperty defaultToWhenTranslationNotFound;
        SerializedProperty presetMessageWhenTranslationNotFound;
        SerializedProperty defaultLanguageWhenTranslationNotFound;
        
        // Defaults field
        AnimBool showDefaultConfigurations;

        // Transations list
        SerializedProperty translations;
        ReorderableList translationsList;

        // Search variables
        SearchField searchField = null;
        string lastSearchedString = null, newSearchString = null;
        bool recalculateSearchResult = false;

        // Search Result scroll positions
        Vector2 scrollPosition = Vector2.zero;
        #endregion

        [MenuItem("Omiya Games/Create/Translation Dictionary")]
        private static void CreateTranslationDictionary()
        {
            // Setup asset
            TranslationDictionary newAsset = ScriptableObject.CreateInstance<TranslationDictionary>();

            // Setup path to file
            string folderName = AssetUtility.GetSelectedFolder();
            string pathOfAsset = Path.Combine(folderName, DefaultFileName);
            pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(pathOfAsset);

            // Create the asset, and prompt the user to rename it
            ProjectWindowUtil.CreateAsset(newAsset, pathOfAsset);
        }

        #region Unity Events
        private void OnEnable()
        {
            // Serialized properties and graphical wrappers
            supportedLanguages = serializedObject.FindProperty("supportedLanguages");
            defaultToWhenKeyNotFound = serializedObject.FindProperty("defaultToWhenKeyNotFound");
            presetMessageWhenKeyNotFound = serializedObject.FindProperty("presetMessageWhenKeyNotFound");
            defaultToWhenTranslationNotFound = serializedObject.FindProperty("defaultToWhenTranslationNotFound");
            presetMessageWhenTranslationNotFound = serializedObject.FindProperty("presetMessageWhenTranslationNotFound");
            defaultLanguageWhenTranslationNotFound = serializedObject.FindProperty("defaultLanguageWhenTranslationNotFound");

            // Setup animations
            showDefaultConfigurations = new AnimBool(false);
            showDefaultConfigurations.valueChanged.AddListener(Repaint);

            // Setup transations list
            translations = serializedObject.FindProperty("translations");
            translationsList = new ReorderableList(serializedObject, translations, true, false, true, true);
            translationsList.drawElementCallback = DrawTranslationsListElement;
            translationsList.elementHeightCallback = CalculateTranslationsListElement;

            // Setup search field
            searchField = new SearchField();
            recalculateSearchResult = true;
        }

        protected virtual void OnDisable()
        {
            showDefaultConfigurations.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            // FIXME: draw a warning if the supported langauges are not set
            // Check if the supported language is not set
            if(supportedLanguages.objectReferenceValue == null)
            {
                // If so, indicate an error
                EditorGUILayout.HelpBox("Field \"Supported Languages\" must be set first!", MessageType.Error);

                // Reset search terms
                newSearchString = null;
                lastSearchedString = null;
            }

            // Draw support langauge field
            DrawConfigurationFields();
            EditorGUILayout.Space();

            // Draw the search bar at the top of the inspector
            GUI.enabled = (supportedLanguages.objectReferenceValue != null);
            DrawSearchBar();
            GUI.enabled = true;
            EditorGUILayout.Space();

            // Check if we're searching for something
            if (string.IsNullOrEmpty(newSearchString) == true)
            {
                // Draw member variables
                DrawMemberVariables();

                // Indicate the search result needs to be re-calculated
                recalculateSearchResult = true;
            }
            else
            {
                // Check if there's a difference in what we're searching
                if(newSearchString != lastSearchedString)
                {
                    // Indicate the search result needs to be re-calculated
                    recalculateSearchResult = true;
                }

                // Draw the search results
                DrawSearchResults();
            }

            // Swap the search strings
            lastSearchedString = newSearchString;
        }
        #endregion

        private void DrawSearchBar()
        {
            // Draw a label indicating what the search bar does
            EditorGUILayout.LabelField("Search For Translation", EditorStyles.boldLabel);

            // Calculate area for the search bar
            Rect area = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            // Draw the search bar
            newSearchString = searchField.OnGUI(area, lastSearchedString);

            // Close the vertical layout
            EditorGUILayout.EndHorizontal();
        }

        private void DrawConfigurationFields()
        {
            // Draw supported languages
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            // Prevent scene objects from being set to this field
            supportedLanguages.objectReferenceValue = EditorGUILayout.ObjectField("Supported Languages", supportedLanguages.objectReferenceValue, typeof(SupportedLanguages), false);

            // Draw the Toggle Group
            showDefaultConfigurations.target = EditorGUILayout.Foldout(showDefaultConfigurations.target, "Default Behavior");
            if (EditorGUILayout.BeginFadeGroup(showDefaultConfigurations.faded) == true)
            {
                // Show header, "When Translation Key Is Not Found..."
                EditorGUILayout.PropertyField(defaultToWhenKeyNotFound, DefaultTextToLabel);

                // Show header, "When Translation Key Is Not Found..."
                EditorGUILayout.PropertyField(defaultToWhenTranslationNotFound, DefaultTextToLabel);
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DrawMemberVariables()
        {
            // Update whether the rest of the UI should be enabled or not
            GUI.enabled = (supportedLanguages.objectReferenceValue != null);
            GUI.enabled = true;

            // FIXME: show a regular editable list of strings
            base.OnInspectorGUI();
        }

        private void DrawSearchResults()
        {
            // If we are, start a scroll view
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Check if we need to search for a new set of results
            if(recalculateSearchResult == true)
            {
                // FIXME: calculate which translations matches the search result
            }

            // FIXME: show the actual entry, editable, and add a button to remove it
            // print some stuff for testing purposes
            EditorGUILayout.HelpBox("Testing....", MessageType.Info);

            // End the scroll view
            EditorGUILayout.EndScrollView();

            recalculateSearchResult = true;
        }

        private void DrawTranslationsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Grab the relevant element
            SerializedProperty element = translationsList.serializedProperty.GetArrayElementAtIndex(index);

            // FIXME: draw the element...somehow
        }

        private float CalculateTranslationsListElement(int index)
        {
            // Grab the relevant element
            SerializedProperty element = translationsList.serializedProperty.GetArrayElementAtIndex(index);

            // FIXME: calculate the height of the element...somehow
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
