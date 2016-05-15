using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
    public static class Utility
    {
        public const float SnapToThreshold = 0.01f;
        public const string ScriptableObjectFileExtension = ".asset";

        /// <summary>
        /// Shuffles the list.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="upTo">Number of elements to shuffle, starting at index 0.
        /// Elements outside of this range maybe be shuffled between this range as well.
        /// If negative, will shuffle all list elements.</param>
        /// <typeparam name="H">The list type parameter.</typeparam>
        public static void ShuffleList<H>(IList<H> list, int upTo = -1)
        {
            // Check if we want to shuffle the entire list
            if((upTo < 0) || (upTo > list.Count))
            {
                upTo = list.Count;
            }

            // Go through every list element
            H swapObject = default(H);
            int index = 0, randomIndex = 0;
            for(; index < upTo; ++index)
            {
                // Swap a random element
                randomIndex = Random.Range(0, list.Count);
                if(index != randomIndex)
                {
                    swapObject = list[index];
                    list[index] = list[randomIndex];
                    list[randomIndex] = swapObject;
                }
            }
        }

        public static void RemoveDuplicateEntries<H>(List<H> list, IEqualityComparer<H> comparer = null)
        {
            // Go through every list element
            int focusIndex = 0, compareIndex = 0;
            bool isDuplicate = false;
            for (; focusIndex < list.Count; ++focusIndex)
            {
                // Start the loop with the next element the next element
                for(compareIndex = (focusIndex + 1); compareIndex < list.Count; ++compareIndex)
                {
                    // Check if the elements are the same
                    if (comparer == null)
                    {
                        isDuplicate = list[focusIndex].Equals(list[compareIndex]);
                    }
                    else
                    {
                        isDuplicate = comparer.Equals(list[focusIndex], list[compareIndex]);
                    }

                    // Check if this element is a dupicate
                    if (isDuplicate == true)
                    {
                        // If so, remove from the list
                        list.RemoveAt(compareIndex);
                        --compareIndex;
                    }
                }
            }
        }

        public static void Log(string message)
        {
#if DEBUG
            // Only do something if we're in debug mode
            Debug.Log(message);
#endif
        }

        public static DomainList GetDomainList(AssetBundle bundle, string assetNameNoExtension = null)
        {
            DomainList returnDomain = null;

            // Search for an *.asset file
            string[] allAssets = bundle.GetAllAssetNames();
            string firstAsset = null;
            if(allAssets != null)
            {
                for(int index = 0; index < allAssets.Length; ++index)
                {
                    if((string.IsNullOrEmpty(allAssets[index]) == false) &&
                        (Path.GetExtension(allAssets[index]) == ScriptableObjectFileExtension) &&
                        ((string.IsNullOrEmpty(assetNameNoExtension) == true) || (Path.GetFileNameWithoutExtension(allAssets[index]) == assetNameNoExtension)))
                    {
                        firstAsset = allAssets[index];
                        break;
                    }
                }
            }

            // Check if an asset is found
            if(string.IsNullOrEmpty(firstAsset) == false)
            {
                try
                {
                    // Convert it to an AcceptedDomainList
                    returnDomain = bundle.LoadAsset<DomainList>(firstAsset);
                }
                catch(System.Exception)
                {
                    returnDomain = null;
                }
            }
            return returnDomain;
        }

        public static string BuildTestMessage(StringBuilder builder, WebLocationChecker webChecker)
        {
            builder.AppendLine("Information according to the WebLocationChecker:");

            // Indicate the object's state
            int bulletNumber = 1;
            builder.Append(bulletNumber);
            builder.AppendLine(") the WebLocationChecker state is:");
            builder.AppendLine(webChecker.CurrentState.ToString());

            // Indicate the current domain information
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") this game's domain is:");
            builder.AppendLine(webChecker.RetrievedHostName);

            // List entries from the default domain list
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") the default domain list is:");
            int index = 0;
            for (; index < webChecker.DefaultDomainList.Length; ++index)
            {
                builder.Append("- ");
                builder.AppendLine(webChecker.DefaultDomainList[index]);
            }

            // Check if there's a download URL to list
            if (string.IsNullOrEmpty(webChecker.DownloadDomainsUrl) == false)
            {
                // Print that URL
                ++bulletNumber;
                builder.Append(bulletNumber);
                builder.AppendLine(") downloaded a list of domains from:");
                builder.AppendLine(webChecker.DownloadDomainsUrl);

                // Check if there are any downloaded domains
                if (webChecker.DownloadedDomainList != null)
                {
                    ++bulletNumber;
                    builder.Append(bulletNumber);
                    builder.AppendLine(") downloaded the following domains:");
                    for (index = 0; index < webChecker.DownloadedDomainList.Length; ++index)
                    {
                        builder.Append("- ");
                        builder.AppendLine(webChecker.DownloadedDomainList[index]);
                    }
                }
                else
                {
                    ++bulletNumber;
                    builder.Append(bulletNumber);
                    builder.AppendLine(") downloading that list failed, however.");
                }
            }

            // Show unique list of domains
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") together, the full domain list is as follows:");
            foreach (string domain in webChecker.AllUniqueDomains.Keys)
            {
                builder.Append("- ");
                builder.AppendLine(domain);
            }

            // Return URL
            return builder.ToString();
        }
    }
}
