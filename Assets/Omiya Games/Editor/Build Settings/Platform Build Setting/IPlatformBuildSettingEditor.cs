using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using OmiyaGames.Builds;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IPlatformBuildSettingEditor.cs" company="Omiya Games">
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
    /// <date>11/16/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper script for <code>IPlatformBuildSetting</code>
    /// </summary>
    /// <seealso cref="IPlatformBuildSetting"/>
    [CustomEditor(typeof(IPlatformBuildSetting))]
    public abstract class IPlatformBuildSettingEditor : IChildBuildSettingEditor
    {
        // name stuff
        private SerializedProperty fileName;
        private SerializedProperty folderName;

        // build settings
        private AnimBool customSettingsAnimation;
        private SerializedProperty customScriptDefineSymbols;
        private SerializedProperty customScenes;

        // development settings
        private AnimBool developmentAnimation;
        private SerializedProperty enableStrictMode;
        private SerializedProperty enableAssertions;

        // debugging settings
        private AnimBool debugAnimation;
        private SerializedProperty debugEnable;
        private SerializedProperty debugEnableScriptDebugging;
        private SerializedProperty debugBuildScriptOnly;

        // Archive settings
        private AnimBool archiveAnimation;
        private SerializedProperty archiveEnable;
        private SerializedProperty archiveType;
        private SerializedProperty archiveIncludeParentFolder;
        private SerializedProperty archiveFileName;
        private SerializedProperty archiveDeleteOriginals;

        public virtual string FileExtension
        {
            get
            {
                return ((IPlatformBuildSetting)target).FileExtension;
            }
        }

        protected abstract void DrawPlatformSpecificSettings();

        public override void OnEnable()
        {
            base.OnEnable();
            SerializedProperty setting;

            // name stuff
            fileName = serializedObject.FindProperty("fileName");
            folderName = serializedObject.FindProperty("folderName");

            // build settings
            customSettingsAnimation = new AnimBool(true, Repaint);
            customScriptDefineSymbols = serializedObject.FindProperty("customScriptDefineSymbols");
            customScenes = serializedObject.FindProperty("customScenes");

            // archive settings
            setting = serializedObject.FindProperty("archiveSettings");
            archiveEnable = setting.FindPropertyRelative("enable");
            archiveType = setting.FindPropertyRelative("type");
            archiveIncludeParentFolder = setting.FindPropertyRelative("includeParentFolder");
            archiveDeleteOriginals = setting.FindPropertyRelative("deleteOriginals");
            archiveFileName = setting.FindPropertyRelative("fileName");
            archiveAnimation = new AnimBool(archiveEnable.boolValue, Repaint);

            // Development settings
            developmentAnimation = new AnimBool(false, Repaint);
            enableStrictMode = serializedObject.FindProperty("enableStrictMode");
            enableAssertions = serializedObject.FindProperty("enableAssertions");
            setting = serializedObject.FindProperty("debugSettings");

            // Debugging settings
            debugEnable = setting.FindPropertyRelative("enable");
            debugEnableScriptDebugging = setting.FindPropertyRelative("enableDebuggingScripts");
            debugBuildScriptOnly = setting.FindPropertyRelative("buildScriptsOnly");
            debugAnimation = new AnimBool(debugEnable.boolValue, Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw name of this build
            DrawBeginningOfSetting();

            // Draw custom settings
            EditorGUILayout.Space();
            DrawCustomSettings();

            // Draw development settings
            EditorGUILayout.Space();
            DrawDevelopmentSettings();

            // Draw archive settings
            EditorGUILayout.Space();
            DrawArchiveSettings();

            serializedObject.ApplyModifiedProperties();

            // Draw the bottom of the setting
            DrawEndOfSetting();
        }

        protected void DrawBeginningOfSetting()
        {
            // Draw build folder group
            DrawBuildFile(DrawCustomNameControls, AppendFileName,
            "File Name");
        }

        protected void DrawEndOfSetting()
        {
            // Draw name of this group
            EditorGUILayout.Space();
            DrawBackButton();

            // Build button
            EditorGUILayout.Space();
            DrawBuildButton();
        }

        protected void DrawCustomSettings()
        {
            // Draw foldout
            DrawBoldFoldout(customSettingsAnimation, "Custom Build Settings");
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(customSettingsAnimation.faded))
            {
                if (scope.visible == true)
                {
                    DrawPlatformSpecificSettings();
                    EditorGUILayout.PropertyField(customScriptDefineSymbols);
                    EditorGUILayout.PropertyField(customScenes);
                }
            }
        }

        protected void DrawDevelopmentSettings()
        {
            DrawBoldFoldout(developmentAnimation, "Development Settings");
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(developmentAnimation.faded))
            {
                if (scope.visible == true)
                {
                    // Show the enabled bool
                    debugEnable.boolValue = EditorGUILayout.Toggle("Enable Debugging", debugEnable.boolValue);
                    debugAnimation.target = debugEnable.boolValue;

                    // Draw the rest of the controls
                    using (new EditorGUI.IndentLevelScope())
                    using (EditorGUILayout.FadeGroupScope fadeScope = new EditorGUILayout.FadeGroupScope(debugAnimation.faded))
                    {
                        if (fadeScope.visible == true)
                        {
                            debugEnableScriptDebugging.boolValue = EditorGUILayout.Toggle("Debug Script", debugEnableScriptDebugging.boolValue);
                            debugBuildScriptOnly.boolValue = EditorGUILayout.Toggle("Build Scripts Only", debugBuildScriptOnly.boolValue);
                        }
                    }

                    // Draw checkboxes
                    EditorGUILayout.PropertyField(enableStrictMode);
                    EditorGUILayout.PropertyField(enableAssertions);
                }
            }
        }

        protected void DrawArchiveSettings()
        {
            // Show the enabled bool
            Rect indentLeft = EditorGUILayout.GetControlRect();
            indentLeft.x -= EditorUiUtility.IndentSpace;
            indentLeft.x += EditorUiUtility.VerticalMargin;
            archiveEnable.boolValue = EditorGUI.ToggleLeft(indentLeft, "Zip The Build", archiveEnable.boolValue, EditorStyles.boldLabel);
            archiveAnimation.target = archiveEnable.boolValue;

            // Draw the rest of the controls
            using (EditorGUILayout.FadeGroupScope fadeScope = new EditorGUILayout.FadeGroupScope(archiveAnimation.faded))
            {
                if (fadeScope.visible == true)
                {
                    DrawFileNamePreview(DrawArchiveControls, AppendArchiveFileName);
                }
            }
        }

        #region Archive Settings Helpers
        private void DrawArchiveControls()
        {
            EditorGUILayout.PropertyField(archiveType);
            archiveIncludeParentFolder.boolValue = EditorGUILayout.Toggle("Zip Under a Single Folder", archiveIncludeParentFolder.boolValue);
            archiveDeleteOriginals.boolValue = EditorGUILayout.Toggle("Delete The Original Files", archiveDeleteOriginals.boolValue);
            EditorGUILayout.PropertyField(archiveFileName);
        }

        private string AppendArchiveFileName(string originalString, System.Text.StringBuilder builder)
        {
            builder.Clear();
            builder.Append(originalString);
            builder.Append(Utility.PathDivider);

            CustomFileName name = CustomFileNameDrawer.GetTarget(archiveFileName);
            builder.Append(name.ToString((IBuildSetting)target));
            return builder.ToString();
        }
        #endregion

        #region DrawBeginningOfSetting Helpers
        private void DrawCustomNameControls()
        {
            // Draw name of the group
            DrawName();

            // Draw the folder and the file it's going to create
            EditorGUILayout.PropertyField(fileName);
            EditorGUILayout.PropertyField(folderName);
        }

        private string AppendFileName(string originalString, System.Text.StringBuilder builder)
        {
            builder.Clear();
            builder.Append(originalString);
            builder.Append(Utility.PathDivider);

            // Add file name
            CustomFileName name = CustomFileNameDrawer.GetTarget(fileName);
            builder.Append(name.ToString((IBuildSetting)target));
            builder.Append(FileExtension);
            return builder.ToString();
        }
        #endregion
    }
}
