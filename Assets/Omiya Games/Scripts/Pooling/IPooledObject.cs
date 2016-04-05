using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IPooledObject.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Abstract class with functions that triggers during
    /// <code>PoolingManager</code>'s events.
    /// </summary>
    /// <seealso cref="PoolingManager"/>
    public abstract class IPooledObject : MonoBehaviour
    {
        /// <summary>
        /// Gets the original prefab this object was pooled from.
        /// Do NOT set it to any other value, especially from any class other than PoolingManager.
        /// </summary>
        /// <value>The original prefab.</value>
        public GameObject OriginalPrefab
        {
            get;
            set;
        }

        /// <summary>
        /// Called when this instance is initialized by PoolingManager.
        /// </summary>
        public virtual void Initialized(PoolingManager manager)
        {
        }

        /// <summary>
        /// Called when this instance -- already initialized and pooled -- is activated by PoolingManager for re-use.
        /// </summary>
        public virtual void Activated(PoolingManager manager)
        {
            // Since this is called right after reactivating an existing GameObject,
            // for easier transition, simulate calling Awake and Start.
            Awake();
            Start();
        }

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public void OnDestroy()
        {
            OnDestroy(null);
        }

        public virtual void OnDestroy(PoolingManager manager)
        {

        }
    }
}
