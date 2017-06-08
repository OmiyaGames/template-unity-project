using System;
using System.Collections.Generic;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="Clamp.cs" company="Omiya Games">
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
    /// Prevents a setting from going beyond the specified bounds.
    /// </summary>
    /// <seealso cref="PropertyStoredSettingsGenerator{T}.Processor"/>
    public class Clamp<T> : PropertyStoredSettingsGenerator<T>.ValueProcessor where T : IComparable
    {
        private static readonly Dictionary<KeyValuePair<T, T>, Clamp<T>> allProcessors = new Dictionary<KeyValuePair<T, T>, Clamp<T>>();

        public static Clamp<T> Get(T min, T max)
        {
            Clamp<T> returnProcessor = null;
            KeyValuePair<T, T> key = new KeyValuePair<T, T>(min, max);
            if (allProcessors.TryGetValue(key, out returnProcessor) == false)
            {
                returnProcessor = new Clamp<T>(min, max);
                allProcessors.Add(key, returnProcessor);
            }
            return returnProcessor;
        }

        private readonly T min;
        private readonly T max;

        private Clamp(T min, T max)
        {
            if (max.CompareTo(min) <= 0)
            {
                throw new ArgumentException("max is not less than min.");
            }

            this.min = min;
            this.max = max;
        }

        public T Process(T value)
        {
            if(value.CompareTo(min) < 0)
            {
                value = min;
            }
            else if(value.CompareTo(max) > 0)
            {
                value = max;
            }
            return value;
        }
    }
}
