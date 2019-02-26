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
    /// <date>10/9/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Abstract class with functions that triggers during
    /// <code>PoolingManager</code>'s events.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>5/18/2015</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// <item>
    /// <description>10/9/2018</description>
    /// <description>Taro</description>
    /// <description>Adding <code>PoolSet</code> and <code>event</code>s.</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="PoolingManager"/>
    [DisallowMultipleComponent]
    public abstract class IPooledObject : MonoBehaviour
    {
        /// <summary>
        /// Called when this instance is initialized by PoolingManager,
        /// but before Start or Awake is called.
        /// </summary>
        public event System.Action<IPooledObject, PoolingManager> OnAfterInitialized;
        /// <summary>
        /// Called when this instance -- already initialized and pooled --
        /// is activated by PoolingManager for re-use. Note this method will
        /// be called before Start or Awake.
        /// </summary>
        public event System.Action<IPooledObject, PoolingManager> OnAfterActivated;
        /// <summary>
        /// Called when this instance -- already initialized and pooled -- is deactivated.
        /// </summary>
        public event System.Action<IPooledObject, PoolingManager> OnAfterDeactivated;

        /// <summary>
        /// Gets the original prefab this object was pooled from.
        /// </summary>
        /// <value>The original prefab.</value>
        public GameObject OriginalPrefab
        {
            get
            {
                GameObject returnPrefab = null;
                if (Pool != null)
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

        public abstract void Awake();
        public abstract void Start();

        public virtual void OnDestroy()
        {
            AfterDeactivate(null);
        }

        public virtual void OnDisable()
        {
            AfterDeactivate(null);
        }

        /// <summary>
        /// Called when this instance is initialized by PoolingManager,
        /// but before Start or Awake is called.
        /// </summary>
        public virtual void AfterInitialized(PoolingManager manager)
        {
            SetPoolState(true);
            OnAfterInitialized?.Invoke(this, manager);
        }

        /// <summary>
        /// Called when this instance -- already initialized and pooled --
        /// is activated by PoolingManager for re-use. Note this method will
        /// be called before Start or Awake.
        /// </summary>
        public virtual void AfterActivated(PoolingManager manager)
        {
            SetPoolState(true);
            OnAfterActivated?.Invoke(this, manager);

            // Since this is called right after reactivating an existing GameObject,
            // for easier transition, simulate calling Awake and Start.
            Awake();
            Start();
        }

        /// <summary>
        /// Called when this instance -- already initialized and pooled -- is deactivated.
        /// </summary>
        public virtual void AfterDeactivate(PoolingManager manager)
        {
            SetPoolState(false);
            OnAfterDeactivated?.Invoke(this, manager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        /// <returns>True if any changes were made to the Pool.</returns>
        protected bool SetPoolState(bool active)
        {
            // Check if the pool is set
            bool returnChangeDetected = false;
            if ((Pool != null) && (OriginalPrefab != gameObject))
            {
                // Check if this game object is in the inactive list
                if ((active == true) && (Pool.InactiveInstances.ContainsKey(gameObject) == true))
                {
                    // If so, since we're activating this object, remove
                    // it from the inactive list.
                    Pool.InactiveInstances.Remove(gameObject);
                    returnChangeDetected = true;
                }
                else if ((active == false) && (Pool.InactiveInstances.ContainsKey(gameObject) == false))
                {
                    // If not, add this object into the inactive list
                    Pool.InactiveInstances.Add(gameObject, this);
                    returnChangeDetected = true;
                }

                // Check if this game object is in the active list
                if ((active == true) && (Pool.ActiveInstances.ContainsKey(gameObject) == false))
                {
                    // If not, add this object into the gameobject list
                    Pool.ActiveInstances.Add(gameObject, this);
                    returnChangeDetected = true;
                }
                else if ((active == false) && (Pool.ActiveInstances.ContainsKey(gameObject) == true))
                {
                    // If so, since we're deactivating this object,
                    // remove it from the active list.
                    Pool.ActiveInstances.Remove(gameObject);
                    returnChangeDetected = true;
                }
            }
            return returnChangeDetected;
        }
    }
}
