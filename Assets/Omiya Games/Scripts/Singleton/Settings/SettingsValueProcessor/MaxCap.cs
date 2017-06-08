using System;
using System.Collections.Generic;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MaxCap.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2017 Omiya Games
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
    /// <date>5/24/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Prevents a setting from becoming larger than the specified bound.
    /// </summary>
    /// <seealso cref="PropertyStoredSettingsGenerator{T}.Processor"/>
    public class MaxCap<T> : PropertyStoredSettingsGenerator<T>.ValueProcessor where T : IComparable
    {
        private static readonly Dictionary<T, MaxCap<T>> allProcessors = new Dictionary<T, MaxCap<T>>();

        public static MaxCap<T> Get(T max)
        {
            MaxCap<T> returnProcessor = null;
            if (allProcessors.TryGetValue(max, out returnProcessor) == false)
            {
                returnProcessor = new MaxCap<T>(max);
                allProcessors.Add(max, returnProcessor);
            }
            return returnProcessor;
        }

        private readonly T max;

        private MaxCap(T max)
        {
            this.max = max;
        }

        public T Process(T value)
        {
            if (value.CompareTo(max) > 0)
            {
                value = max;
            }
            return value;
        }
    }
}
