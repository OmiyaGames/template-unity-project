using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace OmiyaGames.UI
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
    /// <date>6/10/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor script for <code>SuperLayoutElement</code>.
    /// </summary>
    /// <seealso cref="SuperLayoutElement"/>
    [CustomEditor(typeof(SuperLayoutElement), true)]
    [CanEditMultipleObjects]
    public class SuperLayoutElementEditor : LayoutElementEditor
    {
        SerializedProperty m_IgnoreLayout;

        SerializedProperty m_MaxHeight;
        SerializedProperty m_MaxWidth;

        bool m_ShowMinHeightElement = false;
        SerializedProperty m_MinHeightElement;
        bool m_ShowMinWidthElement = false;
        SerializedProperty m_MinWidthElement;

        bool m_ShowPreferredHeightElement = false;
        SerializedProperty m_PreferredHeightElement;
        bool m_ShowPreferredWidthElement = false;
        SerializedProperty m_PreferredWidthElement;

        bool m_ShowMaxHeightElement = false;
        SerializedProperty m_MaxHeightElement;
        bool m_ShowMaxWidthElement = false;
        SerializedProperty m_MaxWidthElement;

        protected override void OnEnable()
        {
            // Call base class
            base.OnEnable();
            m_IgnoreLayout = serializedObject.FindProperty("m_IgnoreLayout");

            // Grab float-based max width and height
            m_MaxHeight = serializedObject.FindProperty("m_MaxHeight");
            m_MaxWidth = serializedObject.FindProperty("m_MaxWidth");

            // Grab RectTransform min elements
            m_MinHeightElement = serializedObject.FindProperty("m_MinHeightElement");
            m_MinWidthElement = serializedObject.FindProperty("m_MinWidthElement");

            // Update bools
            m_ShowMinHeightElement = (m_MinHeightElement.objectReferenceValue != null);
            m_ShowMinWidthElement = (m_MinWidthElement.objectReferenceValue != null);

            // Grab RectTransform preferred elements
            m_PreferredHeightElement = serializedObject.FindProperty("m_PreferredHeightElement");
            m_PreferredWidthElement = serializedObject.FindProperty("m_PreferredWidthElement");

            // Update bools
            m_ShowPreferredHeightElement = (m_PreferredHeightElement.objectReferenceValue != null);
            m_ShowPreferredWidthElement = (m_PreferredWidthElement.objectReferenceValue != null);

            // Grab RectTransform max elements
            m_MaxHeightElement = serializedObject.FindProperty("m_MaxHeightElement");
            m_MaxWidthElement = serializedObject.FindProperty("m_MaxWidthElement");

            // Update bools
            m_ShowMaxHeightElement = (m_MaxHeightElement.objectReferenceValue != null);
            m_ShowMaxWidthElement = (m_MaxWidthElement.objectReferenceValue != null);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (m_IgnoreLayout.boolValue == false)
            {
                serializedObject.Update();
                EditorGUILayout.Space();

                LayoutElementFloatField(m_MaxWidth, t => t.rect.width);
                LayoutElementFloatField(m_MaxHeight, t => t.rect.height);

                LayoutElementRectTransformField(m_MinWidthElement, ref m_ShowMinWidthElement);
                LayoutElementRectTransformField(m_MinHeightElement, ref m_ShowMinHeightElement);

                LayoutElementRectTransformField(m_PreferredWidthElement, ref m_ShowPreferredWidthElement);
                LayoutElementRectTransformField(m_PreferredHeightElement, ref m_ShowPreferredHeightElement);

                LayoutElementRectTransformField(m_MaxWidthElement, ref m_ShowMaxWidthElement);
                LayoutElementRectTransformField(m_MaxHeightElement, ref m_ShowMaxHeightElement);
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Taken from https://bitbucket.org/Unity-Technologies/ui/src/2017.3/UnityEditor.UI/UI/LayoutElementEditor.cs
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        void LayoutElementFloatField(SerializedProperty property, System.Func<RectTransform, float> defaultValue)
        {
            Rect toggleRect, floatFieldRect;
            SetupPositioning(property, out toggleRect, out floatFieldRect);

            // Checkbox
            EditorGUI.BeginChangeCheck();
            bool enabled = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, property.floatValue >= 0);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = -1;
                if (enabled == true)
                {
                    property.floatValue = defaultValue((target as LayoutElement).transform as RectTransform);
                }
            }

            if (!property.hasMultipleDifferentValues && (enabled == true))
            {
                // Float field
                EditorGUIUtility.labelWidth = 4; // Small invisible label area for drag zone functionality
                EditorGUI.BeginChangeCheck();
                float newValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(" "), property.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = Mathf.Max(0, newValue);
                }
                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Taken from https://bitbucket.org/Unity-Technologies/ui/src/2017.3/UnityEditor.UI/UI/LayoutElementEditor.cs
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        void LayoutElementRectTransformField(SerializedProperty property, ref bool show)
        {
            Rect toggleRect, objectFieldRect;
            SetupPositioning(property, out toggleRect, out objectFieldRect);

            // Checkbox
            EditorGUI.BeginChangeCheck();
            show = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, show);
            if ((EditorGUI.EndChangeCheck() == true) && (show == false))
            {
                property.objectReferenceValue = null;
            }
            else if (!property.hasMultipleDifferentValues && (show == true))
            {
                // Object field
                EditorGUIUtility.labelWidth = 4; // Small invisible label area for drag zone functionality
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(objectFieldRect, property);
                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Taken from https://bitbucket.org/Unity-Technologies/ui/src/2017.3/UnityEditor.UI/UI/LayoutElementEditor.cs
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        private static void SetupPositioning(SerializedProperty property, out Rect toggleRect, out Rect fieldRect)
        {
            Rect position = EditorGUILayout.GetControlRect();

            // Label
            GUIContent label = EditorGUI.BeginProperty(position, null, property);

            // Rects
            Rect fieldPosition = EditorGUI.PrefixLabel(position, label);

            toggleRect = fieldPosition;
            toggleRect.width = 16;

            fieldRect = fieldPosition;
            fieldRect.xMin += 16;
        }
    }
}
