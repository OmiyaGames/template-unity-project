using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Text;
using System.IO;
using OmiyaGames.Translations;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslationDictionaryEditor.cs" company="Omiya Games">
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
    /// <date>9/12/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor to allow editing <code>TranslationDictionary</code> scripts.
    /// </summary>
    /// <seealso cref="TranslationDictionary"/>
    [CustomEditor(typeof(TranslationDictionary), true)]
    public class TranslationDictionaryEditor : Editor
    {
        public const string DefaultFileName = "New Translation Dictionary" + Utility.FileExtensionScriptableObject;
        public const string BundleId = "translation";

        [MenuItem("Omiya Games/Create Translation Dictionary")]
        private static void CreateTranslationDictionary()
        {
            string folderName = AssetUtility.GetSelectedFolder();
            StringBuilder builder = new StringBuilder();

            // Check if file already exists
            string pathOfAsset = Path.Combine(folderName, DefaultFileName);
            if (AssetUtility.ConfirmFileIsWriteable(pathOfAsset, DefaultFileName) == true)
            {
                // Setup asset
                TranslationDictionary newAsset = ScriptableObject.CreateInstance<TranslationDictionary>();
                newAsset.name = DefaultFileName;

                // Generate the asset bundle
                AssetUtility.SaveAsAssetBundle(newAsset, folderName, DefaultFileName, BundleId, builder);
            }
        }

        private void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.HelpBox("TEsting....", MessageType.Info);
        }
    }
}
