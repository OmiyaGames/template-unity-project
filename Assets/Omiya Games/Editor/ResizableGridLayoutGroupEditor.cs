using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;

namespace UnityEditor.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ResizableGridLayoutGroupEditor.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor script for <code>ResizableGridLayoutGroup</code>.
    /// </summary>
    /// <seealso cref="ResizableGridLayoutGroup"/>
    [CustomEditor(typeof(ResizableGridLayoutGroup), true)]
    [CanEditMultipleObjects]
    public class ResizableGridLayoutGroupEditor : Editor
    {
        SerializedProperty padding;
        SerializedProperty spacing;
        SerializedProperty startCorner;
        SerializedProperty startAxis;
        SerializedProperty childAlignment;
        SerializedProperty constraint;
        SerializedProperty constraintCount;

        protected virtual void OnEnable()
        {
            padding = serializedObject.FindProperty("m_Padding");
            spacing = serializedObject.FindProperty("m_Spacing");
            startCorner = serializedObject.FindProperty("m_StartCorner");
            startAxis = serializedObject.FindProperty("m_StartAxis");
            childAlignment = serializedObject.FindProperty("m_ChildAlignment");
            constraint = serializedObject.FindProperty("m_menuConstraint");
            constraintCount = serializedObject.FindProperty("m_ConstraintCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(padding, true);
            EditorGUILayout.PropertyField(spacing, true);
            EditorGUILayout.PropertyField(startCorner, true);
            EditorGUILayout.PropertyField(startAxis, true);
            EditorGUILayout.PropertyField(childAlignment, true);
            EditorGUILayout.PropertyField(constraint, true);
            EditorGUILayout.PropertyField(constraintCount, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}