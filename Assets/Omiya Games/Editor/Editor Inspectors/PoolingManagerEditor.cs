using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PoolingManagerEditor.cs" company="Omiya Games">
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
    /// <date>5/25/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Editor script for <code>PoolingManager</code>
    /// </summary>
    /// <seealso cref="PoolingManager"/>
    [CustomEditor(typeof(PoolingManager))]
    public class PoolingManagerEditor : Editor
    {
        const float VerticalMargin = 2;

        ReorderableList objectsToPreloadList;
        SerializedProperty objectsToPreload;

        public void OnEnable()
        {
            objectsToPreload = serializedObject.FindProperty("objectsToPreload");

            objectsToPreloadList = new ReorderableList(serializedObject, objectsToPreload, true, true, true, true);
            objectsToPreloadList.drawHeaderCallback = DrawObjectsToPreloadListHeader;
            objectsToPreloadList.drawElementCallback = DrawObjectsToPreloadListElement;
            objectsToPreloadList.elementHeight = AssetUtility.SingleLineHeight(VerticalMargin);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            objectsToPreloadList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawObjectsToPreloadListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Preloaded Objects");
        }

        void DrawObjectsToPreloadListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = objectsToPreloadList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }
    }
}
