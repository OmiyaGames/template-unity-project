using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Builds;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="RootBuildSettingEditor.cs" company="Omiya Games">
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
    /// <date>11/01/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Editor script for <code>RootBuildSetting</code>
    /// </summary>
    /// <seealso cref="RootBuildSetting"/>
    [CustomEditor(typeof(RootBuildSetting))]
    public class RootBuildSettingEditor : Editor
    {
        SerializedProperty rootBuildFolder;
        SerializedProperty newBuildFolderName;
        SerializedProperty onBuildFailed;
        SerializedProperty onBuildCancelled;
        SerializedProperty allSettings;

        AnimBool folderAnimation;
        AnimBool buildSettingsAnimation;
        AnimBool interruptionsAnimation;
        CustomFileNameReorderableList newBuildFolderNameList;
        ChildBuildSettingReorderableList childBuildSettingsList;
        string previewPath = null;
        readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();

        public void OnEnable()
        {
            rootBuildFolder = serializedObject.FindProperty("rootBuildFolder");
            newBuildFolderName = serializedObject.FindProperty("newBuildFolderName");
            onBuildFailed = serializedObject.FindProperty("onBuildFailed");
            onBuildCancelled = serializedObject.FindProperty("onBuildCancelled");
            allSettings = serializedObject.FindProperty("allSettings");

            folderAnimation = new AnimBool(true, Repaint);
            buildSettingsAnimation = new AnimBool(true, Repaint);
            interruptionsAnimation = new AnimBool(true, Repaint);

            newBuildFolderNameList = new CustomFileNameReorderableList(newBuildFolderName, new GUIContent("New Build Folder Name"));
            childBuildSettingsList = new ChildBuildSettingReorderableList(this.target, allSettings, new GUIContent("All Settings"));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw build folder group
            DrawBuildFolder();

            // Draw stuff
            EditorGUILayout.Space();
            DrawBuildSettingList();

            // Draw interruptions
            EditorGUILayout.Space();
            DrawInterruptions();

            // Build button
            EditorGUILayout.Space();
            if (GUI.Button(EditorGUILayout.GetControlRect(), "Build All") == true)
            {
                RootBuildSetting setting = target as RootBuildSetting;
                if (setting != null)
                {
                    BuildPlayersResult results = setting.Build();
                    Debug.Log(results);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawBoldFoldout(AnimBool buildSettingsAnimation, string displayLabel)
        {
            GUIStyle boldFoldoutStyle = EditorStyles.foldout;
            FontStyle lastFontStyle = boldFoldoutStyle.fontStyle;
            boldFoldoutStyle.fontStyle = FontStyle.Bold;
            buildSettingsAnimation.target = EditorGUILayout.Foldout(buildSettingsAnimation.target, displayLabel, boldFoldoutStyle);
            boldFoldoutStyle.fontStyle = lastFontStyle;
        }

        private void DrawInterruptions()
        {
            DrawBoldFoldout(interruptionsAnimation, "Interruptions");
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(interruptionsAnimation.faded))
            {
                if (scope.visible == true)
                {
                    EditorGUILayout.PropertyField(onBuildFailed);
                    EditorGUILayout.PropertyField(onBuildCancelled);
                }
            }
        }

        private void DrawBuildFolder()
        {
            // Draw the build folder
            DrawBoldFoldout(folderAnimation, "Build Folder");
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(folderAnimation.faded))
            {
                if (scope.visible == true)
                {
                    if (string.IsNullOrEmpty(previewPath) == true)
                    {
                        previewPath = GetPathPreview();
                    }
                    EditorGUILayout.HelpBox(previewPath, MessageType.None);


                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(rootBuildFolder);
                    newBuildFolderNameList.List.DoLayoutList();
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        previewPath = null;
                    }
                }
            }
        }

        private void DrawBuildSettingList()
        {
            // Draw foldout
            DrawBoldFoldout(buildSettingsAnimation, "Platforms");

            // Draw the list
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(buildSettingsAnimation.faded))
            {
                if (scope.visible == true)
                {
                    childBuildSettingsList.List.DoLayoutList();
                }
            }
        }

        private string GetPathPreview()
        {
            // Setup variables
            CustomFileName name = CustomFileNameDrawer.GetTarget(newBuildFolderName);
            builder.Clear();
            builder.AppendLine("Preview:");
            builder.Append(rootBuildFolder.stringValue);
            if (builder[builder.Length - 1] != '/')
            {
                builder.Append('/');
            }
            builder.Append(name.ToString((RootBuildSetting)target));
            return builder.ToString();
        }
    }
}
