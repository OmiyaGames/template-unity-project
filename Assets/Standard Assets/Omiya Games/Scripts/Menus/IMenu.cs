using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames
{
    public abstract class IMenu : MonoBehaviour
    {
        [SerializeField]
        bool displayedOnStart = false;
        [SerializeField]
        bool managed = true;

        internal bool IsDisplayedOnStart
        {
            get
            {
                return displayedOnStart;
            }
        }
        internal bool IsManaged
        {
            get
            {
                return managed;
            }
        }
        
        public virtual void Start()
        {
            IsDisplayed = IsDisplayedOnStart;
            Setup();
        }

        abstract public bool IsDisplayed
        {
            get;
            set;
        }
        abstract public GUIContent DefaultUi
        {
            get;
        }

        abstract protected void Setup();
    }
}
