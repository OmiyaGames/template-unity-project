using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Text;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OmiyaGamesBuildScript.cs" company="Omiya Games">
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
    /// <date>10/30/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Quick editor window script to enter Android Keystore credentials.
    /// </summary>
    public class AndroidKeystoreCredentialsWindow : ScriptableWizard
    {
        public delegate void OnCredentialsEntered(AndroidKeystoreCredentialsWindow source);

        [System.Flags]
        public enum Error
        {
            None = 0x00,
            KeyStorePasswordIsEmpty = (0x01 << 0),
            KeyStorePasswordDoesntMatch = (0x01 << 1),
            KeyAliasIsEmpty = (0x01 << 2),
            KeyAliasPasswordIsEmpty = (0x01 << 3),
            KeyAliasPasswordDoesntMatch = (0x01 << 4),
        }

        OnCredentialsEntered afterSuccess = null;
        string keyStorePassword = null;
        string confirmKeyStorePassword = null;
        string keyAlias = null;
        AnimBool showAliasPasswords = null;
        string keyAliasPassword = null;
        string confirmKeyAliasPassword = null;
        Error errors = Error.None;
        readonly StringBuilder builder = new StringBuilder();

        public static void Display(OnCredentialsEntered onEnter = null)
        {
            // Retrieve window
            AndroidKeystoreCredentialsWindow window = DisplayWizard<AndroidKeystoreCredentialsWindow>("Android Keystore Credentials", "Derp", "Cancel");

            // Setup window
            window.errors = Error.None;
            window.keyAlias = PlayerSettings.Android.keyaliasName;
            window.afterSuccess = onEnter;
        }

        void OnEnable()
        {
            // Setup alias password
            if (showAliasPasswords == null)
            {
                showAliasPasswords = new AnimBool(false);
                showAliasPasswords.valueChanged.AddListener(Repaint);
            }
        }

        void OnGUI()
        {
            // Show Help box
            DrawHelpBox();
            EditorGUILayout.Space();

            // Show keystore passwords
            keyStorePassword = EditorGUILayout.PasswordField("Enter keystore password: ", keyStorePassword);
            confirmKeyStorePassword = EditorGUILayout.PasswordField("Confirm keystore password: ", confirmKeyStorePassword);
            EditorGUILayout.Space();

            // Show key alias
            keyAlias = EditorGUILayout.TextField("Enter alias name: ", keyAlias);
            EditorGUILayout.Space();

            // Show alias password
            DrawAliasPasswordGroup();
            EditorGUILayout.Space();

            // Show confirmation button
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if ((GUILayout.Button("OK") == true) && (IsPasswordsValid(out errors) == true))
            {
                // Populate the Android settings
                UpdateAndroidSettings();

                // Close window
                Close();

                // Clear out all the information stored by this dialog
                ClearInformation();

                // Run success
                if (afterSuccess != null)
                {
                    afterSuccess(this);
                }
            }
            else if (GUILayout.Button("Cancel") == true)
            {
                // Close window
                Close();

                // Clear out all the information stored by this dialog
                ClearInformation();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawAliasPasswordGroup()
        {
            showAliasPasswords.target = EditorGUILayout.ToggleLeft("Is alias password different from keystore?", showAliasPasswords.target);
            if (EditorGUILayout.BeginFadeGroup(showAliasPasswords.faded))
            {
                int previous = EditorGUI.indentLevel;
                EditorGUI.indentLevel += 1;
                keyAliasPassword = EditorGUILayout.PasswordField("Enter alias password: ", keyAliasPassword);
                confirmKeyAliasPassword = EditorGUILayout.PasswordField("Confirm alias password: ", confirmKeyAliasPassword);
                EditorGUI.indentLevel = previous;
            }
            EditorGUILayout.EndFadeGroup();
        }

        void DrawHelpBox()
        {
            if(errors == Error.None)
            {
                EditorGUILayout.HelpBox("Don't forget to enter your Android Keystore credentials!", MessageType.Info);
            }
            else
            {
                builder.Length = 0;
                builder.Append("Some fields are missing:");
                if((errors & Error.KeyStorePasswordIsEmpty) != 0)
                {
                    builder.AppendLine();
                    builder.Append("* Keystore password is not filled in!");
                }
                if ((errors & Error.KeyStorePasswordDoesntMatch) != 0)
                {
                    builder.AppendLine();
                    builder.Append("* Keystore password and confirmation does not match!");
                }
                if ((errors & Error.KeyAliasIsEmpty) != 0)
                {
                    builder.AppendLine();
                    builder.Append("* Keystore alias name is not filled in!");
                }
                if ((errors & Error.KeyAliasPasswordIsEmpty) != 0)
                {
                    builder.AppendLine();
                    builder.Append("* Alias password is not filled in!");
                }
                if ((errors & Error.KeyAliasPasswordDoesntMatch) != 0)
                {
                    builder.AppendLine();
                    builder.Append("* Alias password and confirmation does not match!");
                }
                EditorGUILayout.HelpBox(builder.ToString(), MessageType.Error);
            }
        }

        bool IsPasswordsValid(out Error allErrors)
        {
            allErrors = Error.None;

            // Verify keystore password
            if(string.IsNullOrEmpty(keyStorePassword) == true)
            {
                allErrors |= Error.KeyStorePasswordIsEmpty;
            }
            else if(string.Equals(keyStorePassword, confirmKeyStorePassword) == false)
            {
                allErrors |= Error.KeyStorePasswordDoesntMatch;
            }

            // Verify alias
            if (string.IsNullOrEmpty(keyAlias) == true)
            {
                allErrors |= Error.KeyAliasIsEmpty;
            }

            // Verify alias password
            if(showAliasPasswords.target == true)
            {
                if (string.IsNullOrEmpty(keyAliasPassword) == true)
                {
                    allErrors |= Error.KeyAliasPasswordIsEmpty;
                }
                else if (string.Equals(keyAliasPassword, confirmKeyAliasPassword) == false)
                {
                    allErrors |= Error.KeyAliasPasswordDoesntMatch;
                }
            }
            return (allErrors == Error.None);
        }

        void UpdateAndroidSettings()
        {
            // Update Android settings
            PlayerSettings.Android.keystorePass = keyStorePassword;
            PlayerSettings.Android.keyaliasName = keyAlias;
            if (showAliasPasswords.target == true)
            {
                PlayerSettings.Android.keyaliasPass = keyAliasPassword;
            }
            else
            {
                PlayerSettings.Android.keyaliasPass = keyStorePassword;
            }
        }

        void ClearInformation()
        {
            // Clear out the temporarily stored information
            keyStorePassword = null;
            confirmKeyStorePassword = null;
            keyAlias = null;
            keyAliasPassword = null;
            confirmKeyAliasPassword = null;

            // Remove previous activity information
            errors = Error.None;
            builder.Length = 0;
        }
    }
}
