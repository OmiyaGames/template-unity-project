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
    /// Editor script for <code>SceneInfo</code>
    /// </summary>
    /// <seealso cref="SceneInfo"/>
    [CustomPropertyDrawer(typeof(SceneInfo))]
    public class SceneInfoDrawer : PropertyDrawer
    {
        const int FileNameLabelWidth = 90;
        const float RevertTimeLabelWidthRatio = 110;
        const float RevertTimeFieldWidthRatio = 20;
        const float CursorModeLabelWidthRatio = 45;
        const int VerticalMargin = 2;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetHeight(base.GetPropertyHeight(property, label));
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

            // Draw the File Name label
            position.height = base.GetPropertyHeight(property, label);
            DrawTextField(position, property, "Scene Path", "scenePath");

            // Dock the rest of the fields down a bit
            position.y += (position.height + VerticalMargin);

            // Draw the Display Name label
            DrawTextField(position, property, "Display Name", "displayName");

            // Dock the rest of the fields down a bit
            position.y += (position.height + VerticalMargin);

            // We're going through this from right to left
            float xPosition = position.x;

            // Draw Revert Time field
            Rect fieldRect = position;
            fieldRect.x = xPosition;
            fieldRect.width = RevertTimeFieldWidthRatio;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("revertTimeScale"), GUIContent.none);
            xPosition += RevertTimeFieldWidthRatio;

            // Draw Revert Time label
            Rect labelRect = position;
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

            // Draw Cursor field
            fieldRect = position;
            fieldRect.x = xPosition;
            fieldRect.width = labelRect.x - xPosition;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("sceneCursorLockMode"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        static void DrawTextField(Rect position, SerializedProperty property, string label, string variableName)
        {
            Rect labelRect = position;
            labelRect.width = FileNameLabelWidth;
            EditorGUI.LabelField(labelRect, label, GUIStyle.none);

            // Draw the Scene Name field
            Rect fieldRect = position;
            fieldRect.x += labelRect.width;
            fieldRect.width -= labelRect.width;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(variableName), GUIContent.none);
        }

        public static float GetHeight(float baseHeight)
        {
            return (baseHeight * 3) + (VerticalMargin * 2);
        }
    }

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
            levelList.elementHeight = SceneInfoDrawer.GetHeight(EditorGUIUtility.singleLineHeight) + VerticalMargin;
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
                DrawDefaultLevelFields();
            }
            serializedObject.ApplyModifiedProperties();
        }

        void DrawLevelListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "All Levels");
        }

        void DrawLevelListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = levelList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += VerticalMargin;
            rect.height = SceneInfoDrawer.GetHeight(EditorGUIUtility.singleLineHeight);
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

            // Show Append button
            EditorGUILayout.Space();
            controlRect = EditorGUILayout.GetControlRect();
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