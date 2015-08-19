using UnityEngine;
using UnityEngine.UI;
using MiniJSON;
using System.Collections.Generic;
using System.Text;

namespace OmiyaGames
{
    [RequireComponent(typeof(Text))]
    public class VersionLabel : MonoBehaviour
    {
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
