#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AudioFinder.cs" company="Omiya Games">
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
    /// A helper scripts to search through AudioSources with audio scripts.
    /// Run it by attaching this script to a GameObject, then using the right-click context menu.
    /// </summary>
    /// <seealso cref="SoundEffect"/>
    /// <seealso cref="AmbientMusic"/>
    /// <seealso cref="AudioMixerReference"/>
    public class AudioFinder : MonoBehaviour
    {
        [Header("Search through...")]
        public GameObject[] searchThrough;

        [Header("Audio Sources")]
        public List<AudioSource> soundEffects = new List<AudioSource>();
        public List<AudioSource> ambientMusics = new List<AudioSource>();
        public List<AudioSource> unknownSources = new List<AudioSource>();

        [Header("Audio Sources")]
        public AudioMixerGroup soundEffectsGroup = null;
        public AudioMixerGroup ambientMusicGroup = null;

        [ContextMenu("Find all audio sources")]
        void FindAllAudioSources()
        {
            // Clear all lists
            soundEffects.Clear();
            ambientMusics.Clear();
            unknownSources.Clear();

            // Seek for all AudioSources
            SoundEffect soundComponent = null;
            AmbientMusic ambientComponent = null;
            BackgroundMusic backgroundComponent = null;
            foreach (GameObject search in searchThrough)
            {
                AudioSource[] allSources = search.GetComponentsInChildren<AudioSource>(true);
                foreach (AudioSource source in allSources)
                {
                    soundComponent = source.GetComponent<SoundEffect>();
                    ambientComponent = source.GetComponent<AmbientMusic>();
                    backgroundComponent = source.GetComponent<BackgroundMusic>();
                    if ((soundComponent != null) && (ambientComponent == null) && (backgroundComponent == null))
                    {
                        soundEffects.Add(source);
                    }
                    else if ((soundComponent == null) && (ambientComponent != null) && (backgroundComponent == null))
                    {
                        ambientMusics.Add(source);
                    }
                    else
                    {
                        unknownSources.Add(source);
                    }
                }
            }
        }

        [ContextMenu("Set mixer group")]
        void SetMixerGroup()
        {
            // Set the audio group
            foreach (AudioSource source in soundEffects)
            {
                source.outputAudioMixerGroup = soundEffectsGroup;
            }
            foreach (AudioSource source in ambientMusics)
            {
                source.outputAudioMixerGroup = ambientMusicGroup;
            }
        }
    }
}
#endif
