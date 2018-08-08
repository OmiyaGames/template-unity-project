using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MenuNavigator.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2018 Omiya Games
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
    /// <date>7/12/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A script to setup navigation of a menu.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>7/12/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IMenu))]
    public class MenuNavigator : MonoBehaviour
    {
        // FIXME: somehow add a UI that handles changes in the scrollbar,
        // and updates the scrollbar navigation to left to the center element on the scrollrect.
        [SerializeField]
        ScrollRect scrollable;
        [SerializeField]
        UiEventNavigation[] uiElementsInScrollable;
        [SerializeField]
        UiEventNavigation[] uiElementsBelowScrollable;

        IMenu cachedMenu = null;
        Action<UiEventNavigation, BaseEventData> scrollableSelected = null;
        Action<UiEventNavigation, bool> scrollableEnableAndActiveChanged = null;
        Action<UiEventNavigation, BaseEventData> scrollableSubmitted = null;
        Action<UiEventNavigation, BaseEventData> scrollableCancelled = null;
        readonly HashSet<UiEventNavigation> uiElementsInScrollableSet = new HashSet<UiEventNavigation>();

        #region Properties
        public UiEventNavigation LastSelectedElement
        {
            get;
            private set;
        } = null;

        public HashSet<UiEventNavigation> UiElementsInScrollableSet
        {
            get
            {
                if (uiElementsInScrollableSet.Count != UiElementsBelowScrollable.Length)
                {
                    uiElementsInScrollableSet.Clear();
                    foreach (UiEventNavigation scrollableElement in UiElementsBelowScrollable)
                    {
                        if ((scrollableElement != null) && (uiElementsInScrollableSet.Contains(scrollableElement) == false))
                        {
                            uiElementsInScrollableSet.Add(scrollableElement);
                        }
                    }
                }
                return uiElementsInScrollableSet;
            }
        }

        public UiEventNavigation[] UiElementsInScrollable
        {
            get
            {
                return uiElementsInScrollable;
            }
            set
            {
                if (uiElementsInScrollable != value)
                {
                    OnDestroy();
                    uiElementsInScrollable = value;
                }
            }
        }

        public UiEventNavigation[] UiElementsBelowScrollable
        {
            get
            {
                return uiElementsBelowScrollable;
            }
            set
            {
                if (uiElementsBelowScrollable != value)
                {
                    uiElementsBelowScrollable = value;
                }
            }
        }

        Scrollbar VerticalScrollbar
        {
            get
            {
                Scrollbar returnScrollbar = null;
                if ((scrollable != null) && (scrollable.vertical == true))
                {
                    // Grab the scroll bar
                    returnScrollbar = scrollable.verticalScrollbar;

                    // Check if the scrollbar is usable
                    if ((returnScrollbar != null) && (scrollable.viewport.rect.height >= scrollable.content.rect.height))
                    {
                        returnScrollbar = null;
                    }
                }
                return returnScrollbar;
            }
        }

        IMenu Menu
        {
            get
            {
                if (cachedMenu == null)
                {
                    cachedMenu = GetComponent<IMenu>();
                }
                return cachedMenu;
            }
        }
        #endregion

        public void BindToEvents()
        {
            // Unbind to previous events
            OnDestroy();

            // Setup actions
            scrollableSelected = new Action<UiEventNavigation, BaseEventData>(MenuNavigator_OnAfterSelect);
            scrollableEnableAndActiveChanged = new Action<UiEventNavigation, bool>(MenuNavigator_OnAfterEnabledAndActiveChanged);
            scrollableSubmitted = new Action<UiEventNavigation, BaseEventData>(MenuNavigator_OnAfterSubmit);
            scrollableCancelled = new Action<UiEventNavigation, BaseEventData>(MenuNavigator_OnAfterCancel);

            // Bind to events
            foreach (UiEventNavigation element in UiElementsInScrollable)
            {
                element.OnAfterSelect += scrollableSelected;
                // FIXME: look into getting rid of this line, due to it calling the setup function far too much.
                element.OnAfterEnabledAndActiveChanged += scrollableEnableAndActiveChanged;
                element.OnAfterSubmit += scrollableSubmitted;
                element.OnAfterCancel += scrollableCancelled;
            }
            foreach (UiEventNavigation element in uiElementsBelowScrollable)
            {
                // FIXME: look into getting rid of this line, due to it calling the setup function far too much.
                element.OnAfterEnabledAndActiveChanged += scrollableEnableAndActiveChanged;
                element.OnAfterCancel += scrollableCancelled;
            }
        }

        public void UpdateNavigation()
        {
            if (Menu.CurrentVisibility == IMenu.VisibilityState.Visible)
            {
                // Cache the top-most and bottom-most element
                UiEventNavigation topMostElement = null;
                UiEventNavigation lastElement = null;
                int allElements = 0;

                // Setup navigating to UI Elements in the scrollable area
                foreach (UiEventNavigation nextElement in UiElementsInScrollable)
                {
                    // Enable navigation to elements that are active
                    if ((nextElement != null) && (nextElement.isActiveAndEnabled == true) && (nextElement.Selectable.interactable == true))
                    {
                        SetupUiElementsInScrollable(nextElement, ref lastElement, ref topMostElement);
                        ++allElements;
                    }
                }

                // Setup navigating to the horizontal scroll bar
                Scrollbar horizontalScrollbar = SetupHorizontalScrollBar(lastElement);
                if(horizontalScrollbar != null)
                {
                    ++allElements;
                }

                // Setup navigating to UI Elements below the scrollable area
                foreach (UiEventNavigation nextElement in UiElementsBelowScrollable)
                {
                    // Enable navigation to elements that are active
                    if ((nextElement != null) && (nextElement.isActiveAndEnabled == true) && (nextElement.Selectable.interactable == true))
                    {
                        SetupUiElementsBelowScrollable(nextElement, horizontalScrollbar, ref lastElement, ref topMostElement);
                        ++allElements;
                        horizontalScrollbar = null;
                    }
                }

                // Finally, allow looping controls
                if ((topMostElement != null) && (lastElement != null))
                {
                    SetNextNavigation(lastElement, topMostElement.Selectable);
                    SetPreviousNavigation(lastElement.Selectable, topMostElement);
                }

                // Double-check if the currently selected UI is active and interactable
                GuaranteUiElementIsSelected(allElements);
            }
        }

        public void ScrollToLastSelectedElement(UiEventNavigation defaultElement)
        {
            // Make sure the last selected element is within the scrollable list
            if ((LastSelectedElement != null) && (UiElementsInScrollableSet.Contains(LastSelectedElement) == true))
            {
                // Change the default element to the last selected one
                defaultElement = LastSelectedElement;
            }

            // Scroll to this element
            ScrollToSelectable(defaultElement);
        }

        public void ScrollToSelectable(UiEventNavigation selectable)
        {
            ScrollToSelectable(selectable, true);
        }

        private void ScrollToSelectable(UiEventNavigation selectable, bool forceCenter)
        {
            // Check if we have the scroll view open
            if ((scrollable != null) && (selectable != null))
            {
                // Scroll to this control
                ScrollVerticallyTo(scrollable, selectable, boundsCache, forceCenter);

                // Highlight this element
                MenuManager manager = Singleton.Get<MenuManager>();
                if(manager != null)
                {
                    manager.SelectGui(selectable.Selectable);
                }
            }
        }

        private void OnDestroy()
        {
            if (scrollableSelected != null)
            {
                // Unbind to events
                foreach (UiEventNavigation element in UiElementsInScrollable)
                {
                    element.OnAfterSelect -= scrollableSelected;
                    // FIXME: look into getting rid of this line, due to it calling the setup function far too much.
                    element.OnAfterEnabledAndActiveChanged -= scrollableEnableAndActiveChanged;
                    element.OnAfterSubmit -= scrollableSubmitted;
                    element.OnAfterCancel -= scrollableCancelled;
                }
                foreach (UiEventNavigation element in uiElementsBelowScrollable)
                {
                    // FIXME: look into getting rid of this line, due to it calling the setup function far too much.
                    element.OnAfterEnabledAndActiveChanged -= scrollableEnableAndActiveChanged;
                    element.OnAfterCancel -= scrollableCancelled;
                }

                // Reset variables
                scrollableSelected = null;
                scrollableEnableAndActiveChanged = null;
                scrollableSubmitted = null;
                scrollableCancelled = null;
            }
        }

        #region UpdateNavigation Helper Methods
        private static void SetupUiElementsBelowScrollable(UiEventNavigation nextElement, Scrollbar horizontalScrollbar, ref UiEventNavigation lastElement, ref UiEventNavigation topMostElement)
        {
            // Check if this is the top-most element
            if (topMostElement == null)
            {
                // If so, set the variable
                topMostElement = nextElement;
            }

            // Check if there's an element above this
            ResetNavigation(nextElement.Selectable);
            SetNextNavigation(lastElement, nextElement.Selectable);
            if (horizontalScrollbar != null)
            {
                // If the horizontal scroll bar is above these elements, setup navigation to that first
                SetPreviousNavigation(horizontalScrollbar, nextElement);
            }
            else if (lastElement != null)
            {
                // If last element is available, setup that element
                SetPreviousNavigation(lastElement.Selectable, nextElement);
            }
            lastElement = nextElement;
        }

        private Scrollbar SetupHorizontalScrollBar(UiEventNavigation lastElement)
        {
            Scrollbar horizontalScrollbar = null;
            if ((lastElement != null) && (scrollable != null) && (scrollable.horizontal == true) && (scrollable.horizontalScrollbar != null) && (scrollable.viewport.rect.width < scrollable.content.rect.width))
            {
                horizontalScrollbar = scrollable.horizontalScrollbar;
                SetNextNavigation(lastElement, horizontalScrollbar);

                // Grab the current navigation values
                Navigation newNavigation = horizontalScrollbar.navigation;

                // Customize the navigation
                newNavigation.mode = Navigation.Mode.Explicit;
                newNavigation.selectOnUp = lastElement.Selectable;
                horizontalScrollbar.navigation = newNavigation;
            }
            return horizontalScrollbar;
        }

        private void SetupUiElementsInScrollable(UiEventNavigation nextElement, ref UiEventNavigation lastElement, ref UiEventNavigation topMostElement)
        {
            // Check if this is the top-most element
            if (topMostElement == null)
            {
                // If so, set the variable
                topMostElement = nextElement;
            }

            // Check if there's an element above this
            ResetNavigation(nextElement.Selectable);
            SetNextNavigation(lastElement, nextElement.Selectable, VerticalScrollbar);
            if (lastElement != null)
            {
                SetPreviousNavigation(lastElement.Selectable, nextElement);
            }
            lastElement = nextElement;
        }

        private static void SetNextNavigation(UiEventNavigation lastElement, Selectable currentElement, Scrollbar verticalScrollbar = null)
        {
            // Check if the last and current element is available
            if (lastElement != null)
            {
                // Grab the last navigation values
                Navigation newNavigation = lastElement.Selectable.navigation;

                // Customize the navigation for vertical
                if ((lastElement.ToNextUi & UiEventNavigation.Direction.Up) != 0)
                {
                    newNavigation.selectOnUp = currentElement;
                }
                if ((lastElement.ToNextUi & UiEventNavigation.Direction.Down) != 0)
                {
                    newNavigation.selectOnDown = currentElement;
                }

                // Check if element is not a slider
                if ((lastElement.Selectable is Slider) == false)
                {
                    // Customize the navigation for horizontal
                    if ((lastElement.ToNextUi & UiEventNavigation.Direction.Left) != 0)
                    {
                        newNavigation.selectOnLeft = currentElement;
                    }
                    if ((lastElement.ToNextUi & UiEventNavigation.Direction.Right) != 0)
                    {
                        newNavigation.selectOnRight = currentElement;
                    }
                    else if (verticalScrollbar != null)
                    {
                        // If right-direction navigation isn't overridden, and a scrollbar is provided,
                        // navigate to the scroll on right
                        newNavigation.selectOnRight = verticalScrollbar;
                    }
                }
                lastElement.Selectable.navigation = newNavigation;
            }
        }

        private static void SetPreviousNavigation(Selectable lastElement, UiEventNavigation currentElement)
        {
            // Check if the last and current element is available
            if (currentElement != null)
            {
                // Grab the current navigation values
                Navigation newNavigation = currentElement.Selectable.navigation;

                // Customize the navigation for vertical
                if ((currentElement.ToPreviousUi & UiEventNavigation.Direction.Up) != 0)
                {
                    newNavigation.selectOnUp = lastElement;
                }
                if ((currentElement.ToPreviousUi & UiEventNavigation.Direction.Down) != 0)
                {
                    newNavigation.selectOnDown = lastElement;
                }

                // Check if element is not a slider
                if ((currentElement.Selectable is Slider) == false)
                {
                    if ((currentElement.ToPreviousUi & UiEventNavigation.Direction.Left) != 0)
                    {
                        newNavigation.selectOnLeft = lastElement;
                    }
                    if ((currentElement.ToPreviousUi & UiEventNavigation.Direction.Right) != 0)
                    {
                        newNavigation.selectOnRight = lastElement;
                    }
                }
                currentElement.Selectable.navigation = newNavigation;
            }
        }

        private static void ResetNavigation(Selectable currentElement)
        {
            Navigation newNavigation = currentElement.navigation;

            // Customize the navigation
            newNavigation.mode = Navigation.Mode.Explicit;
            newNavigation.selectOnUp = null;
            newNavigation.selectOnDown = null;
            newNavigation.selectOnLeft = null;
            newNavigation.selectOnRight = null;

            // Set the navigation values
            currentElement.navigation = newNavigation;
        }

        private void GuaranteUiElementIsSelected(int fullSize)
        {
            if (LastSelectedElement != null)
            {
                // If not, go back up until an active control is found
                Selectable nextElement = LastSelectedElement.Selectable;
                Navigation navigation;
                while ((nextElement != null) && (fullSize > 0) && ((nextElement.isActiveAndEnabled == false) || (nextElement.interactable == false)))
                {
                    navigation = nextElement.navigation;
                    if (navigation.mode == Navigation.Mode.Explicit)
                    {
                        nextElement = navigation.selectOnUp;
                    }
                    else
                    {
                        break;
                    }
                    --fullSize;
                }

                // If one is found, select this element automatically
                if (nextElement != null)
                {
                    UiEventNavigation uiNavigation = nextElement.GetComponent<UiEventNavigation>();
                    if(uiNavigation != null)
                    {
                        ScrollToSelectable(uiNavigation, false);
                    }
                }
            }
        }
        #endregion

        #region Event Listeners
        private void MenuNavigator_OnAfterEnabledAndActiveChanged(UiEventNavigation source, bool arg)
        {
            UpdateNavigation();
        }

        private void MenuNavigator_OnAfterSelect(UiEventNavigation source, BaseEventData arg)
        {
            // Check if we have the scroll view open
            if (scrollable != null)
            {
                // Scroll to this control
                ScrollVerticallyTo(scrollable, source);
            }
            LastSelectedElement = source;
        }

        private void MenuNavigator_OnAfterSubmit(UiEventNavigation source, BaseEventData arg)
        {
            // Check if submitting to this menu causes some UI to be enabled/disabled
            if ((source != null) && (source.DoesSubmitChangesInteractable == true))
            {
                // If so, update navigation UI
                UpdateNavigation();
            }
        }

        private void MenuNavigator_OnAfterCancel(UiEventNavigation source, BaseEventData arg)
        {
            // Make sure the menu is managed and NOT the default
            if ((Menu != null) && (Menu.MenuType == IMenu.Type.ManagedMenu))
            {
                // Hide the menu
                Menu.Hide();
            }
        }
        #endregion

#if UNITY_EDITOR
        [ContextMenu("Setup Fields")]
        void SetupFields()
        {
            scrollable = GetComponentInChildren<ScrollRect>(true);
            UiEventNavigation[] allUiElements = GetComponentsInChildren<UiEventNavigation>(true);

            List<UiEventNavigation> uiElementsInScrollableList = new List<UiEventNavigation>();
            List<UiEventNavigation> uiElementsBelowScrollableList = new List<UiEventNavigation>();
            foreach (UiEventNavigation uiElement in allUiElements)
            {
                if (scrollable != null)
                {
                    // Go up the hierarch of this element and see if this element is a child of the scrollable
                    Transform check = uiElement.transform;
                    while ((check != null) && (check != transform) && (check != scrollable.transform))
                    {
                        check = check.parent;
                    }

                    if (check == scrollable.transform)
                    {
                        uiElementsInScrollableList.Add(uiElement);
                    }
                    else
                    {
                        uiElementsBelowScrollableList.Add(uiElement);
                    }
                }
                else
                {
                    uiElementsBelowScrollableList.Add(uiElement);
                }
            }

            uiElementsInScrollableList.Sort(UiElementsSorter);
            UiElementsInScrollable = uiElementsInScrollableList.ToArray();
            uiElementsBelowScrollableList.Sort(UiElementsSorter);
            UiElementsBelowScrollable = uiElementsBelowScrollableList.ToArray();
        }

        private int UiElementsSorter(UiEventNavigation left, UiEventNavigation right)
        {
            // Take priority on right by default
            int returnNum = 1;
            if ((left != null) && (right != null))
            {
                // Check difference in vertical placement of 2 elements
                Vector2 leftPlacement = left.RectTransform.rect.min;
                Vector2 rightPlacement = right.RectTransform.rect.min;
                if (Mathf.Approximately(leftPlacement.y, rightPlacement.y) == true)
                {
                    if (Mathf.Approximately(leftPlacement.x, rightPlacement.x) == true)
                    {
                        returnNum = 0;
                    }
                    else if (leftPlacement.x < rightPlacement.x)
                    {
                        returnNum = -1;
                    }
                }
                else if (leftPlacement.y < rightPlacement.y)
                {
                    returnNum = -1;
                }
            }
            else if (left != null)
            {
                // Take priority on left
                returnNum = -1;
            }
            else if (right == null)
            {
                // If both null, they're equal
                returnNum = 0;
            }
            return returnNum;
        }
#endif
        #region Scrolling Helper Methods
        private enum ScrollVerticalSnap
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

        private static readonly Dictionary<UiEventNavigation, SingleAxisBounds> boundsCache = new Dictionary<UiEventNavigation, SingleAxisBounds>();

        public static void ScrollVerticallyTo(ScrollRect parentScrollRect, UiEventNavigation childControl, Dictionary<UiEventNavigation, SingleAxisBounds> cacheDict = null, bool centerTo = false)
        {
            if ((parentScrollRect != null) && (childControl != null) && (childControl.Selectable != null))
            {
                Utility.Log("ScrollVerticallyTo(): " + childControl.name);

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
                    Utility.Log("ScrollVerticallyTo(): " + scrollPosition.ToString());
                }
            }
        }

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
                Utility.Log("childControl: " + topOfChildControl + ", " + bottomOfChildControl);
                Utility.Log("contentTransform.anchoredPosition: " + contentTransform.anchoredPosition.y + ", " + (contentTransform.anchoredPosition.y + viewportHeight));
                if (Mathf.Abs(topOfChildControl) < contentTransform.anchoredPosition.y)
                {
                    returnOffset = ScrollVerticalSnap.TopOfChild;
                }
                else if (Mathf.Abs(bottomOfChildControl) > (contentTransform.anchoredPosition.y + viewportHeight))
                {
                    returnOffset = ScrollVerticalSnap.BottomOfChild;
                }
            }
            Utility.Log("GetVerticalSnapping(): " + returnOffset);
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
                childControlPosition = viewportTransform.rect.height;
                childControlPosition -= childBounds.min;
                Utility.Log("GetScrollToPosition(" + snapTo.ToString() + "): " + viewportTransform.rect.height + " - " + childBounds.min + " = " + childControlPosition);
            }
            else if (snapTo == ScrollVerticalSnap.CenterToChild)
            {
                // Shift the scroll position to the center of the scrollrect
                childControlPosition = (viewportTransform.rect.height / 2f);
                childControlPosition += childBounds.middle;
                childControlPosition -= childBounds.min;
            }
            else if(snapTo == ScrollVerticalSnap.TopOfChild)
            {
                Utility.Log("GetScrollToPosition(" + snapTo.ToString() + "): " + childControlPosition);
            }
            childControlPosition *= -1f;
            Utility.Log("GetScrollToPosition(): " + childControlPosition);
            return childControlPosition;
        }
        #endregion
    }
}
