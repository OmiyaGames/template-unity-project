using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="DomainListAssetBundleGenerator.cs" company="Omiya Games">
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
    /// <date>5/14/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Creates a window that, after pushing the "Generate Domain Asset," creates
    /// an Asset Bundle containing an instance of <code>AcceptedDomainList</code>.
    /// </summary>
    /// <seealso cref="DomainList"/>
    /// <seealso cref="WebLocationChecker"/>
    public class DomainListAssetBundleGenerator : EditorWindow
    {
        const float VerticalMargin = 2;
        const string CreateScriptableObjectAtFolder = "Assets/";
        const string ManifestFileExtension = ".manifest";
        const string BundleId = "domains";
        const string HelpMessage = "What is a Domain List Asset?\n\n" +
            "It's an asset containing a list of domains the WebLocationChecker script uses to" +
            " check if the domain of the website the WebGL build is running in is an accepted" +
            " host or not. Since Asset Bundles are serialized, and thus, harder to edit, it " +
            "creates a more secure method for developers to change the list of approved domains" +
            " on-the-fly while making it harder for others to change it.";
        const string TestErrorInvalidFileMessage = "Unable to read file. Is it really an Asset Bundle?";
        const string TestErrorInvalidAssetMessage = "Was able to read the Asset Bundle, but the asset contained in it was not an AcceptedDomainList object.";
        const string TestEmptyWarningMessage = "Was able to read the Asset Bundle, but the asset contained in it was not an AcceptedDomainList object.";
        const string TestInfoMessage = "Asset Bundle contains the following domains:";

        string nameOfFile = BundleId, nameOfFolder = "Assets/WebGLTemplates/Embedding/TemplateData", testResult = null;
        MessageType testResultType = MessageType.None;
        bool toggleGenerateArea = true, toggleTestArea = false;
        Object testAsset = null, lastTestAsset = null;
        List<string> allDomains = new List<string> { "localhost", "build.cloud.unity3d.com" };
        ReorderableList allDomainsField = null;
        //GUIStyle cachedFoldOutStyle = null;
        readonly StringBuilder builder = new StringBuilder();

        GUIStyle FoldOutstyle
        {
            get
            {
                //if(cachedFoldOutStyle == null)
                //{
                //    cachedFoldOutStyle = new GUIStyle(EditorStyles.foldout);
                //    cachedFoldOutStyle.fontStyle = FontStyle.Bold;
                //}
                return EditorStyles.foldout;
            }
        }

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
            EditorGUILayout.HelpBox(HelpMessage, MessageType.None);

            // Draw the area for generating domain assets
            DrawGenerateAssetArea();

            // Put a space between generated and test area
            EditorGUILayout.Space();

            // Draw the area for testing an asset bundle
            DrawTestAssetArea();
        }

        void OnEnable()
        {
            // Setup toggle
            toggleGenerateArea = true;
            toggleTestArea = false;

            // Setup Reordable list
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
        void DrawGenerateAssetArea()
        {
            // Draw foldout area
            EditorGUILayout.BeginVertical();
            toggleGenerateArea = EditorGUILayout.Foldout(toggleGenerateArea, "Generate Domain Asset", FoldOutstyle);
            if (toggleGenerateArea == true)
            {
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
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<AssetBundle>(pathOfAsset);
                }
            }
            EditorGUILayout.EndVertical();
        }

        void DrawTestAssetArea()
        {
            // Draw foldout area
            EditorGUILayout.BeginVertical();
            toggleTestArea = EditorGUILayout.Foldout(toggleTestArea, "Test Domain Asset", FoldOutstyle);
            if (toggleTestArea == true)
            {
                // Create a field to set an object
                testAsset = EditorGUILayout.ObjectField(lastTestAsset, typeof(Object), false);
                if ((testAsset == null) || (testAsset != lastTestAsset))
                {
                    // Clear results
                    testResult = null;
                    testResultType = MessageType.None;
                }
                lastTestAsset = testAsset;

                // Create a generate button
                if (GUILayout.Button("Test Domain Asset") == true)
                {
                    TestDomainAsset(testAsset, builder, out testResult, out testResultType);
                }

                // Print out results, if there are any
                if (string.IsNullOrEmpty(testResult) == false)
                {
                    EditorGUILayout.HelpBox(testResult, testResultType);
                }
            }
            EditorGUILayout.EndVertical();
        }

        static void TestDomainAsset(Object testAsset, StringBuilder builder, out string testResult, out MessageType testResultType)
        {
            // Setup variables
            AssetBundle bundle = null;
            testResult = TestErrorInvalidFileMessage;
            testResultType = MessageType.Error;
            try
            {
                // Load the bundle, and convert it to a domain list
                bundle = AssetBundle.LoadFromFile(AssetDatabase.GetAssetPath(testAsset));
                DomainList domainList = Utility.GetDomainList(bundle);

                // By default, indicate the bundle doesn't contain DomainList
                testResult = TestErrorInvalidAssetMessage;
                testResultType = MessageType.Error;

                // Check if the bundle contains an AcceptedDomainList
                if (domainList != null)
                {
                    // By default, indicate the domain list is empty
                    testResult = TestEmptyWarningMessage;
                    testResultType = MessageType.Warning;
                    if ((domainList != null) && (domainList.Count > 0))
                    {
                        // FIXME: list out all the domains in the list
                        builder.Length = 0;
                        builder.AppendLine(TestInfoMessage);
                        for(int index = 0; index < domainList.Count; ++index)
                        {
                            builder.Append("* \"");
                            builder.Append(domainList[index]);
                            builder.Append("\"");
                            if (index < (domainList.Count - 1))
                            {
                                builder.AppendLine();
                            }
                        }
                        testResult = builder.ToString();
                        testResultType = MessageType.Info;
                    }
                }
            }
            finally
            {
                if(bundle != null)
                {
                    // Clean-up
                    bundle.Unload(true);
                }
            }
        }

        // TODO: consider moving this logic to a separate editor utility script
        static DomainList GenerateAcceptedDomainList(StringBuilder builder, string folderName, string fileName, string[] content, out string pathOfAsset)
        {
            DomainList returnAsset = ScriptableObject.CreateInstance<DomainList>();
            returnAsset.name = fileName;
            returnAsset.Domains = content;

            // Generate a path to create an AcceptedDomainList
            builder.Length = 0;
            builder.Append(Path.Combine(folderName, fileName));
            builder.Append(Utility.ScriptableObjectFileExtension);
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
