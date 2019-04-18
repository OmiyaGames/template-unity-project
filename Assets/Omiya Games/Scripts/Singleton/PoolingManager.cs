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
    /// <date>10/9/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that pools <code>GameObjects</code> for re-use.  It will
    /// dynamically create more if any of its <code>GameObjects</code> are active.
    /// 
    /// Will also call events on <code>IPooledObject</code>s.
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
    /// <description>Optimizing performance, which requires forcing the type
    /// Pooling Manager manages <code>IPooledObject</code>-only.</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IPooledObject"/>
    public class PoolingManager : ISingletonScript
    {
        [SerializeField]
        IPooledObject[] objectsToPreload = null;

        Transform poolingParent = null;
        readonly Dictionary<IPooledObject, PoolSet> allPooledObjects = new Dictionary<IPooledObject, PoolSet>();

        public static void ReturnToPool(IPooledObject script)
        {
            if (script != null)
            {
                ReturnToPool(script.gameObject);
            }
        }

        public static void ReturnToPool(GameObject gameObject)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }

        public static void DestroyFromPool(IPooledObject script)
        {
            // Deactivate the script first
            ReturnToPool(script);

            // Check if the script isn't null
            if (script != null)
            {
                // Destroy the game object
                Destroy(script.gameObject);
            }
        }

        /// <summary>
        /// Called when the first scene is loaded.
        /// </summary>
        internal override void SingletonAwake()
        {
            // Cache the transform everything will be pooled to
            poolingParent = transform;

            // Preload everything
            IPooledObject clone = null;
            foreach (IPooledObject preloadObject in objectsToPreload)
            {
                clone = GetInstance(preloadObject);
                clone.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called when any scene after the first one is loaded.
        /// </summary>
        internal override void SceneAwake()
        {
            // Do nothing
        }

        public C GetInstance<C>(C prefab, Vector3 position, Quaternion rotation) where C : IPooledObject
        {
            // Cover the edge case
            C returnScript = null;
            if (prefab != null)
            {
                // Check if the prefab is already pooled
                PoolSet pooledSet;
                if (allPooledObjects.TryGetValue(prefab, out pooledSet) == true)
                {
                    // Check if there are any deactivated objects
                    if (pooledSet.InactiveInstances.Count > 0)
                    {
                        foreach (KeyValuePair<GameObject, IPooledObject> cloneInstance in pooledSet.InactiveInstances)
                        {
                            // If so, position this object properly
                            GameObject clone = cloneInstance.Key;
                            clone.transform.position = position;
                            clone.transform.rotation = rotation;

                            // Activate this object and return it
                            clone.SetActive(true);

                            // If this set has a script, indicate this GameObject was activated
                            returnScript = (C)cloneInstance.Value;
                            returnScript.AfterActivated(this);
                            break;
                        }
                    }
                    else
                    {
                        // If not, initialized a new GameObject
                        GameObject clone;
                        CloneNewInstance(prefab, pooledSet, position, rotation, out clone, out returnScript);
                    }
                }
                else
                {
                    // Create a new entry in the dictionary
                    pooledSet = new PoolSet(prefab);
                    allPooledObjects.Add(prefab, pooledSet);

                    // Initialized a new GameObject
                    GameObject clone;
                    CloneNewInstance(prefab, pooledSet, position, rotation, out clone, out returnScript);
                }
            }
            return returnScript;
        }

        public C GetInstance<C>(C prefab) where C : IPooledObject
        {
            return GetInstance(prefab, prefab.transform.position, prefab.transform.rotation);
        }

        private void CloneNewInstance<C>(C prefab, PoolSet pooledSet, Vector3 position, Quaternion rotation, out GameObject instanceObject, out C instanceScript) where C : IPooledObject
        {
            // Initialized a new GameObject
            instanceObject = Instantiate(prefab.gameObject, position, rotation);

            // Update this object's parent so it won't get destroyed easily
            instanceObject.transform.parent = poolingParent;

            // Grab the script from the clone if there is any in the original
            instanceScript = null;
            if (pooledSet.ContainsPoolScript == true)
            {
                instanceScript = instanceObject.GetComponent<C>();
                instanceScript.Pool = pooledSet;
            }

            // Add this new GameObject and script into the dictionary
            pooledSet.ActiveInstances.Add(instanceObject, instanceScript);

            // Indicate to the script the object was initialized
            if (instanceScript != null)
            {
                instanceScript.AfterInitialized(this);
            }
        }

        internal void DestroyAll()
        {
            List<GameObject> objectsToDeactivate = new List<GameObject>();

            // Deactivate everything
            foreach (PoolSet pool in allPooledObjects.Values)
            {
                // Clear the scripts to deactivate
                objectsToDeactivate.Clear();

                // Expand the capacity, if we need to
                if ((pool.ContainsPoolScript == true) && (pool.ActiveInstances.Count > objectsToDeactivate.Capacity))
                {
                    objectsToDeactivate.Capacity = pool.ActiveInstances.Count;
                }

                // Go through all the active pooled items
                foreach (GameObject set in pool.ActiveInstances.Keys)
                {
                    objectsToDeactivate.Add(set);
                }

                // Indicate script is deactivated
                foreach (GameObject deactivate in objectsToDeactivate)
                {
                    deactivate.SetActive(false);
                }
            }
        }
    }
}
