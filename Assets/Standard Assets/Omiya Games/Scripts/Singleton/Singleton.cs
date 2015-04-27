using UnityEngine;
using System;
using System.Collections.Generic;

namespace OmiyaGames
{
    public class Singleton : MonoBehaviour
    {
        private static Singleton msInstance = null;
        private readonly Dictionary<Type, Component> mCacheRetrievedComponent = new Dictionary<Type, Component>();

        public event Action<float> OnUpdate;
        public event Action<float> OnRealTimeUpdate;

        public static COMPONENT Get<COMPONENT>() where COMPONENT : Component
        {
            COMPONENT returnObject = null;
            Type retrieveType = typeof(COMPONENT);
            if (msInstance != null)
            {
                if (msInstance.mCacheRetrievedComponent.ContainsKey(retrieveType) == true)
                {
                    returnObject = msInstance.mCacheRetrievedComponent[retrieveType] as COMPONENT;
                }
                else
                {
                    returnObject = msInstance.GetComponentInChildren<COMPONENT>();
                    msInstance.mCacheRetrievedComponent.Add(retrieveType, returnObject);
                }
            }
            return returnObject;
        }

        // Use this for initialization
        void Awake()
        {
            ISingletonScript[] allSingletonScripts = null;
            if (msInstance == null)
            {
                // Set the instance variable
                msInstance = this;

                // Prevent this object from destroying itself
                DontDestroyOnLoad(gameObject);

                // Go through every ISingletonScript, and run singleton awake
                allSingletonScripts = GetComponentsInChildren<ISingletonScript>();
                foreach (ISingletonScript script in allSingletonScripts)
                {
                    // Run singleton awake
                    script.SingletonAwake(msInstance);
                }
            }
            else
            {
                // Destroy this gameobject
                Destroy(gameObject);

                // Go through every ISingletonScript
                allSingletonScripts = msInstance.GetComponentsInChildren<ISingletonScript>();
            }

            // Go through every ISingletonScript, and run scene awake
            foreach (ISingletonScript script in allSingletonScripts)
            {
                script.SceneAwake(msInstance);
            }
        }

        void Update()
        {
            if (OnUpdate != null)
            {
                OnUpdate(Time.deltaTime);
            }
            if (OnRealTimeUpdate != null)
            {
                OnRealTimeUpdate(Time.unscaledDeltaTime);
            }
        }
    }
}
