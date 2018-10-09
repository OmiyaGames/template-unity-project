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
        GameObject[] objectsToPreload = null;

        Transform poolingParent = null;
        readonly Dictionary<GameObject, PoolSet> allPooledObjects = new Dictionary<GameObject, PoolSet>();

        public static void ReturnToPool(IPooledObject script)
        {
            if (script != null)
            {
                ReturnToPool(script.gameObject);
            }
        }

        public static void ReturnToPool(GameObject gameObject)
        {
            if(gameObject != null)
            {
                gameObject.SetActive(false);
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
            GameObject clone = null;
            for (int index = 0; index < objectsToPreload.Length; ++index)
            {
                clone = GetInstance(objectsToPreload[index]);
                clone.SetActive(false);
            }
        }

        /// <summary>
        /// Called when any scene after the first one is loaded.
        /// </summary>
        internal override void SceneAwake()
        {
            // Do nothing
        }

        public GameObject GetInstance(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            // Cover the edge case
            GameObject returnObject = null;
            if (prefab != null)
            {
                // Check if the prefab is already pooled
                if (allPooledObjects.ContainsKey(prefab) == true)
                {
                    // Grab the pooled objects
                    PoolSet pooledSet = allPooledObjects[prefab];

                    // Check if there are any deactivated objects
                    IPooledObject script = null;
                    foreach (KeyValuePair<GameObject, IPooledObject> cloneInstance in pooledSet.InactiveInstances)
                    {
                        // If so, position this object properly
                        returnObject = cloneInstance.Key;
                        returnObject.transform.position = position;
                        returnObject.transform.rotation = rotation;

                        // Activate this object and return it
                        returnObject.SetActive(true);

                        // If this set has a script, indicate this GameObject was activated
                        script = cloneInstance.Value;
                        break;
                    }

                    // If not, initialize a new object
                    if (returnObject == null)
                    {
                        // Initialized a new GameObject
                        returnObject = CloneNewInstance(prefab, pooledSet, position, rotation);
                    }
                    else if (script != null)
                    {
                        script.AfterActivated(this);
                    }
                }
                else
                {
                    // Create a new entry in the dictionary
                    PoolSet pooledSet = new PoolSet(prefab);
                    allPooledObjects.Add(prefab, pooledSet);

                    // Initialized a new GameObject
                    returnObject = CloneNewInstance(prefab, pooledSet, position, rotation);
                }
            }
            return returnObject;
        }

        public GameObject GetInstance(GameObject prefab)
        {
            // Cover the edge case
            GameObject returnObject = null;
            if (prefab != null)
            {
                returnObject = GetInstance(prefab, prefab.transform.position, prefab.transform.rotation);
            }
            return returnObject;
        }

        public C GetInstance<C>(C script, Vector3 position, Quaternion rotation) where C : IPooledObject
        {
            C returnScript = default(C);
            if(script != null)
            {
                GameObject instance = GetInstance(script.gameObject, position, rotation);
                returnScript = instance.GetComponent<C>();
            }
            return returnScript;
        }

        public C GetInstance<C>(C script) where C : IPooledObject
        {
            C returnScript = default(C);
            if (script != null)
            {
                GameObject instance = GetInstance(script.gameObject);
                returnScript = instance.GetComponent<C>();
            }
            return returnScript;
        }

        GameObject CloneNewInstance(GameObject prefab, PoolSet pooledSet, Vector3 position, Quaternion rotation)
        {
            // Initialized a new GameObject
            GameObject returnObject = (GameObject)Instantiate(prefab, position, rotation);

            // Update this object's parent so it won't get destroyed easily
            returnObject.transform.parent = poolingParent;

            // Grab the script from the clone if there is any in the original
            IPooledObject script = null;
            if (pooledSet.ContainsPoolScript == true)
            {
                script = returnObject.GetComponent<IPooledObject>();
                script.Pool = pooledSet;
            }

            // Add this new GameObject and script into the dictionary
            pooledSet.ActiveInstances.Add(returnObject, script);

            // Indicate to the script the object was initialized
            if (script != null)
            {
                script.AfterInitialized(this);
            }
            return returnObject;
        }

        internal void DestroyAll()
        {
            List<IPooledObject> deactivatedScripts = new List<IPooledObject>();

            // Deactivate everything
            foreach (PoolSet pool in allPooledObjects.Values)
            {
                // Clear the scripts to deactivate
                deactivatedScripts.Clear();
                if (pool.ContainsPoolScript == true)
                {
                    deactivatedScripts.Capacity = pool.ActiveInstances.Count;
                }

                // Go through all the active pooled items
                foreach (KeyValuePair<GameObject, IPooledObject> set in pool.ActiveInstances)
                {
                    set.Key.SetActive(false);
                    if (set.Value != null)
                    {
                        deactivatedScripts.Add(set.Value);
                    }
                }

                // Indicate script is deactivated
                foreach(IPooledObject deactivatedScript in deactivatedScripts)
                {
                    deactivatedScript.AfterDeactivate(this);
                }
            }
        }
    }
}
