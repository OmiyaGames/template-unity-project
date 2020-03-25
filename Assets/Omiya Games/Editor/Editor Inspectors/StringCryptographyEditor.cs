using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using OmiyaGames.Translations;
using OmiyaGames.Editor;

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
    /// <date>2/11/2019</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor script for <see cref="StringCryptographer"/>.
    /// </summary>
    /// <seealso cref="StringCryptographer"/>
    [CustomEditor(typeof(StringCryptographer), true)]
    public class StringCryptographyEditor : UnityEditor.Editor
    {
        public const string DefaultFileName = "New Cryptographer" + Utility.FileExtensionScriptableObject;
        public const int RandomPasswordLength = 32;

        // Member variables
        private SerializedProperty passwordHash;
        private SerializedProperty saltKey;
        private SerializedProperty viKey;
        private AnimBool encryptionGroup, decryptionGroup;
        private string testEncryption, testDecryption;
        private GUIStyle foldoutStyle;

        [MenuItem("Assets/Create/Omiya Games/Cryptographer", priority = 203)]
        private static void CreateSupportedLanguages()
        {
            // Setup asset
            StringCryptographer newAsset = CreateInstance<StringCryptographer>();

            // Automatically fill out the fields
            newAsset.PasswordHash = StringCryptographer.GetRandomPassword(RandomPasswordLength);
            newAsset.SaltKey = StringCryptographer.GetRandomPassword(RandomPasswordLength);
            newAsset.ViKey = StringCryptographer.GetRandomPassword(StringCryptographer.ViKeyBlockSize);

            // Setup path to file
            string folderName = AssetUtility.GetSelectedFolder();
            string pathOfAsset = Path.Combine(folderName, DefaultFileName);
            pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(pathOfAsset);

            // Create the asset, and prompt the user to rename it
            ProjectWindowUtil.CreateAsset(newAsset, pathOfAsset);
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object
            serializedObject.Update();

            // Display all fields
            EditorGUILayout.PropertyField(passwordHash);
            EditorGUILayout.PropertyField(saltKey);
            EditorGUILayout.PropertyField(viKey);

            // Display a button to randomize all fields
            EditorGUILayout.Space();
            if (GUILayout.Button("Randomize all fields") == true)
            {
                passwordHash.stringValue = StringCryptographer.GetRandomPassword(RandomPasswordLength);
                saltKey.stringValue = StringCryptographer.GetRandomPassword(RandomPasswordLength);
                viKey.stringValue = StringCryptographer.GetRandomPassword(StringCryptographer.ViKeyBlockSize);
            }

            // Display test encryption
            EditorGUILayout.Space();
            EditorUiUtility.DrawBoldFoldout(encryptionGroup, "Test Encryption");
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(encryptionGroup.faded))
            {
                if (scope.visible == true)
                {
                    testEncryption = EditorGUILayout.DelayedTextField("Input", testEncryption);
                    string output = null;
                    if (string.IsNullOrEmpty(testEncryption) == false)
                    {
                        output = ((StringCryptographer)target).Encrypt(testEncryption);
                    }
                    EditorGUILayout.TextField("Output", output);
                }
            }

            // Display test decryption
            EditorGUILayout.Space();
            EditorUiUtility.DrawBoldFoldout(decryptionGroup, "Test Decryption");
            using (EditorGUILayout.FadeGroupScope scope = new EditorGUILayout.FadeGroupScope(decryptionGroup.faded))
            {
                if (scope.visible == true)
                {
                    testDecryption = EditorGUILayout.DelayedTextField("Input", testDecryption);
                    string output = null;
                    if (string.IsNullOrEmpty(testDecryption) == false)
                    {
                        output = ((StringCryptographer)target).Decrypt(testDecryption);
                    }
                    EditorGUILayout.TextField("Output", output);
                }
            }

            // Apply modifications
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            // Grab all properties
            passwordHash = serializedObject.FindProperty("passwordHash");
            saltKey = serializedObject.FindProperty("saltKey");
            viKey = serializedObject.FindProperty("viKey");

            // Setup the animations
            encryptionGroup = new AnimBool(false, Repaint);
            decryptionGroup = new AnimBool(false, Repaint);
        }
    }
}
