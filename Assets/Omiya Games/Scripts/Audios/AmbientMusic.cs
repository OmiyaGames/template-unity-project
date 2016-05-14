using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AmbientMusic.cs" company="Omiya Games">
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
    /// An interface for ambient music.  More useful when used in conjunction with AudioFinder.
    /// </summary>
    /// <seealso cref="AudioSource"/>
    /// <seealso cref="BackgroundMusic"/>
    /// <seealso cref="AmbientMusic"/>
    /// <seealso cref="AudioFinder"/>
    [RequireComponent(typeof(AudioSource))]
    public class AmbientMusic : IAudio
    {
        static readonly HashSet<AmbientMusic> allAmbientMusics = new HashSet<AmbientMusic>();
        AudioSource audioCache = null;

        #region Static Properties
        public static float GlobalVolume
        {
            get
            {
                return BackgroundMusic.GlobalVolume;
            }
            set
            {
                // Set volume
                BackgroundMusic.GlobalVolume = value;
            }
        }

        public static bool GlobalMute
        {
            get
            {
                return BackgroundMusic.GlobalMute;
            }
            set
            {
                // Set mute
                BackgroundMusic.GlobalMute = value;
            }
        }

        public static float GlobalPitch
        {
            get
            {
                return BackgroundMusic.GlobalPitch;
            }
            set
            {
                BackgroundMusic.GlobalPitch = value;
            }
        }

        public static IEnumerable<AmbientMusic> AllAmbientMusics
        {
            get
            {
                return allAmbientMusics;
            }
        }
        #endregion

        public override AudioSource Audio
        {
            get
            {
                if (audioCache == null)
                {
                    audioCache = GetComponent<AudioSource>();
                }
                return audioCache;
            }
        }

        #region Unity Events
        protected override void Awake()
        {
            base.Awake();
            allAmbientMusics.Add(this);
        }

        void OnDestroy()
        {
            allAmbientMusics.Remove(this);
        }
        #endregion
    }
}
