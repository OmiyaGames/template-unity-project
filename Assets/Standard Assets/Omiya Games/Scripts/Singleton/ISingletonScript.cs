using UnityEngine;
using System.Collections;

public abstract class ISingletonScript : MonoBehaviour
{
    abstract public void SingletonStart(Singleton instance);
    abstract public void SceneStart(Singleton instance);
}
