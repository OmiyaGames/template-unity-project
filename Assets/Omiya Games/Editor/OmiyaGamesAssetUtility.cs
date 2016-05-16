using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

namespace OmiyaGames
{
    public static class AssetUtility
    {
        public static float SingleLineHeight(float verticalMargin)
        {
            return EditorGUIUtility.singleLineHeight + (verticalMargin * 2);
        }

        public static DomainList GenerateAcceptedDomainList(StringBuilder builder, string folderName, string fileName, string[] content, out string pathOfAsset, string bundleId = null)
        {
            DomainList returnAsset = ScriptableObject.CreateInstance<DomainList>();
            returnAsset.name = fileName;
            returnAsset.Domains = content;

            // Generate a path to create an AcceptedDomainList
            builder.Length = 0;
            builder.Append(Path.Combine(folderName, fileName));
            builder.Append(Utility.FileExtensionScriptableObject);
            pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(builder.ToString());

            // Create the AcceptedDomainList at the assigned path
            AssetDatabase.CreateAsset(returnAsset, pathOfAsset);

            // Set its asset bundle name
            if(string.IsNullOrEmpty(bundleId) == false)
            {
                AssetImporter.GetAtPath(pathOfAsset).assetBundleName = bundleId;
            }

            // Save and refresh this asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return returnAsset;
        }

        public static void GenerateAssetBundle(string folderName, string bundleId, params string[] objectPaths)
        {
            // Create the array of bundle build details.
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            buildMap[0].assetBundleName = bundleId;
            buildMap[0].assetNames = objectPaths;

            // Put the bundles in folderName
            BuildPipeline.BuildAssetBundles(folderName, buildMap, BuildAssetBundleOptions.None, BuildTarget.WebGL);

            // Save and refresh this asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
