using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Settings
{
    public abstract class SettingsVersionGeneratorDecorator : ISettingsVersionGenerator
    {
        /// <summary>
        /// A map of <see cref="IGenerator"/>.
        /// </summary>
        protected readonly Dictionary<string, IGenerator> allSettingsGenerator;

        /// <summary>
        /// Sets up <see cref="allSettingsGenerator"/>.
        /// </summary>
        public SettingsVersionGeneratorDecorator()
        {
            // Setup member variables
            allSettingsGenerator = new Dictionary<string, IGenerator>();

            // Grab all the new generators
            IGenerator[] newGenerators = GetNewGenerators();
            if (newGenerators != null)
            {
                foreach (IGenerator generator in newGenerators)
                {
                    if (generator == null)
                    {
                        throw new NullReferenceException("No elements from GetNewGenerators can be null.");
                    }
                    else if (string.IsNullOrEmpty(generator.Key) == true)
                    {
                        throw new Exception("ISingleGenerator cannot have a Key that returns null or an empty string.");
                    }
                    else if (allSettingsGenerator.ContainsKey(generator.Key) == true)
                    {
                        throw new Exception("An ISingleGenerator with the same key is found. They must be unique.");
                    }
                    else
                    {
                        allSettingsGenerator.Add(generator.Key, generator);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a unique number indicating the version of this setting.
        /// The larget the number, the newer the class.
        /// </summary>
        public abstract ushort Version
        {
            get;
        }

        /// <summary>
        /// Gets a list of keys to remove from previous
        /// <see cref="ISettingsVersion"/> in
        /// <see cref="UpdateNamespaceArgs(NamespaceGeneratorArgs)"/>
        /// </summary>
        protected abstract string[] GetKeysToRemove();

        /// <summary>
        /// Gets a list of generators to add in
        /// <see cref="UpdateNamespaceArgs(NamespaceGeneratorArgs)"/>
        /// </summary>
        protected abstract IGenerator[] GetNewGenerators();

#if UNITY_EDITOR
        /// <summary>
        /// Adds no new namespaces to <see cref="NamespaceGeneratorArgs"/>.
        /// </summary>
        public virtual void UpdateNamespaceArgs(NamespaceGeneratorArgs args)
        {
            // By default, do nothing!
        }

        /// <summary>
        /// Writes the default constructor of this decorator.
        /// </summary>
        public virtual void WriteCodeForConstructor(StreamWriter writer, int numTabs)
        {
            GameSettingsGenerator.WriteStartOfLine(writer, numTabs, "new ");
            writer.Write(GetType());
            writer.Write("()");
        }

        public void UpdateSettingArgs(int versionArrayIndex, SettingsGeneratorArgs args)
        {
            string errorMessage;

            // Check if there are any settings to remove
            string[] keysToRemove = GetKeysToRemove();
            if ((keysToRemove != null) && (keysToRemove.Length > 1))
            {
                // If so, remove them from args
                foreach (string key in keysToRemove)
                {
                    if (args.RemoveSetting(key, out errorMessage) == false)
                    {
                        Debug.Log(errorMessage);
                    }
                }
            }

            // If so, remove them from args
            foreach (IGenerator newGenerator in allSettingsGenerator.Values)
            {
                if (args.AddSetting(versionArrayIndex, newGenerator, out errorMessage) == false)
                {
                    Debug.Log(errorMessage);
                }
            }
        }
#endif
        /// <summary>
        /// Checks if there's a matching <see cref="IStoredSetting.Key"/>
        /// in the collection of <see cref="IStoredSetting"/> this version
        /// is storing.
        /// </summary>
        /// <returns>
        /// True if a <see cref="IStoredSetting"/> stored in this version
        /// has the same <see cref="IStoredSetting.Key"/>.
        /// </returns>
        public bool Contains(string key)
        {
            return allSettingsGenerator.ContainsKey(key);
        }

        /// <summary>
        /// Checks if there's a matching <see cref="IGenerator.Key"/>
        /// in the collection of <see cref="IGenerator"/> this version
        /// is storing, AND if it's an instance of <see cref="IStoredSetting"/>.
        /// </summary>
        /// <returns>
        /// True if a <see cref="IGenerator"/> stored in this version
        /// has the same <see cref="IGenerator.Key"/>, AND it's an
        /// instance of <see cref="IStoredSetting"/>.
        /// </returns>
        public bool IsSetting(string key)
        {
            bool returnFlag = false;
            if(Contains(key) == true)
            {
                returnFlag = allSettingsGenerator[key] is IStoredSetting;
            }
            return returnFlag;
        }

        /// <summary>
        /// Checks if there's a matching <see cref="IGenerator.Key"/>
        /// in the collection of <see cref="IGenerator"/> this version
        /// is storing, AND if it's an instance of <see cref="IStoredSetting"/>.
        /// </summary>
        /// <typeparam name="T">
        /// A <see cref="IGenerator"/>.
        /// Can be an instance to a struct.
        /// </typeparam>
        /// <returns>
        /// True if a <see cref="IGenerator"/> stored in this version
        /// has the same <see cref="IGenerator.Key"/>, AND it's an
        /// instance of <code>T</code>.
        /// </returns>
        public bool IsGenerator<T>(string key) where T : IGenerator
        {
            bool returnFlag = false;
            if (Contains(key) == true)
            {
                returnFlag = allSettingsGenerator[key] is T;
            }
            return returnFlag;
        }

        /// <summary>
        /// Returns an instance of <see cref="IStoredSetting"/>
        /// from this version.
        /// </summary>
        /// <param name="key">
        /// A reference to <see cref="IStoredSetting.Key"/>.
        /// </param>
        /// <returns>
        /// A <see cref="IStoredSetting"/> stored in this version,
        /// with the same <see cref="IStoredSetting.Key"/>.
        /// </returns>
        public IStoredSetting GetSetting(string key)
        {
            return (IStoredSetting)allSettingsGenerator[key];
        }

        /// <summary>
        /// Returns an instance of <see cref="IStoredSetting"/>
        /// from this version.
        /// </summary>
        /// <typeparam name="T">
        /// A <see cref="IStoredSetting"/>.
        /// Can be an instance to a struct.
        /// </typeparam>
        /// <param name="key">
        /// A reference to <see cref="IStoredSetting.Key"/>.
        /// </param>
        /// <returns>
        /// A <see cref="IStoredSetting"/> stored in this version,
        /// with the same <see cref="IStoredSetting.Key"/>.
        /// </returns>
        public T GetGenerator<T>(string key) where T : IGenerator
        {
            return (T)allSettingsGenerator[key];
        }
    }
}