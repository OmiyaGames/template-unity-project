using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Menu
{
    public class TestUiPlacement : MonoBehaviour
    {
        [SerializeField]
        RectTransform top;
        [SerializeField]
        RectTransform middle;
        [SerializeField]
        RectTransform bottom;

        [Header("Align above transforms to")]
        [SerializeField]
        RectTransform contentTransform;
        [SerializeField]
        UiEventNavigation childControl;

        // Use this for initialization
        void Start()
        {
            Vector2 anchor = middle.anchoredPosition;
            float topPos, bottomPos, centerPos;
            centerPos = ScrollingHelper.GetVerticalAnchoredPositionInContent(contentTransform, childControl, out topPos, out bottomPos);

            // Setup positions
            anchor.y = centerPos;
            middle.anchoredPosition = anchor;
            anchor.y = topPos;
            top.anchoredPosition = anchor;
            anchor.y = bottomPos;
            bottom.anchoredPosition = anchor;
        }
    }
}