using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Text;
using System.IO;
using OmiyaGames.Web;
using OmiyaGames.Cryptography;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AcceptedDomainList.cs" company="Omiya Games">
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
    /// <date>5/14/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// <see cref="ScriptableObject"/> that contains a list of strings. Used to
    /// create a list of domains the <see cref="WebLocationChecker"/> can download.
    /// </summary>
    /// <seealso cref="WebLocationChecker"/>
    public class DomainList : ScriptableObject, ICollection<string>
    {
        [SerializeField]
        string[] domains = null;

        #region Helper Static Fucntions
        public static DomainList Generate(string nameOfFile, IList<string> allDomains, StringCryptographer encrypter)
        {
            // Setup asset
            DomainList newAsset = CreateInstance<DomainList>();
            newAsset.name = nameOfFile;

            // Copy over all the domain names
            string[] domains = new string[allDomains.Count];
            if (encrypter != null)
            {
                // Encrypt all entries
                for (int index = 0; index < allDomains.Count; ++index)
                {
                    domains[index] = encrypter.Encrypt(allDomains[index]);
                }
            }
            else
            {
                // Copy directly to the array
                allDomains.CopyTo(domains, 0);
            }
            newAsset.Domains = domains;
            return newAsset;
        }

        public static void Decrypt(DomainList domainList, StringCryptographer decrypter, IList<string> decryptedDomains)
        {
            decryptedDomains.Clear();
            if (decrypter != null)
            {
                foreach (string encryptedDomain in domainList)
                {
                    decryptedDomains.Add(decrypter.Decrypt(encryptedDomain));
                }
            }
            else
            {
                foreach (string domain in domainList)
                {
                    decryptedDomains.Add(domain);
                }
            }
        }

        public static string[] Decrypt(DomainList domainList, StringCryptographer decrypter)
        {
            string[] allDomains = new string[domainList.Count];
            if (decrypter != null)
            {
                for (int index = 0; index < allDomains.Length; ++index)
                {
                    allDomains[index] = decrypter.Decrypt(domainList[index]);
                }
            }
            else
            {
                for (int index = 0; index < allDomains.Length; ++index)
                {
                    allDomains[index] = domainList[index];
                }
            }
            return allDomains;
        }
        #endregion

        public string[] Domains
        {
            private get
            {
                return domains;
            }
            set
            {
                domains = value;
            }
        }

        public string this[int index]
        {
            get
            {
                return Domains[index];
            }
            set
            {
                Domains[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return Domains.Length;
            }
        }

        #region ICollection Implementation
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            arrayIndex = Mathf.Clamp(arrayIndex, 0, array.Length);
            for (int index = 0; ((index < Count) && ((index + arrayIndex) < array.Length)); ++index)
            {
                array[index + arrayIndex] = Domains[index];
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)Domains).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Domains.GetEnumerator();
        }

        public bool Contains(string item)
        {
            bool returnFlag = false;
            for (int index = 0; index < Count; ++index)
            {
                if (Domains[index] == item)
                {
                    returnFlag = true;
                    break;
                }
            }
            return returnFlag;
        }
        #endregion

        #region Unimplemented methods
        public void Add(string item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Remove(string item)
        {
            throw new NotImplementedException();
        }
        #endregion

        public static DomainList Get(AssetBundle bundle, string assetNameNoExtension = null)
        {
            DomainList returnDomain = null;

            // Search for an *.asset file
            string[] allAssets = bundle.GetAllAssetNames();
            string firstAsset = null;
            if (allAssets != null)
            {
                for (int index = 0; index < allAssets.Length; ++index)
                {
                    if ((string.IsNullOrEmpty(allAssets[index]) == false) &&
                        (Path.GetExtension(allAssets[index]) == OmiyaGames.Helpers.FileExtensionScriptableObject) &&
                        ((string.IsNullOrEmpty(assetNameNoExtension) == true) || (Path.GetFileNameWithoutExtension(allAssets[index]) == assetNameNoExtension)))
                    {
                        firstAsset = allAssets[index];
                        break;
                    }
                }
            }

            // Check if an asset is found
            if (string.IsNullOrEmpty(firstAsset) == false)
            {
                try
                {
                    // Convert it to an AcceptedDomainList
                    returnDomain = bundle.LoadAsset<DomainList>(firstAsset);
                }
                catch (System.Exception)
                {
                    returnDomain = null;
                }
            }
            return returnDomain;
        }
    }
}
