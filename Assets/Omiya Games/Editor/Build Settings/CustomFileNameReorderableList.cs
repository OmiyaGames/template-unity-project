using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using OmiyaGames.Builds;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="CustomFileNameReorderableList.cs" company="Omiya Games">
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
    /// <date>11/02/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Property drawer for <code>CustomFileName</code>.
    /// </summary>
    public class CustomFileNameReorderableList
    {
        public const float TypeWidth = 110f;
        public const float SlugWidth = 107f;

        public static int[] PrefillTypeValues
        {
            get
            {
                CustomFileName.PrefillType[] values = (CustomFileName.PrefillType[])Enum.GetValues(typeof(CustomFileName.PrefillType));
                int[] returnValues = new int[values.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    returnValues[i] = (int)values[i];
                }
                return returnValues;
            }
        }

        public CustomFileNameReorderableList(SerializedProperty property, GUIContent label) : this(property.FindPropertyRelative("names"), property.FindPropertyRelative("asSlug"), label)
        { }

        public CustomFileNameReorderableList(SerializedProperty names, SerializedProperty asSlug, GUIContent label)
        {
            // Member Variable
            NamesProperty = names;
            AsSlugProperty = asSlug;
            Label = label;

            // Setup List
            List = new ReorderableList(names.serializedObject, names, true, true, true, true);
            List.drawHeaderCallback = DrawNamesListHeader;
            List.drawElementCallback = DrawNamesListElement;
            List.onAddDropdownCallback = DrawNameListDropdown;
            List.elementHeight = EditorUiUtility.SingleLineHeight(EditorUiUtility.VerticalMargin);
        }

        #region Properties
        public SerializedProperty NamesProperty
        {
            get;
        }

        public SerializedProperty AsSlugProperty
        {
            get;
        }

        public ReorderableList List
        {
            get;
        }

        public GUIContent Label
        {
            get;
        }
        #endregion

        private void DrawNamesListHeader(Rect rect)
        {
            int originalIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Draw name of the variable
            float originalWidth = rect.width;
            rect.width -= SlugWidth + EditorUiUtility.VerticalMargin;
            EditorGUI.LabelField(rect, Label);

            // Draw the checkbox
            rect.x += rect.width + EditorUiUtility.VerticalMargin;
            rect.width = SlugWidth;
            AsSlugProperty.boolValue = EditorGUI.ToggleLeft(rect, "Convert As Slug", AsSlugProperty.boolValue);

            EditorGUI.indentLevel = originalIndentLevel;
        }

        private void DrawNamesListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Get all the properties
            SerializedProperty element = NamesProperty.GetArrayElementAtIndex(index);
            SerializedProperty type = element.FindPropertyRelative("type");
            SerializedProperty text = element.FindPropertyRelative("text");

            // Draw the text
            float originalWidth = rect.width;
            rect.width = TypeWidth;
            rect.y += EditorUiUtility.VerticalMargin;

            // Draw enumerator
            type.enumValueIndex = EditorGUI.IntPopup(rect, type.enumValueIndex, type.enumDisplayNames, PrefillTypeValues);

            // Draw the text field (if necessary)
            rect.x += (EditorUiUtility.VerticalMargin + TypeWidth);
            rect.width = (originalWidth - (TypeWidth + EditorUiUtility.VerticalMargin));

            // Draw text field
            bool originalEnabled = GUI.enabled;
            if (CustomFileName.CanEditText(type.enumValueIndex) == true)
            {
                rect.y -= EditorUiUtility.VerticalMargin / 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                text.stringValue = EditorGUI.TextField(rect, text.stringValue);
            }
            //else
            //{
            //    GUI.enabled = false;
            //    EditorGUI.SelectableLabel(rect, text.stringValue);
            //    GUI.enabled = originalEnabled;
            //}
        }

        private void DrawNameListDropdown(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
            CustomFileName.PrefillType[] allValues = (CustomFileName.PrefillType[])Enum.GetValues(typeof(CustomFileName.PrefillType));
            foreach (CustomFileName.PrefillType value in allValues)
            {
                menu.AddItem(new GUIContent(value.ToString()), false, OnAdd, value);
            }
            menu.ShowAsContext();
        }

        private void OnAdd(object arg)
        {
            int index = List.serializedProperty.arraySize;
            List.serializedProperty.arraySize++;
            List.index = index;

            SerializedProperty element = List.serializedProperty.GetArrayElementAtIndex(index);

            // Setup data field
            CustomFileName.PrefillType data = (CustomFileName.PrefillType)arg;
            element.FindPropertyRelative("type").enumValueIndex = (int)data;

            // Setup string value
            string text;
            if (CustomFileName.Prefill.DefaultTextMapper.TryGetValue(data, out text) == true)
            {
                element.FindPropertyRelative("text").stringValue = text;
            }

            // Apply the property
            List.serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
