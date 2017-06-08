using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MenuManager.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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

        WaitForSeconds delaySelection = null;
        PopUpManager popUpManager = null;
        readonly Dictionary<Type, IMenu> typeToMenuMap = new Dictionary<Type, IMenu>();
        readonly Stack<IMenu> managedMenusStack = new Stack<IMenu>();

        public event Action<MenuManager> OnManagedMenusStackChanged;

        #region Properties
        public EventSystem Events
        {
            get
            {
                return Singleton.Get<EventSystem>();
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

        public PopUpManager PopUps
        {
            get
            {
                return popUpManager;
            }
        }

        public string PauseInput
        {
            get
            {
                return pauseInput;
            }
        }

        SceneTransitionManager TransitionManager
        {
            get
            {
                return Singleton.Get<SceneTransitionManager>();
            }
        }
        #endregion

        public override void SingletonAwake(Singleton instance)
        {
            // Enable events
            Events.enabled = true;

            // Bind to update
            instance.OnRealTimeUpdate += QueryInput;


            delaySelection = new WaitForSeconds(delaySelectingDefaultUiBy);
        }

        public override void SceneAwake(Singleton instance)
        {
            // Clear out all the menus
            managedMenusStack.Clear();

            // Populate typeToMenuMap dictionary
            SceneTransitionMenu transitionMenu = null;
            PopulateTypeToMenuDictionary(typeToMenuMap, out transitionMenu);

            // Attempt to find a pop-up manager
            popUpManager = FindObjectOfType<PopUpManager>();

            // Check to see if there was a transition menu
            if (transitionMenu == null)
            {
                // If not, run the scene manager's transition-in events immediately
                TransitionManager.TransitionIn(null);
            }
            else
            {
                // If so, run the transition menu's transition-in animation
                transitionMenu.Hide(TransitionManager.TransitionIn);
            }
        }

        public void SetLabelTextToReturnToMenu(TranslatedText label)
        {
            if ((label != null) && (string.IsNullOrEmpty(returnToTextTemplateTranslationKey) == false))
            {
                label.SetTranslationKey(returnToTextTemplateTranslationKey, TransitionManager.MainMenu.DisplayName);
            }
        }

        public void SetLabelTextToRestartCurrentScene(TranslatedText label)
        {
            SetLabelTextTo(label, restartTextTemplateTranslationKey);
        }

        public void SetLabelTextToCompletedCurrentScene(TranslatedText label)
        {
            SetLabelTextTo(label, completeTextTemplateTranslationKey);
        }

        public void SetLabelTextToFailedCurrentScene(TranslatedText label)
        {
            SetLabelTextTo(label, failedTextTemplateTranslationKey);
        }

        public void SetLabelTextToNextScene(TranslatedText label)
        {
            SetLabelTextTo(label, nextTextTemplateTranslationKey, TransitionManager.NextScene);
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

        public MENU Show<MENU>(Action<IMenu> action = null) where MENU : IMenu
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
                        managedMenusStack.Peek().CurrentState = IMenu.State.StandBy;
                    }
                    else
                    {
                        // Unlock the cursor
                        SceneTransitionManager.CursorMode = CursorLockMode.None;
                    }

                    // Push the current menu onto the stack
                    managedMenusStack.Push(menu);

                    // Run the event that indicates the stack changed
                    if (OnManagedMenusStackChanged != null)
                    {
                        OnManagedMenusStackChanged(this);
                    }
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
                    managedMenusStack.Peek().CurrentState = IMenu.State.Visible;
                }
                else if (TransitionManager.CurrentScene != null)
                {
                    // Lock the cursor to what the scene is set to
                    SceneTransitionManager.CursorMode = TransitionManager.CurrentScene.LockMode;
                }

                // Run the event that indicates the stack changed
                if (OnManagedMenusStackChanged != null)
                {
                    OnManagedMenusStackChanged(this);
                }
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
        void SetLabelTextTo(TranslatedText label, string templateKey)
        {
            if ((label != null) && (string.IsNullOrEmpty(templateKey) == false))
            {
                SetLabelTextTo(label, templateKey, TransitionManager.CurrentScene);
            }
        }

        void SetLabelTextTo(TranslatedText label, string templateKey, SceneInfo scene)
        {
            if ((label != null) && (string.IsNullOrEmpty(templateKey) == false) && (scene != null))
            {
                label.SetTranslationKey(templateKey, scene.DisplayName);
            }
        }

        void PopulateTypeToMenuDictionary(Dictionary<Type, IMenu> typeToMenuDictionary, out SceneTransitionMenu transitionMenu)
        {
            // Setup variables
            int index = 0;
            transitionMenu = null;
            typeToMenuDictionary.Clear();

            // Populate items to ignore into the type map
            for (; index < IgnoreTypes.Length; ++index)
            {
                typeToMenuDictionary.Add(IgnoreTypes[index], null);
            }

            // Search for all menus in the scene
            IMenu[] menus = UnityEngine.Object.FindObjectsOfType<IMenu>();
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

                            // Check if this menu is a SceneTransitionMenu
                            if (menuType == typeof(SceneTransitionMenu))
                            {
                                transitionMenu = (SceneTransitionMenu)menus[index];
                            }
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
            if((NumManagedMenus <= 0) && (Input.GetButtonDown(PauseInput) == true))
            {
                // Attempt to grab the pause menu
                if (PauseMenu != null)
                {
                    if(PauseMenu.CurrentState == IMenu.State.Hidden)
                    {
                        PauseMenu.Show();

                        // Indicate button is clicked
                        ButtonClick.Play();
                    }
                    else if(PauseMenu.CurrentState == IMenu.State.Visible)
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
            Events.SetSelectedGameObject(guiElement);
        }
        #endregion
    }
}
