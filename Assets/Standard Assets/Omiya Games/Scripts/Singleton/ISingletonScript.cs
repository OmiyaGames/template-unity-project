using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    public abstract class ISingletonScript : MonoBehaviour
    {
        abstract public void SingletonAwake(Singleton instance);
        abstract public void SceneAwake(Singleton instance);
    }
}
