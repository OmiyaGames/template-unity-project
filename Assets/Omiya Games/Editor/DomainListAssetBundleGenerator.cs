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
        const string BundleId = "accepteddomains";

        string nameOfFile = "domains";
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
            GUILayout.Label("Name of folder to generate this asset");
            nameOfFolder = GUILayout.TextField(nameOfFolder);

            // Ask the name of the file this will generate
            GUILayout.Label("Name of asset to generate");
            nameOfFile = GUILayout.TextField(nameOfFile);

            // Generate the asset
            if (GUILayout.Button("Generate Domain Asset") == true)
            {
                string pathOfAsset;
                GenerateAcceptedDomainList(builder, CreateScriptableObjectAtFolder, nameOfFile, allDomains.ToArray(), out pathOfAsset);
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
            AssetImporter importer = AssetImporter.GetAtPath(pathOfAsset);
            importer.assetBundleName = BundleId;
            AssetDatabase.SaveAssets();

            // Select this asset
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = returnAsset;

            return returnAsset;
        }

        void GenerateAssetBundle()
        {
            //// Create the array of bundle build details.
            //AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

            //buildMap[0].assetBundleName = "EnemyBundle";
            //string[] enemyAssets = new[] { "EnemyAlienShip", "EnemyAlienShipDamaged" };
            //buildMap[0].assetNames = enemyAssets;

            //buildMap[1].assetBundleName = "HeroBundle";
            //string[] heroAssets = new[] { "HeroShip", "HeroShipDamaged" };
            //buildMap[1].assetNames = heroAssets;

            //// Put the bundles in a folder called "AssetBundles" within the Assets folder.
            //BuildPipeline.BuildAssetBundles("Assets/AssetBundles", buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);
        }
        #endregion
    }
}
