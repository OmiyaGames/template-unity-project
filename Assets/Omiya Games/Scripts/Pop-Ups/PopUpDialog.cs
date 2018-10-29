﻿using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PopUpDialog.cs" company="Omiya Games">
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
    /// <date>8/21/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// TODO: documentation
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PopUpDialog : IMenu
    {
        [SerializeField]
        Translations.TranslatedTextMeshPro label;
        [SerializeField]
        RectTransform panel;

        [Header("Animation")]
        [SerializeField]
        float highlightStateYScale = 1f;
        [SerializeField]
        float normalStateYScale = 0.8f;

        RectTransform cacheTransform = null;
        Vector2? targetAnchorPosition = null;

        #region Properties
        public ulong ID
        {
            get;
            set;
        }

        public Translations.TranslatedTextMeshPro Label
        {
            get
            {
                return label;
            }
        }

        public RectTransform CachedTransform
        {
            get
            {
                if(cacheTransform == null)
                {
                    cacheTransform = transform as RectTransform;
                }
                return cacheTransform;
            }
        }

        public float Height
        {
            get
            {
                float returnHeight = -1f;
                if((CurrentVisibility != VisibilityState.Hidden) && (panel.gameObject.activeInHierarchy == true))
                {
                    if(CurrentVisibility == VisibilityState.Visible)
                    {
                        returnHeight = panel.sizeDelta.y * highlightStateYScale;
                    }
                    else
                    {
                        returnHeight = panel.sizeDelta.y * normalStateYScale;
                    }
                }
                return returnHeight;
            }
        }

        public override Type MenuType
        {
            get
            {
                return Type.UnmanagedMenu;
            }
        }

        public override Selectable DefaultUi
        {
            get
            {
                return null;
            }
        }

        public Vector2 TargetAnchorPosition
        {
            get
            {
                if(targetAnchorPosition.HasValue == true)
                {
                    return targetAnchorPosition.Value;
                }
                else
                {
                    return CachedTransform.anchoredPosition;
                }
            }
            set
            {
                targetAnchorPosition = value;
            }
        }

        public bool Highlight
        {
            get
            {
                return (CurrentVisibility == VisibilityState.Visible);
            }
            set
            {
                if(CurrentVisibility != VisibilityState.Hidden)
                {
                    if(value == true)
                    {
                        CurrentVisibility = VisibilityState.Visible;
                    }
                    else
                    {
                        CurrentVisibility = VisibilityState.StandBy;
                    }
                }
            }
        }
        #endregion

        internal void UpdateAnchorPosition(float deltaTime, float lerpSpeed)
        {
            // Check if this panel is ripe for positionining
            if((targetAnchorPosition.HasValue == true) && (panel.gameObject.activeInHierarchy == true))
            {
                // Check if we're close enough to the target position
                if(Vector2.Distance(CachedTransform.anchoredPosition, targetAnchorPosition.Value) < Utility.SnapToThreshold)
                {
                    // Snap to this position
                    CachedTransform.anchoredPosition = targetAnchorPosition.Value;

                    // Flag to stop animating
                    targetAnchorPosition = null;
                }
                else
                {
                    // Lerp to the target position
                    CachedTransform.anchoredPosition = Vector2.Lerp(CachedTransform.anchoredPosition, targetAnchorPosition.Value, (deltaTime * lerpSpeed));
                }
            }
        }
    }
}
