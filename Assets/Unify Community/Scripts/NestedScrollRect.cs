using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Inpsired by http://www.programmierstube.com/unity-4-6-ui-and-nested-scrollrect/
/// </summary>
public class NestedScrollRect : ScrollRect
{
    [SerializeField]
    ScrollRect parentScrollRect;

    bool routeEventsToParent = false;

    public override void OnInitializePotentialDrag (PointerEventData eventData)
    {
        // This event gets called before all the other ones
        if(parentScrollRect != null)
        {
            parentScrollRect.OnInitializePotentialDrag(eventData);
        }
        base.OnInitializePotentialDrag (eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        // By default, don't send any drag event information to the parent
        routeEventsToParent = false;

        // Make sure we have a parent to send our information to
        if(parentScrollRect != null)
        {
            // Check to see which direction we dragging towards, and whether we support that direction
            if ((horizontal == false) && (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y)))
            {
                // If we don't support horizontal scrolling, yet we're scrolling horizontally,
                // route the events to the parent
                routeEventsToParent = true;
            }
            else if ((vertical == false) && (Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y)))
            {
                // If we don't support vertical scrolling, yet we're scrolling vertically,
                // route the events to the parent
                routeEventsToParent = true;
            }
        }

        // Check where to route our event
        if (routeEventsToParent == true)
        {
            // Route event to the parent scroll rect
            parentScrollRect.OnBeginDrag(eventData);
        }
        else
        {
            // Otherwise, route the event to this nested scroll rect
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnDrag (PointerEventData eventData)
    {
        // Check where to route our event
        if (routeEventsToParent == true)
        {
            // Route event to the parent scroll rect
            parentScrollRect.OnDrag(eventData);
        }
        else
        {
            // Otherwise, route the event to this nested scroll rect
            base.OnDrag(eventData);
        }
    }

    public override void OnEndDrag (PointerEventData eventData)
    {
        // Check where to route our event
        if (routeEventsToParent == true)
        {
            // Route event to the parent scroll rect
            parentScrollRect.OnEndDrag(eventData);
        }
        else
        {
            // Otherwise, route the event to this nested scroll rect
            base.OnEndDrag(eventData);
        }

        // Since we're done dragging, stop routing events to the parent
        routeEventsToParent = false;
    }
}