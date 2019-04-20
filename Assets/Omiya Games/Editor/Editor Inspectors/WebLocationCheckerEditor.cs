﻿using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Web;

namespace OmiyaGames.UI.Web
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
    [CustomEditor(typeof(WebLocationChecker))]
    public class WebLocationCheckerEditor : Editor
    {
        const string AcceptedDomainsFieldName = "Domain Names To Accept By Default";
        const string DescriptionMessage = "Any domain string received from any" +
            " sources (in the \"" + AcceptedDomainsFieldName + "\" list below or" +
            " a file downloaded from the \"" +
            WebLocationChecker.RemoteDomainListHeader + "\" fields) will be compared" +
            " to the hostname of the website this application's WebGL build is" +
            " running on. For example, the hostname for \"www.google.com/search?q=help\"" +
            " is \"www.google.com\", while \"google.com/?search?q=help\" is" +
            " \"google.com\". The status of this comparison will be set to this script's" +
            " CurrentState property, and optional redirect the player to" +
            " a specified website.\n\n" +
            "Domain string can contain wild cards: * matches a string of characters," +
            " while ? matches zero or one character. For example, \"*.google.com\"" +
            " will match \"www.google.com\", \"o.google.com\", and \".google.com\"," +
            " while \"?.google.com\" will only match \"o.google.com\", and" +
            " \".google.com\"";
        const int VerticalMargin = 2;

        SerializedProperty domainMustContain;
        SerializedProperty remoteDomainListUrl;
        SerializedProperty domainDecrypter;
        SerializedProperty waitObjects;
        SerializedProperty forceRedirectIfDomainDoesntMatch;
        SerializedProperty redirectURL;

        ReorderableList domainMustContainList;
        ReorderableList waitObjectsList;

        public void OnEnable()
        {
            // Grab all serialized properties
            domainMustContain = serializedObject.FindProperty("domainMustContain");
            remoteDomainListUrl = serializedObject.FindProperty("remoteDomainListUrl");
            domainDecrypter = serializedObject.FindProperty("domainDecrypter");
            waitObjects = serializedObject.FindProperty("waitObjects");
            forceRedirectIfDomainDoesntMatch = serializedObject.FindProperty("forceRedirectIfDomainDoesntMatch");
            redirectURL = serializedObject.FindProperty("redirectURL");

            // Setup domainMustContain list
            domainMustContainList = new ReorderableList(serializedObject, domainMustContain, true, true, true, true);
            domainMustContainList.drawHeaderCallback = DrawDomainHeader;
            domainMustContainList.drawElementCallback = DrawDomainElement;
            domainMustContainList.elementHeight = EditorUiUtility.SingleLineHeight(VerticalMargin);

            // Setup waitObjects list
            waitObjectsList = new ReorderableList(serializedObject, waitObjects, true, true, true, true);
            waitObjectsList.drawHeaderCallback = DrawWaitHeader;
            waitObjectsList.drawElementCallback = DrawWaitElement;
            waitObjectsList.elementHeight = EditorUiUtility.SingleLineHeight(VerticalMargin);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display Help Box
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(DescriptionMessage, MessageType.None);

            // Display the URL to where this application downloads a list of domains
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(remoteDomainListUrl, true);
            if (string.IsNullOrEmpty(remoteDomainListUrl.stringValue) == false)
            {
                // Display the decrypter
                EditorGUILayout.PropertyField(domainDecrypter, true);

                // Display list of objects to disable
                waitObjectsList.DoLayoutList();
            }

            // Display list of built-in accepted domains
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(AcceptedDomainsFieldName, EditorStyles.boldLabel);
            domainMustContainList.DoLayoutList();


            // Display option to force redirecting the web application to a specific website
            EditorGUILayout.PropertyField(forceRedirectIfDomainDoesntMatch, true);
            if (forceRedirectIfDomainDoesntMatch.boolValue == true)
            {
                // If the player wants to force redirect, show the URL field to redirect to.
                EditorGUILayout.PropertyField(redirectURL, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #region ReordableList Events
        void DrawDomainHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Domain Must Contain");
        }

        void DrawDomainElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = domainMustContain.GetArrayElementAtIndex(index);
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        void DrawWaitHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "GameObjects to Deactivate While Processing Hostname");
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
