using System.IO;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ISettingsVersion.cs" company="Omiya Games">
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
    /// <date>5/17/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// <para>
    /// Interface that represents a version of saved settings by either
    /// adding or modifying on top of settings stored in previous
    /// <see cref="ISettingsVersion"/> (classes with lower <see cref="Version"/>).
    /// </para>
    /// <para>
    /// Also where one can retrieve an <see cref="IGenerator"/>
    /// for that version.
    /// </para>
    /// <para>
    /// Anything that implements this interface must be a
    /// <code>class</code>.
    /// </para>
    /// </summary>
    /// <seealso cref="GameSettings"/>
    /// <seealso cref="NamespaceGeneratorArgs"/>
    /// <seealso cref="SettingsGeneratorArgs"/>
    /// <seealso cref="IGenerator"/>
    public interface ISettingsVersion
    {
        ushort Version
        {
            get;
        }

        /// <summary>
        /// Checks if there's a matching <see cref="IGenerator.Key"/>
        /// in the collection of <see cref="IGenerator"/> this version
        /// is storing.
        /// </summary>
        /// <returns>
        /// True if a <see cref="IGenerator"/> stored in this version
        /// has the same <see cref="IGenerator.Key"/>.
        /// </returns>
        bool Contains(string key);

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
        bool IsSetting(string key);

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
        bool IsGenerator<T>(string key) where T : IGenerator;

        /// <summary>
        /// Returns an instance of <see cref="IStoredSetting"/>
        /// from this version.
        /// </summary>
        /// <param name="key">
        /// A reference to <see cref="IStoredSetting.Key"/>.
        /// </param>
        /// <returns>
        /// A <see cref="IGenerator"/> stored in this version,
        /// with the same <see cref="IStoredSetting.Key"/>.
        /// </returns>
        IStoredSetting GetSetting(string key);

        /// <summary>
        /// Returns an instance of <see cref="IGenerator"/>
        /// from this version.
        /// </summary>
        /// <typeparam name="T">
        /// A <see cref="IGenerator"/>.
        /// Can be an instance to a struct.
        /// </typeparam>
        /// <param name="key">
        /// A reference to <see cref="IGenerator.Key"/>.
        /// </param>
        /// <returns>
        /// A <see cref="IGenerator"/> stored in this version,
        /// with the same <see cref="IGenerator.Key"/>.
        /// </returns>
        T GetGenerator<T>(string key) where T : IGenerator;
    }
}
