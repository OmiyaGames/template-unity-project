using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        [SerializeField]
        ScrollRect scrollable;
        [SerializeField]
        UiEventNavigation[] uiElementsInScrollable;
        [SerializeField]
        UiEventNavigation[] uiElementsBelowScrollable;

        UiEventNavigation lastSelectedElement = null;
        System.Action<UiEventNavigation, BaseEventData> scrollableSelected = null;
        System.Action<UiEventNavigation, bool> scrollableEnableAndActiveChanged = null;

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

        public void BindToEvents()
        {
            OnDestroy();
            scrollableSelected = new System.Action<UiEventNavigation, BaseEventData>(MenuNavigator_OnAfterSelect);
            scrollableEnableAndActiveChanged = new System.Action<UiEventNavigation, bool>(MenuNavigator_OnAfterEnabledAndActiveChanged);
            foreach(UiEventNavigation element in uiElementsInScrollable)
            {
                element.OnAfterSelect += scrollableSelected;
                element.OnAfterEnabledAndActiveChanged += scrollableEnableAndActiveChanged;
            }
        }

        public void UpdateNavigation()
        {
            // Cache the top-most and bottom-most element
            UiEventNavigation topMostElement = null;
            UiEventNavigation lastElement = null;

            // Setup navigating to UI Elements in the scrollable area
            foreach (UiEventNavigation nextElement in uiElementsInScrollable)
            {
                if ((nextElement != null) && (nextElement.isActiveAndEnabled == true))
                {
                    SetupUiElementsInScrollable(nextElement, ref lastElement, ref topMostElement);
                }
            }

            // Setup navigating to the horizontal scroll bar
            Scrollbar horizontalScrollbar = SetupHorizontalScrollBar(lastElement);

            // Setup navigating to UI Elements below the scrollable area
            foreach (UiEventNavigation nextElement in uiElementsBelowScrollable)
            {
                if ((nextElement != null) && (nextElement.isActiveAndEnabled == true))
                {
                    SetupUiElementsBelowScrollable(nextElement, horizontalScrollbar, ref lastElement, ref topMostElement);
                    horizontalScrollbar = null;
                }
            }

            // Finally, allow looping controls
            if ((topMostElement != null) && (lastElement != null))
            {
                SetNextNavigation(lastElement, topMostElement.Selectable);
                SetPreviousNavigation(lastElement.Selectable, topMostElement);
            }
        }

        private void OnDestroy()
        {
            if(scrollableSelected != null)
            {
                foreach (UiEventNavigation element in uiElementsInScrollable)
                {
                    element.OnAfterSelect -= scrollableSelected;
                }
                scrollableSelected = null;
            }
            if (scrollableEnableAndActiveChanged != null)
            {
                foreach (UiEventNavigation element in uiElementsInScrollable)
                {
                    element.OnAfterEnabledAndActiveChanged -= scrollableEnableAndActiveChanged;
                }
                scrollableEnableAndActiveChanged = null;
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
                SetPreviousNavigation(lastElement.Selectable, nextElement, VerticalScrollbar);
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

                // Customize the navigation
                if ((lastElement.ToNextUi & UiEventNavigation.Direction.Up) != 0)
                {
                    newNavigation.selectOnUp = currentElement;
                }
                if ((lastElement.ToNextUi & UiEventNavigation.Direction.Down) != 0)
                {
                    newNavigation.selectOnDown = currentElement;
                }
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
                lastElement.Selectable.navigation = newNavigation;
            }
        }

        private static void SetPreviousNavigation(Selectable lastElement, UiEventNavigation currentElement, Scrollbar verticalScrollbar = null)
        {
            // Check if the last and current element is available
            if (currentElement != null)
            {
                // Grab the current navigation values
                Navigation newNavigation = currentElement.Selectable.navigation;

                // Customize the navigation
                if ((currentElement.ToPreviousUi & UiEventNavigation.Direction.Up) != 0)
                {
                    newNavigation.selectOnUp = lastElement;
                }
                if ((currentElement.ToPreviousUi & UiEventNavigation.Direction.Down) != 0)
                {
                    newNavigation.selectOnDown = lastElement;
                }
                if ((currentElement.ToPreviousUi & UiEventNavigation.Direction.Left) != 0)
                {
                    newNavigation.selectOnLeft = lastElement;
                }
                if ((currentElement.ToPreviousUi & UiEventNavigation.Direction.Right) != 0)
                {
                    newNavigation.selectOnRight = lastElement;
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
        }
        #endregion

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
                Utility.ScrollVerticallyTo(scrollable, source.transform as RectTransform);
            }
        }
    }
}
