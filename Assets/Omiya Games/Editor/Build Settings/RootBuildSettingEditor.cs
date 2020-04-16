using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using OmiyaGames.Builds;
using OmiyaGames.Common.Editor;

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
    /// Editor script for <see cref="RootBuildSetting"/>.
    /// </summary>
    /// <seealso cref="RootBuildSetting"/>
    [CustomEditor(typeof(RootBuildSetting))]
    public class RootBuildSettingEditor : IBuildSettingEditor
    {
        public const string DefaultFileName = "New Build Settings" + Helpers.FileExtensionScriptableObject;

        SerializedProperty rootBuildFolder;
        SerializedProperty newBuildFolderName;
        SerializedProperty onBuildFailed;
        SerializedProperty onBuildCancelled;
        SerializedProperty allSettings;

        AnimBool buildSettingsAnimation;
        AnimBool interruptionsAnimation;
        CustomFileNameReorderableList newBuildFolderNameList;
        ChildBuildSettingReorderableList childBuildSettingsList;

        [MenuItem("Assets/Create/Omiya Games/Build Settings", priority = 200)]
        public static RootBuildSetting CreateBuildSettings()
        {
            // Setup asset
            RootBuildSetting newAsset = CreateInstance<RootBuildSetting>();

            // Setup path to file
            string folderName = AssetHelpers.GetSelectedFolder();
            string pathOfAsset = System.IO.Path.Combine(folderName, DefaultFileName);
            pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(pathOfAsset);

            // Create the asset, and prompt the user to rename it
            ProjectWindowUtil.CreateAsset(newAsset, pathOfAsset);
            return newAsset;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            rootBuildFolder = serializedObject.FindProperty("rootBuildFolder");
            newBuildFolderName = serializedObject.FindProperty("newBuildFolderName");
            onBuildFailed = serializedObject.FindProperty("onBuildFailed");
            onBuildCancelled = serializedObject.FindProperty("onBuildCancelled");
            allSettings = serializedObject.FindProperty("allSettings");

            buildSettingsAnimation = new AnimBool(true, Repaint);
            interruptionsAnimation = new AnimBool(true, Repaint);

            newBuildFolderNameList = new CustomFileNameReorderableList(newBuildFolderName, new GUIContent("New Build Folder Name"));
            childBuildSettingsList = new ChildBuildSettingReorderableList(this.target, allSettings, new GUIContent("All Settings"));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw build folder group
            DrawBuildFolder(() =>
            {
                EditorGUILayout.PropertyField(rootBuildFolder);
                newBuildFolderNameList.List.DoLayoutList();
            });

            // Draw stuff
            EditorGUILayout.Space();
            DrawBuildSettingList();

            // Draw interruptions
            EditorGUILayout.Space();
            DrawInterruptions();

            // Build button
            EditorGUILayout.Space();
            DrawBuildAllButton();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInterruptions()
        {
            EditorHelpers.DrawBoldFoldout(interruptionsAnimation, "Interruptions");
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(interruptionsAnimation.faded))
            {
                if (scope.visible == true)
                {
                    EditorGUILayout.PropertyField(onBuildFailed);
                    EditorGUILayout.PropertyField(onBuildCancelled);
                }
            }
        }

        private void DrawBuildSettingList()
        {
            // Draw foldout
            EditorHelpers.DrawBoldFoldout(buildSettingsAnimation, "Platforms");

            // Draw the list
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(buildSettingsAnimation.faded))
            {
                if (scope.visible == true)
                {
                    childBuildSettingsList.List.DoLayoutList();
                }
            }
        }
    }
}
