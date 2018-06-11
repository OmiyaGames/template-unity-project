using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SnappingScrollRect.cs" company="Omiya Games">
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
    /// <date>6/10/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An extension of <code>LayoutElement</code> that uses RectTransforms to
    /// calculate preferred width and height.
    /// </summary>
    /// <seealso cref="ScrollRect"/>
    public class SuperLayoutElement : LayoutElement
    {
        [SerializeField]
        float m_MaxHeight = -1;
        [SerializeField]
        float m_MaxWidth = -1;
        [SerializeField]
        RectTransform m_MinHeightElement = null;
        [SerializeField]
        RectTransform m_MinWidthElement = null;
        [SerializeField]
        RectTransform m_PreferredHeightElement = null;
        [SerializeField]
        RectTransform m_PreferredWidthElement = null;
        [SerializeField]
        RectTransform m_MaxHeightElement = null;
        [SerializeField]
        RectTransform m_MaxWidthElement = null;

        public override float preferredHeight
        {
            get
            {
                // Choose which element to return the height of
                float returnHeight = base.preferredHeight;
                if (m_PreferredHeightElement != null)
                {
                    returnHeight = m_PreferredHeightElement.rect.height;
                }

                // Prevent the height from exceeding max
                float heightLimit = maxHeight;
                if((heightLimit > 0) && (returnHeight > heightLimit))
                {
                    returnHeight = heightLimit;
                }
                return returnHeight;
            }
        }

        public override float preferredWidth
        {
            get
            {
                // Choose which element to return the width of
                float returnWidth = base.preferredWidth;
                if (m_PreferredWidthElement != null)
                {
                    returnWidth = m_PreferredWidthElement.rect.height;
                }

                // Prevent the width from exceeding max
                float widthLimit = maxWidth;
                if ((widthLimit > 0) && (returnWidth > widthLimit))
                {
                    returnWidth = widthLimit;
                }
                return returnWidth;
            }
        }

        public override float minHeight
        {
            get
            {
                if(m_MinHeightElement != null)
                {
                    return m_MinHeightElement.rect.height;
                }
                else
                {
                    return base.minHeight;
                }
            }
        }

        public override float minWidth
        {
            get
            {
                if (m_MinWidthElement != null)
                {
                    return m_MinWidthElement.rect.width;
                }
                else
                {
                    return base.minWidth;
                }
            }
        }

        public virtual float maxHeight
        {
            get
            {
                if(m_MaxHeightElement != null)
                {
                    return m_MaxHeightElement.rect.height;
                }
                else
                {
                    return m_MaxHeight;
                }
            }
            set
            {
                m_MaxHeight = value;
            }
        }

        public virtual float maxWidth
        {
            get
            {
                if (m_MaxWidthElement != null)
                {
                    return m_MaxWidthElement.rect.height;
                }
                else
                {
                    return m_MaxWidth;
                }
            }
            set
            {
                m_MaxWidth = value;
            }
        }
    }
}
