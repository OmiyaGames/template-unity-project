using UnityEngine;
using UnityEngine.UI;
using MiniJSON;
using System.Collections.Generic;
using System.Text;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="VersionLabel.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
    /// <date>8/21/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A script that updates a <code>Text</code> to include Unity Cloud version information.
    /// </summary>
    /// <seealso cref="Text"/>
    [RequireComponent(typeof(Text))]
    public class VersionLabel : MonoBehaviour
    {
        // FIXME: consider adding boolean field options to choose which information to show.
        const string ManifestFileName = "UnityCloudBuildManifest.json";

        const string CommitKey = "scmCommitId";
        const string CommitLabel = "Commit: ";

        const string BuildKey = "buildNumber";
        const string BuildLabel = "Build: #";

        const string UnityKey = "unityVersion";
        const string UnityLabel = "Unity: ";

        void Start()
        {
            // Setup variables
            bool showLabel = false;
            Text label = GetComponent<Text>();
            TextAsset manifest = Resources.Load(ManifestFileName) as TextAsset;

            // Check to see if a label and json values are available
            if ((manifest != null) && (label != null))
            {
                // Parse the file
                Dictionary<string, object> buildMapping = Json.Deserialize(manifest.text) as Dictionary<string, object>;

                // Make sure the file parsing succeeded
                if(buildMapping != null)
                {
                    // Generate string to display
                    string versionString = GenerateVersionString(buildMapping);
                    if(string.IsNullOrEmpty(versionString) == false)
                    {
                        // Set the label's text
                        label.text = versionString;

                        // Indicate the label should be shown
                        showLabel = true;
                    }
                }
            }

            // Check if the text string has changed
            if(showLabel == false)
            {
                // Hide the text
                gameObject.SetActive(false);
            }
        }

        static string GenerateVersionString(Dictionary<string, object> buildMapping)
        {
            StringBuilder builder = new StringBuilder();
            System.Object checkValue = null;

            // Grab each field
            if ((buildMapping.TryGetValue(CommitKey, out checkValue) == true) && (checkValue != null))
            {
                AppendInfo(builder, CommitLabel, checkValue);
            }
            if ((buildMapping.TryGetValue(BuildKey, out checkValue) == true) && (checkValue != null))
            {
                AppendBuildNumber(builder, checkValue);
            }
            if ((buildMapping.TryGetValue(UnityKey, out checkValue) == true) && (checkValue != null))
            {
                AppendInfo(builder, UnityLabel, checkValue);
            }
            return builder.ToString();
        }

        static void AppendInfo(StringBuilder builder, string label, System.Object number)
        {
            // Add comma
            if (builder.Length > 0)
            {
                builder.Append(", ");
            }
            
            // Add build number
            builder.Append(label);
            builder.Append(number.ToString());
        }

        static void AppendBuildNumber(StringBuilder builder, System.Object checkValue)
        {
            // Add comma
            if (builder.Length > 0)
            {
                builder.Append(", ");
            }

            // Add platform
#if UNITY_IOS
            builder.Append("iOS ");
#elif UNITY_WINRT
            builder.Append("Windows ");
#elif UNITY_AMAZON
            builder.Append("Amazon ");
#elif UNITY_ANDROID
            builder.Append("Android ");
#endif

            // Add build number
            builder.Append(BuildLabel);
            builder.Append(checkValue.ToString());
        }
    }
}
