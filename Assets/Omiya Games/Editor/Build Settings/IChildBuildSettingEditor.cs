using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Builds;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IChildBuildSettingEditor.cs" company="Omiya Games">
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
    /// Helper script for <code>IChildBuildSetting</code>
    /// </summary>
    /// <seealso cref="IChildBuildSetting"/>
    [CustomEditor(typeof(IChildBuildSetting))]
    public abstract class IChildBuildSettingEditor : IBuildSettingEditor
    {
        private SerializedProperty nameProperty;
        private SerializedProperty parentProperty;
        private AnimBool backAnimation;

        public override void OnEnable()
        {
            base.OnEnable();
            backAnimation = new AnimBool(false, Repaint);

            nameProperty = serializedObject.FindProperty("m_Name");
            parentProperty = serializedObject.FindProperty("parentSetting");
        }

        protected void DrawName()
        {
            EditorGUILayout.DelayedTextField(nameProperty);
        }

        protected void DrawBackButton()
        {
            DrawBoldFoldout(backAnimation, "Return Back to Parent");
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(backAnimation.faded))
            {
                if (scope.visible == true)
                {
                    IBuildSetting parentSetting = parentProperty.objectReferenceValue as IBuildSetting;
                    while (parentSetting != null)
                    {
                        // Draw the button
                        if (GUI.Button(EditorGUILayout.GetControlRect(), parentSetting.name) == true)
                        {
                            Selection.activeObject = parentSetting;
                        }

                        // Check if this setting has a parent
                        if (parentSetting is IChildBuildSetting)
                        {
                            // If so, grab it
                            parentSetting = ((IChildBuildSetting)parentSetting).Parent;
                        }
                        else
                        {
                            // If not, terminate the loop
                            break;
                        }
                    }
                }
            }
        }
    }
}
