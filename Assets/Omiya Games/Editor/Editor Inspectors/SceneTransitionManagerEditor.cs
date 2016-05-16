using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SceneTransitionManagerEditor.cs" company="Omiya Games">
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
    /// <date>4/15/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Editor script for <code>SceneInfo</code>
    /// </summary>
    /// <seealso cref="SceneInfo"/>
    [CustomPropertyDrawer(typeof(SceneInfo))]
    public class SceneInfoDrawer : PropertyDrawer
    {
        const int FileNameLabelWidth = 96;
        const float RevertTimeLabelWidth = 120;
        const float RevertTimeFieldTotalMargin = 14;
        const int CursorModeLabelWidth = 160;
        const int VerticalMargin = 2;

        static GUIStyle rightAlignStyleCache = null;
        public static GUIStyle RightAlignStyle
        {
            get
            {
                if(rightAlignStyleCache == null)
                {
                    rightAlignStyleCache = new GUIStyle();
                    rightAlignStyleCache.alignment = TextAnchor.MiddleRight;
                }
                return rightAlignStyleCache;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(!string.IsNullOrEmpty(label.text));
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
            Rect labelRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (string.IsNullOrEmpty(label.text) == false)
            {
                position.y += (EditorGUIUtility.singleLineHeight + VerticalMargin);
                EditorGUI.indentLevel = 1;
            }

            // Draw the File Name label
            position.height = base.GetPropertyHeight(property, label);
            DrawTextField(position, property, "Scene Path", "scenePath");

            // Dock the rest of the fields down a bit
            position.y += (position.height + VerticalMargin);

            // We're going through this from right to left
            // Draw Revert Time field
            Rect fieldRect = position;
            fieldRect.width = EditorGUIUtility.singleLineHeight;
            fieldRect.x = (position.xMax - fieldRect.width);
            if (string.IsNullOrEmpty(label.text) == false)
            {
                fieldRect.x -= RevertTimeFieldTotalMargin;
            }
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("revertTimeScale"), GUIContent.none);

            // Draw Revert Time label
            labelRect = position;
            labelRect.width = RevertTimeLabelWidth;
            labelRect.x = (fieldRect.x - labelRect.width);
            if (string.IsNullOrEmpty(label.text) == true)
            {
                labelRect.x -= RevertTimeFieldTotalMargin;
            }
            EditorGUI.LabelField(labelRect, "Reset TimeScale?", RightAlignStyle);

            // Draw the Display Name label
            fieldRect.x = position.x;
            fieldRect.width = (labelRect.x - position.x);
            DrawTextField(fieldRect, property, "Display Name", "displayName");

            // Dock the rest of the fields down a bit
            position.y += (position.height + VerticalMargin);

            // Draw Cursor label
            DrawTextField(position, property, "Cursor Lock Mode (App)", "cursorMode", CursorModeLabelWidth);

            // Dock the rest of the fields down a bit
            position.y += (position.height + VerticalMargin);
            DrawTextField(position, property, "Cursor Lock Mode (Web)", "cursorModeWeb", CursorModeLabelWidth);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        static void DrawTextField(Rect position, SerializedProperty property, string label, string variableName, float labelWidth = FileNameLabelWidth)
        {
            Rect labelRect = position;
            labelRect.width = labelWidth;
            EditorGUI.LabelField(labelRect, label, GUIStyle.none);

            // Draw the Scene Name field
            Rect fieldRect = position;
            fieldRect.x += labelRect.width;
            fieldRect.width -= labelRect.width;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(variableName), GUIContent.none);
        }

        internal static float GetHeight(bool containsPrefixLabel)
        {
            int numRows = 4;
            if(containsPrefixLabel == true)
            {
                numRows += 1;
            }
            return (EditorGUIUtility.singleLineHeight * numRows) + (VerticalMargin * (numRows - 1));
        }
    }

    ///-----------------------------------------------------------------------
    /// <copyright file="SceneTransitionManagerEditor.cs" company="Omiya Games">
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
    /// <date>4/15/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Editor script for <code>SceneTransitionManager</code>
    /// </summary>
    /// <seealso cref="SceneTransitionManager"/>
    [CustomEditor(typeof(SceneTransitionManager))]
    public class SceneTransitionManagerEditor : Editor
    {
        const float VerticalMargin = 2;

        SerializedProperty loadLevelAsynchronously;
        SerializedProperty soundEffect;
        SerializedProperty splash;
        SerializedProperty mainMenu;
        SerializedProperty credits;
        SerializedProperty levels;
        ReorderableList levelList;

        static bool displayDefaults = false;
        static int defaultFillIn = 0;
        static string defaultDisplayName = "Level {0}";
        static bool defaultRevertsTimeScale = true;
        static CursorLockMode defaultLockMode = CursorLockMode.None;
        static readonly string[] DefaultFillInOptions = new string[]
        {
            "Ordinal",
            "Scene Name"
        };
        static readonly GUIContent DefaultDisplayNameLabel = new GUIContent("New Display Name");
        static readonly GUIContent DefaultFillInLabel = new GUIContent("Fill-in {0} with...");
        static readonly GUIContent DefaultRevertsTimeScaleLabel = new GUIContent("New Revert TimeScale");
        static readonly GUIContent DefaultCursorLockModeLabel = new GUIContent("New Cursor Lock Mode");

        public void OnEnable()
        {
            // Grab all serialized properties
            loadLevelAsynchronously = serializedObject.FindProperty("loadLevelAsynchronously");
            soundEffect = serializedObject.FindProperty("soundEffect");
            splash = serializedObject.FindProperty("splash");
            mainMenu = serializedObject.FindProperty("mainMenu");
            credits = serializedObject.FindProperty("credits");
            levels = serializedObject.FindProperty("levels");

            // Setup level list
            levelList = new ReorderableList(serializedObject, levels, true, true, true, true);
            levelList.drawHeaderCallback = DrawLevelListHeader;
            levelList.drawElementCallback = DrawLevelListElement;
            levelList.elementHeight = SceneInfoDrawer.GetHeight(false) + VerticalMargin;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(loadLevelAsynchronously, true);
            EditorGUILayout.PropertyField(soundEffect, true);
            EditorGUILayout.PropertyField(splash, true);
            EditorGUILayout.PropertyField(mainMenu, true);
            EditorGUILayout.PropertyField(credits, true);
            levelList.DoLayoutList();

            // Display the scene appending stuff
            EditorGUILayout.Separator();
            displayDefaults = EditorGUILayout.Foldout(displayDefaults, "Populate All Levels with scenes in Build Settings");
            if (displayDefaults == true)
            {
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;
                DrawDefaultLevelFields();
                DrawLevelListButtons();
                EditorGUI.indentLevel = indent;
            }
            serializedObject.ApplyModifiedProperties();
        }

        void DrawLevelListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "All Levels");
        }

        void DrawLevelListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = levels.GetArrayElementAtIndex(index);
            rect.y += VerticalMargin;
            rect.height = SceneInfoDrawer.GetHeight(false);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        void DrawDefaultLevelFields()
        {
            // Default Display Name
            Rect controlRect = EditorGUILayout.GetControlRect();
            controlRect = EditorGUI.PrefixLabel(controlRect, GUIUtility.GetControlID(FocusType.Passive), DefaultDisplayNameLabel);
            defaultDisplayName = EditorGUI.TextField(controlRect, defaultDisplayName);


            // Default Fill-In field
            controlRect = EditorGUILayout.GetControlRect();
            controlRect = EditorGUI.PrefixLabel(controlRect, GUIUtility.GetControlID(FocusType.Passive), DefaultFillInLabel);
            defaultFillIn = EditorGUI.Popup(controlRect, defaultFillIn, DefaultFillInOptions);

            // Default Revert TimeScale field
            controlRect = EditorGUILayout.GetControlRect();
            controlRect = EditorGUI.PrefixLabel(controlRect, GUIUtility.GetControlID(FocusType.Passive), DefaultRevertsTimeScaleLabel);
            defaultRevertsTimeScale = EditorGUI.Toggle(controlRect, defaultRevertsTimeScale);


            // Default Cursor field
            controlRect = EditorGUILayout.GetControlRect();
            controlRect = EditorGUI.PrefixLabel(controlRect, GUIUtility.GetControlID(FocusType.Passive), DefaultCursorLockModeLabel);
            defaultLockMode = (CursorLockMode)EditorGUI.EnumPopup(controlRect, defaultLockMode);
        }

        void DrawLevelListButtons()
        {
            // Show Append button
            EditorGUILayout.Space();
            Rect controlRect = EditorGUILayout.GetControlRect();
            controlRect.height += (VerticalMargin * 2);
            if (GUI.Button(controlRect, "Append new scenes in Build Settings to All Levels list") == true)
            {
                // Actually append scenes to the list
                SceneTransitionManager manager = ((SceneTransitionManager)target);
                manager.SetupLevels(defaultDisplayName, (defaultFillIn != 0), defaultRevertsTimeScale, defaultLockMode, true);

                // Untoggle
                displayDefaults = false;
            }

            // Show Replace button
            EditorGUILayout.Space();
            controlRect = EditorGUILayout.GetControlRect();
            controlRect.height += (VerticalMargin * 2);
            if (GUI.Button(controlRect, "Replace All Levels list with scenes in Build Settings") == true)
            {
                // Actually append scenes to the list
                SceneTransitionManager manager = ((SceneTransitionManager)target);
                manager.SetupLevels(defaultDisplayName, (defaultFillIn != 0), defaultRevertsTimeScale, defaultLockMode, false);

                // Untoggle
                displayDefaults = false;
            }

            EditorGUILayout.Space();
        }
    }
}