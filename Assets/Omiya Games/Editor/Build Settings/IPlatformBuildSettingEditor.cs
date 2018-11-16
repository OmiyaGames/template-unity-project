using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
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
        private SerializedProperty changeScriptDefineSymbols;
        private SerializedProperty customScriptDefineSymbols;
        private SerializedProperty customScenes;
        private SerializedProperty archiveSettings;

        // debugging settings
        private SerializedProperty enableStrictMode;
        private SerializedProperty enableAssertions;
        private SerializedProperty debugSettings;

        public override void OnEnable()
        {
            base.OnEnable();

            // name stuff
            fileName = serializedObject.FindProperty("fileName");
            folderName = serializedObject.FindProperty("folderName");

            // build settings
            changeScriptDefineSymbols = serializedObject.FindProperty("changeScriptDefineSymbols");
            customScriptDefineSymbols = serializedObject.FindProperty("customScriptDefineSymbols");
            customScenes = serializedObject.FindProperty("customScenes");
            archiveSettings = serializedObject.FindProperty("archiveSettings");

            // debugging settings
            enableStrictMode = serializedObject.FindProperty("enableStrictMode");
            enableAssertions = serializedObject.FindProperty("enableAssertions");
            debugSettings = serializedObject.FindProperty("debugSettings");
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
    }
}
