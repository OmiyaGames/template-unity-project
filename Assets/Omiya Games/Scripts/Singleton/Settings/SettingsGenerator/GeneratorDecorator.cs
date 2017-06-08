using System;
using System.IO;
using System.Text;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GeneratorDecorator.cs" company="Omiya Games">
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
    /// Helper class that has a couple of built-in stuff to implement a <see cref="IGenerator"/> more easily.
    /// </summary>
    public abstract class GeneratorDecorator : IGenerator
    {
        private AccessModifier getterScope = AccessModifier.Public;
        private AccessModifier setterScope = AccessModifier.Public;

        public abstract string TypeName
        {
            get;
        }

        public virtual string Key
        {
            get
            {
                return PropertyName;
            }
        }

        public abstract string PropertyName
        {
            get;
            set;
        }

        public AccessModifier GetterScope
        {
            get
            {
                AccessModifier returnModifier = AccessModifier.None;
                if (string.IsNullOrEmpty(GetterCode) == false)
                {
                    returnModifier = getterScope;
                }
                return returnModifier;
            }
            set
            {
                // Make sure the scopes allows us to generate a property
                if ((value == AccessModifier.None) && (setterScope == AccessModifier.None))
                {
                    throw new ArgumentException("Parameter getterScope and setterScope cannot be set to Scope.None at the same time.");
                }
                else
                {
                    getterScope = value;
                }
            }
        }

        public AccessModifier SetterScope
        {
            get
            {
                AccessModifier returnModifier = AccessModifier.None;
                if (string.IsNullOrEmpty(SetterCode) == false)
                {
                    returnModifier = setterScope;
                }
                return returnModifier;
            }
            set
            {
                // Make sure the scopes allows us to generate a property
                if ((value == AccessModifier.None) && (getterScope == AccessModifier.None))
                {
                    throw new ArgumentException("Parameter getterScope and setterScope cannot be set to Scope.None at the same time.");
                }
                else
                {
                    setterScope = value;
                }
            }
        }

        public string[] TooltipDocumentation
        {
            get;
            set;
        }

        public abstract string GetterCode
        {
            get;
            set;
        }

        public abstract string SetterCode
        {
            get;
            set;
        }

#if UNITY_EDITOR
        public virtual void WriteCodeToAccessSetting(StreamWriter writer, int numTabs, int versionArrayIndex)
        {
            StringBuilder builder = new StringBuilder();
            AccessModifier defaultScope;

            // Start the property
            if ((TooltipDocumentation != null) && (TooltipDocumentation.Length > 0))
            {
                GameSettingsGenerator.WriteTooltipComment(writer, numTabs, TooltipDocumentation);
            }
            numTabs = GameSettingsGenerator.WriteStartEncapsulation(writer, numTabs, GetCodeForPropertyDeclaration(builder, out defaultScope));

            // Check if we should write the getter
            if ((GetterScope != AccessModifier.None) && (string.IsNullOrEmpty(GetterCode) == false))
            {
                WriteCodeForGetter(writer, numTabs, builder, defaultScope, versionArrayIndex);
            }

            // Check if we should write the setter
            if ((SetterScope != AccessModifier.None) && (string.IsNullOrEmpty(SetterCode) == false))
            {
                WriteCodeForSetter(writer, numTabs, builder, defaultScope, versionArrayIndex);
            }

            // End the property
            numTabs = GameSettingsGenerator.WriteEndEncapsulation(writer, numTabs);
        }

        public abstract bool CanWriteCodeToInstance
        {
            get;
        }

        public abstract void WriteCodeToInstance(StreamWriter writer, int versionArrayIndex, bool includeGeneric);

        protected string GetCodeForPropertyDeclaration(StringBuilder builder, out AccessModifier defaultScope)
        {
            builder.Length = 0;

            // Get the scope to define the property
            defaultScope = AccessModifier.Internal;
            if ((GetterScope == AccessModifier.Public) || (SetterScope == AccessModifier.Public))
            {
                // If either the getter or setter is public,
                // set the scope of this property to public as well
                defaultScope = AccessModifier.Public;
            }

            AppendScopeToStringBuilder(builder, defaultScope);
            builder.Append(TypeName);
            builder.Append(' ');
            builder.Append(PropertyName);
            return builder.ToString();
        }

        private void AppendScopeToStringBuilder(StringBuilder builder, AccessModifier scope)
        {
            if (scope == AccessModifier.Public)
            {
                builder.Append("public");
            }
            else
            {
                builder.Append("internal");
            }
            builder.Append(' ');
        }

        protected void WriteCodeForGetter(StreamWriter writer, int numTabs, StringBuilder builder, AccessModifier defaultScope, int versionArrayIndex)
        {
            builder.Length = 0;

            // Start get
            if (GetterScope != defaultScope)
            {
                AppendScopeToStringBuilder(builder, GetterScope);
            }
            builder.Append("get");
            numTabs = GameSettingsGenerator.WriteStartEncapsulation(writer, numTabs, builder.ToString());

            // Check if instance will be written
            GameSettingsGenerator.WriteTabs(writer, numTabs);
            if (CanWriteCodeToInstance == true)
            {
                // Write return
                writer.Write("return ");
                WriteCodeToInstance(writer, versionArrayIndex, true);
                writer.Write('.');
                writer.Write(GetterCode);
                writer.WriteLine(';');
            }
            else
            {
                // Otherwise, plop the getter code directly
                writer.WriteLine(GetterCode);
            }

            // End the property
            GameSettingsGenerator.WriteEndEncapsulation(writer, numTabs);
        }

        protected void WriteCodeForSetter(StreamWriter writer, int numTabs, StringBuilder builder, AccessModifier defaultScope, int versionArrayIndex)
        {
            builder.Length = 0;

            // Start set
            if (SetterScope != defaultScope)
            {
                AppendScopeToStringBuilder(builder, SetterScope);
            }
            builder.Append("set");
            numTabs = GameSettingsGenerator.WriteStartEncapsulation(writer, numTabs, builder.ToString());

            // Write value
            GameSettingsGenerator.WriteTabs(writer, numTabs);
            if (CanWriteCodeToInstance == true)
            {
                // Write return
                WriteCodeToInstance(writer, versionArrayIndex, true);
                writer.Write('.');
                writer.Write(SetterCode);
                writer.WriteLine(';');
            }
            else
            {
                // Otherwise, plop the getter code directly
                writer.WriteLine(SetterCode);
            }

            // End the property
            GameSettingsGenerator.WriteEndEncapsulation(writer, numTabs);
        }
#endif
    }
}
