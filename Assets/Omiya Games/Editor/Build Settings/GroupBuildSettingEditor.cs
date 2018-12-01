using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Builds;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GroupBuildSettingEditor.cs" company="Omiya Games">
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
    /// Editor script for <code>GroupBuildSetting</code>
    /// </summary>
    /// <seealso cref="GroupBuildSetting"/>
    [CustomEditor(typeof(GroupBuildSetting))]
    public class GroupBuildSettingEditor : IChildBuildSettingEditor
    {
        SerializedProperty createEmbeddedFolder;
        SerializedProperty folderName;
        SerializedProperty allSettings;

        AnimBool folderNameAnimation;
        AnimBool allSettingsAnimation;
        ChildBuildSettingReorderableList allSettingsList;

        public override void OnEnable()
        {
            base.OnEnable();

            createEmbeddedFolder = serializedObject.FindProperty("createEmbeddedFolder");
            folderName = serializedObject.FindProperty("folderName");
            allSettings = serializedObject.FindProperty("allSettings");

            folderNameAnimation = new AnimBool(createEmbeddedFolder.boolValue, Repaint);
            allSettingsAnimation = new AnimBool(true, Repaint);

            allSettingsList = new ChildBuildSettingReorderableList(target, allSettings, new GUIContent("All Settings"));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw build folder group
            DrawBuildFolder(DrawFolderName);

            // Draw platforms
            EditorGUILayout.Space();
            DrawBuildSettingList();
            serializedObject.ApplyModifiedProperties();

            // Draw back buttons
            EditorGUILayout.Space();
            DrawBackButton();

            // Build button
            EditorGUILayout.Space();
            DrawBuildAllButton();
        }

        private void DrawFolderName()
        {
            // Draw name of the group
            DrawName();

            // Show checkbox
            createEmbeddedFolder.boolValue = EditorGUILayout.ToggleLeft("Create Folder For Group", createEmbeddedFolder.boolValue);
            folderNameAnimation.target = createEmbeddedFolder.boolValue;

            // Fade the rest of the controls in
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(folderNameAnimation.faded))
            {
                if (scope.visible == true)
                {
                    EditorGUILayout.PropertyField(folderName);
                }
            }
        }

        private void DrawBuildSettingList()
        {
            // Draw foldout
            DrawBoldFoldout(allSettingsAnimation, "Platforms");

            // Draw the list
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(allSettingsAnimation.faded))
            {
                if (scope.visible == true)
                {
                    allSettingsList.List.DoLayoutList();
                }
            }
        }
    }
}
