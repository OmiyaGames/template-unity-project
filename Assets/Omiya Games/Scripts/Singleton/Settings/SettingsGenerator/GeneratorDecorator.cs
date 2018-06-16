using System;
using System.IO;
using System.Text;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GeneratorDecorator.cs" company="Omiya Games">
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
    /// <date>5/29/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper class that has a couple of built-in stuff to implement a <see cref="IGenerator"/> more easily.
    /// </summary>
    public abstract class GeneratorDecorator : IGenerator
    {
        public delegate void PropertyWriter(GeneratorDecorator source, GeneratePropertyEventArgs args);

        private AccessModifier getterScope = AccessModifier.Public;
        private AccessModifier setterScope = AccessModifier.Public;

        public static PropertyWriter CreatePropertyWriter(string variableName, string propertyName)
        {
            return (GeneratorDecorator source, GeneratePropertyEventArgs args) =>
            {
                if (args != null)
                {
                    args.WriteTabs();
                    args.writer.Write("return ");
                    args.writer.Write(variableName);
                    args.writer.Write('.');
                    args.writer.Write(propertyName);
                    args.writer.Write(';');
                    args.writer.WriteLine();
                }
            };
        }

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
                if (GetterCode != null)
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
                if (SetterCode != null)
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

        public abstract PropertyWriter GetterCode
        {
            get;
            set;
        }

        public abstract PropertyWriter SetterCode
        {
            get;
            set;
        }

#if UNITY_EDITOR
        public virtual void WriteCodeToAccessSetting(TextWriter writer, int numTabs, int versionArrayIndex)
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
            GeneratePropertyEventArgs args = new GeneratePropertyEventArgs(writer, numTabs, GetCodeToInstance(builder, versionArrayIndex, true), versionArrayIndex);
            if (GetterScope != AccessModifier.None)
            {
                WriteCodeForGetter(args, builder, defaultScope);
            }

            // Check if we should write the setter
            if (SetterScope != AccessModifier.None)
            {
                WriteCodeForSetter(args, builder, defaultScope);
            }

            // End the property
            numTabs = GameSettingsGenerator.WriteEndEncapsulation(writer, numTabs);
        }

        public abstract bool CanWriteCodeToInstance
        {
            get;
        }

        public abstract void WriteCodeToInstance(TextWriter writer, int versionArrayIndex, bool includeGeneric);

        protected string GetCodeToInstance(StringBuilder builder, int versionArrayIndex, bool includeGeneric)
        {
            builder.Clear();
            TextWriter writer = new StringWriter(builder);
            WriteCodeToInstance(writer, versionArrayIndex, includeGeneric);
            return builder.ToString();
        }

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

        protected void WriteCodeForGetter(GeneratePropertyEventArgs args, StringBuilder builder, AccessModifier defaultScope)
        {
            builder.Clear();

            // Start get
            if (GetterScope != defaultScope)
            {
                AppendScopeToStringBuilder(builder, GetterScope);
            }
            builder.Append("get");
            args.numTabs = GameSettingsGenerator.WriteStartEncapsulation(args.writer, args.numTabs, builder.ToString());

            // Write instance
            GetterCode?.Invoke(this, args);

            // End the property
            args.numTabs = GameSettingsGenerator.WriteEndEncapsulation(args.writer, args.numTabs);
        }

        protected void WriteCodeForSetter(GeneratePropertyEventArgs args, StringBuilder builder, AccessModifier defaultScope)
        {
            builder.Clear();

            // Start set
            if (SetterScope != defaultScope)
            {
                AppendScopeToStringBuilder(builder, SetterScope);
            }
            builder.Append("set");
            args.numTabs = GameSettingsGenerator.WriteStartEncapsulation(args.writer, args.numTabs, builder.ToString());

            // Write instance
            SetterCode?.Invoke(this, args);

            // End the property
            args.numTabs = GameSettingsGenerator.WriteEndEncapsulation(args.writer, args.numTabs);
        }
#endif
    }
}
