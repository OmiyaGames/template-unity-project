using UnityEngine;
using OmiyaGames.Global;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IPooledObject.cs" company="Omiya Games">
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
        /// </summary>
        /// <value>The original prefab.</value>
        public GameObject OriginalPrefab
        {
            get
            {
                GameObject returnPrefab = null;
                if(Pool != null)
                {
                    returnPrefab = Pool.OriginalPrefab;
                }
                return returnPrefab;
            }
        }

        /// <summary>
        /// Gets the set of scripts this object belongs in.
        /// Handled by <code>PoolingManager</code>.
        /// </summary>
        /// <value>The original prefab.</value>
        internal PoolSet Pool
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Called when this instance is initialized by PoolingManager.
        /// </summary>
        public virtual void AfterInitialized(PoolingManager manager)
        {
            MarkActive();
        }

        /// <summary>
        /// Called when this instance -- already initialized and pooled -- is activated by PoolingManager for re-use.
        /// </summary>
        public virtual void AfterActivated(PoolingManager manager)
        {
            MarkActive();

            // Since this is called right after reactivating an existing GameObject,
            // for easier transition, simulate calling Awake and Start.
            Awake();
            Start();
        }

        /// <summary>
        /// Called when this instance -- already initialized and pooled -- is activated by PoolingManager for re-use.
        /// </summary>
        /// <param name="manager"></param>
        public virtual void AfterDeactivate(PoolingManager manager)
        {
            MarkInactive();
        }

        public virtual void Awake()
        {
            // Do nothing by default.
        }

        public virtual void Start()
        {
            // Do nothing by default.
        }

        public void OnDestroy()
        {
            AfterDeactivate(null);
        }

        public void OnDisable()
        {
            AfterDeactivate(null);
        }

        protected void MarkActive()
        {
            // Check if the pool is set
            if ((Pool != null) && (OriginalPrefab != gameObject))
            {
                // Check if this game object is in the inactive list
                if (Pool.InactiveInstances.ContainsKey(gameObject) == true)
                {
                    // If so, since we're activating this object, remove
                    // it from the inactive list.
                    Pool.InactiveInstances.Remove(gameObject);
                }

                // Check if this game object is in the active list
                if (Pool.ActiveInstances.ContainsKey(gameObject) == false)
                {
                    // If not, add this object into the gameobject list
                    Pool.ActiveInstances.Add(gameObject, this);
                }
            }
        }

        protected void MarkInactive()
        {
            // Check if the pool is set
            if ((Pool != null) && (OriginalPrefab != gameObject))
            {
                // Check if this game object is in the active list
                if (Pool.ActiveInstances.ContainsKey(gameObject) == true)
                {
                    // If so, since we're deactivating this object,
                    // remove it from the active list.
                    Pool.ActiveInstances.Remove(gameObject);
                }

                // Check if this game object is in the inactive list
                if (Pool.InactiveInstances.ContainsKey(gameObject) == false)
                {
                    // If not, add this object into the inactive list
                    Pool.InactiveInstances.Add(gameObject, this);
                }
            }
        }
    }
}
