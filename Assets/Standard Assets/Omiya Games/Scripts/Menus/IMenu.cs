using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames
{
    [RequireComponent(typeof(Animator))]
    public abstract class IMenu : MonoBehaviour
    {
        public const string StateField = "State";

        public enum State
        {
            Hidden  = 0,
            Visible = 1,
            StandBy = 2
        }

        public enum Type
        {
            /// <summary>
            /// A type of menu that will not overlap with other managed menus.
            /// Also appears visible on Awake().
            /// </summary>
            DefaultManagedMenu,
            /// <summary>
            /// A type of menu that will not overlap with other managed menus.
            /// Also is hidden on Awake().
            /// </summary>
            ManagedMenu,
            /// <summary>
            /// A type of menu that can appear on top or below other menus.
            /// </summary>
            UnmanagedMenu
        }

        State currentState = State.Hidden;
        Animator animatorCache = null;

        #region Properties
        public Animator Animator
        {
            get
            {
                if (animatorCache == null)
                {
                    animatorCache = GetComponent<Animator>();
                }
                return animatorCache;
            }
        }

        public State CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                if (currentState != value)
                {
                    // Grab the before and after state
                    State lastState = currentState;
                    currentState = value;

                    // Run the event indicating the state changed
                    OnStateChanged(lastState, currentState);
                }
            }
        }
        #endregion

        public abstract Type MenuType
        {
            get;
        }

        public abstract GameObject DefaultUi
        {
            get;
        }

        protected virtual void OnStateChanged(State from, State to)
        {
            // Update the animator
            Animator.SetInteger(StateField, (int)to);

            // Grab the menu manager
            MenuManager manager = Singleton.Get<MenuManager>();

            // Check to see if we're visible
            if((to == State.Visible) && (DefaultUi != null))
            {
                // If so, update the menu manager to select the default UI
                manager.Events.firstSelectedGameObject = DefaultUi;
            }

            // Check if this is managed
            if(MenuType != Type.UnmanagedMenu)
            {
                // Check if we're becoming visible or hidden
                if ((from == State.Hidden) && (to == State.Visible))
                {
                    // If we're going from hidden to visible, add this menu to the managed stack
                    // This will prompt the manager to push the last menu into stand-by
                    manager.PushToManagedStack(this);
                }
                else if ((from == State.Visible) && (to == State.Hidden))
                {
                    // If we're going from visible to hidden, remove this menu from the managed stack
                    // This will prompt the manager to pop the last menu into visible
                    if(manager.LastManagedMenu == this)
                    {
                        manager.PopFromManagedStack();
                    }
                }
            }
        }
    }
}
