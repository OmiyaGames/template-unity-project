using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SoundEffect))]
    public class SceneTransitionMenu : IMenu
    {
        public enum Transition
        {
            None,
            SceneTransitionInStart,
            SceneTransitionInEnd,
            SceneTransitionOutStart,
            SceneTransitionOutEnd
        }

        [SerializeField]
        string transitionInTrigger = "transitionIn";
        [SerializeField]
        string transitionOutTrigger = "transitionOut";

        Transition currentTransition = Transition.None;
        SoundEffect audioCache = null;

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

        public Transition CurrentTransition
        {
            get
            {
                return currentTransition;
            }
            protected set
            {
                currentTransition = value;
            }
        }

        public SoundEffect Sound
        {
            get
            {
                if (audioCache == null)
                {
                    audioCache = GetComponent<SoundEffect>();
                }
                return audioCache;
            }
        }

        protected override void OnStateChanged(State from, State to)
        {
            // Do nothing
        }

        public override void Show(System.Action<IMenu> stateChanged)
        {
            // Run show as normal
            base.Show(stateChanged);

            // Update the current transition state
            CurrentTransition = Transition.SceneTransitionOutStart;

            // Run the animation
            Animator.SetTrigger(transitionOutTrigger);

            // Play the sound effect
            Sound.Play();

            // Check if there's an action associated with this dialog
            if(onStateChanged != null)
            {
                onStateChanged(this);
            }
        }

        public override void Hide()
        {
            this.Hide(null);
        }

        public virtual void Hide(System.Action<IMenu> stateChanged)
        {
            // Set the next action
            onStateChanged = stateChanged;

            // Run hide as normal
            base.Hide();

            // Update the current transition state
            CurrentTransition = Transition.SceneTransitionInStart;

            // Run the animation
            Animator.SetTrigger(transitionInTrigger);
           
            // Check if there's an action associated with this dialog
            if(onStateChanged != null)
            {
                onStateChanged(this);
            }
        }

        public void OnSceneTransitionInEnd()
        {
            // Update the current transition state
            CurrentTransition = Transition.SceneTransitionInEnd;

            // Check if there's an action associated with this dialog
            if(onStateChanged != null)
            {
                onStateChanged(this);
                onStateChanged = null;
            }
        }

        public void OnSceneTransitionOutEnd()
        {
            // Update the current transition state
            CurrentTransition = Transition.SceneTransitionOutEnd;

            // Check if there's an action associated with this dialog
            if(onStateChanged != null)
            {
                onStateChanged(this);
                onStateChanged = null;
            }
        }
    }
}
