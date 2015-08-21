using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames
{
    public class PopUpDialog : MonoBehaviour
    {
        [SerializeField]
        Text label;

        RectTransform cacheTransform = null;

        public Text Label
        {
            get
            {
                return label;
            }
        }

        public RectTransform CachedTransform
        {
            get
            {
                if(cacheTransform == null)
                {
                    cacheTransform = transform as RectTransform;
                }
                return cacheTransform;
            }
        }
    }
}
