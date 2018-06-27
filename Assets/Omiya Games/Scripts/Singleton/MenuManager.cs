using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using OmiyaGames.Audio;
using OmiyaGames.Global;
using OmiyaGames.Translations;
using OmiyaGames.Scenes;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MenuManager.cs" company="Omiya Games">
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
    /// <date>8/21/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that retrieves all <code>IMenu</code>s in the scene.
    /// </summary>
    /// <seealso cref="IMenu"/>
    /// <seealso cref="Singleton"/>
    [RequireComponent(typeof(EventSystem))]
    public class MenuManager : ISingletonScript
    {
        static readonly Type[] IgnoreTypes = new Type[]
        {
            typeof(PopUpDialog)
        };

        [Header("Behaviors")]
        [Tooltip("Name of input under the InputManager that is going to pause the game")]
        [SerializeField]
        string pauseInput = "Pause";
        [SerializeField]
        float delaySelectingDefaultUiBy = 0.5f;

        [Header("Menu Label Templates")]
        [Tooltip("Template for any menus with button text indicating to return to a scene")]
        [SerializeField]
        string returnToTextTemplateTranslationKey = "Return To Menu Button";
        [Tooltip("Template for any menus with button text indicating to restart a scene")]
        [SerializeField]
        string restartTextTemplateTranslationKey = "Restart Button";
        [Tooltip("Template for any menus with button text indicating a scene was completed")]
        [SerializeField]
        string completeTextTemplateTranslationKey = "Level Complete Title";
        [Tooltip("Template for any menus with button text indicating game over")]
        [SerializeField]
        string failedTextTemplateTranslationKey = "Level Failed Title";
        [Tooltip("Template for any menus with button text indicating a scene was completed")]
        [SerializeField]
        string nextTextTemplateTranslationKey = "Proceed Button";

        [Header("Sound Templates")]
        [SerializeField]
        SoundEffect buttonClickSound = null;
        [SerializeField]
        SoundEffect buttonHoverSound = null;

        WaitForSeconds delaySelection = null;
        readonly Dictionary<Type, IMenu> typeToMenuMap = new Dictionary<Type, IMenu>();
        readonly Stack<IMenu> managedMenusStack = new Stack<IMenu>();

        public event Action<MenuManager> OnManagedMenusStackChanged;

        #region Properties
        public PopUpManager PopUps
        {
            get;
            private set;
        } = null;

        public EventSystem Events
        {
            get
            {
                return Singleton.Get<EventSystem>();
            }
        }

        // TODO: look into whether just grabbing BaseInputModule works
        public StandaloneInputModule InputModule
        {
            get
            {
                return Singleton.Get<StandaloneInputModule>();
            }
        }

        public PauseMenu PauseMenu
        {
            get
            {
                return GetMenu<PauseMenu>();
            }
        }

        public SoundEffect ButtonClick
        {
            get
            {
                return buttonClickSound;
            }
        }

        public SoundEffect ButtonHover
        {
            get
            {
                return buttonHoverSound;
            }
        }

        public IMenu LastManagedMenu
        {
            get
            {
                IMenu returnMenu = null;
                if (NumManagedMenus > 0)
                {
                    returnMenu = managedMenusStack.Peek();
                }
                return returnMenu;
            }
        }

        public int NumManagedMenus
        {
            get
            {
                return managedMenusStack.Count;
            }
        }

        public string PauseInput
        {
            get
            {
                return pauseInput;
            }
        }

        SceneTransitionManager SceneChanger
        {
            get
            {
                return Singleton.Get<SceneTransitionManager>();
            }
        }

        public bool IsPausingEnabled
        {
            get
            {
                bool returnFlag = true;
                if ((managedMenusStack.Count > 0) && (managedMenusStack.Peek() != null))
                {
                    returnFlag = managedMenusStack.Peek().IsPausingEnabledWhileVisible;
                }
                return returnFlag;
            }
        }
        #endregion

        internal override void SingletonAwake()
        {
            // Enable events
            Events.enabled = true;

            // Bind to update
            Singleton.Instance.OnRealTimeUpdate += QueryInput;

            // Setup selection
            delaySelection = new WaitForSeconds(delaySelectingDefaultUiBy);
        }

        internal override void SceneAwake()
        {
            // Clear out all the menus
            managedMenusStack.Clear();

            // Populate typeToMenuMap dictionary
            PopulateTypeToMenuDictionary(typeToMenuMap);

            // Attempt to find a pop-up manager
            PopUps = FindObjectOfType<PopUpManager>();
        }

        // FIXME: think real hard here, do we *really* need these?
        public void SetLabelTextToReturnToMenu(TranslatedText label)
        {
            if ((label != null) && (string.IsNullOrEmpty(returnToTextTemplateTranslationKey) == false))
            {
                label.SetTranslationKey(returnToTextTemplateTranslationKey, SceneChanger.MainMenu.DisplayName);
            }
        }

        // FIXME: think real hard here, do we *really* need these?
        public void SetLabelTextToRestartCurrentScene(TranslatedText label)
        {
            SetLabelTextTo(label, restartTextTemplateTranslationKey);
        }

        // FIXME: think real hard here, do we *really* need these?
        public void SetLabelTextToCompletedCurrentScene(TranslatedText label)
        {
            SetLabelTextTo(label, completeTextTemplateTranslationKey);
        }

        // FIXME: think real hard here, do we *really* need these?
        public void SetLabelTextToFailedCurrentScene(TranslatedText label)
        {
            SetLabelTextTo(label, failedTextTemplateTranslationKey);
        }

        // FIXME: think real hard here, do we *really* need these?
        public void SetLabelTextToNextScene(TranslatedText label)
        {
            SetLabelTextTo(label, nextTextTemplateTranslationKey, SceneChanger.NextScene);
        }

        public MENU GetMenu<MENU>() where MENU : IMenu
        {
            IMenu returnMenu = null;
            if (typeToMenuMap.TryGetValue(typeof(MENU), out returnMenu) == false)
            {
                returnMenu = null;
            }
            return returnMenu as MENU;
        }

        public MENU Show<MENU>(IMenu.VisibilityChanged action = null) where MENU : IMenu
        {
            MENU returnMenu = GetMenu<MENU>();
            if (returnMenu != null)
            {
                returnMenu.Show(action);
            }
            return returnMenu;
        }

        public MENU Hide<MENU>() where MENU : IMenu
        {
            MENU returnMenu = GetMenu<MENU>();
            if (returnMenu != null)
            {
                returnMenu.Hide();
            }
            return returnMenu;
        }

        public void SelectGuiGameObject(GameObject guiElement)
        {
            StartCoroutine(DelaySelection(guiElement));
        }

        #region Internal Methods
        /// <summary>
        /// Pushes a visible menu into the stack, and
        /// changes the other menus already in the stack to stand-by
        /// </summary>
        internal void PushToManagedStack(IMenu menu)
        {
            if (menu != null)
            {
                // Make sure the menu isn't already in the stack
                // (the stack is usually small, so this should be pretty efficient)
                if (managedMenusStack.Contains(menu) == false)
                {
                    // Change the top-most menu (if any) to stand-by
                    if (NumManagedMenus > 0)
                    {
                        managedMenusStack.Peek().CurrentVisibility = IMenu.VisibilityState.StandBy;
                    }
                    else
                    {
                        // Unlock the cursor
                        SceneTransitionManager.CursorMode = CursorLockMode.None;
                    }

                    // Push the current menu onto the stack
                    managedMenusStack.Push(menu);

                    // Unselect the highlighted item
                    Events.SetSelectedGameObject(null);

                    // Run the event that indicates the stack changed
                    OnManagedMenusStackChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Pops a hidden menu out of the stack, and
        /// changes the last menu already in the stack to visible
        /// </summary>
        internal IMenu PopFromManagedStack()
        {
            // Make sure this menu is already on top of the stack
            IMenu returnMenu = null;
            if (NumManagedMenus > 0)
            {
                // If so, pop the menu
                returnMenu = managedMenusStack.Pop();

                // Check if there are any other menus left
                if (NumManagedMenus > 0)
                {
                    // Change the top-most menu into visible
                    managedMenusStack.Peek().CurrentVisibility = IMenu.VisibilityState.Visible;
                }
                else
                {
                    // Lock the cursor to what the scene is set to
                    SceneChanger.RevertCursorLockMode();
                }

                // Unselect the highlighted item
                Events.SetSelectedGameObject(null);

                // Run the event that indicates the stack changed
                OnManagedMenusStackChanged?.Invoke(this);
            }
            return returnMenu;
        }

        /// <summary>
        /// Pops a hidden menu out of the stack, and
        /// changes the last menu already in the stack to visible
        /// </summary>
        internal IMenu PeekFromManagedStack()
        {
            // Make sure this menu is already on top of the stack
            IMenu returnMenu = null;
            if (NumManagedMenus > 0)
            {
                // If so, peek the stack
                returnMenu = managedMenusStack.Peek();
            }
            return returnMenu;
        }
        #endregion

        #region Helper Methods
        // FIXME: Consider making static public
        void SetLabelTextTo(TranslatedText label, string templateKey)
        {
            if ((label != null) && (string.IsNullOrEmpty(templateKey) == false))
            {
                SetLabelTextTo(label, templateKey, SceneChanger.CurrentScene);
            }
        }

        // FIXME: Consider making static public
        void SetLabelTextTo(TranslatedText label, string templateKey, SceneInfo scene)
        {
            if ((label != null) && (string.IsNullOrEmpty(templateKey) == false) && (scene != null))
            {
                label.SetTranslationKey(templateKey, scene.DisplayName);
            }
        }

        void PopulateTypeToMenuDictionary(Dictionary<Type, IMenu> typeToMenuDictionary)
        {
            // Setup variables
            int index = 0;
            typeToMenuDictionary.Clear();

            // Populate items to ignore into the type map
            for (; index < IgnoreTypes.Length; ++index)
            {
                typeToMenuDictionary.Add(IgnoreTypes[index], null);
            }

            // Search for all menus in the scene
            IMenu[] menus = FindObjectsOfType<IMenu>();
            if (menus != null)
            {
                // Add them into the dictionary
                Type menuType;
                IMenu displayedManagedMenu = null;
                for (index = 0; index < menus.Length; ++index)
                {
                    if (menus[index] != null)
                    {
                        // Add the menu to the dictionary
                        menuType = menus[index].GetType();
                        if (typeToMenuDictionary.ContainsKey(menuType) == false)
                        {
                            // Add the menu
                            typeToMenuDictionary.Add(menuType, menus[index]);
                        }

                        // Check if this is the first displayed, managed menu
                        if ((menus[index].MenuType == IMenu.Type.DefaultManagedMenu) && (displayedManagedMenu == null))
                        {
                            // Grab this menu
                            displayedManagedMenu = menus[index];

                            // Indicate it should be visible
                            displayedManagedMenu.Show();
                        }
                    }
                }
            }
        }

        void QueryInput(float unscaledDeltaTime)
        {
            // Detect input for pause button (make sure no managed dialogs are shown, either).
            if((Input.GetButtonDown(PauseInput) == true) && (IsPausingEnabled == true))
            {
                // Attempt to grab the pause menu
                if (PauseMenu != null)
                {
                    if(PauseMenu.CurrentVisibility == IMenu.VisibilityState.Hidden)
                    {
                        PauseMenu.Show();

                        // Indicate button is clicked
                        ButtonClick.Play();
                    }
                    else if(PauseMenu.CurrentVisibility == IMenu.VisibilityState.Visible)
                    {
                        PauseMenu.Hide();

                        // Indicate button is clicked
                        ButtonClick.Play();
                    }
                }
            }
        }

        IEnumerator DelaySelection(GameObject guiElement)
        {
            yield return delaySelection;

            if(Events.currentSelectedGameObject == null)
            {
                Events.SetSelectedGameObject(guiElement);
            }
        }
        #endregion
    }
}
