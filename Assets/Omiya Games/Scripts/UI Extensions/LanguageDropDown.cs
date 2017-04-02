using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace OmiyaGames
{
    public class LanguageDropDown : Dropdown
    {
        readonly Dictionary<DropdownItem, Text> allDropDownItems = new  Dictionary<DropdownItem, Text>();
        bool isSetup = false, isItemsSetup = false;

        public bool IsSetup
        {
            get
            {
                return isSetup;
            }
        }
        
        public void Setup()
        {
            if(isSetup == false)
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
                isSetup = true;
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
