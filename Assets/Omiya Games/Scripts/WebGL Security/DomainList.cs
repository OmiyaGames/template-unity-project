using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AcceptedDomainList.cs" company="Omiya Games">
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
    /// <code>ScriptableObject</code> that contains a list of strings. Used to
    /// create a list of domains the <code>WebLocationChecker</code> can download.
    /// </summary>
    /// <seealso cref="DomainListAssetBundleGenerator"/>
    /// <seealso cref="WebLocationChecker"/>
    public class DomainList : ScriptableObject, ICollection<string>
    {
        [SerializeField]
        string[] domains = null;

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
            for(int index = 0; ((index < Count) && ((index + arrayIndex) < array.Length)); ++index)
            {
                array[index + arrayIndex] = Domains[index];
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>) Domains).GetEnumerator();
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
                if(Domains[index] == item)
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
    }
}
