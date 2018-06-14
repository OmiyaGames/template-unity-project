using UnityEngine;

namespace OmiyaGames.Scenes
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SceneTransitionMenu.cs" company="Omiya Games">
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// UI that animates switching into and out of a scene.
    /// </summary>
    /// <seealso cref="SceneManager"/>
    /// <seealso cref="MenuManager"/>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>5/18/2015</description>
    /// <description>Taro</description>
    /// <description>Initial verison.</description>
    /// 
    /// <description>6/13/2018</description>
    /// <description>Taro</description>
    /// <description>
    /// Extending <code>MonoBehaviour</code> to remove
    /// all the cruft from <code>IMenu</code>.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class SceneTransition : MonoBehaviour
    {
        public enum Transition
        {
            None,
            SceneTransitionInStart,
            SceneTransitionInEnd,
            SceneTransitionOutStart,
            SceneTransitionOutEnd
        }

        public delegate void TransitionChanged(Transition from, Transition to);
        public static event TransitionChanged OnBeforeTransitionChanged;
        public static event TransitionChanged OnAfterTransitionChanged;

        static Transition currentTransition = Transition.None;

        public static Transition CurrentTransition
        {
            get
            {
                return currentTransition;
            }
            private set
            {
                if (currentTransition != value)
                {
                    // Execute event before setting value
                    OnBeforeTransitionChanged(currentTransition, value);

                    // Set the value, while caching the old one
                    Transition lastValue = currentTransition;
                    currentTransition = value;

                    // Execute event after value is set
                    OnAfterTransitionChanged(lastValue, currentTransition);
                }
            }
        }

        public static SceneTransition Instance
        {
            get;
            private set;
        } = null;

        public static bool IsInMiddleOfTransitioning
        {
            get
            {
                bool returnFlag = false;
                switch (CurrentTransition)
                {
                    case Transition.SceneTransitionInStart:
                    case Transition.SceneTransitionOutStart:
                        returnFlag = true;
                        break;
                }
                return returnFlag;
            }
        }

        [SerializeField]
        string transitionInTrigger = "transitionIn";
        [SerializeField]
        string transitionOutTrigger = "transitionOut";

        Animator cachedAnimator = null;

        public Animator Animator
        {
            get
            {
                if (cachedAnimator == null)
                {
                    cachedAnimator = GetComponent<Animator>();
                }
                return cachedAnimator;
            }
        }

        void Awake()
        {
            // Make sure this is the only instance running Awake()
            if (Instance == null)
            {
                // Update the instance
                Instance = this;
                TransitionIn();
            }
            else
            {
                // Destroy the entire game object
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            // Make sure this is the script that prompted the animation to trigger
            if (Instance == this)
            {
                // Clear the instance
                Instance = null;

                // Update the current transition state
                CurrentTransition = Transition.None;
            }
        }

        internal void TransitionIn()
        {
            // Make sure we're not in the middle of transitioning animation
            if (IsInMiddleOfTransitioning == false)
            {
                // Update the current transition state
                CurrentTransition = Transition.SceneTransitionInStart;

                // Run the animation
                Animator.SetTrigger(transitionInTrigger);
            }
        }

        internal void TransitionOut()
        {
            // Make sure we're not in the middle of transitioning animation
            if (IsInMiddleOfTransitioning == false)
            {
                // Update the current transition state
                CurrentTransition = Transition.SceneTransitionOutStart;

                // Run the animation
                Animator.SetTrigger(transitionOutTrigger);
            }
        }

        #region Animation Events
        public void OnSceneTransitionInEnd()
        {
            // Update the current transition state
            CurrentTransition = Transition.SceneTransitionInEnd;
        }

        public void OnSceneTransitionOutEnd()
        {
            // Update the current transition state
            CurrentTransition = Transition.SceneTransitionOutEnd;
        }
        #endregion
    }
}
