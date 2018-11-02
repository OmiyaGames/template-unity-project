using UnityEditor;
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

        public void OnEnable()
        {
            rootBuildFolder = serializedObject.FindProperty("rootBuildFolder");
            newBuildFolderName = serializedObject.FindProperty("newBuildFolderName");
            onBuildFailed = serializedObject.FindProperty("onBuildFailed");
            onBuildCancelled = serializedObject.FindProperty("onBuildCancelled");
            allSettings = serializedObject.FindProperty("allSettings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(onBuildFailed);
            EditorGUILayout.PropertyField(onBuildCancelled);
            EditorGUILayout.PropertyField(rootBuildFolder);
            EditorGUILayout.PropertyField(newBuildFolderName);
            EditorGUILayout.PropertyField(allSettings);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
