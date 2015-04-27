using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace OmiyaGames
{
    [RequireComponent(typeof(EventSystem))]
    public class MenuManager : ISingletonScript
    {
        [Serializable]
        string pauseInput = "Pause";

        EventSystem eventSystemCache = null;
        readonly Dictionary<Type, IMenu> typeToMenuMap = new Dictionary<Type, IMenu>();

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

        public void SingletonStart(Singleton instance)
        {
            instance.OnRealTimeUpdate += QueryInput;
        }

        public void SceneStart(Singleton instance)
        {
            // Clear out all the menus
            typeToMenuMap.Clear();

            // Search for all menus in the scene
            IMenu[] menus = UnityEngine.Object.FindObjectsOfType<IMenu>();

            // Add them into the dictionary
            Type menuType;
            foreach(IMenu menu in menus)
            {
                menuType = menu.GetType();
                if (typeToMenuMap.ContainsKey(menuType) == true)
                {
                    // Overwrite the previous menu
                    typeToMenuMap[menuType] = menu;
                }
                else
                {
                    // Add the menu
                    typeToMenuMap.Add(menuType, menu);
                }
            }

            // FIXME: Manage UI elements, and select a default UI
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

        void QueryInput(float unscaledDeltaTime)
        {
            // FIXME: detect pause
        }
    }
}
