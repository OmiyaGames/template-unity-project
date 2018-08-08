using UnityEngine.UI;
using UnityEngine;

namespace OmiyaGames.Menu
{
    public static class ScrollingHelper
    {
        public static float GetVerticalAnchoredPositionInContent(RectTransform contentTransform, UiEventNavigation childControl)
        {
            float selectionPosition = 0f;
            if((childControl != null) && (childControl.Selectable != null))
            {
                selectionPosition = GetVerticalAnchoredPositionInContent(contentTransform, ((RectTransform)childControl.Selectable.transform));
            }
            return selectionPosition;
        }

        public static float GetVerticalAnchoredPositionInContent(RectTransform contentTransform, UiEventNavigation childControl, out float top, out float bottom)
        {
            top = 0f;
            bottom = 0f;
            float returnCenter = 0f;
            if ((childControl != null) && (childControl.Selectable != null))
            {
                // Calculate as normal
                GetVerticalAnchoredPositionInContent(contentTransform, ((RectTransform)childControl.Selectable.transform), out top, out bottom);
                returnCenter = (top + bottom) / 2f;

                // Grab new top value
                if (childControl.UpperBoundToScrollTo != null)
                {
                    float dummyBottom;
                    GetVerticalAnchoredPositionInContent(contentTransform, childControl.UpperBoundToScrollTo, out top, out dummyBottom);
                }

                // Grab new bottom value
                if (childControl.LowerBoundToScrollTo != null)
                {
                    float dummyTop;
                    GetVerticalAnchoredPositionInContent(contentTransform, childControl.LowerBoundToScrollTo, out dummyTop, out bottom);
                }
            }
            return returnCenter;
        }

        public static float GetVerticalAnchoredPositionInContent(RectTransform contentTransform, RectTransform childControl)
        {
            float top, bottom;
            GetVerticalAnchoredPositionInContent(contentTransform, childControl, out top, out bottom);
            return (top + bottom) / 2f;
        }

        public static void GetVerticalAnchoredPositionInContent(RectTransform contentTransform, RectTransform childControl, out float top, out float bottom)
        {
            top = 0f;
            bottom = 0f;
            if ((contentTransform != null) && (childControl != null))
            {
                // Calculate the child control's Y-position relative to the ScrollRect's content
                RectTransform checkControl = childControl;
                float offset;
                while ((checkControl != null) && (checkControl != contentTransform))
                {
                    offset = checkControl.anchoredPosition.y + (checkControl.rect.height * checkControl.pivot.y);
                    Utility.Log(checkControl.name + "'s offset: " + offset.ToString());
                    top += offset;

                    // Get the parent of the control
                    checkControl = checkControl.parent as RectTransform;
                }
                //Utility.Log("GetVerticalAnchoredPositionInContent(): " + selectionPosition);
                bottom = top - childControl.rect.height;
            }
        }
    }
}
