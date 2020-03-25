using UnityEngine;
using UnityEditor;
using OmiyaGames.Builds;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="CustomSettingDrawer.cs" company="Omiya Games">
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
    /// <date>11/20/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Property drawer for <code>CustomSetting</code>.
    /// </summary>
    /// <seealso cref="CustomSetting{TYPE}"/>
    public abstract class CustomSettingDrawer : PropertyDrawer
    {
        protected static void Indent(ref Rect position)
        {
            position.x += EditorUiUtility.IndentSpace;
            position.width -= EditorUiUtility.IndentSpace;
        }

        protected abstract float CustomValueHeight(SerializedProperty property, GUIContent label);

        protected abstract void DrawCustomValue(ref Rect position, SerializedProperty property, GUIContent label);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Get List
            float returnHeight = base.GetPropertyHeight(property, label);
            //returnHeight -= EditorGUIUtility.singleLineHeight;

            // Check if control is enabled
            SerializedProperty childProperty = property.FindPropertyRelative("enable");
            if (childProperty.boolValue == true)
            {
                returnHeight += CustomValueHeight(property.FindPropertyRelative("customValue"), label);
            }

            // Calculate Height
            return returnHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Setup position
            Rect childPosition = position;
            childPosition.height = EditorGUIUtility.singleLineHeight;

            // Draw enabled
            SerializedProperty childProperty = property.FindPropertyRelative("enable");
            EditorGUI.PropertyField(childPosition, childProperty, label);

            // Check if control is enabled
            if (childProperty.boolValue == true)
            {
                // Setup next control's position
                childProperty = property.FindPropertyRelative("customValue");
                childPosition.y += childPosition.height;
                childPosition.y += EditorUiUtility.VerticalMargin;
                childPosition.height = CustomValueHeight(childProperty, label);

                // Draw custom control
                DrawCustomValue(ref childPosition, childProperty, label);
            }

            // End this property
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// Property drawer for <code>SceneSettings</code>.
    /// </summary>
    /// <seealso cref="SceneSetting"/>
    [CustomPropertyDrawer(typeof(SceneSetting))]
    public class SceneSettingDrawer : CustomSettingDrawer
    {
        const float ButtonWidth = 64f;
        const float Space = 4f;
        static readonly string[] SceneFileFilter = new string[]
        {
            "Scene files", "unity",
            "All files", "*"
        };

        private SerializedProperty property = null;
        private UnityEditorInternal.ReorderableList list = null;

        private void CreateList(SerializedProperty property)
        {
            if ((list == null) || (this.property.serializedObject != property.serializedObject))
            {
                this.property = property;
                list = new UnityEditorInternal.ReorderableList(property.serializedObject, property);
                list.headerHeight = EditorUiUtility.VerticalMargin;
                list.drawElementCallback += DrawScene;
                list.elementHeight = EditorUiUtility.SingleLineHeight(EditorUiUtility.VerticalMargin);
            }
        }

        private void DrawScene(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (property != null)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                rect.y += EditorUiUtility.VerticalMargin;
                rect.height = EditorGUIUtility.singleLineHeight;

                // Draw the scene field
                ScenePathDrawer.DrawSceneAssetField(rect, element);
            }
        }

        protected override float CustomValueHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            CreateList(property);
            height = list.GetHeight();
            return height;
        }

        protected override void DrawCustomValue(ref Rect position, SerializedProperty property, GUIContent label)
        {
            CreateList(property);
            Indent(ref position);
            list.DoList(position);
        }
    }

    /// <summary>
    /// Property drawer for <code>SceneSettings</code>.
    /// </summary>
    /// <seealso cref="SceneSetting"/>
    [CustomPropertyDrawer(typeof(ScriptDefineSymbolsSetting))]
    public class ScriptDefineSymbolsSettingDrawer : CustomSettingDrawer
    {
        protected override float CustomValueHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void DrawCustomValue(ref Rect position, SerializedProperty property, GUIContent label)
        {
            Indent(ref position);
            property.stringValue = EditorGUI.DelayedTextField(position, property.stringValue);
        }
    }
}
