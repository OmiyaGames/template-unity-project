using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PoolingManagerEditor.cs" company="Omiya Games">
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
    /// <date>4/15/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Editor script for <code>SceneTransitionManager</code>
    /// </summary>
    /// <seealso cref="SceneTransitionManager"/>
    [CustomPropertyDrawer(typeof(SceneInfo))]
    public class SceneInfoDrawer : PropertyDrawer
    {
        const int FileNameLabelWidth = 90;
        const float RevertTimeLabelWidthRatio = 110;
        const float RevertTimeFieldWidthRatio = 20;
        const float CursorModeLabelWidthRatio = 45;
        const int HorizontalGap = 3;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (base.GetPropertyHeight(property, label) * 3) + (HorizontalGap * 2);
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Revert the position height to normal
            position.height = base.GetPropertyHeight(property, label);
            //position.width -= 20;

            // Draw the File Name label
            Rect labelRect = position;
            labelRect.width = FileNameLabelWidth;
            EditorGUI.LabelField(labelRect, "File Name", GUIStyle.none);

            // Draw the Scene Name field
            Rect fieldRect = position;
            fieldRect.x += labelRect.width;
            fieldRect.width -= labelRect.width;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("sceneName"), GUIContent.none);

            // Dock the rest of the fields down a bit
            position.y += (position.height + HorizontalGap);

            // Draw the Display Name label
            labelRect = position;
            labelRect.width = FileNameLabelWidth;
            EditorGUI.LabelField(labelRect, "Display Name", GUIStyle.none);

            // Draw the Scene Name field
            fieldRect = position;
            fieldRect.x += labelRect.width;
            fieldRect.width -= labelRect.width;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("displayName"), GUIContent.none);

            // Dock the rest of the fields down a bit
            position.y += (position.height + HorizontalGap);

            // We're going through this from right to left
            float xPosition = position.x;

            // Draw Revert Time field
            fieldRect = position;
            fieldRect.x = xPosition;
            fieldRect.width = RevertTimeFieldWidthRatio;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("revertTimeScale"), GUIContent.none);
            xPosition += RevertTimeFieldWidthRatio;

            // Draw Revert Time label
            labelRect = position;
            labelRect.x = xPosition;
            labelRect.width = RevertTimeLabelWidthRatio;
            EditorGUI.LabelField(labelRect, "Reset TimeScale", GUIStyle.none);
            xPosition += RevertTimeLabelWidthRatio;

            // Draw Cursor label
            labelRect = position;
            labelRect.x = position.xMax - CursorModeLabelWidthRatio;
            labelRect.width = CursorModeLabelWidthRatio;
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleRight;
            EditorGUI.LabelField(labelRect, "Cursor", style);
            //xPosition += CursorModeLabelWidthRatio;

            // Draw Cursor field
            fieldRect = position;
            fieldRect.x = xPosition;
            fieldRect.width = labelRect.x - xPosition;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("sceneCursorLockMode"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

    //[CustomEditor(typeof(SceneTransitionManager))]
    //public class SceneTransitionManagerEditor : Editor
    //{

    //    // Use this for initialization
    //    void Start()
    //    {

    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {

    //    }
    //}
}