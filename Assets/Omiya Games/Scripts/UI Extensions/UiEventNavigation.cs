using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Community.UI;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="UiEventNavigation.cs" company="Omiya Games">
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
    /// A script to handle navigation on select.
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
    [RequireComponent(typeof(Selectable))]
    public class UiEventNavigation : MonoBehaviour, ISelectHandler
    {
        public enum Direction
        {
            Up =    1 << 0,
            Down =  1 << 1,
            Left =  1 << 2,
            Right = 1 << 3,
        }

        public event System.Action<UiEventNavigation, BaseEventData> OnAfterSelect;
        public event System.Action<UiEventNavigation, bool> OnAfterEnabledAndActiveChanged;

        [SerializeField]
        [EnumFlags]
        Direction toPreviousUi = Direction.Up;
        [SerializeField]
        [EnumFlags]
        Direction toNextUi = Direction.Down;

        Selectable selectable = null;
        RectTransform rectTransform = null;

        public RectTransform RectTransform
        {
            get
            {
                if(rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }
                return rectTransform;
            }
        }

        public Selectable Selectable
        {
            get
            {
                if(selectable == null)
                {
                    selectable = GetComponentInChildren<Selectable>(true);
                }
                return selectable;
            }
        }

        public Direction ToPreviousUi
        {
            get
            {
                return toPreviousUi;
            }

            set
            {
                toPreviousUi = value;
            }
        }

        public Direction ToNextUi
        {
            get
            {
                return toNextUi;
            }

            set
            {
                toNextUi = value;
            }
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            OnAfterSelect?.Invoke(this, eventData);
        }

        public virtual void OnEnable()
        {
            OnAfterEnabledAndActiveChanged?.Invoke(this, true);
        }

        public virtual void OnDisable()
        {
            OnAfterEnabledAndActiveChanged?.Invoke(this, false);
        }
    }
}
