using UnityEngine;
using UnityEditor;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ISerializedContainerEditor.cs" company="Omiya Games">
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
    /// <date>6/26/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor script for serialized classes or structs.
    /// </summary>
    public abstract class ISerializedContainerEditor : PropertyDrawer
    {
        const int VerticalMargin = 2;

        protected abstract void DrawAllControls(SerializedProperty property, Rect singleLineRect);
        protected abstract int GetNumLinesToDraw(SerializedProperty property, GUIContent label);

        // Draw the property inside the given rect
        public override void OnGUI(Rect fullRect, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(fullRect, label, property);

            // Draw label
            EditorGUI.PrefixLabel(fullRect, GUIUtility.GetControlID(FocusType.Passive), label);
            var labelRect = fullRect;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            labelRect.y += VerticalMargin;

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            if (ShowLabel(label) == true)
            {
                EditorGUI.indentLevel += 1;
                labelRect.y += labelRect.height;
            }

            DrawAllControls(property, labelRect);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorHelpers.GetHeight(label, GetNumLinesToDraw(property, label), VerticalMargin);
        }

        public static bool ShowLabel(GUIContent label)
        {
            return (string.IsNullOrEmpty(label.text) == false);
        }

        public static void MoveDownOneLine(ref Rect singleLineRect)
        {
            singleLineRect.y += singleLineRect.height + VerticalMargin;
        }
    }
}
