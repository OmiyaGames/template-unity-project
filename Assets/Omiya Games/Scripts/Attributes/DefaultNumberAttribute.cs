using UnityEngine;
using System;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="DefaultNumberAttribute.cs" company="Omiya Games">
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
    /// If checked, reveals a number field, allowing the user to change this value.
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
    public class DefaultNumberAttribute : PropertyAttribute
    {
        /// <summary>
        /// Indicates whether the editor number is limited to being greater, less, or neither.
        /// </summary>
        public enum Range
        {
            FullRange,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo
        }

        /// <summary>
        /// Creates a checkbox in the editor. If unchecked, argument is set to first argument.
        /// Otherwise, the user is allowed to set the 
        /// </summary>
        /// <param name="defaultNumber">
        /// The number set to the value if editor's checkbox is unchecked.
        /// </param>
        public DefaultNumberAttribute(float defaultNumber)
        {
            this.DefaultNumber = defaultNumber;
            StartNumber = defaultNumber;
            NumberRange = Range.FullRange;
        }

        /// <summary>
        /// Creates a checkbox in the editor. If unchecked, argument is set to first argument.
        /// Otherwise, the user is allowed to set the 
        /// </summary>
        /// <param name="defaultNumber">
        /// The number set to the value if editor's checkbox is unchecked.
        /// </param>
        /// <param name="greaterThan">
        /// Indicates whether the editor number is limited to being greater or less.
        /// </param>
        /// <param name="startNumber">
        /// If <code>greaterThan</code> is true, the number in the editor must be greater than this parameter.
        /// If false, then the number in the editor must be less.
        /// </param>
        public DefaultNumberAttribute(float defaultNumber, bool greaterThan, float startNumber)
        {
            this.DefaultNumber = defaultNumber;
            this.StartNumber = startNumber;

            // Set the number range
            NumberRange = Range.LessThanOrEqualTo;
            if (greaterThan == true)
            {
                NumberRange = Range.GreaterThanOrEqualTo;
            }
        }

        /// <summary>
        /// The number set to the value if editor's checkbox is unchecked.
        /// </summary>
        public float DefaultNumber
        {
            get;
        }

        /// <summary>
        /// Indicates whether the editor number is limited to being greater, less, or neither.
        /// </summary>
        public Range NumberRange
        {
            get;
        }

        /// <summary>
        /// If <code>numberRange</code> is <code>Range.GreaterThanOrEqualTo</code>,
        /// the number in the editor must be greater than this parameter.
        /// If <code>Range.LessThanOrEqualTo</code>, then the number in the editor must be less.
        /// Otherwise, this value is the first value the editor is set to when it's checked.
        /// </summary>
        public float StartNumber
        {
            get;
        }
    }
}
