using UnityEngine;
using UnityEditor;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PlatformSpecificButtonEditor.cs" company="Omiya Games">
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
    /// <date>6/15/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Property drawer for <code>PlatformSpecificButton</code>.
    /// </summary>
    /// <seealso cref="PlatformSpecificButton"/>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>6/15/2018</description>
    /// <description>Taro</description>
    /// <description>Initial version</description>
    /// </item>
    /// </list>
    /// </remarks>
    [CustomPropertyDrawer(typeof(PlatformSpecificButton))]
    public class PlatformSpecificButtonEditor : PropertyDrawer
    {
        const int VerticalMargin = 2;
        const int HorizontalMargin = 6;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorHelpers.GetHeight(label, 1, VerticalMargin);
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Draw label
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (string.IsNullOrEmpty(label.text) == false)
            {
                position.y += (EditorGUIUtility.singleLineHeight + VerticalMargin);
                EditorGUI.indentLevel = 1;
            }

            // Setup rect
            position.height = base.GetPropertyHeight(property, label);
            Rect fieldRect = position;
            fieldRect.width /= 2;

            // Draw the Component
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("component"), GUIContent.none);

            // Draw the Enabled for
            fieldRect.x += fieldRect.width + HorizontalMargin;
            fieldRect.width -= HorizontalMargin;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("enabledFor"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
