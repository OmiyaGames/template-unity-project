using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="RectTransformResizer.cs" company="Omiya Games">
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
    /// <date>6/5/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Resizes a RectTransform.
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
    /// <description>6/5/2018</description>
    /// <description>Taro</description>
    /// <description>Initial version</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformResizer : IResizer
    {
        [Header("Resize")]
        [SerializeField]
        bool resizeWidth = true;
        [SerializeField]
        bool resizeHeight = true;

        [Header("Reposition")]
        [SerializeField]
        bool repositionX = false;
        [SerializeField]
        bool repositionY = false;

        Rect originalDimensions;
        RectTransform transformCache = null;
        ResizeMultiplierChanged lastAction = null;
        Vector2 neato;

        public RectTransform Transform
        {
            get
            {
                if(transformCache == null)
                {
                    transformCache = GetComponent<RectTransform>();
                    originalDimensions = new Rect(transformCache.anchoredPosition, transformCache.sizeDelta);
                }
                return transformCache;
            }
        }

        public void OnEnable()
        {
            // Update the font size if the multiplier is not set to 1
            UpdateTransform();

            // Bind to the resize event
            if (lastAction == null)
            {
                lastAction = new ResizeMultiplierChanged(UpdateTransform);
                OnAfterResizeMultiplierChanged += lastAction;
            }
        }

        public void OnDestroy()
        {
            if (lastAction != null)
            {
                OnAfterResizeMultiplierChanged -= lastAction;
                lastAction = null;
            }
        }

        public void UpdateTransform()
        {
            // Do NOT attempt to resize the text if it's set to auto-size
            if ((isActiveAndEnabled == true) && (Transform != null))
            {
                // Resize the dimensions first
                if(resizeWidth == true)
                {
                    neato.x = originalDimensions.width * ResizeMultiplier;
                }
                if (resizeHeight == true)
                {
                    neato.y = originalDimensions.height * ResizeMultiplier;
                }
                Transform.sizeDelta = neato;

                // Adjust the position
                if (repositionX == true)
                {
                    neato.x = originalDimensions.x * ResizeMultiplier;
                }
                if (repositionY == true)
                {
                    neato.y = originalDimensions.y * ResizeMultiplier;
                }
                neato = Transform.anchoredPosition;
            }
        }

        private void UpdateTransform(float lastMultiplier, float newMultiplier)
        {
            UpdateTransform();
        }
    }
}
