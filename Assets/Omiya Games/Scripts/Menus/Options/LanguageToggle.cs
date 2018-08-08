using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OptionsLanguageMenu.cs" company="Omiya Games">
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
    /// <date>6/16/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Script to handle listening to a toggle button on a langauge.
    /// </summary>
    /// <seealso cref="OptionsLanguageMenu"/>
    [RequireComponent(typeof(Toggle))]
    public class LanguageToggle : MonoBehaviour
    {
        public event System.Action<LanguageToggle> OnChecked;

        [SerializeField]
        TMPro.TextMeshProUGUI label;
        [SerializeField]
        UiEventNavigation navigation;

        string language = null;
        Toggle cachedCheckbox = null;

        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                language = value;
                label.text = language;
            }
        }

        public Toggle Checkbox
        {
            get
            {
                if(cachedCheckbox == null)
                {
                    cachedCheckbox = GetComponent<Toggle>();
                }
                return cachedCheckbox;
            }
        }

        public UiEventNavigation Navigation
        {
            get
            {
                return navigation;
            }
        }

        public void OnToggleChanged(bool isChecked)
        {
            if(isChecked == true)
            {
                OnChecked?.Invoke(this);
            }
        }
    }
}
