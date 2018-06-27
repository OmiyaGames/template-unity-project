using UnityEngine;
using UnityEditor;
using OmiyaGames.Menu;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IntroMenuPlatformSettingsEditor.cs" company="Omiya Games">
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
    /// <date>6/26/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor script for <code>LevelIntroMenu.PlatformSettings</code>.
    /// </summary>
    /// <seealso cref="LevelIntroMenu.PlatformSettings"/>
    [CustomPropertyDrawer(typeof(LevelIntroMenu.PlatformSettings))]
    public class IntroMenuPlatformSettingsEditor : ISerializedContainerEditor
    {
        private const string StartStateName = "startState";

        protected override void DrawAllControls(SerializedProperty property, Rect singleLineRect)
        {
            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(singleLineRect, property.FindPropertyRelative("showCustomMessage"));
            singleLineRect.y += singleLineRect.height;
            EditorGUI.PropertyField(singleLineRect, property.FindPropertyRelative("showMouseLockMessageLabel"));
            singleLineRect.y += singleLineRect.height;
            EditorGUI.PropertyField(singleLineRect, property.FindPropertyRelative("background"));

            // Setup duration
            SerializedProperty startState = property.FindPropertyRelative(StartStateName);
            singleLineRect.y += singleLineRect.height;
            EditorGUI.PropertyField(singleLineRect, startState);
            if (startState.enumValueIndex == (int)LevelIntroMenu.StateOnStart.DisplayForDuration)
            {
                singleLineRect.y += singleLineRect.height;
                //EditorGUI.Slider
                EditorGUI.PropertyField(singleLineRect, property.FindPropertyRelative("displayDuration"));
            }
        }

        protected override int GetNumLinesToDraw(SerializedProperty property, GUIContent label)
        {
            int numLines = 4;

            // Add one if startState is a specific value
            SerializedProperty startState = property.FindPropertyRelative(StartStateName);
            if (startState.enumValueIndex == (int)LevelIntroMenu.StateOnStart.DisplayForDuration)
            {
                numLines += 1;
            }

            return numLines;
        }
    }
}
