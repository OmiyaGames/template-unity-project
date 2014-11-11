using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Abstract class with functions that trigger during PoolingManager's events.
/// </summary>
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
}
