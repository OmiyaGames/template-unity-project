using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames
{
    [RequireComponent(typeof(EventSystem))]
    public class MenuManager : ISingletonScript
    {
        [Header("Behaviors")]
        [Tooltip("Name of input under the InputManager that is going to pause the game")]
        [SerializeField]
        string pauseInput = "Pause";
        [SerializeField]
        float delaySelectingDefaultUiBy = 0.5f;

        [Header("Menu Label Templates")]
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating to return to a scene")]
        [SerializeField]
        string returnToTextTemplate = "Return to {0}";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating to restart a scene")]
        [SerializeField]
        string restartTextTemplate = "Restart {0}";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating a scene was completed")]
        [SerializeField]
        string completeTextTemplate = "{0} Complete";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating game over")]
        [SerializeField]
        string failedTextTemplate = "{0} Failed";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating a scene was completed")]
        [SerializeField]
        string nextTextTemplate = "Proceed to {0}";

        EventSystem eventSystemCache = null;
        WaitForSeconds delaySelection = null;
        string menuTextCache = null;
        readonly Dictionary<Type, IMenu> typeToMenuMap = new Dictionary<Type, IMenu>();
        readonly Stack<IMenu> managedMenusStack = new Stack<IMenu>();

        public event Action<MenuManager> OnManagedMenusStackChanged;

        #region Properties
        public EventSystem Events
        {
            get
            {
                if(eventSystemCache == null)
                {
                    eventSystemCache = GetComponent<EventSystem>();
                }
                return eventSystemCache;
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
        public string ReturnToMenuText
        {
            get
            {
                if (menuTextCache == null)
                {
                    menuTextCache = Singleton.Get<SceneManager>().MainMenu.DisplayName;
                    if (string.IsNullOrEmpty(returnToTextTemplate) == false)
                    {
                        menuTextCache = string.Format(returnToTextTemplate, menuTextCache);
                    }
                }
                return menuTextCache;
            }
        }

        public string RestartCurrentSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo currentScene = Singleton.Get<SceneManager>().CurrentScene;
                if (currentScene != null)
                {
                    returnText = currentScene.DisplayName;
                    if (string.IsNullOrEmpty(restartTextTemplate) == false)
                    {
                        returnText = string.Format(restartTextTemplate, currentScene.DisplayName);
                    }
                }
                return returnText;
            }
        }

        public string CompletedCurrentSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo currentScene = Singleton.Get<SceneManager>().CurrentScene;
                if (currentScene != null)
                {
                    returnText = currentScene.DisplayName;
                    if (string.IsNullOrEmpty(completeTextTemplate) == false)
                    {
                        returnText = string.Format(completeTextTemplate, currentScene.DisplayName);
                    }
                }
                return returnText;
            }
        }

        public string FailedCurrentSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo currentScene = Singleton.Get<SceneManager>().CurrentScene;
                if (currentScene != null)
                {
                    returnText = currentScene.DisplayName;
                    if (string.IsNullOrEmpty(failedTextTemplate) == false)
                    {
                        returnText = string.Format(failedTextTemplate, currentScene.DisplayName);
                    }
                }
                return returnText;
            }
        }

        public string NextSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo nextScene = Singleton.Get<SceneManager>().NextScene;
                if (nextScene != null)
                {
                    returnText = nextScene.DisplayName;
                    if (string.IsNullOrEmpty(nextTextTemplate) == false)
                    {
                        returnText = string.Format(nextTextTemplate, nextScene.DisplayName);
                    }
                }
                return returnText;
            }
        }
        #endregion

        public override void SingletonAwake(Singleton instance)
        {
            instance.OnRealTimeUpdate += QueryInput;
            delaySelection = new WaitForSeconds(delaySelectingDefaultUiBy);
        }

        public override void SceneAwake(Singleton instance)
        {
            // Clear out all the menus
            typeToMenuMap.Clear();
            managedMenusStack.Clear();

            // Search for all menus in the scene
            IMenu[] menus = UnityEngine.Object.FindObjectsOfType<IMenu>();

            // Add them into the dictionary
            Type menuType;
            IMenu displayedManagedMenu = null;
            SceneTransitionMenu transitionMenu = null;
            foreach(IMenu menu in menus)
            {
                // Add the menu to the dictionary
                menuType = menu.GetType();
                if (typeToMenuMap.ContainsKey(menuType) == false)
                {
                    // Add the menu
                    typeToMenuMap.Add(menuType, menu);

                    // Check if this menu is a SceneTransitionMenu
                    if(menuType == typeof(SceneTransitionMenu))
                    {
                        transitionMenu = (SceneTransitionMenu)menu;
                    }
                }

                // Check if this is the first displayed, managed menu
                if ((menu.MenuType == IMenu.Type.DefaultManagedMenu) && (displayedManagedMenu == null))
                {
                    // Grab this menu
                    displayedManagedMenu = menu;

                    // Indicate it should be visible
                    displayedManagedMenu.Show();
                }
            }

            // Check to see if there was a transition menu
            if(transitionMenu == null)
            {
                // If not, run the scene manager's transition-in events immediately
                Singleton.Get<SceneManager>().TransitionIn(null);
            }
            else
            {
                // If so, run the transition menu's transition-in animation
                transitionMenu.Hide(Singleton.Get<SceneManager>().TransitionIn);
            }
        }

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

                    // Push the current menu onto the stack
                    managedMenusStack.Push(menu);

                    // Run the event that indicates the stack changed
                    if(OnManagedMenusStackChanged != null)
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

                // Run the event that indicates the stack changed
                if (OnManagedMenusStackChanged != null)
                {
                    OnManagedMenusStackChanged(this);
                }
            }
            return returnMenu;
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

        void QueryInput(float unscaledDeltaTime)
        {
            // Detect input for pause button (make sure no managed dialogs are shown, either).
            if((NumManagedMenus <= 0) && (Input.GetButtonDown(pauseInput) == true))
            {
                // Attempt to grab the pause menu
                PauseMenu pauseMenu = GetMenu<PauseMenu>();
                if(pauseMenu != null)
                {
                    if(pauseMenu.CurrentState == IMenu.State.Hidden)
                    {
                        pauseMenu.Show();
                    }
                    else if(pauseMenu.CurrentState == IMenu.State.Visible)
                    {
                        pauseMenu.Hide();
                    }
                }
            }
        }

        IEnumerator DelaySelection(GameObject guiElement)
        {
            yield return delaySelection;
            Events.SetSelectedGameObject(guiElement);
        }
    }
}
