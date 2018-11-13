using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Builds;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IBuildSettingEditor.cs" company="Omiya Games">
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
    /// <date>11/12/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper script for <code>IBuildSetting</code>
    /// </summary>
    /// <seealso cref="IBuildSetting"/>
    public abstract class IBuildSettingEditor : Editor
    {
        protected readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
        private string previewPath = null;
        private AnimBool folderAnimation;
        private SerializedProperty nameProperty;

        public abstract string GetPathPreview();

        public static void DrawBoldFoldout(AnimBool buildSettingsAnimation, string displayLabel)
        {
            GUIStyle boldFoldoutStyle = EditorStyles.foldout;
            FontStyle lastFontStyle = boldFoldoutStyle.fontStyle;
            boldFoldoutStyle.fontStyle = FontStyle.Bold;
            buildSettingsAnimation.target = EditorGUILayout.Foldout(buildSettingsAnimation.target, displayLabel, boldFoldoutStyle);
            boldFoldoutStyle.fontStyle = lastFontStyle;
        }

        public virtual void OnEnable()
        {
            folderAnimation = new AnimBool(true, Repaint);
            nameProperty = serializedObject.FindProperty("name");
        }

        protected void DrawName()
        {
            EditorGUILayout.PropertyField(nameProperty);
        }

        protected void DrawBuildFolder(SerializedProperty pathNameProperty, CustomFileNameReorderableList pathNameList)
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

                    if (pathNameList != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(pathNameProperty);
                        pathNameList.List.DoLayoutList();
                        if (EditorGUI.EndChangeCheck() == true)
                        {
                            previewPath = null;
                        }
                    }
                }
            }
        }
    }
}
