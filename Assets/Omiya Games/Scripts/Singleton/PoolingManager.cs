using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PoolingManager.cs" company="Omiya Games">
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that pools <code>GameObjects</code> for re-use.  It will
    /// dynamically create more if any of its <code>GameObjects</code> are active.
    /// 
    /// Will also call events on <code>IPooledObject</code>s.
    /// </summary>
    /// <seealso cref="IPooledObject"/>
    public class PoolingManager : ISingletonScript
    {
        [SerializeField]
        GameObject[] objectsToPreload = null;

        struct PoolSet
        {
            public readonly bool containsPoolScript;
            public readonly Dictionary<GameObject, IPooledObject> allClonedInstances;

            public PoolSet(GameObject original)
            {
                IPooledObject script = original.GetComponent<IPooledObject>();
                if (script != null)
                {
                    script.OriginalPrefab = original;
                    containsPoolScript = true;
                }
                else
                {
                    containsPoolScript = false;
                }
                allClonedInstances = new Dictionary<GameObject, IPooledObject>();
            }
        }

        Transform poolingParent = null;
        readonly Dictionary<GameObject, PoolSet> allPooledObjects = new Dictionary<GameObject, PoolSet>();

        public static void ReturnToPool(Component script)
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
        public override void SingletonAwake(Singleton globalGameObject)
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
        public override void SceneAwake(Singleton globalGameObject)
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
                    foreach (KeyValuePair<GameObject, IPooledObject> cloneInstance in pooledSet.allClonedInstances)
                    {
                        if (cloneInstance.Key.activeSelf == false)
                        {
                            // If so, position this object properly
                            returnObject = cloneInstance.Key;
                            Transform cloneTransform = returnObject.transform;
                            cloneTransform.position = position;
                            cloneTransform.rotation = rotation;

                            // Activate this object and return it
                            returnObject.SetActive(true);

                            // If this set has a script, indicate this GameObject was activated
                            if (cloneInstance.Value != null)
                            {
                                cloneInstance.Value.Activated(this);
                            }
                            break;
                        }
                    }

                    // If not, initialize a new object
                    if (returnObject == null)
                    {
                        // Initialized a new GameObject
                        returnObject = CloneNewInstance(prefab, pooledSet, position, rotation);
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

        public C GetInstance<C>(C script, Vector3 position, Quaternion rotation) where C : Component
        {
            C returnScript = default(C);
            if(script != null)
            {
                GameObject instance = GetInstance(script.gameObject, position, rotation);
                returnScript = instance.GetComponent<C>();
            }
            return returnScript;
        }

        public C GetInstance<C>(C script) where C : Component
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
            if (pooledSet.containsPoolScript == true)
            {
                script = returnObject.GetComponent<IPooledObject>();
                script.OriginalPrefab = prefab;
            }

            // Add this new GameObject and script into the dictionary
            pooledSet.allClonedInstances.Add(returnObject, script);

            // Indicate to the script the object was initialized
            if (script != null)
            {
                script.Initialized(this);
            }
            return returnObject;
        }

        internal void DestroyAll()
        {
            // Deactivate everything
            foreach (PoolSet pool in allPooledObjects.Values)
            {
                foreach (KeyValuePair<GameObject, IPooledObject> set in pool.allClonedInstances)
                {
                    if (set.Key.activeSelf == true)
                    {
                        set.Key.SetActive(false);
                        if (set.Value != null)
                        {
                            set.Value.OnDestroy(this);
                        }
                    }
                }
            }
        }
    }
}
