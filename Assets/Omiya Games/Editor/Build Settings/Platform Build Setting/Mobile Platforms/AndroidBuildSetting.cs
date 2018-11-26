using UnityEngine;
using UnityEditor;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AndroidBuildSetting.cs" company="Omiya Games">
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
    /// <date>10/31/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Build settings for Android platform.
    /// </summary>
    public class AndroidBuildSetting : MacBuildSetting
    {
        // FIXME: figure out Android specific build options
        [SerializeField]
        protected bool acceptExternalModificationsToPlayer = false;

        public static bool AndroidCredentialsFilled
        {
            get
            {
                // By default, return true
                bool returnFlag = true;

                // Check if there's an Android keystore name
                if (string.IsNullOrEmpty(PlayerSettings.Android.keystoreName) == false)
                {
                    // If so, by default, return false
                    returnFlag = false;

                    // Make sure all the passwords are filled in
                    if ((string.IsNullOrEmpty(PlayerSettings.keystorePass) == false) && (string.IsNullOrEmpty(PlayerSettings.keyaliasPass) == false))
                    {
                        // We're going to assume it's all good to go!
                        returnFlag = true;
                    }
                }
                return returnFlag;
            }
        }

        #region Overrides
        protected override BuildTargetGroup TargetGroup
        {
            get
            {
                return BuildTargetGroup.Android;
            }
        }

        protected override BuildTarget Target
        {
            get
            {
                return BuildTarget.Android;
            }
        }

        protected override BuildOptions Options
        {
            get
            {
                BuildOptions options = base.Options;

                // FIXME: add more options once fileds are determined
                if (acceptExternalModificationsToPlayer == true)
                {
                    options |= BuildOptions.AcceptExternalModificationsToPlayer;
                }
                return options;
            }
        }

        public override bool PreBuildCheck(out string message)
        {
            // Check if the android credentials are set
            message = null;
            bool returnFlag = AndroidCredentialsFilled;
            if (returnFlag == false)
            {
                // If not, prompt the user to fill in the Android credentials
                message = "Please fill out the Android Keystore credentials first, before building again.";
                AndroidKeystoreCredentialsWindow.Display(null);
            }
            return returnFlag;
        }
        #endregion
    }
}
