using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace OmiyaGames.Translations
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
    /// <date>4/2/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A drop-down for languages
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
    /// <description>4/2/2017</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    // FIXME: Replace Dropdown with Text Mesh Pro equivalent
    [DisallowMultipleComponent]
    public class LanguageDropDown : Dropdown
    {
        readonly Dictionary<DropdownItem, Text> allDropDownItems = new  Dictionary<DropdownItem, Text>();
        bool isItemsSetup = false;

        public bool IsSetup
        {
            get;
            private set;
        } = false;

        public void Setup()
        {
            if(IsSetup == false)
            {
                // Remove all language options
                ClearOptions();

                // Setting up all the language options
                TranslationManager manager = Singleton.Get<TranslationManager>();
                AddOptions(manager.SupportedLanguages);
                value = manager.SupportedLanguages.IndexOf(manager.CurrentLanguage);

                // Adding listener to value changing
                onValueChanged.AddListener(UpdateCaptionFont);

                // Indicate we're setup
                IsSetup = true;
            }
        }

        void UpdateDropDownItems()
        {
            if(isItemsSetup == false)
            {
                // Update each item's fonts
                Dictionary<string, TranslationManager.FontMap> fontMap = Singleton.Get<TranslationManager>().FontDictionary;
                foreach(Text label in allDropDownItems.Values)
                {
                    // Setup the font
                    if(fontMap.ContainsKey(label.text) == true)
                    {
                        label.font = fontMap[label.text].DefaultFont;
                    }
                }

                // Indicate items are setup
                isItemsSetup = true;
            }
        }

        protected override DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            // Generate a DropdownItem
            DropdownItem itemToReplace = base.CreateItem(itemTemplate);
            allDropDownItems.Add(itemToReplace, itemToReplace.text);

            // Indicate items needs to change
            isItemsSetup = false;
            return itemToReplace;
        }

        protected override void DestroyItem(DropdownItem item)
        {
            base.DestroyItem(item);
            if(allDropDownItems.ContainsKey(item) == true)
            {
                allDropDownItems.Remove(item);
            }

            // Indicate items needs to change
            isItemsSetup = false;
        }

        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            base.DestroyDropdownList(dropdownList);
            allDropDownItems.Clear();

            // Indicate items needs to change
            isItemsSetup = false;
        }

        protected override GameObject CreateDropdownList(GameObject dropdownList)
        {
            // Indicate items needs to change
            isItemsSetup = false;
            return base.CreateDropdownList(dropdownList);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            UpdateDropDownItems();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            UpdateDropDownItems();
        }

        void UpdateCaptionFont(int newValue)
        {
            // Update each item's fonts
            Dictionary<string, TranslationManager.FontMap> fontMap = Singleton.Get<TranslationManager>().FontDictionary;

            // Setup the font
            if(fontMap.ContainsKey(options[value].text) == true)
            {
                captionText.font = fontMap[options[value].text].DefaultFont;
            }
        }
    }
}
