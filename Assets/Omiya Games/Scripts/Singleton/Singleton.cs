using UnityEngine;
using System;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="Singleton.cs" company="Omiya Games">
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
    /// <date>9/22/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Any GameObject with this script will not be destroyed when switching between
    /// scenes. However, only one instance of this script may exist in a scene.
    /// Allows retrieving any components in itself or its children.
    /// </summary>
    /// <seealso cref="ISingletonScript"/>
    public class Singleton : MonoBehaviour
    {
        public enum GenuineStatus
        {
            Unchecked = -1,
            IsGenuine = 0,
            NotGenuine,
            VerificationNotSupported
        }

        private static Singleton msInstance = null;
        private readonly Dictionary<Type, Component> mCacheRetrievedComponent = new Dictionary<Type, Component>();

        public event Action<float> OnUpdate;
        public event Action<float> OnRealTimeUpdate;
        public event Action<float> OnFixedUpdate;

        ISingletonScript[] allSingletonScriptsCache = null;

        [Header("Simulation")]
        [SerializeField]
        bool simulateMalformedGame = false;
#if UNITY_EDITOR
        [SerializeField]
        bool simulateWebplayer = false;
#endif

        [Header("Store Information")]
        [SerializeField]
        PlatformSpecificLink storeUrls;

        GenuineStatus genuineStatus = GenuineStatus.Unchecked;

        public static Singleton Instance
        {
            get
            {
                return msInstance;
            }
            private set
            {
                msInstance = value;
            }
        }

        public static COMPONENT Get<COMPONENT>() where COMPONENT : Component
        {
            COMPONENT returnObject = null;
            Type retrieveType = typeof(COMPONENT);
            if (Instance != null)
            {
                if (Instance.mCacheRetrievedComponent.ContainsKey(retrieveType) == true)
                {
                    returnObject = Instance.mCacheRetrievedComponent[retrieveType] as COMPONENT;
                }
                else
                {
                    returnObject = Instance.GetComponentInChildren<COMPONENT>();
                    Instance.mCacheRetrievedComponent.Add(retrieveType, returnObject);
                }
            }
            return returnObject;
        }

        public bool IsWebplayer
        {
            get
            {
#if UNITY_EDITOR
                // Check if webplayer simulation checkbox is checked
                return simulateWebplayer;
#elif (UNITY_WEBPLAYER || UNITY_WEBGL)
                // Always return true if already on a webplayer
                return true;
#else
                // Always return false, otherwise
                return false;
#endif
            }
        }

        public bool IsSimulatingMalformedGame
        {
            get
            {
                bool returnFlag = simulateMalformedGame;

                // Check if we're not in the editor, and this build is in debug mode
#if !UNITY_EDITOR
                if (Debug.isDebugBuild == false)
                {
                    // Always return false
                    returnFlag = false;
                }
#endif
                // Check if simulation checkbox is checked
                return returnFlag;
            }
        }

        public string PlatformSpecificStoreLink
        {
            get
            {
                return storeUrls.PlatformLink;
            }
        }

        public string WebsiteLink
        {
            get
            {
                return storeUrls.WebLink;
            }
        }

        public string GetStoreLink(PlatformSpecificLink.SupportedPlatforms platform)
        {
            return storeUrls.GetPlatformLink(platform);
        }

        public GenuineStatus CheckGenuine
        {
            get
            {
                if (genuineStatus == GenuineStatus.Unchecked)
                {
                    // We'll be using Unity's own method of checking piracy for now
                    genuineStatus = GenuineStatus.VerificationNotSupported;

                    // Make sure the check is available
                    if (Application.genuineCheckAvailable == true)
                    {
                        genuineStatus = GenuineStatus.NotGenuine;

                        // Make sure this copy is genuine
                        if (Application.genuine == true)
                        {
                            genuineStatus = GenuineStatus.IsGenuine;
                        }
                    }
                }
                return genuineStatus;
            }
        }

        public bool IsPiracyDetected
        {
            get
            {
                return (CheckGenuine == GenuineStatus.NotGenuine);
            }
        }

        // Use this for initialization
        void Awake()
        {
            if (Instance == null)
            {
                // Set the instance variable
                Instance = this;

                // Run all the events
                Instance.RunSingletonEvents();

                // Prevent this object from destroying itself
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Run all the events
                Instance.RunSingletonEvents();

                // Destroy this gameobject
                Destroy(gameObject);
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

        void FixedUpdate()
        {
            if (OnFixedUpdate != null)
            {
                OnFixedUpdate(Time.fixedDeltaTime);
            }
        }

        void RunSingletonEvents()
        {
            int index = 0;
            if(allSingletonScriptsCache == null)
            {
                // Cache all the singleton scripts
                allSingletonScriptsCache = Instance.GetComponentsInChildren<ISingletonScript>();

                // Go through every ISingletonScript, and run singleton awake
                for (index = 0; index < allSingletonScriptsCache.Length; ++index)
                {
                    // Run singleton awake
                    allSingletonScriptsCache[index].SingletonInstance = Instance;
                    allSingletonScriptsCache[index].SingletonAwake(Instance);
                }
            }

            // Go through every ISingletonScript, and run scene awake
            for (index = 0; index < allSingletonScriptsCache.Length; ++index)
            {
                allSingletonScriptsCache[index].SceneAwake(Instance);
            }
        }
    }
}
