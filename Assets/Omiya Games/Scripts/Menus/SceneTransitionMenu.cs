using UnityEngine;

namespace OmiyaGames
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
    /// Taking out <code>IMenu</code> extension.
    /// Switching the Transition to just be a regular script.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class SceneTransitionMenu : MonoBehaviour
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

        public Transition CurrentTransition
        {
            get;
            private set;
        } = Transition.None;

        protected override void OnStateChanged(VisibilityState from, VisibilityState to)
        {
            // Do nothing
            if(to == VisibilityState.Visible)
            {
                // Update the current transition state
                CurrentTransition = Transition.SceneTransitionOutStart;

                // Run the animation
                Animator.SetTrigger(transitionOutTrigger);
            }
            else if(to == VisibilityState.Hidden)
            {
                // Update the current transition state
                CurrentTransition = Transition.SceneTransitionInStart;

                // Run the animation
                Animator.SetTrigger(transitionInTrigger);
            }
        }

        public void OnSceneTransitionInEnd()
        {
            // Update the current transition state
            CurrentTransition = Transition.SceneTransitionInEnd;

            // Check if there's an action associated with this dialog
            if(onStateChangedWhileManaged != null)
            {
                onStateChangedWhileManaged(this);
                onStateChangedWhileManaged = null;
            }
        }

        public void OnSceneTransitionOutEnd()
        {
            // Update the current transition state
            CurrentTransition = Transition.SceneTransitionOutEnd;

            // Check if there's an action associated with this dialog
            if(onStateChangedWhileManaged != null)
            {
                onStateChangedWhileManaged(this);
                onStateChangedWhileManaged = null;
            }
        }
    }
}
