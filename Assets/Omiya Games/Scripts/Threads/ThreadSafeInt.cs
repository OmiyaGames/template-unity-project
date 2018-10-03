using System.Threading;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ThreadSafeInt.cs" company="Omiya Games">
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
    /// <date>10/2/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary cref="C < T >">
    /// An integer version of <code>ThreadSafe</code> with more performant helper methods.
    /// </summary>
    public class ThreadSafeInt : ThreadSafe<int>
    {
        // Constructors
        public ThreadSafeInt() : this(0) { }
        public ThreadSafeInt(int value) : base(value) { }

        /// <summary>
        /// A more performant version of <code>Value++</code>.
        /// </summary>
        public void Increment()
        {
            Interlocked.Increment(ref value);
        }

        /// <summary>
        /// A more performant version of <code>Value--</code>.
        /// </summary>
        public void Decrement()
        {
            Interlocked.Decrement(ref value);
        }

        /// <summary>
        /// A more performant version of <code>Value += addBy</code>.
        /// </summary>
        public void Add(int addBy)
        {
            Interlocked.Add(ref value, addBy);
        }

        /// <summary>
        /// A more performant version of <code>Value -= subtractBy</code>.
        /// </summary>
        public void Subtract(int subtractBy)
        {
            Add(-subtractBy);
        }
    }
}
