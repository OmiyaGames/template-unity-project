using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ILabelResizer.cs" company="Omiya Games">
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
    /// <date>9/11/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Automatically resizes the font size of a label component based on settings.
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
    /// <description>9/11/2018</description>
    /// <description>Taro</description>
    /// <description>Initial version</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    public abstract class ILabelResizer<LABEL> : IResizer where LABEL : UnityEngine.UI.ILayoutElement
    {
        LABEL label = default(LABEL);
        float originalFontSize = -1f;
        ResizeMultiplierChanged lastAction = null;

        public LABEL Label
        {
            get
            {
                if (label == null)
                {
                    label = GetComponent<LABEL>();
                }
                return label;
            }
        }

        public float OriginalFontSize
        {
            get
            {
                if (originalFontSize < 0)
                {
                    originalFontSize = FontSize;
                }
                return originalFontSize;
            }
        }

        public abstract float FontSize
        {
            get;
        }

        public void OnEnable()
        {
            // Update the font size if the multiplier is not set to 1
            UpdateLabelSize();

            // Bind to the resize event
            if (lastAction == null)
            {
                lastAction = new ResizeMultiplierChanged(UpdateLabelSize);
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

        public abstract void UpdateLabelSize();

        private void UpdateLabelSize(float lastMultiplier, float newMultiplier)
        {
            UpdateLabelSize();
        }
    }
}
