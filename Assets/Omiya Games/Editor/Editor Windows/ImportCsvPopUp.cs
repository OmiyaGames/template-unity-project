using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OmiyaGames.Translations;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ImportCsvPopUp.cs" company="Omiya Games">
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
    /// <date>10/1/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Creates a window that imports CSV files.
    /// </summary>
    public class ImportCsvPopUp : EditorWindow
    {
        public enum ConflictResolution
        {
            AppendIgnore = 0,
            AppendOverwrite,
            Overwrite
        }

        const float ImportButtonWidth = 50f;
        const float SideMargin = 6f;
        const float Space = 4f;
        static readonly string[] CsvFileFilter = new string[]
        {
            "CSV files", "csv",
            "All files", "*"
        };
        static readonly string[] ConflictResolutionStrings = new string[]
        {
            "Append, Ignore Conflicting Keys",
            "Append, Overwrite Conflicting Keys",
            "Overwrite File"
        };
        static readonly int[] ConflictResolutionValues = new int[]
        {
            (int)ConflictResolution.AppendIgnore,
            (int)ConflictResolution.AppendOverwrite,
            (int)ConflictResolution.Overwrite
        };
        static readonly Vector2 DefaultWindowSize = new Vector2(350f, 100f);
        static GUIStyle browseButtonFont = null;

        public static void ShowPopUp(TranslationDictionary dictionaryToEdit)
        {
            ImportCsvPopUp window = GetWindow<ImportCsvPopUp>(true, "Import CSV", true);
            window.DictionaryToEdit = dictionaryToEdit;
            window.minSize = DefaultWindowSize;
            window.Show();
        }

        #region Properties
        TranslationDictionary DictionaryToEdit
        {
            get;
            set;
        } = null;

        string CsvFileName
        {
            get;
            set;
        } = null;

        ConflictResolution Resolution
        {
            get;
            set;
        } = ConflictResolution.AppendIgnore;

        float Progress
        {
            get;
            set;
        } = -1f;

        StringBuilder ErrorMessage
        {
            get;
        } = new StringBuilder();
        #endregion

        #region Unity Events
        void OnGUI()
        {
            ErrorMessage.Clear();
            GUI.enabled = (Progress < 0);
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            EditorGUILayout.Space();

            // Draw the file we'll be importing
            DrawCsvFileName();

            // Draw the conflict resolution
            Resolution = (ConflictResolution)EditorGUILayout.IntPopup("Conflict Resolution", ((int)Resolution), ConflictResolutionStrings, ConflictResolutionValues);

            // Draw the file that'll be edited
            DrawTranslationDictionary();

            // Explain what this dialog does
            if (ErrorMessage.Length > 0)
            {
                EditorGUILayout.HelpBox(ErrorMessage.ToString(), MessageType.Error);
            }

            DrawProgressBar();
            EditorGUILayout.EndVertical();
            GUI.enabled = true;
        }

        GUILayoutOption BrowseButtonHeight
        {
            get
            {
                return GUILayout.Height(EditorGUIUtility.singleLineHeight - 2f);
            }
        }

        GUIStyle BrowseButtonFont
        {
            get
            {
                if (browseButtonFont == null)
                {
                    browseButtonFont = new GUIStyle(GUI.skin.button);
                    browseButtonFont.fontSize = 9;
                }
                return browseButtonFont;
            }
        }

        private void DrawCsvFileName()
        {
            EditorGUILayout.BeginHorizontal();

            // Draw the label
            EditorGUILayout.PrefixLabel("Import From");

            // Draw the text field
            CsvFileName = EditorGUILayout.DelayedTextField(CsvFileName);

            // Draw the browse button
            if (GUILayout.Button("Browse...", BrowseButtonFont, BrowseButtonHeight) == true)
            {
                string newFileName = UnityEditor.EditorUtility.OpenFilePanelWithFilters("Import CSV File", "/Assets/", CsvFileFilter);
                if(string.IsNullOrEmpty(newFileName) == false)
                {
                    CsvFileName = newFileName;
                }
            }

            // Check if there are any errors
            if (UnityEngine.Windows.File.Exists(CsvFileName) == false)
            {
                if(ErrorMessage.Length > 0)
                {
                    ErrorMessage.AppendLine();
                }
                ErrorMessage.Append("CSV file '");
                ErrorMessage.Append(CsvFileName);
                ErrorMessage.Append("' doesn't exist!");
            }
            EditorGUILayout.EndHorizontal();
        }

        void OnEnable()
        {

        }
        #endregion

        private void DrawTranslationDictionary()
        {
            bool isEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Import To", DictionaryToEdit, typeof(TranslationDictionary), false);
            GUI.enabled = isEnabled;
        }

        private void DrawProgressBar()
        {
            // Calculate the size of the progress bar
            EditorGUILayout.Space();
            Rect bounds = EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(EditorGUIUtility.singleLineHeight));
            float originalWidth = bounds.width;
            bounds.x = SideMargin;
            bounds.width -= (ImportButtonWidth + Space + (SideMargin * 2));
            bounds.height = EditorGUIUtility.singleLineHeight + Space;

            // Draw the progress bar
            float displayProgress = Mathf.Clamp01(Progress);
            EditorGUI.ProgressBar(bounds, displayProgress, displayProgress.ToString("0%"));

            // Only enable if no error messages are there
            bool isEnabled = GUI.enabled;
            GUI.enabled = ((ErrorMessage.Length == 0) && (Progress < 0f));

            // Calculate the size of the button
            bounds.x += (bounds.width + Space);
            bounds.width = ImportButtonWidth;

            // Draw the button
            if (GUI.Button(bounds, "Import") == true)
            {
                // FIXME: do soemthing!
            }

            // Revert enable behavior
            GUI.enabled = isEnabled;

            // Close the layout
            EditorGUILayout.EndVertical();
        }
    }
}
