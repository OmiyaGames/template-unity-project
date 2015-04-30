using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    [RequireComponent(typeof(Animator))]
    public class BackgroundMenu : IMenu
    {
        public const string VisibleField = "Visible";

        System.Action<MenuManager> onMenuNumberChanged = null;

        public override Type MenuType
        {
            get
            {
                return Type.UnmanagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return null;
            }
        }

        protected virtual void Start()
        {
            // Grab the Menu manager and update the background visibility
            MenuManager manager = Singleton.Get<MenuManager>();
            if(manager != null)
            {
                UpdateBackgroundVisibility(manager);

                // Bind to the manager event
                if (onMenuNumberChanged != null)
                {
                    manager.OnManagedMenusStackChanged -= onMenuNumberChanged;
                    onMenuNumberChanged = null;
                }
                onMenuNumberChanged = new System.Action<MenuManager>(UpdateBackgroundVisibility);
                manager.OnManagedMenusStackChanged += onMenuNumberChanged;
            }

            // Always make this the background
            transform.SetAsFirstSibling();
        }

        protected virtual void OnDestroy()
        {
            MenuManager manager = Singleton.Get<MenuManager>();
            if ((manager != null) && (onMenuNumberChanged != null))
            {
                manager.OnManagedMenusStackChanged -= onMenuNumberChanged;
                onMenuNumberChanged = null;
            }
        }

        protected override void OnStateChanged(State from, State to)
        {
            // Update the animator
            if (to == State.Visible)
            {
                Animator.SetBool(VisibleField, true);
            }
            else
            {
                Animator.SetBool(VisibleField, false);
            }
        }

        protected void UpdateBackgroundVisibility(MenuManager manager)
        {
            if (manager.NumManagedMenus > 0)
            {
                CurrentState = State.Visible;
            }
            else
            {
                CurrentState = State.Hidden;
            }
        }
    }
}