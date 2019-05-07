﻿using UnityEngine;
using OmiyaGames;
using OmiyaGames.Audio;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="BackgroundMusicChanger.cs" company="Omiya Games">
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
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>A simple test script: changes the background music when the scene starts</summary>
    /// <seealso cref="BackgroundMusic"/>
    public class BackgroundMusicChanger : MonoBehaviour
    {
        [SerializeField]
        AudioClip backgroundMusic;
        [SerializeField]
        bool randomizeStartPoint = false;

        // Use this for initialization
        void Start()
        {
            if (backgroundMusic != null)
            {
                Singleton.Get<BackgroundMusic>().CurrentMusic = backgroundMusic;
                if(randomizeStartPoint)
                {
                    Singleton.Get<BackgroundMusic>().Audio.time = Random.Range(0f, backgroundMusic.length);
                }
            }
        }
    }
}
