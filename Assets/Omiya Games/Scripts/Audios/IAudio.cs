using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IAudio.cs" company="Omiya Games">
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
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An abstract class that implements methods shared in <code>BackgroundMusic</code>,
    /// <code>AmbientMusic</code>, and <code>SoundEffect</code>.
    /// </summary>
    /// <seealso cref="AudioSource"/>
    /// <seealso cref="BackgroundMusic"/>
    /// <seealso cref="AmbientMusic"/>
    /// <seealso cref="SoundEffect"/>
    public abstract class IAudio : MonoBehaviour
    {
        public enum State
        {
            Stopped,
            Playing,
            Paused
        }

        State currentState = State.Stopped;

        public abstract AudioSource Audio
        {
            get;
        }

        public State CurrentState
        {
            get
            {
                UpdateStateToAudioIsPlaying(ref currentState);
                return currentState;
            }
            set
            {
                UpdateStateToAudioIsPlaying(ref currentState);
                if (ChangeAudioSourceState(currentState, value) == true)
                {
                    currentState = value;
                }
            }
        }

        public void Play()
        {
            CurrentState = State.Playing;
        }

        protected virtual void Awake()
        {
            if(Audio.playOnAwake == true)
            {
                currentState = State.Playing;
            }
        }

        #region Helper Methods
        protected virtual void UpdateStateToAudioIsPlaying(ref State state)
        {
            if ((state == State.Playing) && (Audio.isPlaying == false))
            {
                state = State.Stopped;
            }
            else if ((state == State.Stopped) && (Audio.isPlaying == true))
            {
                state = State.Playing;
            }
        }

        protected virtual bool ChangeAudioSourceState(State before, State after)
        {
            // Make sure the before and after are two different states
            bool returnFlag = false;
            if (before != after)
            {
                // Check the state we're switching into
                switch (after)
                {
                    case State.Playing:
                        if (before == State.Stopped)
                        {
                            // Play the audio if we're switching from stopped to playing
                            Audio.Play();
                        }
                        else
                        {
                            // Unpause the audio if we're switching from paused to play
                            Audio.UnPause();
                        }
                        returnFlag = true;
                        break;
                    case State.Paused:
                        if (before == State.Playing)
                        {
                            // Pause the audio if we're switching from playing to paused
                            Audio.Pause();
                            returnFlag = true;
                        }
                        break;
                    default:
                        // Check if we're paused
                        if (before == State.Paused)
                        {
                            // Unpause the audio
                            Audio.UnPause();
                        }

                        // Stop the audio
                        Audio.Stop();
                        returnFlag = true;
                        break;
                }
            }
            return returnFlag;
        }
        #endregion
    }
}
