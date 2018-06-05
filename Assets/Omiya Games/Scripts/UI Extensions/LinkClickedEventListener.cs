using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OmiyaGamesUtility.cs" company="Omiya Games">
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
    /// <date>6/4/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A series of utilities used throughout the <code>OmiyaGames</code> namespace.
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
    /// <description>6/4/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [DisallowMultipleComponent]
    public class LinkClickedEventListener : MonoBehaviour, IPointerClickHandler
    {
        [System.Serializable]
        public class LinkSelectionEvent : UnityEvent<string, string, int> { }

        [SerializeField]
        Canvas parentCanvas;
        [SerializeField]
        LinkSelectionEvent onLinkSelection = new LinkSelectionEvent();

        TextMeshProUGUI labelCache = null;

        public TextMeshProUGUI Label
        {
            get
            {
                if (labelCache == null)
                {
                    labelCache = GetComponent<TextMeshProUGUI>();
                }
                return labelCache;
            }
        }

        public Camera CanvasCamera
        {
            get
            {
                Camera camera = null;
                if ((parentCanvas != null) && (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay))
                {
                    camera = parentCanvas.worldCamera;
                }
                return camera;
            }
        }

        /// <summary>
        /// Event delegate triggered when pointer is over a link.
        /// </summary>
        public LinkSelectionEvent OnLinkSelection
        {
            get
            {
                return onLinkSelection;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if ((enabled == true) && (parentCanvas != null))
            {
                // Check to see what the intersecting link is
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(Label, Input.mousePosition, CanvasCamera);
                if (linkIndex != -1)
                {
                    // Grab the link info
                    TMP_LinkInfo linkInfo = Label.textInfo.linkInfo[linkIndex];
                    OnLinkSelection.Invoke(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkIndex);
                }
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Set Parent Canvas")]
        void SetParentCanvas()
        {
            // Grab the current game object
            Transform checkTransform = transform;

            // Check if it has a canvas
            parentCanvas = checkTransform.GetComponent<Canvas>();

            // Loop while canvas isn't set, and there is a parent to be concerned of
            while ((checkTransform != null) && (checkTransform.parent != null) && (parentCanvas == null))
            {
                // Grab the next parent
                checkTransform = checkTransform.parent;

                // Check if parent has a canvas
                parentCanvas = checkTransform.GetComponent<Canvas>();
            }
        }
#endif
    }
}
