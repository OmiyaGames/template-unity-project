using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    [System.Serializable]
    public class SceneInfo
    {
        [SerializeField]
        string sceneName = "";
        [SerializeField]
        string displayName = "";
        [SerializeField]
        bool revertTimeScale = true;
        [SerializeField]
        CursorLockMode sceneCursorLockMode = CursorLockMode.None;

        int ordinal = 0;

        public SceneInfo(string scene, string display, bool revertTime = true, CursorLockMode lockMode = CursorLockMode.None, int index = 0)
        {
            sceneName = scene;
            displayName = display;
            revertTimeScale = revertTime;
            sceneCursorLockMode = lockMode;
            ordinal = index;
        }

        public string SceneName
        {
            get
            {
                return sceneName;
            }
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }

        public int Ordinal
        {
            get
            {
                return ordinal;
            }
            internal set
            {
                ordinal = value;
            }
        }

        public CursorLockMode LockMode
        {
            get
            {
                return sceneCursorLockMode;
            }
        }

        public bool RevertTimeScale
        {
            get
            {
                return revertTimeScale;
            }
        }
    }
}
