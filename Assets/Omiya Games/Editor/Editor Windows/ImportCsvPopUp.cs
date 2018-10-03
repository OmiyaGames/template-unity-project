using UnityEngine;
using UnityEditor;
using System.Text;
using System.Threading;
using System.Collections.Generic;
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
    public partial class ImportCsvPopUp : EditorWindow
    {
        public enum ConflictResolution
        {
            AppendIgnore = 0,
            AppendOverwrite,
            Overwrite
        }

        #region Constants
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
        static readonly Vector2 DefaultWindowSize = new Vector2(350f, 150f);
        #endregion

        static GUIStyle browseButtonFont = null;

        public static void ShowPopUp(TranslationDictionary dictionaryToEdit)
        {
            ImportCsvPopUp window = GetWindow<ImportCsvPopUp>(true, "Import CSV", true);
            window.DictionaryToEdit = dictionaryToEdit;
            window.minSize = DefaultWindowSize;
            window.Show();
        }

        readonly ThreadSafe<float> progress = new ThreadSafe<float>(-1f);
        readonly CSVReader.ReadStatus csvReadStatus = new CSVReader.ReadStatus();

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

        string KeysColumnName
        {
            get;
            set;
        } = "Keys";

        ConflictResolution Resolution
        {
            get;
            set;
        } = ConflictResolution.AppendIgnore;

        StringBuilder ErrorMessage
        {
            get;
        } = new StringBuilder();

        float Progress
        {
            get
            {
                return progress.Value;
            }
            set
            {
                progress.Value = value;
            }
        }

        ThreadSafeStringBuilder ProgressMessage
        {
            get;
        } = new ThreadSafeStringBuilder();

        bool IsInMiddleOfImporting
        {
            get
            {
                return Progress.CompareTo(0f) >= 0;
            }
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
        #endregion

        #region Unity Events
        void OnGUI()
        {
            ErrorMessage.Clear();
            GUI.enabled = !IsInMiddleOfImporting;
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            EditorGUILayout.Space();

            // Draw the file we'll be importing
            MessageType messageType = MessageType.None;
            DrawCsvFileName(ref messageType);

            // Draw the key column
            DrawKeyColumnName(ref messageType);

            // Draw the conflict resolution
            Resolution = (ConflictResolution)EditorGUILayout.IntPopup("Conflict Resolution", ((int)Resolution), ConflictResolutionStrings, ConflictResolutionValues);

            // Draw the file that'll be edited
            DrawTranslationDictionary();

            // Explain what this dialog does
            if (ErrorMessage.Length > 0)
            {
                EditorGUILayout.HelpBox(ErrorMessage.ToString(), messageType);
            }

            // Draw the rest of the buttons
            DrawImportButton();
            DrawProgressBar();
            EditorGUILayout.EndVertical();
            GUI.enabled = true;
        }

        void OnEnable()
        {
            Progress = -1f;
        }

        void OnInspectorUpdate()
        {
            if(IsInMiddleOfImporting == true)
            {
                // FIXME: update the message
                Repaint();
            }
        }

        void OnDestroy()
        {
            Progress = -1f;
        }
        #endregion

        #region UI Helper Methods
        private void DrawCsvFileName(ref MessageType messageType)
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
                if (string.IsNullOrEmpty(newFileName) == false)
                {
                    CsvFileName = newFileName;
                }
            }

            // Check if there are any errors
            if (string.IsNullOrEmpty(CsvFileName) == true)
            {
                if (ErrorMessage.Length > 0)
                {
                    ErrorMessage.AppendLine();
                }
                ErrorMessage.Append("Assign a CSV file to import.");
                messageType = MessageType.Info;
            }
            else if (UnityEngine.Windows.File.Exists(CsvFileName) == false)
            {
                if (ErrorMessage.Length > 0)
                {
                    ErrorMessage.AppendLine();
                }
                ErrorMessage.Append("Cannot import file \"");
                ErrorMessage.Append(CsvFileName);
                ErrorMessage.Append("\"!");
                messageType = MessageType.Error;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawKeyColumnName(ref MessageType messageType)
        {
            KeysColumnName = EditorGUILayout.TextField("Name of the Column with Keys", KeysColumnName);
            if (string.IsNullOrEmpty(KeysColumnName) == true)
            {
                if (ErrorMessage.Length > 0)
                {
                    ErrorMessage.AppendLine();
                }
                ErrorMessage.Append("Name of the Column with Keys must be defined.");
                messageType = MessageType.Error;
            }
        }

        private void DrawTranslationDictionary()
        {
            bool isEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Import To", DictionaryToEdit, typeof(TranslationDictionary), false);
            GUI.enabled = isEnabled;
        }

        private void DrawProgressBar()
        {
            if (IsInMiddleOfImporting == true)
            {
                UnityEditor.EditorUtility.DisplayProgressBar("Import CSV Progress", ProgressMessage.ToString(), Progress);

                // TODO: When we're ready, add a method to cancel the import progress
                //if (UnityEditor.EditorUtility.DisplayCancelableProgressBar("Import CSV Progress", ProgressMessage.ToString(), Progress) == true)
                //{
                //    // User canceled, reset progress
                //    Progress = -1f;
                //}
            }
            else
            {
                UnityEditor.EditorUtility.ClearProgressBar();
            }
        }

        private void DrawImportButton()
        {
            // Calculate the size of the progress bar
            EditorGUILayout.Space();

            // Only enable if no error messages are there
            bool isEnabled = GUI.enabled;
            GUI.enabled = ((ErrorMessage.Length == 0) && (IsInMiddleOfImporting == false));

            // Draw the button
            if ((GUILayout.Button("Import") == true) && (GUI.enabled == true))
            {
                // Reset the progress
                Progress = 0f;

                // Start a new thread
                ThreadPool.QueueUserWorkItem(ImportCsvFile);
            }

            // Revert enable behavior
            GUI.enabled = isEnabled;
        }
        #endregion

        private void ImportCsvFile(object stateInfo)
        {
            // Indicate we're processing something!
            Progress = 0f;

            List<Dictionary<string, string>> results = null;
            try
            {
                results = CSVReader.ReadFile(CsvFileName, csvReadStatus);
            }
            catch (System.Exception ex)
            {
                // FIXME: indicate the error that the file could not be read
                results = null;
            }

            // Check if we have any results
            // FIXME: report progress, too!
            if(results != null)
            {
                ProcessResults(results);
            }
        }

        private void ProcessResults(List<Dictionary<string, string>> results)
        {
            // FIXME: report progress, too!
            // Check if we want to overwrite the translations
            if (Resolution == ConflictResolution.Overwrite)
            {
                // If so, completely clear the translations
                DictionaryToEdit.AllTranslations.Clear();
            }

            // Go through each row of the results
            string key;
            Dictionary<int, string> translations;
            foreach (Dictionary<string, string> row in results)
            {
                // First, search for the key column
                if ((row != null) && (row.TryGetValue(KeysColumnName, out key) == true))
                {
                    // Check if this key introduces a conflict
                    translations = null;
                    if (DictionaryToEdit.AllTranslations.ContainsKey(key) == false)
                    {
                        // If no conflicts, add a new translations dictionary
                        translations = new Dictionary<int, string>(row.Count - 1);
                        DictionaryToEdit.AllTranslations.Add(key, translations);
                    }
                    else if (Resolution == ConflictResolution.AppendIgnore)
                    {
                        // If we're ignoring conflicts, skip this row.
                        continue;
                    }
                    else
                    {
                        // Otherwise, overwrite the key with a new translations dictionary
                        translations = new Dictionary<int, string>(row.Count - 1);
                        DictionaryToEdit.AllTranslations[key] = translations;
                    }

                    // Go through each cell in each row
                    foreach (KeyValuePair<string, string> cell in row)
                    {
                        // Make sure the language list is supported
                        if (DictionaryToEdit.SupportedLanguages.Contains(cell.Key) == true)
                        {
                            // If so, add it and its text into the translations dictionary
                            translations.Add(DictionaryToEdit.SupportedLanguages[cell.Key], cell.Value);
                        }
                    }
                }
            }

            // Apply the changes
            DictionaryToEdit.UpdateSerializedTranslations();

            // Indicate we're done
            Progress = 1f;
        }
    }
}
