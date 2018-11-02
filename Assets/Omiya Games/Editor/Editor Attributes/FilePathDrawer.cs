using UnityEngine;
using UnityEditor;
using System.IO;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="FilePathDrawer.cs" company="Omiya Games">
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
    /// Editor for <code>FilePathAttribute</code>.
    /// </summary>
    /// <seealso cref="FilePathAttribute"/>
    [CustomPropertyDrawer(typeof(FilePathAttribute))]
    public class FilePathDrawer : FolderPathDrawer
    {
        public override bool IsMessageBoxShown(SerializedProperty property)
        {
            return !File.Exists(property.stringValue);
        }

        protected override bool IsValid
        {
            get
            {
                return attribute is FilePathAttribute;
            }
        }

        public override string WrongAttributeMessage
        {
            get
            {
                return "Use FilePath attribute with a string.";
            }
        }

        protected override void OpenDialog(SerializedProperty property, GUIContent label)
        {
            // Open a file panel
            FilePathAttribute path = (FilePathAttribute)attribute;
            string browsedFile = UnityEditor.EditorUtility.OpenFilePanel(label.text, path.DefaultPath, path.FileExtension);

            // Check if a folder was found
            if (string.IsNullOrEmpty(browsedFile) == false)
            {
                property.stringValue = browsedFile;
            }
        }
    }
}
