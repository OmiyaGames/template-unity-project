using UnityEngine;
using System.Collections;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OmiyaGamesUtility.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
    /// A series of utilities used throughout the <code>OmiyaGames</code> namespace.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Shuffles the list.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <typeparam name="H">The list type parameter.</typeparam>
        public static void ShuffleList<H>(H[] list)
        {
            int index = 0, randomIndex = 0;
            H swapObject = default(H);
            for(; index < list.Length; ++index)
            {
                // Swap a random element
                randomIndex = Random.Range(0, list.Length);
                if(index != randomIndex)
                {
                    swapObject = list[index];
                    list[index] = list[randomIndex];
                    list[randomIndex] = swapObject;
                }
            }
        }
    }
}
