using System;
using System.IO;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PropertyGenerator.cs" company="Omiya Games">
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
    /// <date>5/29/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Abstract class that helps generate a property in <see cref="GameSettings"/>.
    /// </summary>
    public class PropertyGenerator : GeneratorDecorator
    {
        private readonly string propertyName;
        private readonly Type type;

        public PropertyGenerator(string propertyName, Type type)
        {
            string errorMessage;
            if (GeneratorHelper.IsPropertyNameValid(propertyName, out errorMessage) == false)
            {
                throw new ArgumentException(errorMessage, "propertyName");
            }
            else
            {
                this.propertyName = propertyName;
                this.type = type;
            }
        }

        public override string TypeName
        {
            get
            {
                return GeneratorHelper.CorrectedTypeName(type);
            }
        }

        public override string PropertyName
        {
            get
            {
                return propertyName;
            }
            set
            {
                throw new NotImplementedException("Cannot set PropertyName");
            }
        }

        public override string GetterCode
        {
            get;
            set;
        }

        public override string SetterCode
        {
            get;
            set;
        }

#if UNITY_EDITOR
        public override bool CanWriteCodeToInstance
        {
            get
            {
                return false;
            }
        }

        public override void WriteCodeToInstance(StreamWriter writer, int versionArrayIndex, bool includeGeneric)
        {
            // Do nothing!
        }
#endif
    }
}