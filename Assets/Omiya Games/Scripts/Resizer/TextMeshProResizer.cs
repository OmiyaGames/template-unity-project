using UnityEngine;
using TMPro;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslationManager.cs" company="Omiya Games">
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
    /// <date>6/29/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Resizes a TextMeshPro label.
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
    /// <description>Initial verison</description>
    /// </item>
    /// <item>
    /// <description>6/5/2018</description>
    /// <description>Taro</description>
    /// <description>Actual implementation.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProResizer : IResizer
    {
        TextMeshProUGUI label = null;
        float originalFontSize = -1f;
        ResizeMultiplierChanged lastAction = null;

        public TextMeshProUGUI Label
        {
            get
            {
                if(label == null)
                {
                    label = GetComponent<TextMeshProUGUI>();
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
                    originalFontSize = Label.fontSize;
                }
                return originalFontSize;
            }
        }

        private void OnEnable()
        {
            // Update the font size if the multiplier is not set to 1
            UpdateLabelSize();

            // Bind to the resize event
            if(lastAction == null)
            {
                lastAction = new ResizeMultiplierChanged(UpdateLabelSize);
                OnAfterResizeMultiplierChanged += lastAction;
            }
        }

        private void OnDestroy()
        {
            if(lastAction != null)
            {
                OnAfterResizeMultiplierChanged -= lastAction;
                lastAction = null;
            }
        }

        public void UpdateLabelSize()
        {
            // Do NOT attempt to resize the text if it's set to auto-size
            if ((isActiveAndEnabled == true) && (Label != null) && (Label.enableAutoSizing == false))
            {
                Label.fontSize = OriginalFontSize * ResizeMultiplier;
            }
        }

        private void UpdateLabelSize(float lastMultiplier, float newMultiplier)
        {
            UpdateLabelSize();
        }
    }
}
