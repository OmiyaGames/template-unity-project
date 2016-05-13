using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SnappingScrollRect.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
    /// <date>8/21/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An extension of <code>ScrollRect</code> that snaps at discrete intervals.
    /// </summary>
    /// <seealso cref="ScrollRect"/>
    public class SnappingScrollRect : ScrollRect
    {
        public enum State
        {
            Still,
            Dragging,
            SeekingClosestPoint,
            Snapping
        }

        [System.Serializable]
        public struct SnappingPositionIndexes : System.IEquatable<SnappingPositionIndexes>
        {
            [SerializeField]
            int horizontal;
            [SerializeField]
            int vertical;

            public SnappingPositionIndexes(int horizontalIndex, int verticalIndex)
            {
                horizontal = horizontalIndex;
                if (horizontal < 0)
                {
                    horizontal = 0;
                }

                vertical = verticalIndex;
                if (vertical < 0)
                {
                    vertical = 0;
                }
            }

            public int Horizontal
            {
                get
                {
                    return horizontal;
                }
                set
                {
                    horizontal = value;
                    if(horizontal < 0)
                    {
                        horizontal = 0;
                    }
                }
            }

            public int Vertical
            {
                get
                {
                    return vertical;
                }
                set
                {
                    vertical = value;
                    if (vertical < 0)
                    {
                        vertical = 0;
                    }
                }
            }

            public float HorizontalRatio(int maxHorizontal)
            {
                float returnRatio = 0.5f;
                if(maxHorizontal > 1)
                {
                    returnRatio = Horizontal;
                    returnRatio /= (maxHorizontal - 1);
                }
                return returnRatio;
            }

            public float VerticalRatio(int maxVertical)
            {
                float returnRatio = 0.5f;
                if (maxVertical > 1)
                {
                    returnRatio = Vertical;
                    returnRatio /= (maxVertical - 1);
                }
                return returnRatio;
            }

            public bool Equals(SnappingPositionIndexes other)
            {
                return ((Horizontal == other.horizontal) && (Vertical == other.Vertical));
            }
        }

        public const float SnapThreshold = 0.01f;

        public event System.Action<SnappingScrollRect, SnappingPositionIndexes> OnAutoSnappingBegin;
        public event System.Action<SnappingScrollRect, SnappingPositionIndexes> OnClosestContentChanged;

        [SerializeField]
        SnappingPositionIndexes numberOfSnappingPoints = new SnappingPositionIndexes(1, 1);
        [SerializeField]
        float snapIfVelocityIsBelow = 200f;

        State currentState = State.Snapping;
        float closestDistance = 0, tempDistance = 0, horizontalSpeed = 0, verticalSpeed = 0;
        SnappingPositionIndexes snapTo = new SnappingPositionIndexes(0, 0),
            lastClosestContent = new SnappingPositionIndexes(0, 0),
            currentClosestContent = new SnappingPositionIndexes(0, 0);
        bool teleportOnNextSnap = false;

        #region Properties
        public SnappingPositionIndexes SnapTo
        {
            get
            {
                return snapTo;
            }
            set
            {
                snapTo = value;
                teleportOnNextSnap = false;
                currentState = State.Snapping;
            }
        }

        public SnappingPositionIndexes TeleportTo
        {
            set
            {
                snapTo = value;
                teleportOnNextSnap = true;
                currentState = State.Snapping;
            }
        }

        public State CurrentState
        {
            get
            {
                return currentState;
            }
        }

        public SnappingPositionIndexes ClosestContent
        {
            get
            {
                SnappingPositionIndexes returnContent = new SnappingPositionIndexes(0, 0);

                // Get the closest horizontal snap point
                closestDistance = float.MaxValue;
                for (int horizontalIndex = 0; horizontalIndex < numberOfSnappingPoints.Horizontal; ++horizontalIndex)
                {
                    tempDistance = horizontalIndex;
                    tempDistance /= (numberOfSnappingPoints.Horizontal - 1);
                    tempDistance = Mathf.Abs(horizontalNormalizedPosition - tempDistance);
                    if (tempDistance < closestDistance)
                    {
                        returnContent.Horizontal = horizontalIndex;
                        closestDistance = tempDistance;
                    }
                }

                // Get the closest vertical snap point
                closestDistance = float.MaxValue;
                for (int verticalIndex = 0; verticalIndex < numberOfSnappingPoints.Vertical; ++verticalIndex)
                {
                    tempDistance = verticalIndex;
                    tempDistance /= (numberOfSnappingPoints.Vertical - 1);
                    tempDistance = Mathf.Abs(verticalNormalizedPosition - tempDistance);
                    if(tempDistance < closestDistance)
                    {
                        returnContent.Vertical = verticalIndex;
                        closestDistance = tempDistance;
                    }
                }

                return returnContent;
            }
        }

        bool IsSnappedToContent
        {
            get
            {
                bool returnFlag = true;
                if(horizontal == true)
                {
                    returnFlag = (Mathf.Abs(horizontalNormalizedPosition - SnapTo.HorizontalRatio(numberOfSnappingPoints.Horizontal)) < SnapThreshold);
                }
                if((vertical == true) && (returnFlag == true))
                {
                    returnFlag = (Mathf.Abs(verticalNormalizedPosition - SnapTo.VerticalRatio(numberOfSnappingPoints.Vertical)) < SnapThreshold);
                }
                return returnFlag;
            }
        }
        #endregion

        #region Overrides
        protected override void Awake()
        {
            // Call base function
            base.Awake();

            // Snap to the current closest object
            snapTo.Vertical = 0;
            snapTo.Horizontal = 0;
            currentState = State.SeekingClosestPoint;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            currentState = State.Dragging;
            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            currentState = State.SeekingClosestPoint;
            base.OnEndDrag(eventData);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (CurrentState == State.Snapping)
            {
                UpdateSnapping(ref currentState);
            }
            else if (CurrentState == State.SeekingClosestPoint)
            {
                UpdateClosestPoint(ref currentState);
            }

            if ((CurrentState != State.Still) && (OnClosestContentChanged != null))
            {
                UpdateContentChanged();
            }
        }
        #endregion

        #region Helper Methods
        void UpdateClosestPoint(ref State updateState)
        {
            // Make sure all parameters are valid, and velocity is low enough
            if ((snapIfVelocityIsBelow > 0) && (Mathf.Abs(velocity.x) < snapIfVelocityIsBelow))
            {
                // Get the content to snap to
                SnapTo = ClosestContent;
                if (OnAutoSnappingBegin != null)
                {
                    OnAutoSnappingBegin(this, SnapTo);
                }
            }
        }

        void UpdateSnapping(ref State updateState)
        {
            // Check where to snap to
            if ((teleportOnNextSnap == false) && (IsSnappedToContent == false))
            {
                // Move to the closest object
                if (horizontal == true)
                {
                    horizontalNormalizedPosition = Mathf.SmoothDamp(horizontalNormalizedPosition, SnapTo.HorizontalRatio(numberOfSnappingPoints.Horizontal), ref horizontalSpeed, elasticity);
                }
                if (vertical == true)
                {
                    verticalNormalizedPosition = Mathf.SmoothDamp(verticalNormalizedPosition, SnapTo.VerticalRatio(numberOfSnappingPoints.Vertical), ref verticalSpeed, elasticity);
                }
            }
            else
            {
                // Move to the closest object
                if (horizontal == true)
                {
                    horizontalNormalizedPosition = SnapTo.HorizontalRatio(numberOfSnappingPoints.Horizontal);
                }
                if (vertical == true)
                {
                    verticalNormalizedPosition = SnapTo.VerticalRatio(numberOfSnappingPoints.Vertical);
                }

                // Update flags
                horizontalSpeed = 0;
                verticalSpeed = 0;
                teleportOnNextSnap = false;
                updateState = State.Still;

                // Run content changed event
                if (OnClosestContentChanged != null)
                {
                    UpdateContentChanged();
                }
            }
        }

        void UpdateContentChanged()
        {
            // Get the closest content
            currentClosestContent = ClosestContent;
            if (horizontal == false)
            {
                currentClosestContent.Horizontal = lastClosestContent.Horizontal;
            }
            if (vertical == false)
            {
                currentClosestContent.Vertical = lastClosestContent.Vertical;
            }

            // Check to see if the closest content changed
            if (lastClosestContent.Equals(currentClosestContent) == false)
            {
                OnClosestContentChanged(this, currentClosestContent);
                lastClosestContent = currentClosestContent;
            }
        }
        #endregion
    }
}
