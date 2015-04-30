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
        [SerializeField]
        string pauseInput = "Pause";
        [SerializeField]
        float delaySelectingDefaultUiBy = 0.5f;

        EventSystem eventSystemCache = null;
        WaitForSeconds delaySelection = null;
        readonly Dictionary<Type, IMenu> typeToMenuMap = new Dictionary<Type, IMenu>();
        readonly Stack<IMenu> managedMenusStack = new Stack<IMenu>();
        readonly Type pauseMenuType = typeof(PauseMenu);

        public event Action<MenuManager> OnManagedMenusStackChanged;

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
            foreach(IMenu menu in menus)
            {
                // Add the menu to the dictionary
                menuType = menu.GetType();
                if (typeToMenuMap.ContainsKey(menuType) == false)
                {
                    // Add the menu
                    typeToMenuMap.Add(menuType, menu);
                }

                // Check if this is the first displayed, managed menu
                if ((menu.MenuType == IMenu.Type.DefaultManagedMenu) && (displayedManagedMenu == null))
                {
                    // Grab this menu
                    displayedManagedMenu = menu;

                    // Indicate it should be visible
                    displayedManagedMenu.CurrentState = IMenu.State.Visible;
                }
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

        public void SelectGuiGameObject(GameObject guiElement)
        {
            StartCoroutine(DelaySelection(guiElement));
        }

        void QueryInput(float unscaledDeltaTime)
        {
            // Detect input for pause button
            if(Input.GetButtonDown(pauseInput) == true)
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
            yield return delaySelectingDefaultUiBy;
            Events.SetSelectedGameObject(guiElement);
        }
    }
}
