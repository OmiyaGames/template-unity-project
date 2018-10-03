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

        public enum ImportState
        {
            ReadFile,
            ConvertToTranslations,
            Serializing,
            NumberOfStates
        }

        #region Constants
        const float ImportButtonWidth = 50f;
        const float Space = 4f;
        const int TotalStates = ((int)CSVReader.ReadStatus.State.NumberOfStates) + ((int)ImportState.NumberOfStates) - 1;
        const float ProgressNotStarted = -1f;
        const float ProgressFinished = -2f;
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
        static readonly GUIContent EmptyLabel = new GUIContent();
        #endregion

        static GUIStyle browseButtonFont = null;

        public static void ShowPopUp(TranslationDictionaryEditor editor)
        {
            ImportCsvPopUp window = GetWindow<ImportCsvPopUp>(true, "Import CSV", true);
            window.Editor = editor;
            window.DictionaryToEdit = (TranslationDictionary)editor.serializedObject.targetObject;
            window.minSize = DefaultWindowSize;
            window.Show();
        }

        readonly ThreadSafe<float> progress = new ThreadSafe<float>(ProgressNotStarted);
        readonly CSVReader.ReadStatus csvReadStatus = new CSVReader.ReadStatus();
        readonly ThreadSafe<ImportState> currentStatus = new ThreadSafe<ImportState>();
        readonly ProgressReport progressReport = new ProgressReport();

        #region Properties
        TranslationDictionaryEditor Editor
        {
            get;
            set;
        } = null;

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
            EditorGUILayout.EndVertical();
            GUI.enabled = true;
        }

        void OnEnable()
        {
            Progress = ProgressNotStarted;
        }

        void OnInspectorUpdate()
        {
            if (IsInMiddleOfImporting == true)
            {
                UpdateProgressMessage();

                // Update the message
                Repaint();
            }
            else if(Mathf.Approximately(Progress, ProgressFinished) == true)
            {
                Close();
            }
        }

        void OnDestroy()
        {
            Editor.serializedObject.ApplyModifiedProperties();
            Progress = ProgressNotStarted;
        }
        #endregion

        #region UI Helper Methods
        private void DrawCsvFileName(ref MessageType messageType)
        {
            // Draw the label
            EditorGUILayout.BeginHorizontal();
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

        private void DrawImportButton()
        {
            // Reserve space for this part
            EditorGUILayout.Space();
            Rect bounds = GUILayoutUtility.GetRect(EmptyLabel, "button");

            // Calculate the progress bar bounds
            float originalWidth = bounds.width;
            bounds.width = originalWidth - (ImportButtonWidth + Space);

            // Check if we need to show the progress bar
            if (IsInMiddleOfImporting == true)
            {
                EditorGUI.ProgressBar(bounds, Progress, ProgressMessage.ToString());
            }
            else
            {
                EditorGUI.ProgressBar(bounds, 1, "Waiting...");
            }

            // Only enable if no error messages are there
            bool isEnabled = GUI.enabled;
            GUI.enabled = ((ErrorMessage.Length == 0) && (IsInMiddleOfImporting == false));

            // Calculate the button bounds
            bounds.width = ImportButtonWidth;
            bounds.x = (bounds.x + originalWidth) - ImportButtonWidth;

            // Draw the button
            if ((GUI.Button(bounds, "Import") == true) && (GUI.enabled == true))
            {
                // Reset the progress
                Progress = 0f;
                currentStatus.Value = ImportState.ReadFile;
                progressReport.SetTotalSteps(1);

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
            currentStatus.Value = ImportState.ReadFile;
            Progress = 0f;

            List<Dictionary<string, string>> results = null;
            try
            {
                results = CSVReader.ReadFile(CsvFileName, csvReadStatus);
            }
            catch (System.Exception)
            {
                // Indicate the error that the file could not be read
                ErrorMessage.Clear();
                ErrorMessage.Append("Could not read CSV file.");
                Progress = ProgressNotStarted;
                results = null;
            }

            // Check if we have any results
            if (results != null)
            {
                // Process the results
                ProcessResults(results);

                // Indicate we're done
                Progress = ProgressFinished;
            }
        }

        private void ProcessResults(List<Dictionary<string, string>> results)
        {
            // Report initial progress
            currentStatus.Value = ImportState.ConvertToTranslations;
            progressReport.SetTotalSteps(results.Count);

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

                // Indicate progress
                progressReport.IncrementCurrentStep();
            }

            // Apply the changes
            currentStatus.Value = ImportState.Serializing;
            DictionaryToEdit.UpdateSerializedTranslations(progressReport);
        }

        private void UpdateProgressMessage()
        {
            // Calculate base progress based on state
            float baseProgress = 0;
            float stateProgress = 0;
            string message = "Serializing translations...";
            ImportState state = currentStatus.Value;
            if (state == ImportState.ReadFile)
            {
                // Get the CSV status
                CSVReader.ReadStatus.State csvState = csvReadStatus.CurrentState;
                if (csvState == CSVReader.ReadStatus.State.ReadingFileIntoRows)
                {
                    // Report that we're reading the file.
                    // Progress is not calculated here because we do not
                    // check how long the CSV file is.
                    message = "Parsing CSV file into text...";
                }
                else
                {
                    // Report the text is being split into cells
                    message = "Splitting text into cells...";

                    // Calculate the progress
                    baseProgress = (int)csvState;
                    stateProgress = csvReadStatus.ProgressPercent;
                }
            }
            else
            {
                // Check the state
                if (state == ImportState.ConvertToTranslations)
                {
                    // Report cells are being converted
                    message = "Converting cells into translations...";
                }

                // Calculate the progress
                baseProgress = ((int)state) + 1;
                stateProgress = progressReport.ProgressPercent;
            }

            // Calculate progress
            baseProgress /= TotalStates;
            stateProgress /= TotalStates;
            baseProgress += stateProgress;
            Progress = baseProgress;

            // Update message
            ProgressMessage.Clear();
            ProgressMessage.Append(baseProgress.ToString("00%"));
            ProgressMessage.Append(": ");
            ProgressMessage.Append(message);
        }
    }
}
