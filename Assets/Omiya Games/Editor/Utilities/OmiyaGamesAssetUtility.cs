using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OmiyaGamesUtility.cs" company="Omiya Games">
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
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A series of utilities used throughout the <code>OmiyaGames</code> namespace.
    /// </summary>
    public static class AssetUtility
    {
        public const string CreateScriptableObjectAtFolder = "Assets/";
        public const string ManifestFileExtension = ".manifest";
        public const string ConfirmationDialogTitle = "Overwrite File?";

        public static string GetLastFolderName(string path, bool pathIncludesFileName)
        {
            string returnPath = Path.GetFileName(path);
            if((pathIncludesFileName == true) || (string.IsNullOrEmpty(returnPath) == true))
            {
                returnPath = Path.GetFileName(Path.GetDirectoryName(path));
            }
            return returnPath;
        }

        public static string CreateFolderRecursively(string newFolderPath)
        {
            // Setup return value
            string returnGuid = null;
            
            // Check to see if the provided path ends properly
            string lastFolder = Path.GetFileName(newFolderPath);
            if(string.IsNullOrEmpty(lastFolder) == true)
            {
                newFolderPath = Path.GetDirectoryName(newFolderPath);
            }
            if(AssetDatabase.IsValidFolder(newFolderPath) == false)
            {
                // Create a stack of folders that doesn't exist yet
                Stack<string> newFolders = new Stack<string>();
                do
                {
                    // Push the last folder into the stack
                    lastFolder = GetLastFolderName(newFolderPath, false);
                    if(string.IsNullOrEmpty(lastFolder) == false)
                    {
                        newFolders.Push(lastFolder);
                    }

                    // Reduce the newFolderPath by the path
                    newFolderPath = Path.GetDirectoryName(newFolderPath);
                } while(AssetDatabase.IsValidFolder(newFolderPath) == false);

                // Go through the stack of new folders to create
                while(newFolders.Count > 0)
                {
                    lastFolder = newFolders.Pop();
                    returnGuid = AssetDatabase.CreateFolder(newFolderPath, lastFolder);
                    newFolderPath = Path.Combine(newFolderPath, lastFolder);
                }
            }
            return returnGuid;
        }

        public static string GetSelectedFolder()
        {
            string returnPath = null;

            // Check if there's a selected object
            var obj = Selection.activeObject;
            if (obj != null)
            {
                returnPath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                if((string.IsNullOrEmpty(returnPath) == false) && (Directory.Exists(returnPath) == false))
                {
                    returnPath = Path.GetDirectoryName(returnPath);
                }
            }

            if (string.IsNullOrEmpty(returnPath) == true)
            {
                returnPath = CreateScriptableObjectAtFolder;
            }
            return returnPath;
        }

        public static bool ConfirmFileIsWriteable(string pathOfAsset, string nameOfFile, bool showWindow = true)
        {
            // Check to see if file exists
            bool isBuildConfirmed = (File.Exists(pathOfAsset) == false);
            if ((isBuildConfirmed == false) && (showWindow == true))
            {
                // Create a message to indicate to the user
                StringBuilder builder = new StringBuilder();
                builder.Append("File \"");
                builder.Append(nameOfFile);
                builder.Append("\" already exists. Are you sure you want to overwrite this file?");

                // Bring up a pop-up confirming the file will be overwritten
                isBuildConfirmed = UnityEditor.EditorUtility.DisplayDialog(ConfirmationDialogTitle, builder.ToString(), "Yes", "No");
            }
            return isBuildConfirmed;
        }

        public static string SaveAsAssetBundle(ScriptableObject newAsset, string newFolderName, string newFileName, string bundleId, StringBuilder builder)
        {
            // Generate the asset bundle at the Assets folder
            string pathOfAsset = GenerateScriptableObject(newAsset, newFileName, bundleId, builder);
            GenerateAssetBundle(bundleId, pathOfAsset);

            // Clean-up the rest of the assets
            CleanUpFiles(builder, pathOfAsset, bundleId);

            if (string.IsNullOrEmpty(newFolderName) == false)
            {
                // Create a new folder if one doesn't exist
                CreateFolderRecursively(newFolderName);

                // Move the created asset bundle to the designated location
                MoveAssetBundleTo(builder, pathOfAsset, newFolderName, newFileName, bundleId);
            }
            return pathOfAsset;
        }

        #region SaveAsAssetBundle helpers
        private static string GenerateScriptableObject(ScriptableObject newAsset, string fileName, string bundleId, StringBuilder builder)
        {
            // Generate a path to create an AcceptedDomainList
            builder.Clear();
            builder.Append(Path.Combine(CreateScriptableObjectAtFolder, fileName));
            builder.Append(Utility.FileExtensionScriptableObject);
            string pathOfAsset = AssetDatabase.GenerateUniqueAssetPath(builder.ToString());

            // Create the AcceptedDomainList at the assigned path
            AssetDatabase.CreateAsset(newAsset, pathOfAsset);

            // Set its asset bundle name
            if (string.IsNullOrEmpty(bundleId) == false)
            {
                AssetImporter.GetAtPath(pathOfAsset).assetBundleName = bundleId;
            }

            // Save and refresh this asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return pathOfAsset;
        }

        private static void GenerateAssetBundle(string bundleId, params string[] objectPaths)
        {
            // Create the array of bundle build details.
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            buildMap[0].assetBundleName = bundleId;
            buildMap[0].assetNames = objectPaths;

            // Put the bundles in folderName
            BuildPipeline.BuildAssetBundles(CreateScriptableObjectAtFolder, buildMap, BuildAssetBundleOptions.None, BuildTarget.WebGL);

            // Save and refresh this asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void MoveAssetBundleTo(StringBuilder builder, string pathOfAsset, string newFolderName, string newFileName, string bundleId)
        {
            // Generate paths for the old file, to move to the new one
            string newPath = Path.Combine(newFolderName, newFileName);
            pathOfAsset = Path.Combine(CreateScriptableObjectAtFolder, bundleId);

            // Move the asset to the folder designated by the user
            FileUtil.ReplaceFile(pathOfAsset, newPath);
            FileUtil.DeleteFileOrDirectory(pathOfAsset);

            // Refresh the project window
            AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(newPath);
        }

        private static void CleanUpFiles(StringBuilder builder, string acceptedDomainListObjectPath, string bundleId)
        {
            // Clean-up the acceptedDomainListObject
            FileUtil.DeleteFileOrDirectory(acceptedDomainListObjectPath);

            // Clean-up the asset bundle for this folder
            string fileName = Path.GetFileName(Path.GetDirectoryName(CreateScriptableObjectAtFolder));
            builder.Clear();
            builder.Append(Path.Combine(CreateScriptableObjectAtFolder, fileName));
            FileUtil.DeleteFileOrDirectory(builder.ToString());

            // Clean-up the manifest files for this folder
            builder.Append(ManifestFileExtension);
            FileUtil.DeleteFileOrDirectory(builder.ToString());

            // Clean-up the manifest files for this folder
            builder.Clear();
            builder.Append(Path.Combine(CreateScriptableObjectAtFolder, bundleId));
            builder.Append(ManifestFileExtension);
            FileUtil.DeleteFileOrDirectory(builder.ToString());

            // Clean-up unused bundle IDs
            AssetDatabase.RemoveAssetBundleName(bundleId, false);
        }
        #endregion
    }
}
