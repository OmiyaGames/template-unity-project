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

        AnimBool foldoutAnimation;
        CustomFileNameReorderableList newBuildFolderNameList;
        string previewPath = null;
        readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();

        public void OnEnable()
        {
            rootBuildFolder = serializedObject.FindProperty("rootBuildFolder");
            newBuildFolderName = serializedObject.FindProperty("newBuildFolderName");
            onBuildFailed = serializedObject.FindProperty("onBuildFailed");
            onBuildCancelled = serializedObject.FindProperty("onBuildCancelled");
            allSettings = serializedObject.FindProperty("allSettings");

            foldoutAnimation = new AnimBool(true, Repaint);

            newBuildFolderNameList = new CustomFileNameReorderableList(newBuildFolderName, new GUIContent("New Build Folder Name"));
        }

        public override void OnInspectorGUI()
        {
            // Setup variables
            CustomFileName name = CustomFileNameDrawer.GetTarget(newBuildFolderName);
            builder.Clear();

            serializedObject.Update();
            EditorGUILayout.LabelField("Build Folder", EditorStyles.boldLabel);

            if (string.IsNullOrEmpty(previewPath) == true)
            {
                builder.AppendLine("Preview:");
                builder.Append(rootBuildFolder.stringValue);
                if (builder[builder.Length - 1] != '/')
                {
                    builder.Append('/');
                }
                builder.Append(name.ToString((RootBuildSetting)target));
                previewPath = builder.ToString();
            }
            EditorGUILayout.HelpBox(previewPath, MessageType.None);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(rootBuildFolder);
            newBuildFolderNameList.List.DoLayoutList();
            if(EditorGUI.EndChangeCheck() == true)
            {
                previewPath = null;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Interruptions", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(onBuildFailed);
            EditorGUILayout.PropertyField(onBuildCancelled);

            // Draw stuff
            DrawFoldout();

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

        private void DrawFoldout()
        {
            // Draw foldout
            EditorGUILayout.Space();
            GUIStyle boldFoldoutStyle = EditorStyles.foldout;
            FontStyle lastFontStyle = boldFoldoutStyle.fontStyle;
            boldFoldoutStyle.fontStyle = FontStyle.Bold;
            foldoutAnimation.target = EditorGUILayout.Foldout(foldoutAnimation.target, "Platforms", boldFoldoutStyle);
            boldFoldoutStyle.fontStyle = lastFontStyle;

            // Draw the list
            using (new EditorGUI.IndentLevelScope())
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(foldoutAnimation.faded))
            {
                if (scope.visible == true)
                {
                    EditorGUILayout.PropertyField(allSettings);
                }
            }
        }
    }
}
