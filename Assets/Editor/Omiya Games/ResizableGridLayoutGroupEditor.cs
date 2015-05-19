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
    /// Copyright (c) 2014-2015 Omiya Games
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
        SerializedProperty m_Padding;
        SerializedProperty m_Spacing;
        SerializedProperty m_StartCorner;
        SerializedProperty m_StartAxis;
        SerializedProperty m_ChildAlignment;
        SerializedProperty m_Constraint;
        SerializedProperty m_ConstraintCount;

        protected virtual void OnEnable()
        {
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_StartCorner = serializedObject.FindProperty("m_StartCorner");
            m_StartAxis = serializedObject.FindProperty("m_StartAxis");
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
            m_Constraint = serializedObject.FindProperty("m_menuConstraint");
            m_ConstraintCount = serializedObject.FindProperty("m_ConstraintCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_StartCorner, true);
            EditorGUILayout.PropertyField(m_StartAxis, true);
            EditorGUILayout.PropertyField(m_ChildAlignment, true);
            EditorGUILayout.PropertyField(m_Constraint, true);
            EditorGUILayout.PropertyField(m_ConstraintCount, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}