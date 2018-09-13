using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Text;
using System.IO;
using OmiyaGames.Translations;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SupportedLanguagesEditor.cs" company="Omiya Games">
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
    /// <date>9/12/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor to allow editing <code>SupportedLanguages</code> scripts.
    /// </summary>
    /// <seealso cref="SupportedLanguages"/>
    [CustomEditor(typeof(SupportedLanguages), true)]
    public class SupportedLanguagesEditor : Editor
    {
        public const string DefaultFileName = "New Supported Languages" + Utility.FileExtensionScriptableObject;

        // FIXME: setup a new list

        [MenuItem("Omiya Games/Create/Supported Languages")]
        private static void CreateSupportedLanguages()
        {
            // Setup asset
            SupportedLanguages newAsset = ScriptableObject.CreateInstance<SupportedLanguages>();

            // Setup path to file
            string folderName = AssetUtility.GetSelectedFolder();
            string pathOfAsset = Path.Combine(folderName, DefaultFileName);
            pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(pathOfAsset);

            // Create the asset, and prompt the user to rename it
            ProjectWindowUtil.CreateAsset(newAsset, pathOfAsset);
        }

        private void OnEnable()
        {
            // FIXME: setup a new list
        }

        public override void OnInspectorGUI()
        {
            // FIXME: show a regular editable list of strings
            base.OnInspectorGUI();
        }
    }
}
