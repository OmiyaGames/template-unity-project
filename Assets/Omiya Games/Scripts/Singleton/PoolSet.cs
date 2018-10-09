using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames.Global
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PoolingManager.cs" company="Omiya Games">
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
    /// <author>Taro Omiya</author>
    /// <date>10/9/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A set keeping track of active and inactive <code>IPooledObject</code>s.
    /// </summary>
    /// <seealso cref="IPooledObject"/>
    internal class PoolSet
    {
        public PoolSet(GameObject originalPrefab)
        {
            if(originalPrefab == null)
            {
                throw new System.ArgumentNullException("originalPrefab");
            }

            OriginalPrefab = originalPrefab;
            OriginalScript = originalPrefab.GetComponent<IPooledObject>();
        }

        public PoolSet(IPooledObject originalPrefab)
        {
            if (originalPrefab == null)
            {
                throw new System.ArgumentNullException("originalPrefab");
            }

            OriginalScript = originalPrefab;
            OriginalPrefab = originalPrefab.gameObject;
        }

        public Dictionary<GameObject, IPooledObject> ActiveInstances
        {
            get;
        } = new Dictionary<GameObject, IPooledObject>();

        public Dictionary<GameObject, IPooledObject> InactiveInstances
        {
            get;
        } = new Dictionary<GameObject, IPooledObject>();

        public GameObject OriginalPrefab
        {
            get;
        }

        public IPooledObject OriginalScript
        {
            get;
        }

        public bool ContainsPoolScript
        {
            get
            {
                return (OriginalScript != null);
            }
        }
    }
}
