using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolingManager : ISingletonScript
{
	public GameObject[] objectsToPreload = null;

	struct PoolSet
	{
		public readonly bool containsPoolScript;
		public readonly Dictionary<GameObject, IPooledObject> allClonedInstances;

		public PoolSet(GameObject original)
		{
			IPooledObject script = original.GetComponent<IPooledObject>();
			if(script != null)
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

	/// <summary>
	/// Called when the first scene is loaded.
	/// </summary>
    public override void SingletonStart(Singleton globalGameObject)
	{
		// Cache the transform everything will be pooled to
		poolingParent = transform;

		// Preload everything
		GameObject clone = null;
		for(int index = 0; index < objectsToPreload.Length; ++index)
		{
			clone = GetInstance(objectsToPreload[index]);
			clone.SetActive(false);
		}
	}
	
	/// <summary>
	/// Called when any scene after the first one is loaded.
	/// </summary>
    public override void SceneStart(Singleton globalGameObject)
	{
		// Do nothing
	}

	public GameObject GetInstance(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		// Cover the edge case
		GameObject returnObject = null;
		if(prefab != null)
		{
			// Check if the prefab is already pooled
			if(allPooledObjects.ContainsKey(prefab) == true)
			{
				// Grab the pooled objects
				PoolSet pooledSet = allPooledObjects[prefab];

				// Check if there are any deactivated objects
				foreach(KeyValuePair<GameObject, IPooledObject> cloneInstance in pooledSet.allClonedInstances)
				{
					if(cloneInstance.Key.activeSelf == false)
					{
						// If so, position this object properly
						returnObject = cloneInstance.Key;
						Transform cloneTransform = returnObject.transform;
						cloneTransform.position = position;
						cloneTransform.rotation = rotation;

						// Activate this object and return it
						returnObject.SetActive(true);

						// If this set has a script, indicate this GameObject was activated
						if(cloneInstance.Value != null)
						{
							cloneInstance.Value.Activated(this);
						}
						break;
					}
				}

				// If not, initialize a new object
				if(returnObject == null)
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
		if(prefab != null)
		{
			Transform returnTransform = prefab.transform;
			returnObject = GetInstance(prefab, returnTransform.position, returnTransform.rotation);
		}
		return returnObject;
	}

	GameObject CloneNewInstance(GameObject prefab, PoolSet pooledSet, Vector3 position, Quaternion rotation)
	{
		// Initialized a new GameObject
		GameObject returnObject = (GameObject)Instantiate(prefab, position, rotation);

		// Update this object's parent so it won't get destroyed easily
		returnObject.transform.parent = poolingParent;
		
		// Grab the script from the clone if there is any in the original
		IPooledObject script = null;
		if(pooledSet.containsPoolScript == true)
		{
			script = returnObject.GetComponent<IPooledObject>();
			script.OriginalPrefab = prefab;
		}
		
		// Add this new GameObject and script into the dictionary
		pooledSet.allClonedInstances.Add(returnObject, script);
		
		// Indicate to the script the object was initialized
		if(script != null)
		{
			script.Initialized(this);
		}
		return returnObject;
	}
}
