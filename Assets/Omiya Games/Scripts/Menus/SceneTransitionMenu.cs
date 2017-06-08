using UnityEngine;
using System.Collections;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SceneTransitionMenu.cs" company="Omiya Games">
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
    /// <date>5/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Menu that animates switching into and out of a scene.  Use the singleton script,
    /// <code>SceneManager</code>, to animate this menu
    /// </summary>
    /// <seealso cref="SceneManager"/>
    /// <seealso cref="MenuManager"/>
    [RequireComponent(typeof(Animator))]
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
