using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="WebLocationCheckerEditor.cs" company="Omiya Games">
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
    /// <date>5/15/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// Editor script for <code>WebLocationChecker</code>
    [CustomPropertyDrawer(typeof(WebLocationChecker))]
    public class WebLocationCheckerEditor : Editor
    {
        const int VerticalMargin = 2;

        SerializedProperty domainMustContain;
        SerializedProperty remoteDomainListUrl;
        SerializedProperty remoteListFileType;
        SerializedProperty splitRemoteDomainListUrlBy;
        SerializedProperty waitObjects;
        SerializedProperty forceRedirectIfDomainDoesntMatch;
        SerializedProperty redirectURL;

        ReorderableList domainMustContainList;
        ReorderableList splitRemoteDomainListUrlByList;
        ReorderableList waitObjectsList;

        public void OnEnable()
        {
            // Grab all serialized properties
            domainMustContain = serializedObject.FindProperty("domainMustContain");
            remoteDomainListUrl = serializedObject.FindProperty("remoteDomainListUrl");
            remoteListFileType = serializedObject.FindProperty("remoteListFileType");
            splitRemoteDomainListUrlBy = serializedObject.FindProperty("splitRemoteDomainListUrlBy");
            waitObjects = serializedObject.FindProperty("waitObjects");
            forceRedirectIfDomainDoesntMatch = serializedObject.FindProperty("forceRedirectIfDomainDoesntMatch");
            redirectURL = serializedObject.FindProperty("redirectURL");

            // Setup domainMustContain list
            domainMustContainList = new ReorderableList(serializedObject, domainMustContain, true, true, true, true);
            domainMustContainList.drawHeaderCallback = DrawDomainHeader;
            domainMustContainList.drawElementCallback = DrawDomainElement;
            domainMustContainList.elementHeight = AssetUtility.SingleLineHeight(VerticalMargin);

            // Setup splitRemoteDomainListUrlBy list
            splitRemoteDomainListUrlByList = new ReorderableList(serializedObject, splitRemoteDomainListUrlBy, true, true, true, true);
            splitRemoteDomainListUrlByList.drawHeaderCallback = DrawSplitHeader;
            splitRemoteDomainListUrlByList.drawElementCallback = DrawSplitElement;
            splitRemoteDomainListUrlByList.elementHeight = AssetUtility.SingleLineHeight(VerticalMargin);

            // Setup waitObjects list
            waitObjectsList = new ReorderableList(serializedObject, waitObjects, true, true, true, true);
            waitObjectsList.drawHeaderCallback = DrawWaitHeader;
            waitObjectsList.drawElementCallback = DrawWaitElement;
            waitObjectsList.elementHeight = AssetUtility.SingleLineHeight(VerticalMargin);
        }

        public override void OnInspectorGUI()
        {
            //serializedObject.Update();
            //EditorGUILayout.PropertyField(loadLevelAsynchronously, true);
            //EditorGUILayout.PropertyField(soundEffect, true);
            //EditorGUILayout.PropertyField(splash, true);
            //EditorGUILayout.PropertyField(mainMenu, true);
            //EditorGUILayout.PropertyField(credits, true);
            //levelList.DoLayoutList();

            //// Display the scene appending stuff
            //EditorGUILayout.Separator();
            //displayDefaults = EditorGUILayout.Foldout(displayDefaults, "Populate All Levels with scenes in Build Settings");
            //if (displayDefaults == true)
            //{
            //    int indent = EditorGUI.indentLevel;
            //    EditorGUI.indentLevel = 1;
            //    DrawDefaultLevelFields();
            //    DrawLevelListButtons();
            //    EditorGUI.indentLevel = indent;
            //}
            //serializedObject.ApplyModifiedProperties();
        }

        #region ReordableList Events
        void DrawDomainHeader(Rect rect)
        {
            // FIXME: add a label
            EditorGUI.LabelField(rect, "");
        }

        void DrawDomainElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = domainMustContain.GetArrayElementAtIndex(index);
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        void DrawSplitHeader(Rect rect)
        {
            // FIXME: add a label
            EditorGUI.LabelField(rect, "");
        }

        void DrawSplitElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = splitRemoteDomainListUrlBy.GetArrayElementAtIndex(index);
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        void DrawWaitHeader(Rect rect)
        {
            // FIXME: add a label
            EditorGUI.LabelField(rect, "");
        }

        void DrawWaitElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = waitObjects.GetArrayElementAtIndex(index);
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }
        #endregion
    }
}
