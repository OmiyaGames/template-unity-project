using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="RandomList.cs" company="Omiya Games">
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
    /// A list that shuffles its elements to randomize its content.
    /// </summary>
    [System.Serializable]
    public class RandomList<T> : List<T>
    {
        int[] randomizedIndexes = null;
        int index = int.MinValue;

        public T CurrentElement
        {
            get
            {
                T returnElement = default(T);
                if (Count > 0)
                {
                    // Check if I need to setup a list
                    if (randomizedIndexes == null)
                    {
                        SetupList();
                        Utility.ShuffleList<int>(randomizedIndexes);
                        index = 0;
                    }
                    else if (randomizedIndexes.Length != Count)
                    {
                        SetupList();
                        Utility.ShuffleList<int>(randomizedIndexes);
                        index = 0;
                    }
                    else if ((index >= randomizedIndexes.Length) || (index < 0))
                    {
                        // Shuffle the list if we got to the last element
                        Utility.ShuffleList<int>(randomizedIndexes);
                        index = 0;
                    }

                    // Grab the current element
                    returnElement = this[randomizedIndexes[index]];
                }
                return returnElement;
            }
        }

        public T RandomElement
        {
            get
            {
                T returnElement = default(T);
                if (Count > 0)
                {
                    ++index;
                    returnElement = CurrentElement;
                }
                return returnElement;
            }
        }

        public void Reshuffle()
        {
            index = int.MinValue;
        }

        #region Helper Methods

        void SetupList()
        {
            // Generate a new list, populated with entries based on frequency
            randomizedIndexes = new int[Count];
            for (index = 0; index < randomizedIndexes.Length; ++index)
            {
                randomizedIndexes[index] = index;
            }
        }

        #endregion
    }
}
