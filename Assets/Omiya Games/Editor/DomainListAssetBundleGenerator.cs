using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OmiyaGames
{
    public class DomainListAssetBundleGenerator : EditorWindow
    {
        const float VerticalMargin = 2;
        const string ScriptableObjectFileExtension = ".asset";
        const string CreateScriptableObjectAtFolder = "Assets/";
        const string ManifestFileExtension = ".manifest";
        const string BundleId = "domains";

        string nameOfFile = BundleId;
        string nameOfFolder = "Assets/WebGLTemplates/Embedding/TemplateData";
        List<string> allDomains = new List<string> { "localhost", "build.cloud.unity3d.com" };
        ReorderableList allDomainsField = null;
        readonly StringBuilder builder = new StringBuilder();

        [MenuItem("Omiya Games/Domain List Asset Generator")]
        static void Initialize()
        {
            DomainListAssetBundleGenerator window = EditorWindow.GetWindow<DomainListAssetBundleGenerator>(title: "Domain List Asset Generator");
            window.Show();
        }

        #region Unity Events
        void OnGUI()
        {
            // Explain what this dialog does
            EditorGUILayout.HelpBox("What is a Domain List Asset?\nIt's an asset containing a list of domains the WebLocationChecker script uses to check if the domain of the website the WebGL build is running in is an accepted host or not. Since Asset Bundles are serialized, and thus, harder to edit, it creates a more secure method for developers to change the list of approved domains on-the-fly while making it harder for others to change it.", MessageType.Info);

            // Ask the list of layouts
            allDomainsField.DoLayoutList();

            // Ask the path this will generate
            GUILayout.Label("Name of folder to generate this asset:");
            nameOfFolder = EditorGUILayout.TextField(nameOfFolder);

            // Ask the name of the file this will generate
            GUILayout.Label("Name of asset to generate:");
            nameOfFile = EditorGUILayout.TextField(nameOfFile);

            // Create a generate button
            if (GUILayout.Button("Generate Domain Asset") == true)
            {
                // Generate the asset bundle at the Assets folder
                string pathOfAsset;
                GenerateAcceptedDomainList(builder, CreateScriptableObjectAtFolder, nameOfFile, allDomains.ToArray(), out pathOfAsset);
                GenerateAssetBundle(CreateScriptableObjectAtFolder, BundleId, pathOfAsset);

                // clean-up the rest of the assets
                CleanUpFiles(builder, pathOfAsset);

                // Move the asset to the folder designated by the user
                pathOfAsset = Path.Combine(nameOfFolder, nameOfFile);
                FileUtil.MoveFileOrDirectory(Path.Combine(CreateScriptableObjectAtFolder, BundleId), pathOfAsset);

                // Refresh the project window
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<AssetBundle>(pathOfAsset);
            }
        }

        void OnEnable()
        {
            allDomainsField = new ReorderableList(allDomains, typeof(string), true, true, true, true);
            allDomainsField.drawHeaderCallback = DrawLevelListHeader;
            allDomainsField.drawElementCallback = DrawLevelListElement;
            allDomainsField.onAddCallback = OnAddDomain;
            allDomainsField.elementHeight = (EditorGUIUtility.singleLineHeight + (VerticalMargin * 2));
        }
        #endregion

        #region Reordable List Events
        void OnAddDomain(ReorderableList list)
        {
            string toAdd = string.Empty;
            if (allDomains.Count > 0)
            {
                if ((list.index >= 0) && (list.index < allDomains.Count))
                {
                    toAdd = allDomains[list.index];
                }
                else
                {
                    toAdd = allDomains[allDomains.Count - 1];
                }
            }
            allDomains.Add(toAdd);
        }

        void DrawLevelListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "All Accepted Domains");
        }

        void DrawLevelListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            allDomains[index] = EditorGUI.TextField(rect, GUIContent.none, allDomains[index]);
        }
        #endregion

        #region Helper Methods
        // TODO: consider moving this logic to a separate editor utility script
        static AcceptedDomainList GenerateAcceptedDomainList(StringBuilder builder, string folderName, string fileName, string[] content, out string pathOfAsset)
        {
            AcceptedDomainList returnAsset = ScriptableObject.CreateInstance<AcceptedDomainList>();
            returnAsset.name = fileName;
            returnAsset.AllDomains = content;

            // Generate a path to create an AcceptedDomainList
            builder.Length = 0;
            builder.Append(Path.Combine(folderName, fileName));
            builder.Append(ScriptableObjectFileExtension);
            pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(builder.ToString());

            // Create the AcceptedDomainList at the assigned path
            AssetDatabase.CreateAsset(returnAsset, pathOfAsset);

            // Set its asset bundle name
            AssetImporter importer = AssetImporter.GetAtPath(pathOfAsset);
            importer.assetBundleName = BundleId;

            // Save and refresh this asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return returnAsset;
        }

        // TODO: consider moving this logic to a separate editor utility script
        static void GenerateAssetBundle(string folderName, string bundleId, params string[] objectPaths)
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

        static void CleanUpFiles(StringBuilder builder, string acceptedDomainListObjectPath)
        {
            // Clean-up the acceptedDomainListObject
            FileUtil.DeleteFileOrDirectory(acceptedDomainListObjectPath);

            // Clean-up the asset bundle for this folder
            string fileName = Path.GetFileName(Path.GetDirectoryName(CreateScriptableObjectAtFolder));
            builder.Length = 0;
            builder.Append(Path.Combine(CreateScriptableObjectAtFolder, fileName));
            FileUtil.DeleteFileOrDirectory(builder.ToString());

            // Clean-up the manifest files for this folder
            builder.Append(ManifestFileExtension);
            FileUtil.DeleteFileOrDirectory(builder.ToString());

            // Clean-up the manifest files for this folder
            builder.Length = 0;
            builder.Append(Path.Combine(CreateScriptableObjectAtFolder, BundleId));
            builder.Append(ManifestFileExtension);
            FileUtil.DeleteFileOrDirectory(builder.ToString());

            // Clean-up unused bundle IDs
            AssetDatabase.RemoveAssetBundleName(BundleId, false);
        }
        #endregion
    }
}
