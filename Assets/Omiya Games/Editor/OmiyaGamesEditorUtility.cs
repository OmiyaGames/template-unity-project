using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OmiyaGamesEditorUtility.cs" company="Omiya Games">
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
    /// <date>9/20/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A series of utilities used throughout the <code>OmiyaGames</code> namespace.
    /// </summary>
    public static class EditorUtility
    {
        public static float SingleLineHeight(float verticalMargin)
        {
            return EditorGUIUtility.singleLineHeight + (verticalMargin * 2);
        }

        public static float GetHeight(GUIContent label, int numRows, float verticalMargin)
        {
            if ((label != null) && (string.IsNullOrEmpty(label.text) == false))
            {
                numRows += 1;
            }
            return (EditorGUIUtility.singleLineHeight + verticalMargin) * numRows;
        }

        public static float GetHelpBoxHeight(string text, float viewWidth, float minHeight)
        {
            var content = new GUIContent(text);
            var style = GUI.skin.GetStyle("helpbox");

            return Mathf.Max(minHeight, style.CalcHeight(content, viewWidth));
        }

        public static void CreateBool(Editor editor, ref AnimBool boolAnimation)
        {
            // Destroy the last animation, if any
            DestroyBool(editor, ref boolAnimation);

            // Setup new animation
            boolAnimation = new AnimBool(false);
            boolAnimation.valueChanged.AddListener(editor.Repaint);
        }

        public static void DestroyBool(Editor editor, ref AnimBool boolAnimation)
        {
            if (boolAnimation != null)
            {
                boolAnimation.valueChanged.RemoveListener(editor.Repaint);
                boolAnimation = null;
            }
        }
    }
}
