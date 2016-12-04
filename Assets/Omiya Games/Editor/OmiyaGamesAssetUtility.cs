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
        static readonly StringBuilder tempBuilder = new StringBuilder();
        
        public static float SingleLineHeight(float verticalMargin)
        {
            return EditorGUIUtility.singleLineHeight + (verticalMargin * 2);
        }
        
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

        public static DomainList GenerateAcceptedDomainList(string folderName, string fileName, string[] content, out string pathOfAsset)
        {
            return GenerateAcceptedDomainList(tempBuilder, folderName, fileName, content, null, out pathOfAsset);
        }

        public static DomainList GenerateAcceptedDomainList(string folderName, string fileName, string[] content, string bundleId, out string pathOfAsset)
        {
            return GenerateAcceptedDomainList(tempBuilder, folderName, fileName, content, bundleId, out pathOfAsset);
        }

        public static DomainList GenerateAcceptedDomainList(StringBuilder builder, string folderName, string fileName, string[] content, out string pathOfAsset)
        {
            return GenerateAcceptedDomainList(builder, folderName, fileName, content, null, out pathOfAsset);
        }

        public static DomainList GenerateAcceptedDomainList(StringBuilder builder, string folderName, string fileName, string[] content, string bundleId, out string pathOfAsset)
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
