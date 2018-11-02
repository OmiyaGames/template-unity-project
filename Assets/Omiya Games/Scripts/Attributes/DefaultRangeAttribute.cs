using UnityEngine;
using System;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="DefaultRangeAttribute.cs" company="Omiya Games">
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
    /// <date>6/26/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Creates a checkbox in the editor.
    /// If unchecked, the default value is set to this value.
    /// If checked, reveals a slider, allowing the user to change this value.
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
    /// <description>6/26/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DefaultRangeAttribute : PropertyAttribute
    {
        /// <summary>
        /// Creates a checkbox in the editor. If unchecked, argument is set to first argument.
        /// Otherwise, the user is allowed to set the value to a specified range between min and max (both inclusive).
        /// </summary>
        /// <param name="defaultNumber">
        /// The number set to the value if editor's checkbox is unchecked.
        /// Best to set to a value that's NOT within the range of <code>min</code> and <code>max</code>.
        /// </param>
        /// <param name="min">Minimum value in the editor's slider.</param>
        /// <param name="max">Maximum value in the editor's slider.</param>
        public DefaultRangeAttribute(float defaultNumber, float min, float max)
        {
            this.DefaultNumber = defaultNumber;
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Creates a checkbox in the editor. If unchecked, argument is set to first argument.
        /// Otherwise, the user is allowed to set the value to a specified range between min and max (both inclusive).
        /// </summary>
        /// <param name="defaultNumber">
        /// The number set to the value if editor's checkbox is unchecked.
        /// Best to set to a value that's NOT within the range of <code>min</code> and <code>max</code>.
        /// </param>
        /// <param name="min">Minimum value in the editor's slider.</param>
        /// <param name="max">Maximum value in the editor's slider.</param>
        public DefaultRangeAttribute(int defaultNumber, int min, int max)
        {
            this.DefaultNumber = defaultNumber;
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// The number set to the value if editor's checkbox is unchecked.
        /// Best to set to a value that's NOT within the range of <code>min</code> and <code>max</code>.
        /// </summary>
        public float DefaultNumber
        {
            get;
        }

        /// <summary>
        /// Minimum value in the editor's slider.
        /// </summary>
        public float Min
        {
            get;
        }

        /// <summary>
        /// Maximum value in the editor's slider.
        /// </summary>
        public float Max
        {
            get;
        }
    }
}
