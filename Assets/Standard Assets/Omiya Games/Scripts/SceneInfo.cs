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

        int ordinal = 0;

        public SceneInfo(string scene, string display, int index = 0)
        {
            sceneName = scene;
            displayName = display;
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
    }
}
