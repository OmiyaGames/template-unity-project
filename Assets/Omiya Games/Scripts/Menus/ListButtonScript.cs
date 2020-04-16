using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Translations;

namespace OmiyaGames.Menus
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LevelSelectMenu.cs" company="Omiya Games">
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
    /// <date>3/14/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A script for a list button.  Holds an index and a references to
    /// buttons and label.
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
    /// <description>3/14/2017</description>
    /// <description>Taro</description>
    /// <description>Initial version.</description>
    /// 
    /// <description>3/14/2017</description>
    /// <description>Taro</description>
    /// <description>Transitioning to using TextMeshPro.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class ListButtonScript : MonoBehaviour
    {
        public event System.Action<ListButtonScript> OnClicked;

        [SerializeField]
        TranslatedTextMeshPro[] labels = null;

        Button buttonCache = null;

        public int Index
        {
            get
            {
                return transform.GetSiblingIndex();
            }
        }

        public Button Button
        {
            get
            {
                if (buttonCache == null)
                {
                    buttonCache = GetComponent<Button>();
                }
                return buttonCache;
            }
        }

        public TranslatedTextMeshPro[] Labels
        {
            get
            {
                return labels;
            }
        }

        public void OnClick()
        {
            OnClicked?.Invoke(this);
        }


#if UNITY_EDITOR
        [ContextMenu("Set Labels")]
        void SetLabels()
        {
            labels = GetComponentsInChildren<TranslatedTextMeshPro>();
        }
#endif
    }
}
