using UnityEngine;
using UnityEditor;
using System.IO;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="FolderPathDrawer.cs" company="Omiya Games">
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
    /// <date>11/01/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Editor for <code>FolderPathAttribute</code>.
    /// </summary>
    /// <seealso cref="FolderPathAttribute"/>
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        const float messageHeight = 36f;
        const float buttonWidth = 63f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = base.GetPropertyHeight(property, label);
            if ((IsValid == true) && (IsMessageBoxShown(property) == true))
            {
                singleLineHeight += messageHeight;
            }
            return singleLineHeight;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // First get the attribute since it contains the range for the slider
            if (IsValid == true)
            {
                // Draw the property with a string field and a button.
                if (property.propertyType == SerializedPropertyType.String)
                {
                    Rect textPosition, buttonPosition;
                    bool showMessageBox = CalculatePositions(position, property, out textPosition, out buttonPosition);

                    // Label
                    EditorGUI.BeginProperty(position, label, property);

                    // Show button
                    if (GUI.Button(buttonPosition, "Browse...") == true)
                    {
                        OpenDialog(property, label);
                    }

                    // Show text field
                    property.stringValue = EditorGUI.TextField(textPosition, label, property.stringValue);

                    // Draw message box
                    if (showMessageBox)
                    {
                        // Draw the message box
                        position.height -= textPosition.height;
                        position.height -= EditorUiUtility.VerticalMargin;
                        EditorGUI.HelpBox(position, WrongPathMessage, MessageType.Error);
                    }
                    EditorGUI.EndProperty();
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, WrongAttributeMessage);
                }
            }
        }

        protected virtual bool IsValid
        {
            get
            {
                return attribute is FolderPathAttribute;
            }
        }

        public string WrongPathMessage
        {
            get
            {
                return "Invalid Path";
            }
        }

        public virtual string WrongAttributeMessage
        {
            get
            {
                return "Use FolderPath attribute with a string.";
            }
        }

        public virtual bool IsMessageBoxShown(SerializedProperty property)
        {
            return !Directory.Exists(property.stringValue);
        }

        private bool CalculatePositions(Rect position, SerializedProperty property, out Rect textPosition, out Rect buttonPosition)
        {
            // Calculate text positioning
            textPosition = position;
            textPosition.width -= buttonWidth + EditorUiUtility.VerticalMargin;

            // Calculate button positioning
            buttonPosition = position;
            buttonPosition.x += textPosition.width;
            buttonPosition.x += EditorUiUtility.VerticalMargin;
            buttonPosition.width = buttonWidth;

            // Draw message box
            bool showMessageBox = IsMessageBoxShown(property);
            if (showMessageBox)
            {
                // Calculate text positioning
                textPosition.y += textPosition.height;
                textPosition.height = EditorGUIUtility.singleLineHeight;
                textPosition.y -= textPosition.height;

                // Calculate button positioning
                buttonPosition.y = textPosition.y;
                buttonPosition.height = textPosition.height;
            }
            return showMessageBox;
        }

        protected virtual void OpenDialog(SerializedProperty property, GUIContent label)
        {
            // Open a folder panel
            FolderPathAttribute path = (FolderPathAttribute)attribute;
            string browsedFolder = UnityEditor.EditorUtility.OpenFolderPanel(label.text, path.DefaultPath, null);

            // Check if a folder was found
            if (string.IsNullOrEmpty(browsedFolder) == false)
            {
                property.stringValue = browsedFolder;
            }
        }
    }
}
