using UnityEngine;
using System.Text;
using System.Collections.Generic;
using OmiyaGames.Translations;

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
    /// <date>6/11/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that provides language options.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class OptionsLanguageMenu : IOptionsMenu
    {
        const string appendName = " Checkbox";

        [Header("Language Controls")]
        [SerializeField]
        LanguageToggle languageCheckbox;

        LanguageToggle currentSelectedCheckbox = null;
        readonly Dictionary<string, LanguageToggle> languageToControlMap = new Dictionary<string, LanguageToggle>();

        #region Properties
        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                GameObject returnControl = null;
                LanguageToggle toggle;
                if ((languageToControlMap.TryGetValue(Translations.CurrentLanguage, out toggle) == true) && (toggle != null) && (toggle.Checkbox != null))
                {
                    returnControl = toggle.Checkbox.gameObject;
                }
                return returnControl;
            }
        }

        public override BackgroundMenu.BackgroundType Background
        {
            get
            {
                return BackgroundMenu.BackgroundType.SolidColor;
            }
        }

        TranslationManager Translations
        {
            get
            {
                return Singleton.Get<TranslationManager>();
            }
        }
        #endregion

        protected override void OnSetup()
        {
            // Call base method
            base.OnSetup();

            // Make sure the arguments are set correctly
            if (languageCheckbox != null)
            {
                GenerateLanguageCheckboxes();
            }
        }

        private void LanguageCheckbox_OnChecked(LanguageToggle obj)
        {
            if((obj != null) && (IsListeningToEvents == true))
            {
                // Update the toggle to select
                currentSelectedCheckbox = obj;

                // Change the language
                Translations.CurrentLanguage = currentSelectedCheckbox.Language;
            }
        }

        private void GenerateLanguageCheckboxes()
        {
            // Setup the first button
            StringBuilder nameBuilder = new StringBuilder();
            SetupToggle(languageCheckbox, Translations.SupportedLanguages[0], nameBuilder);

            // Setup the rest of the buttons
            LanguageToggle clonedToggle;
            for (int index = 1; index < Translations.SupportedLanguages.Count; ++index)
            {
                // Duplicate the toggle
                clonedToggle = DuplicateToggle(languageCheckbox);

                // Setup the toggle
                SetupToggle(clonedToggle, Translations.SupportedLanguages[index], nameBuilder);
            }

            // Setup the currently selected toggle
            if(languageToControlMap.TryGetValue(Translations.CurrentLanguage, out currentSelectedCheckbox) == true)
            {
                // Setup the last selected toggle
                currentSelectedCheckbox.Checkbox.isOn = true;
            }
        }

        private LanguageToggle DuplicateToggle(LanguageToggle toggleToDuplicate)
        {
            // Clone the object
            GameObject clonedObject = Instantiate<GameObject>(toggleToDuplicate.gameObject);

            // Setup the transform
            clonedObject.transform.SetParent(toggleToDuplicate.transform.parent);
            clonedObject.transform.localRotation = Quaternion.identity;
            clonedObject.transform.localScale = Vector3.one;
            clonedObject.transform.SetAsLastSibling();

            // Setup the component
            return clonedObject.GetComponent<LanguageToggle>();
        }

        private void SetupToggle(LanguageToggle clonedToggle, string languageName, StringBuilder nameBuilder)
        {
            // Setup the toggle
            clonedToggle.Language = languageName;
            clonedToggle.OnChecked += LanguageCheckbox_OnChecked;

            // Changing the name of the toggles
            nameBuilder.Clear();
            nameBuilder.Append(clonedToggle.Language);
            nameBuilder.Append(appendName);
            clonedToggle.name = nameBuilder.ToString();

            // Add the toggle to the dictionary
            languageToControlMap.Add(languageName, clonedToggle);
        }
    }
}
