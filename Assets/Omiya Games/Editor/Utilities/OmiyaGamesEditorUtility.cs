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
    public static class EditorUiUtility
    {
        public const float MinHelpBoxHeight = 30f;
        public const float VerticalMargin = 2f;
        public const float VerticalSpace = 8f;
        public const float IndentSpace = 14f;

        public static float SingleLineHeight(float verticalMargin)
        {
            return EditorGUIUtility.singleLineHeight + (verticalMargin * 2);
        }

        public static float GetHeight(GUIContent label, int numRows, float verticalMargin = VerticalMargin)
        {
            if ((label != null) && (string.IsNullOrEmpty(label.text) == false))
            {
                numRows += 1;
            }
            return GetHeight(numRows, verticalMargin);
        }

        public static float GetHeight(int numRows, float verticalMargin = VerticalMargin)
        {
            float height = (EditorGUIUtility.singleLineHeight * numRows);
            height += (verticalMargin * numRows);
            return height;
        }

        public static float GetHelpBoxHeight(string text, float viewWidth, float minHeight = MinHelpBoxHeight)
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

        /// <summary>
        /// Helper method to draw enums from a limited range.
        /// </summary>
        /// <typeparam name="ENUM"></typeparam>
        /// <param name="property"></param>
        /// <param name="supportedEnums">List of supported enums.  The first element is treated as default.</param>
        public static void DrawEnum<ENUM>(SerializedProperty property, params ENUM[] supportedEnums) where ENUM : System.Enum
        {
            DrawEnum(property, supportedEnums, supportedEnums[0]);
        }

        /// <summary>
        /// Helper method to draw enums from a limited range.
        /// </summary>
        /// <typeparam name="ENUM"></typeparam>
        /// <param name="property"></param>
        /// <param name="supportedEnums">List of supported enums.  The first element is treated as default.</param>
        public static void DrawEnum<ENUM>(SerializedProperty property, ENUM[] supportedEnums, ENUM defaultEnum) where ENUM : System.Enum
        {
            // Setup the pop-up
            string[] enumNames = new string[supportedEnums.Length];
            int[] enumValues = new int[supportedEnums.Length];
            for(int index = 0; index < supportedEnums.Length; ++index)
            {
                enumNames[index] = ObjectNames.NicifyVariableName(supportedEnums[index].ToString());
                enumValues[index] = Utility.ConvertToInt(supportedEnums[index]);
            }

            // Disable the pop-up if there's only one option
            bool wasEnabled = GUI.enabled;
            GUI.enabled = (supportedEnums.Length > 1);

            // Show the pop-up
            property.enumValueIndex = EditorGUILayout.IntPopup(property.displayName, property.enumValueIndex, enumNames, enumValues);

            // Revert the later controls
            GUI.enabled = wasEnabled;

            // Verify the selected value is within range
            ENUM selectedValue = Utility.ConvertToEnum<ENUM>(property.enumValueIndex);
            if(ArrayUtility.Contains(supportedEnums, selectedValue) == false)
            {
                // If not, select the default option
                property.enumValueIndex = Utility.ConvertToInt(defaultEnum);
            }
        }

        /// <summary>
        /// Helper method to draw enums from a limited range.
        /// Draws a warning if the target has an enum that doesn't match the property
        /// </summary>
        /// <typeparam name="ENUM"></typeparam>
        /// <param name="property"></param>
        /// <param name="supportedEnums"></param>
        /// <param name="targetsEnum"></param>
        /// <param name="defaultEnum"></param>
        public static void DrawEnum<ENUM>(SerializedProperty property, ENUM[] supportedEnums, ENUM defaultEnum, ENUM targetsEnum, string message = "\"{0}\" is not supported; \"{1}\" will be used instead.") where ENUM : System.Enum
        {
            // Check if we need to display the warning
            int targetEnumValueIndex = Utility.ConvertToInt(targetsEnum);
            if (property.enumValueIndex != targetEnumValueIndex)
            {
                ENUM selectedValue = Utility.ConvertToEnum<ENUM>(property.enumValueIndex);
                string formattedMessage = string.Format(message,
                    ObjectNames.NicifyVariableName(selectedValue.ToString()),
                    ObjectNames.NicifyVariableName(targetsEnum.ToString()));
                EditorGUILayout.HelpBox(formattedMessage, MessageType.Warning);
            }

            // Draw enums as normal
            DrawEnum(property, supportedEnums, defaultEnum);
        }

        public static void DrawBoldFoldout(AnimBool buildSettingsAnimation, string displayLabel)
        {
            // Grab foldout style
            GUIStyle boldFoldoutStyle = EditorStyles.foldout;

            // Change the font to bold
            FontStyle lastFontStyle = boldFoldoutStyle.fontStyle;
            boldFoldoutStyle.fontStyle = FontStyle.Bold;

            // Draw the UI
            buildSettingsAnimation.target = EditorGUILayout.Foldout(buildSettingsAnimation.target, displayLabel, boldFoldoutStyle);

            // Revert the font
            boldFoldoutStyle.fontStyle = lastFontStyle;
        }
    }
}
