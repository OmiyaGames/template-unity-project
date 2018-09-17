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
        const float KeyLength = 30f;

        static readonly GUIContent DefaultTextToLabel = new GUIContent("Default Text To");
        static readonly GUIContent PresetMessageLabel = new GUIContent("Preset Message");
        const string DefaultLanguageLabel = "Default Language";

        #region Member Variables
        // Serialized properties and graphical wrappers
        SerializedProperty supportedLanguages;
        SerializedProperty defaultToWhenKeyNotFound;
        SerializedProperty presetMessageWhenKeyNotFound;
        SerializedProperty defaultToWhenTranslationNotFound;
        SerializedProperty defaultLanguageWhenTranslationNotFound;
        SerializedProperty presetMessageWhenTranslationNotFound;

        // Defaults field
        AnimBool showErrorMessage = null;
        AnimBool showDefaultConfigurations = null;
        AnimBool showPresetMessageForKeyNotFound = null;
        AnimBool showDefaultLanguageForTranslationNotFound = null;
        AnimBool showPresetMessageForTranslationNotFound = null;

        // Transations list
        SerializedProperty translations;
        ReorderableList translationsList;

        // TODO: when I find the time, add Search Fields for finding and removing/editing translations.
        // For now, it's a bit complicated since it'll require some sort of async system to create a smoother experience.
        // Search variables
        //SearchField searchField = null;
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
            CreateBool(ref showErrorMessage);
            CreateBool(ref showDefaultConfigurations);
            CreateBool(ref showPresetMessageForKeyNotFound);
            CreateBool(ref showDefaultLanguageForTranslationNotFound);
            CreateBool(ref showPresetMessageForTranslationNotFound);

            // Setup transations list
            translations = serializedObject.FindProperty("translations");
            translationsList = new ReorderableList(serializedObject, translations, true, true, true, true);
            translationsList.drawHeaderCallback = DrawTranslationsListHeader;
            translationsList.drawElementCallback = DrawTranslationsListElement;
            translationsList.elementHeightCallback = CalculateTranslationsListElementHeight;
            translationsList.onAddCallback = OnAddTranslation;
            translationsList.onRemoveCallback = OnRemoveTranslation;
            translationsList.onReorderCallbackWithDetails = OnReorderTranslationList;

            // Setup search field
            //searchField = new SearchField();
            recalculateSearchResult = true;
        }

        private void OnDisable()
        {
            DestroyBool(ref showErrorMessage);
            DestroyBool(ref showDefaultConfigurations);
            DestroyBool(ref showPresetMessageForKeyNotFound);
            DestroyBool(ref showDefaultLanguageForTranslationNotFound);
            DestroyBool(ref showPresetMessageForTranslationNotFound);
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object
            serializedObject.Update();

            // Draw support language field
            DrawSupportedLanguageField();
            DrawDefaultBehaviorsFields();

            // Draw the search bar at the top of the inspector
            GUI.enabled = (supportedLanguages.objectReferenceValue != null);
            DrawSearchBar();
            EditorGUILayout.Space();

            // Check if we're searching for something
            if (string.IsNullOrEmpty(newSearchString) == true)
            {
                // Draw re-ordable list of translations
                translationsList.DoLayoutList();

                // Indicate the search result needs to be re-calculated
                recalculateSearchResult = true;
            }
            else
            {
                // Check if there's a difference in what we're searching
                if (newSearchString != lastSearchedString)
                {
                    // Indicate the search result needs to be re-calculated
                    recalculateSearchResult = true;
                }

                // Draw the search results
                DrawSearchResults();
            }

            // Reset all variables
            lastSearchedString = newSearchString;
            GUI.enabled = true;

            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Helper Methods
        private void DrawSearchBar()
        {
            // TODO: when bringing back the search bar, bring this back
            //// Draw a label indicating what the search bar does
            //EditorGUILayout.LabelField("Search For Translation", EditorStyles.boldLabel);

            //// Calculate area for the search bar
            //Rect area = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));
            //GUILayout.BeginHorizontal();

            //// Draw the search bar
            //newSearchString = searchField.OnGUI(area, lastSearchedString);

            //// Close the vertical layout
            //EditorGUILayout.EndHorizontal();
        }

        private void DrawDefaultBehaviorsFields()
        {
            // Draw the Toggle Group
            showDefaultConfigurations.target = EditorGUILayout.Foldout(showDefaultConfigurations.target, "Default Behaviors");
            using (EditorGUILayout.FadeGroupScope defaultBehaviorsGroup = new EditorGUILayout.FadeGroupScope(showDefaultConfigurations.faded))
            {
                if (defaultBehaviorsGroup.visible == true)
                {
                    // Indent
                    ++EditorGUI.indentLevel;

                    // Show controls for default behaviors for missing keys
                    DrawDefaultsWhenKeyIsNotFound();
                    EditorGUILayout.Space();

                    // Show controls for default behaviors for missing translations
                    DrawDefaultsWhenTranslationForLanguageIsNotFound();

                    // Undo indentation
                    --EditorGUI.indentLevel;
                }
            }
        }

        private void DrawDefaultsWhenKeyIsNotFound()
        {
            EditorGUILayout.LabelField("When Key Is Not Found...", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(defaultToWhenKeyNotFound, DefaultTextToLabel);

            // Check if we want to show the Preset Message field
            showPresetMessageForKeyNotFound.target = (defaultToWhenKeyNotFound.enumValueIndex == ((int)TranslationDictionary.KeyNotFoundDefaults.PresetMessage));

            // Draw the preset message field
            using (EditorGUILayout.FadeGroupScope presetMessageGroup = new EditorGUILayout.FadeGroupScope(showPresetMessageForKeyNotFound.faded))
            {
                if (presetMessageGroup.visible == true)
                {
                    // Show preset message field
                    EditorGUILayout.PropertyField(presetMessageWhenKeyNotFound, PresetMessageLabel);
                }
            }
        }

        private void DrawDefaultsWhenTranslationForLanguageIsNotFound()
        {
            EditorGUILayout.LabelField("When Translation For a Language Is Not Found...", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(defaultToWhenTranslationNotFound, DefaultTextToLabel);

            // Check if we want to show the Default Language field
            bool showField = false;
            if (supportedLanguages.objectReferenceValue != null)
            {
                switch (defaultToWhenTranslationNotFound.enumValueIndex)
                {
                    case (int)TranslationDictionary.TranslationNotFoundDefaults.DefaultLanguageOrNull:
                    case (int)TranslationDictionary.TranslationNotFoundDefaults.DefaultLanguageOrEmptyString:
                    case (int)TranslationDictionary.TranslationNotFoundDefaults.DefaultLanguageOrPresetMessage:
                        showField = true;

                        break;
                }
            }
            showDefaultLanguageForTranslationNotFound.target = showField;

            // Draw the default language field
            using (EditorGUILayout.FadeGroupScope defaultLanguageGroup = new EditorGUILayout.FadeGroupScope(showDefaultLanguageForTranslationNotFound.faded))
            {
                if ((defaultLanguageGroup.visible == true) && (showField == true))
                {
                    // Show default language field
                    SupportedLanguagesEditor.DrawSupportedLanguages(DefaultLanguageLabel, defaultLanguageWhenTranslationNotFound, ((SupportedLanguages)supportedLanguages.objectReferenceValue));
                }
            }

            // Check if we want to show the Preset Message field
            showField = false;
            switch (defaultToWhenTranslationNotFound.enumValueIndex)
            {
                case (int)TranslationDictionary.TranslationNotFoundDefaults.PresetMessage:
                case (int)TranslationDictionary.TranslationNotFoundDefaults.DefaultLanguageOrPresetMessage:
                    showField = true;
                    break;
            }
            showPresetMessageForTranslationNotFound.target = showField;

            // Draw the preset message field
            using (EditorGUILayout.FadeGroupScope presetMessageGroup = new EditorGUILayout.FadeGroupScope(showPresetMessageForTranslationNotFound.faded))
            {
                if (presetMessageGroup.visible == true)
                {
                    // Show preset message field
                    EditorGUILayout.PropertyField(presetMessageWhenTranslationNotFound, PresetMessageLabel);
                }
            }
        }

        private void DrawSupportedLanguageField()
        {
            // Draw supported languages
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);

            // Check if the supported language is not set
            showErrorMessage.target = (supportedLanguages.objectReferenceValue == null);

            // Error message group
            using (EditorGUILayout.FadeGroupScope errorMessageGroup = new EditorGUILayout.FadeGroupScope(showErrorMessage.faded))
            {
                if (errorMessageGroup.visible == true)
                {
                    // If so, indicate an error
                    EditorGUILayout.HelpBox("Field \"Supported Languages\" must be set first!", MessageType.Error);

                    // Reset search terms
                    newSearchString = null;
                    lastSearchedString = null;
                }
            }

            // Prevent scene objects from being set to this field
            supportedLanguages.objectReferenceValue = EditorGUILayout.ObjectField("Supported Languages", supportedLanguages.objectReferenceValue, typeof(SupportedLanguages), false);
        }

        private void DrawSearchResults()
        {
            // If we are, start a scroll view
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Check if we need to search for a new set of results
            if (recalculateSearchResult == true)
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

        private void DrawTranslationsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "All Translations", EditorStyles.boldLabel);
        }

        private void DrawTranslationsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            float originalX = rect.x;
            float originalWidth = rect.width;

            // Grab the relevant element
            SerializedProperty element = translationsList.serializedProperty.GetArrayElementAtIndex(index);

            // FIXME: draw the element...somehow
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = KeyLength;

            // Draw the key label
            EditorGUI.LabelField(rect, "Key");

            // Draw the key text field
            rect.x += rect.width + VerticalMargin;
            rect.width = originalWidth - KeyLength;
            SerializedProperty property = element.FindPropertyRelative("key");
            property.stringValue = EditorGUI.TextField(rect, property.stringValue);
        }

        private float CalculateTranslationsListElementHeight(int index)
        {
            // Grab the relevant element
            SerializedProperty element = translationsList.serializedProperty.GetArrayElementAtIndex(index);

            // FIXME: calculate the height of the element...somehow
            float height = EditorGUIUtility.singleLineHeight;
            height += VerticalMargin * 3f;
            return height;
        }

        private void CreateBool(ref AnimBool boolAnimation)
        {
            // Destroy the last animation, if any
            DestroyBool(ref boolAnimation);

            // Setup new animation
            boolAnimation = new AnimBool(false);
            boolAnimation.valueChanged.AddListener(Repaint);
        }

        private void DestroyBool(ref AnimBool boolAnimation)
        {
            if (boolAnimation != null)
            {
                boolAnimation.valueChanged.RemoveListener(Repaint);
                boolAnimation = null;
            }
        }

        private static float GetHelpBoxHeight(string text)
        {
            var content = new GUIContent(text);
            var style = GUI.skin.GetStyle("helpbox");

            return style.CalcHeight(content, EditorGUIUtility.currentViewWidth);
        }

        private void OnReorderTranslationList(ReorderableList list, int oldIndex, int newIndex)
        {
            UpdateTranslationListStatus(list, Mathf.Min(oldIndex, newIndex));
        }

        private void OnAddTranslation(ReorderableList list)
        {
            AddEntryFromTranslationListStatus(list);
            ReorderableList.defaultBehaviours.DoAddButton(list);
            UpdateTranslationListStatus(list, list.index);
        }

        private void OnRemoveTranslation(ReorderableList list)
        {
            RemoveEntryFromTranslationListStatus(list);
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            UpdateTranslationListStatus(list, list.index);
        }

        private void UpdateTranslationListStatus(ReorderableList list, int startIndex)
        {
            if ((startIndex >= 0) && (startIndex < list.count))
            {
                // FIXME: do not resize the TranslationListStatus,
                // but do update it to the latest info starting from startIndex to count.
                Debug.Log("UpdateTranslationListStatus(" + startIndex + ')');
            }
        }

        private void AddEntryFromTranslationListStatus(ReorderableList list)
        {
            // FIXME: Add one new entry to the end of TranslationListStatus.
            Debug.Log("AddEntryFromTranslationListStatus()");
        }

        private void RemoveEntryFromTranslationListStatus(ReorderableList list)
        {
            // FIXME: Remove an entry of TranslationListStatus.
            // Make sure to remove the key if index matches.
            int removedIndex = list.index;
            Debug.Log("RemoveEntryFromTranslationListStatus(" + removedIndex + ')');
        }
        #endregion
    }
}
