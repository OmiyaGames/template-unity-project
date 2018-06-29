using UnityEngine;
using UnityEditor;
using OmiyaGames.Menu;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IntroMenuCustomPlatformSettingsEditor.cs" company="Omiya Games">
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
    /// An editor script for <code>LevelIntroMenu.CustomPlatformSettings</code>.
    /// </summary>
    /// <seealso cref="LevelIntroMenu.CustomPlatformSettings"/>
    [CustomPropertyDrawer(typeof(LevelIntroMenu.CustomPlatformSettings))]
    public class IntroMenuCustomPlatformSettingsEditor : IntroMenuPlatformSettingsEditor
    {
        protected override void DrawAllControls(SerializedProperty property, Rect singleLineRect)
        {
            // Draw platforms
            EditorGUI.PropertyField(singleLineRect, property.FindPropertyRelative("platform"));
            MoveDownOneLine(ref singleLineRect);
            EditorGUI.PropertyField(singleLineRect, property.FindPropertyRelative("mouseLockState"));

            // Draw the rest from base class
            MoveDownOneLine(ref singleLineRect);
            base.DrawAllControls(property, singleLineRect);
        }

        protected override int GetNumLinesToDraw(SerializedProperty property, GUIContent label)
        {
            // Add one more line
            return base.GetNumLinesToDraw(property, label) + 2;
        }
    }
}
