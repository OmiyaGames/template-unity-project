using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Builds;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="HostArchiveSettingReorderableList.cs" company="Omiya Games">
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
    /// <date>2/12/2019</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper class to generate a reorderable list.
    /// </summary>
    public class HostArchiveSettingReorderableList : IReorderableList
    {
        public HostArchiveSettingReorderableList(Object target, SerializedProperty property, GUIContent label) : base(target, property, label)
        {
            // Setup List
            List.onAddCallback = OnBuildSettingListAdd;
            List.elementHeight = EditorUiUtility.GetHeight(2, 4f);
        }

        protected override void DrawBuildSettingListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Get all the properties
            SerializedProperty element = Property.GetArrayElementAtIndex(index);

            // Calculate position
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorUiUtility.VerticalMargin;

            // Draw a toggle
            Rect fieldRect = rect;
            fieldRect.width = rect.height + EditorUiUtility.VerticalMargin;
            HostArchiveSetting setting = (HostArchiveSetting)element.objectReferenceValue;
            setting.IsEnabled = EditorGUI.Toggle(fieldRect, setting.IsEnabled);

            // Check whether to enable the next set of UI
            bool originalEnabled = GUI.enabled;
            GUI.enabled = false;

            // Draw the object field
            fieldRect.x += fieldRect.width;
            fieldRect.width = rect.width - fieldRect.width;
            element.objectReferenceValue = EditorGUI.ObjectField(fieldRect, "", element.objectReferenceValue, typeof(IChildBuildSetting), false);
            GUI.enabled = originalEnabled;

            // Calculate position
            rect.y += rect.height;
            rect.y += EditorUiUtility.VerticalMargin;

            // Draw Edit buttons
            DrawButtons(rect, element.objectReferenceValue);
        }

        #region Helper Methods
        private void OnBuildSettingListAdd(ReorderableList list)
        {
            AddAndModify<HostArchiveSetting>("Host");
        }
        #endregion
    }
}
