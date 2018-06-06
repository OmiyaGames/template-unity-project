using UnityEngine;
using UnityEngine.EventSystems;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ButtonAudio.cs" company="Omiya Games">
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
    /// <date>6/6/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that retains information about which scene to switch to.
    /// Where possible, it'll animate the <code>SceneTransitionMenu</code> before
    /// switching scenes.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>6/6/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    public class ButtonAudio : MonoBehaviour
    {
        static SoundEffect lastHoverSound = null;
        static SoundEffect lastClickSound = null;

        [SerializeField]
        SoundEffect hoverSound = null;
        [SerializeField]
        SoundEffect clickSound = null;

        public bool IsHighlighted
        {
            get;
            private set;
        } = false;

        EventSystem Highlighter
        {
            get
            {
                return Singleton.Get<EventSystem>();
            }
        }

        void OnDisable()
        {
            OnUnfocused();
        }

        public void OnHoverPlaySound()
        {
            // Check fo see if we can play hover sound
            if(IsHighlighted == false)
            {
                // Play the hover sound
                PlaySoundEffect(hoverSound, ref lastHoverSound);

                // Prevent this button from playing the hover sound
                IsHighlighted = true;

                // Force event system to recognize this button is highlighted
                if((Highlighter != null) && (Highlighter.currentSelectedGameObject != gameObject))
                {
                    Highlighter.SetSelectedGameObject(gameObject);
                }
            }
        }

        public void OnClickPlaySound()
        {
            // Play clicking sound
            PlaySoundEffect(clickSound, ref lastClickSound);
        }

        public void OnUnfocused()
        {
            // Reset hover sound
            IsHighlighted = false;
        }

        static void PlaySoundEffect(SoundEffect newSound, ref SoundEffect oldSound)
        {
            // Make sure the sound effect is there
            if (newSound != null)
            {
                // Check to see if there is a sound effect still playing
                if (oldSound != null)
                {
                    // Stop that sound effect
                    oldSound.Stop();
                }

                // Play the new sound
                newSound.Play();

                // Set the old sound variable to the new one
                oldSound = newSound;
            }
        }
    }
}
