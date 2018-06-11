using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ButtonAudio.cs" company="Omiya Games">
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
    /// <date>6/6/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A script to handle audio for various UI events.
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
    /// <description>6/6/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    public class ButtonAudio : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [Header("Hover Settings")]
        [SerializeField]
        bool enableHoverSound = true;
        [SerializeField]
        [Tooltip("Customizes the hover sound for the UI; if set to none, the default hover sound set in MenuManager plays.")]
        SoundEffect customHoverSound = null;

        [Header("Click Settings")]
        [SerializeField]
        bool enableClickSound = true;
        [SerializeField]
        [Tooltip("Customizes the click sound for the UI; if set to none, the default click sound set in MenuManager plays.")]
        SoundEffect customClickSound = null;

        [Header("Scroll Settings")]
        [SerializeField]
        ScrollRect parentScrollView = null;

        #region Properties
        public bool IsHighlighted
        {
            get;
            private set;
        } = false;

        public bool EnableHoverSound
        {
            get
            {
                return enableHoverSound;
            }
            set
            {
                enableHoverSound = value;
            }
        }

        public SoundEffect CustomHoverSound
        {
            get
            {
                return customHoverSound;
            }
            set
            {
                customHoverSound = value;
            }
        }

        public bool EnableClickSound
        {
            get
            {
                return enableClickSound;
            }
            set
            {
                enableClickSound = value;
            }
        }

        public SoundEffect CustomClickSound
        {
            get
            {
                return customClickSound;
            }
            set
            {
                customClickSound = value;
            }
        }

        public ScrollRect ParentScrollView
        {
            get
            {
                return parentScrollView;
            }
            set
            {
                parentScrollView = value;
            }
        }

        EventSystem Highlighter
        {
            get
            {
                return Singleton.Get<EventSystem>();
            }
        }

        MenuManager Manager
        {
            get
            {
                return Singleton.Get<MenuManager>();
            }
        }
        #endregion

        #region Unity Events
        void OnDisable()
        {
            Reset();
        }
        #endregion

        public void PlayClickSound()
        {
            if (EnableClickSound == true)
            {
                if (CustomClickSound != null)
                {
                    CustomClickSound.Play();
                }
                else
                {
                    Manager.ButtonClick.Play();
                }
            }
        }

        public void PlayHoverSound()
        {
            if (EnableHoverSound == true)
            {
                if(CustomHoverSound != null)
                {
                    CustomHoverSound.Play();
                }
                else
                {
                    Manager.ButtonHover.Play();
                }
            }
        }

        public void Reset()
        {
            // Reset hover sound
            IsHighlighted = false;
        }

        #region Interface Events

        #region Pointer Events
        public void OnPointerDown(PointerEventData eventData)
        {
            // Play clicking sound
            PlayClickSound();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Play hover
            OnHoverPlaySound();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Reset state of UI
            Reset();
        }
        #endregion

        #region Select Events
        public void OnSelect(BaseEventData eventData)
        {
            // Play hover
            OnHoverPlaySound();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            // Reset state of UI
            Reset();
        }
        #endregion

        public void OnSubmit(BaseEventData eventData)
        {
            // Play clicking sound
            PlayClickSound();
        }
        #endregion

        #region Helper Methods
        void OnHoverPlaySound()
        {
            // Check fo see if we can play hover sound
            if (IsHighlighted == false)
            {
                // Play the hover sound
                PlayHoverSound();

                // Prevent this button from playing the hover sound
                IsHighlighted = true;

                // Force event system to recognize this button is highlighted
                if ((Highlighter != null) && (Highlighter.currentSelectedGameObject != gameObject))
                {
                    Highlighter.SetSelectedGameObject(gameObject);
                }
            }
        }
        #endregion
    }
}
