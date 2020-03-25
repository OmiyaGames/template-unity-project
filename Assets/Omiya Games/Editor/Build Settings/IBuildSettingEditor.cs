using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using System.Collections.Generic;
using OmiyaGames.Builds;
using OmiyaGames.Common.Editor;

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
    [CustomEditor(typeof(IBuildSetting))]
    public abstract class IBuildSettingEditor : UnityEditor.Editor
    {
        private string previewPath = null;
        private AnimBool folderAnimation;
        protected readonly System.Text.StringBuilder builder = new System.Text.StringBuilder();
        protected delegate string AdjustText(string originalString, System.Text.StringBuilder builder);
        private Vector2 scrollPos;

        float BackHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }
        }

        public string GetPathPreview()
        {
            string returnPath = null;
            if (target is IBuildSetting)
            {
                // Calculate the path from the target
                returnPath = ((IBuildSetting)target).GetPathPreview(builder, Utility.PathDivider);

                // Prepend "Preview"
                builder.Clear();
                builder.AppendLine("Preview: ");
                builder.Append(returnPath);
                returnPath = builder.ToString();
            }
            return returnPath;
        }

        public virtual void OnEnable()
        {
            folderAnimation = new AnimBool(true, Repaint);
        }

        protected override bool ShouldHideOpenButton()
        {
            return true;
        }

        protected override void OnHeaderGUI()
        {
            // Setup the title header
            using (new EditorGUILayout.VerticalScope("In BigTitle"))
            {
                // Draw the name of this asset
                GUILayout.Label(target.name, EditorStyles.largeLabel);

                // Draw a quick label indicating the purpose of this slider view
                GUILayout.Label("Navigation:", EditorStyles.miniBoldLabel, GUILayout.ExpandWidth(false));

                // Setup the scroll view
                using (EditorGUILayout.ScrollViewScope sScope = new EditorGUILayout.ScrollViewScope(scrollPos, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUIStyle.none, GUILayout.MinHeight(BackHeight)))
                using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    // Starting with a list of size of 3 (latter number is arbitrary)
                    List<IBuildSetting> parentSettings = GetAllParentSettings(3);

                    if ((parentSettings != null) && (parentSettings.Count > 0))
                    {
                        // Go through the parent settings in reverse order
                        for (int index = (parentSettings.Count - 1); index >= 0; --index)
                        {
                            // Draw the button
                            IBuildSetting parentSetting = parentSettings[index];
                            if (GUILayout.Button(parentSetting.name, EditorStyles.foldout, GUILayout.ExpandWidth(false)) == true)
                            {
                                Selection.activeObject = parentSetting;
                            }
                        }
                    }

                    // Draw a normal text
                    GUILayout.Label(target.name, EditorStyles.foldout, GUILayout.ExpandWidth(false));

                    // Update scroll position
                    scrollPos = sScope.scrollPosition;
                }
            }
        }

        protected void DrawBuildFolder(System.Action drawPath)
        {
            DrawBuildFile(drawPath, null, "Build Folder");
        }

        protected void DrawBuildFile(System.Action drawPath, AdjustText adjustPreviewPath, string foldoutLabel)
        {
            // Draw the build folder
            EditorUiUtility.DrawBoldFoldout(folderAnimation, foldoutLabel);
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(folderAnimation.faded))
            {
                if (scope.visible == true)
                {
                    DrawFileNamePreview(drawPath, adjustPreviewPath);
                }
            }
        }

        protected void DrawFileNamePreview(System.Action drawPath, AdjustText adjustPreviewPath)
        {
            // Draw the build folder
            if (string.IsNullOrEmpty(previewPath) == true)
            {
                previewPath = GetPathPreview();
            }
            string preview = previewPath;
            if (adjustPreviewPath != null)
            {
                preview = adjustPreviewPath(previewPath, builder);
            }
            EditorGUILayout.HelpBox(preview, MessageType.None);

            if (drawPath != null)
            {
                EditorGUI.BeginChangeCheck();
                drawPath();
                if (EditorGUI.EndChangeCheck() == true)
                {
                    previewPath = null;
                }
            }
        }

        protected void DrawBuildAllButton()
        {
            DrawBuildButton("Build All");
        }

        protected void DrawBuildButton(string buttonText = "Build")
        {
            if (GUI.Button(EditorGUILayout.GetControlRect(), buttonText) == true)
            {
                IBuildSetting setting = target as IBuildSetting;
                if (setting != null)
                {
                    BuildPlayersResult results = setting.Build();
                    Debug.Log(results);
                }
            }
        }

        protected virtual List<IBuildSetting> GetAllParentSettings(int initialCapacity)
        {
            return null;
        }
    }
}
