using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames.Menu
{
    public static class ScrollingHelper
    {
        enum ScrollVerticalSnap
        {
            None = -1,
            CenterToChild = 0,
            TopOfChild,
            BottomOfChild
        }

        public struct SingleAxisBounds
        {
            public readonly float max;
            public readonly float middle;
            public readonly float min;

            public SingleAxisBounds(float max, float middle, float min)
            {
                this.max = max;
                this.middle = middle;
                this.min = min;
            }
        }

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
                    // Calculate how much this control offsets the anchored position
                    offset = checkControl.anchoredPosition.y + (checkControl.rect.height * checkControl.pivot.y);

                    // Sum this offset
                    top += offset;

                    // Get the parent of the control
                    checkControl = checkControl.parent as RectTransform;
                }
                bottom = top - childControl.rect.height;
            }
        }

        public static void ScrollVerticallyTo(ScrollRect parentScrollRect, UiEventNavigation childControl, Dictionary<UiEventNavigation, SingleAxisBounds> cacheDict = null, bool centerTo = false)
        {
            if ((parentScrollRect != null) && (childControl != null) && (childControl.Selectable != null))
            {
                // Check the cache, and see if the bounds for this child control already exists
                SingleAxisBounds childBounds;
                if ((cacheDict == null) || (cacheDict.TryGetValue(childControl, out childBounds) == false))
                {
                    // Calculate the top and bottom position of the the control
                    float topPos, centerPos, bottomPos;
                    centerPos = ScrollingHelper.GetVerticalAnchoredPositionInContent(parentScrollRect.content, childControl, out topPos, out bottomPos);
                    childBounds = new SingleAxisBounds(topPos, centerPos, bottomPos);

                    // Cache these values
                    if (cacheDict != null)
                    {
                        cacheDict.Add(childControl, childBounds);
                    }
                }

                // Check whether we need to scroll or not, and if so, in which snapping direction
                ScrollVerticalSnap snapTo = ScrollVerticalSnap.CenterToChild;
                if (centerTo == false)
                {
                    snapTo = GetVerticalSnapping(parentScrollRect.content, parentScrollRect.viewport, ref childBounds);
                }

                // Check whether we want to scroll or not
                if (snapTo != ScrollVerticalSnap.None)
                {
                    // Grab the position to scroll to
                    float selectionPosition = GetScrollToPosition(parentScrollRect.viewport, snapTo, ref childBounds);

                    // Clamp the selection position value
                    float maxPosition = (parentScrollRect.content.rect.height - parentScrollRect.viewport.rect.height);
                    selectionPosition = Mathf.Clamp(selectionPosition, 0, maxPosition);

                    // Directly set the position of the ScrollRect's content
                    Vector3 scrollPosition = parentScrollRect.content.anchoredPosition;
                    scrollPosition.y = selectionPosition;
                    parentScrollRect.content.anchoredPosition = scrollPosition;
                }
            }
        }

        #region ScrollVerticallyTo() Helpers
        private static ScrollVerticalSnap GetVerticalSnapping(RectTransform contentTransform, RectTransform viewportTransform, ref SingleAxisBounds childBounds)
        {
            ScrollVerticalSnap returnOffset = ScrollVerticalSnap.None;
            float topOfChildControl = childBounds.max;
            float bottomOfChildControl = childBounds.min;

            // Check if viewport is smaller than content
            float viewportHeight = viewportTransform.rect.height;
            if (contentTransform.rect.height > viewportHeight)
            {
                // Based on these values, determine whether to snap to the top or bottom of out-of-view child control
                if (Mathf.Abs(topOfChildControl) < contentTransform.anchoredPosition.y)
                {
                    returnOffset = ScrollVerticalSnap.TopOfChild;
                }
                else if (Mathf.Abs(bottomOfChildControl) > (contentTransform.anchoredPosition.y + viewportHeight))
                {
                    returnOffset = ScrollVerticalSnap.BottomOfChild;
                }
            }
            return returnOffset;
        }

        private static float GetScrollToPosition(RectTransform viewportTransform, ScrollVerticalSnap snapTo, ref SingleAxisBounds childBounds)
        {
            // By default, snap to the top of the child control
            float childControlPosition = childBounds.max;

            // Check the snap-to algorithm
            if (snapTo == ScrollVerticalSnap.BottomOfChild)
            {
                // Shift the scroll position to the bottom of the scrollrect
                childControlPosition = childBounds.min;
                childControlPosition += viewportTransform.rect.height;
            }
            else if (snapTo == ScrollVerticalSnap.CenterToChild)
            {
                // Shift the scroll position to the center of the scrollrect
                childControlPosition = childBounds.middle;
                childControlPosition += (viewportTransform.rect.height / 2f);
            }
            childControlPosition *= -1f;
            return childControlPosition;
        }
        #endregion
    }
}
