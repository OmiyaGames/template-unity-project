using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using OmiyaGames.Builds;
using System;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="CustomFileNameDrawer.cs" company="Omiya Games">
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
    /// <seealso cref="SupportedPlatforms"/>
    [CustomPropertyDrawer(typeof(CustomFileName))]
    public class CustomFileNameDrawer : PropertyDrawer
    {
        SerializedProperty names;
        SerializedProperty asSlug;

        CustomFileNameReorderableList namesList = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Get List
            SetupMemberVariables(property, label);

            // Calculate Height
            return namesList.List.GetHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Get List
            SetupMemberVariables(property, label);

            // Draw List
            namesList.List.DoList(position);

            // End this property
            EditorGUI.EndProperty();
        }

        public static CustomFileName GetTarget(SerializedProperty property)
        {
            return GetTarget(property.FindPropertyRelative("names"), property.FindPropertyRelative("asSlug"));
        }

        public static CustomFileName GetTarget(SerializedProperty names, SerializedProperty asSlug)
        {
            CustomFileName.Prefill[]
            prefills = new CustomFileName.Prefill[names.arraySize];
            for (int index = 0; index < prefills.Length; ++index)
            {
                SerializedProperty element = names.GetArrayElementAtIndex(index);
                SerializedProperty type = element.FindPropertyRelative("type");
                SerializedProperty text = element.FindPropertyRelative("text");
                prefills[index] = new CustomFileName.Prefill((CustomFileName.PrefillType)type.enumValueIndex, text.stringValue);
            }
            CustomFileName name = new CustomFileName(asSlug.boolValue, prefills);
            return name;
        }

        private void SetupMemberVariables(SerializedProperty property, GUIContent label)
        {
            names = property.FindPropertyRelative("names");
            asSlug = property.FindPropertyRelative("asSlug");
            if (namesList == null)
            {
                namesList = new CustomFileNameReorderableList(names, asSlug, label);
            }
        }
    }
}
