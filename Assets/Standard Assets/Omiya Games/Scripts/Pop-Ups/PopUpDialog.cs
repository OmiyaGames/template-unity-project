using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PopUpDialog.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
    public class PopUpDialog : MonoBehaviour
    {
        [SerializeField]
        Text label;
        [SerializeField]
        RectTransform panel;

        RectTransform cacheTransform = null;

        public Text Label
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
                if(gameObject.activeInHierarchy == true)
                {
                    // TODO: check to see if this works
                    returnHeight = panel.sizeDelta.y * panel.localScale.y;
                }
                return returnHeight;
            }
        }
    }
}
