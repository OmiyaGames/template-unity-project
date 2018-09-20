using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslationStatus.cs" company="Omiya Games">
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
    /// <date>9/20/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// TODO.
    /// </summary>
    /// <seealso cref="TranslationDictionaryEditor"/>
    public class TranslationStatus : System.IDisposable
    {
        const float VerticalMargin = 2;
        const float VerticalSpace = 4;
        const float KeyLength = 30f;
        const float MinHelpBoxHeight = 30f;

        readonly Editor editor;
        SerializedProperty element;
        //readonly AnimBool showHelpBox;
        //readonly AnimBool showPreview;

        public TranslationStatus(Editor editor, SerializedProperty element)
        {
            // Setup member variables
            this.editor = editor;
            Element = element;
            //Width = EditorGUIUtility.currentViewWidth;

            // Setup the bools
            //EditorUtility.CreateBool(editor, ref showHelpBox);
            //EditorUtility.CreateBool(editor, ref showPreview);
        }

        //public AnimBool ShowHelpBox => showHelpBox;

        //public AnimBool ShowPreview => showPreview;

        public SerializedProperty KeyProperty
        {
            get;
            private set;
        }

        /// <summary>
        /// This is a hack variable.  Can't think of a better way to retrieve the width of the inside of the reorderablelist
        /// </summary>
        private float Width { get; set; }

        public SerializedProperty Element
        {
            get
            {
                return element;
            }
            private set
            {
                // Setup properties
                element = value;
                KeyProperty = element.FindPropertyRelative("key");
            }
        }

        public void Update(SerializedProperty element)
        {
            Element = element;
        }

        public float CalculateHeight(Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Calculate the key height
            float height = EditorGUIUtility.singleLineHeight;
            height += VerticalMargin * 3f;

            // Check if we're showing a warning
            string message = GetWarning(frequencyInKeyAppearance);
            if(string.IsNullOrEmpty(message) == false)
            {
                // If so, calculate the height of this warning
                height += VerticalSpace;
                height += GetHelpBoxHeight(message, Width)/* * ShowHelpBox.faded*/;
            }
            return height;
        }

        public void DrawGui(Rect rect, Dictionary<string, int> frequencyInKeyAppearance)
        {
            // FIXME: draw the element...somehow
            Width = rect.width;

            // Draw the key field
            rect.y += VerticalMargin;
            DrawKeyField(ref rect, frequencyInKeyAppearance);

            // Draw the warning, if any
            rect.y += VerticalSpace;
            if(DrawWarningMessage(ref rect, frequencyInKeyAppearance) == true)
            {
                // If there are, 
                rect.y += VerticalSpace;
            }
        }

        private void DrawKeyField(ref Rect rect, Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Hold onto the original rect position
            float originalX = rect.x;
            float originalWidth = rect.width;

            // Adjust the values 
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = KeyLength;

            // Draw the key label
            EditorGUI.LabelField(rect, "Key");

            // Draw the key text field
            rect.x += rect.width + VerticalSpace;
            rect.width = originalWidth - (KeyLength + VerticalSpace);
            string oldKey = KeyProperty.stringValue;
            KeyProperty.stringValue = EditorGUI.TextField(rect, oldKey);

            // Check if there's a difference
            if (oldKey != KeyProperty.stringValue)
            {
                // Update dictionary
                RemoveKeyFromFrequencyDictionary(frequencyInKeyAppearance, oldKey);
                AddKeyToFrequencyDictionary(frequencyInKeyAppearance, KeyProperty.stringValue);
            }

            // Re-adjust the rectangle, full-width for the next part
            rect.x = originalX;
            rect.y += rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = originalWidth;
        }

        private bool DrawWarningMessage(ref Rect rect, Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Adjust the bools
            string message = GetWarning(frequencyInKeyAppearance);
            bool isShown = false;
            if (string.IsNullOrEmpty(message) == false)
            {
                //ShowHelpBox.target = true;
                isShown = true;
            }

            //bool isShown = ((ShowHelpBox.target == true) || (ShowHelpBox.isAnimating == true));
            if (isShown == true)
            {
                // Calculate range of warning
                float helpBoxHeight = GetHelpBoxHeight(message, rect.width)/* * ShowHelpBox.faded*/;
                rect.height = helpBoxHeight;

                // Show warning
                //GUI.BeginGroup(rect);
                EditorGUI.HelpBox(rect, message, MessageType.Warning);
                //GUI.EndGroup();

                // Adjust the rectangle
                rect.y += helpBoxHeight;
            }
            return isShown;
        }

        public static void AddKeyToFrequencyDictionary(Dictionary<string, int> frequencyInKeyAppearance, string key)
        {
            // Make sure argument is correct
            if (frequencyInKeyAppearance != null)
            {
                if (string.IsNullOrEmpty(key) == false)
                {
                    // Add this key to the dictionary
                    if (frequencyInKeyAppearance.ContainsKey(key) == false)
                    {
                        frequencyInKeyAppearance.Add(key, 1);
                    }
                    else
                    {
                        frequencyInKeyAppearance[key] += 1;
                    }
                }
            }
        }

        public static void RemoveKeyFromFrequencyDictionary(Dictionary<string, int> frequencyInKeyAppearance, string key)
        {
            // Make sure argument is correct
            if (frequencyInKeyAppearance != null)
            {
                if ((string.IsNullOrEmpty(key) == true) && (frequencyInKeyAppearance.ContainsKey(key) == true))
                {
                    // Remove this key from the dictionary
                    frequencyInKeyAppearance[key] -= 1;
                    if (frequencyInKeyAppearance[key] <= 0)
                    {
                        // Remove the key if the value is below 0
                        frequencyInKeyAppearance.Remove(key);
                    }
                }
            }
        }

        public void Dispose()
        {
            //showHelpBox.valueChanged.RemoveListener(editor.Repaint);
            //showPreview.valueChanged.RemoveListener(editor.Repaint);
        }

        private string GetWarning(Dictionary<string, int> frequencyInKeyAppearance)
        {
            // Check what warning to display
            string message = null;
            if (string.IsNullOrEmpty(KeyProperty.stringValue) == true)
            {
                message = "Key cannot be an empty string.";
            }
            else if (frequencyInKeyAppearance[KeyProperty.stringValue] > 1)
            {
                message = "Multiple keys with the same name exists in this set.";
            }

            return message;
        }

        private static float GetHelpBoxHeight(string text, float viewWidth)
        {
            var content = new GUIContent(text);
            var style = GUI.skin.GetStyle("helpbox");

            return Mathf.Max(MinHelpBoxHeight, style.CalcHeight(content, viewWidth));
        }
    }
}

