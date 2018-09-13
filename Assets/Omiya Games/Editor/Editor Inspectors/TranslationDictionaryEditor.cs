using UnityEditor;
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

        private UnityEditor.IMGUI.Controls.SearchField searchField = null;
        private string lastSearchedString = null, newSearchString = null;
        private Vector2 scrollPosition = Vector2.zero;

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

        private void OnEnable()
        {
            // Setup search field
            searchField = new UnityEditor.IMGUI.Controls.SearchField();
        }

        public override void OnInspectorGUI()
        {
            // Calculate area for the search bar
            Rect area = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();

            // Draw the search bar
            newSearchString = searchField.OnGUI(area, lastSearchedString);

            // Close the vertical layout
            EditorGUILayout.EndHorizontal();

            // Check if we're searching for something
            if (string.IsNullOrEmpty(newSearchString) == false)
            {
                // If we are, start a scroll view
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                // Draw the search results
                DrawSearchResults();

                // End the scroll view
                EditorGUILayout.EndScrollView();
            }
            else
            {
                // FIXME: show a regular editable list of strings
                base.OnInspectorGUI();
            }

            // Swap the search strings
            lastSearchedString = newSearchString;
        }

        private void DrawSearchResults()
        {
            // FIXME: show the actual entry, editable, and add a button to remove it
            // print some stuff for testing purposes
            EditorGUILayout.HelpBox("TEsting....", MessageType.Info);
        }
    }
}
