using System;
using System.IO;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PropertyStoredSettingsGenerator.cs" company="Omiya Games">
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
    /// Abstract class that helps generate a property in <see cref="GameSettings"/>.
    /// </summary>
    public abstract class PropertyStoredSettingsGenerator<T> : GeneratorDecorator, IStoredSettingGenerator
    {
        public interface ValueProcessor
        {
            T Process(T value);
        }

        public delegate T ConvertOldSettings(ISettingsRecorder settings, string key, int recordedVersion, int latestVersion, T defaultValue);

        public delegate void ValueChange(PropertyStoredSettingsGenerator<T> source, T oldValue, T newValue);

        //public event ValueChange OnBeforeValueChange;
        //public event ValueChange OnAfterValueChange;

        public static readonly PropertyWriter DefaultGetterCode = WriteGetter;
        public static readonly PropertyWriter DefaultSetterCode = WriteSetter;

        private readonly string key;
        private readonly T defaultValue;
        private string propertyName = null;
        private PropertyWriter customGetterCode = null;
        private PropertyWriter customSetterCode = null;

        public PropertyStoredSettingsGenerator(string key, T defaultValue)
        {
            // Set readonly variables
            this.key = key;
            this.defaultValue = defaultValue;
            Value = defaultValue;
        }

        #region Properties
        public override string Key
        {
            get
            {
                return key;
            }
        }

        public T Value
        {
            get;
            private set;
        }

        public override string TypeName
        {
            get
            {
                return GeneratorHelper.CorrectedTypeName(typeof(T));
            }
        }

        /// <summary>
        /// Allows a value to be saved even after data is reset.
        /// </summary>
        public bool IsValueRetainedOnClear
        {
            get;
            set;
        } = false;

        public ConvertOldSettings Converter
        {
            get;
            set;
        } = null;

        public ValueProcessor Processor
        {
            get;
            set;
        } = null;

        public override string PropertyName
        {
            get
            {
                if (string.IsNullOrEmpty(propertyName) == true)
                {
                    string errorMessage = null;
                    propertyName = GeneratorHelper.ConvertKeyToPropertyName(Key, out errorMessage);
                    if (string.IsNullOrEmpty(propertyName) == true)
                    {
                        throw new Exception(errorMessage + " Please define a different Key, or set valid PropertName.");
                    }
                }
                return propertyName;
            }
            set
            {
                string errorMessage = null;
                if ((string.IsNullOrEmpty(value) == false) && (GeneratorHelper.IsPropertyNameValid(value, out errorMessage) == false))
                {
                    throw new Exception(errorMessage + " Please define a different PropertName.");
                }
                else
                {
                    propertyName = value;
                }
            }
        }
        #endregion

        public abstract bool IsSameValue(T compareValue);

        public abstract void OnSaveSetting(ISettingsRecorder settings, int latestVersion);

        public abstract T DefaultSettingsRetrieval(ISettingsRecorder settings, int recordedVersion, T defaultValue);

        public bool SetValue(T newValue, ISettingsRecorder settings, int latestVersion)
        {
            bool returnFlag = false;

            // Check to see if we need to apply any process to the value
            if (Processor != null)
            {
                newValue = Processor.Process(newValue);
            }

            // Make sure these values are different
            if (IsSameValue(newValue) == false)
            {
                // Run the event before changing the value
                //T oldValue = Value;
                //if (OnBeforeValueChange != null)
                //{
                //    OnBeforeValueChange(this, oldValue, newValue);
                //}

                // Change the value
                Value = newValue;
                returnFlag = true;

                // Run event after changing the value
                //if (OnAfterValueChange != null)
                //{
                //    OnAfterValueChange(this, oldValue, Value);
                //}
            }
            return returnFlag;
        }

        public void OnClearSetting(ISettingsRecorder settings, int recordedVersion, int latestVersion)
        {
            if (IsValueRetainedOnClear == true)
            {
                OnSaveSetting(settings, latestVersion);
            }
        }

        public void OnRetrieveSetting(ISettingsRecorder settings, int recordedVersion, int latestVersion)
        {
            if (Converter != null)
            {
                Value = Converter(settings, Key, recordedVersion, latestVersion, defaultValue);
            }
            else
            {
                Value = DefaultSettingsRetrieval(settings, recordedVersion, defaultValue);
            }
        }

        public override PropertyWriter GetterCode
        {
            get
            {
                if(customGetterCode != null)
                {
                    return customGetterCode;
                }
                else
                {
                    return DefaultGetterCode;
                }
            }
            set
            {
                customGetterCode = value;
            }
        }

        public override PropertyWriter SetterCode
        {
            get
            {
                if(customSetterCode != null)
                {
                    return customSetterCode;
                }
                else
                {
                    return DefaultSetterCode;
                }
            }
            set
            {
                customSetterCode = value;
            }
        }

#if UNITY_EDITOR
        public override bool CanWriteCodeToInstance
        {
            get
            {
                return true;
            }
        }

        public override void WriteCodeToInstance(TextWriter writer, int versionArrayIndex, bool includeGeneric)
        {
            GeneratorHelper.WriteCodeToInstance(this, writer, versionArrayIndex, includeGeneric);
        }
#endif

        private static void WriteGetter(GeneratorDecorator source, GeneratePropertyEventArgs args)
        {
            if(args != null)
            {
                args.WriteTabs();
                args.WriteSingleLine(true, "Value");
            }
        }

        private static void WriteSetter(GeneratorDecorator source, GeneratePropertyEventArgs args)
        {
            if (args != null)
            {
                args.WriteTabs();
                args.WriteSingleLine(false, "SetValue(value, Settings, AppVersion)");
            }
        }
    }
}
