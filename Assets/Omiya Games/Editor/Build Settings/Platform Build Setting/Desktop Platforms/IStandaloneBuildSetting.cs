using UnityEngine;
using UnityEditor;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IStandaloneBuildSetting.cs" company="Omiya Games">
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
    /// <date>11/26/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Base build settings for standalone builds (i.e. PC/Mac/Linux).
    /// </summary>
    public abstract class IStandaloneBuildSetting : IPlatformBuildSetting
    {
        [SerializeField]
        protected Architecture architecture = Architecture.BuildUniversal;
        [SerializeField]
        protected CompressionType compression = CompressionType.Default;
        [SerializeField]
        private ScriptingImplementation scriptingBackend = ScriptingImplementation.Mono2x;

        /// <summary>
        /// A (preferably static) list of supported architectures for this platform
        /// </summary>
        public abstract Architecture[] SupportedArchitectures
        {
            get;
        }

        public Architecture DefaultArchitecture
        {
            get
            {
                return SupportedArchitectures[0];
            }
        }

        public Architecture ArchitectureToBuild
        {
            get
            {
                // Check if we're about to return a supported architecture
                if (ArrayUtility.Contains(SupportedArchitectures, architecture) == false)
                {
                    // If not, force the member variable back to default
                    architecture = DefaultArchitecture;
                }
                return architecture;
            }
            set
            {
                // Check if we're going to set to a supported architecture
                if (ArrayUtility.Contains(SupportedArchitectures, architecture) == true)
                {
                    // If so, handle it normally
                    architecture = value;
                }
                else
                {
                    // If not, set it to default
                    architecture = DefaultArchitecture;
                }
            }
        }

        /// <summary>
        /// A list of supported scripting backends for this platform
        /// </summary>
        /// <remarks>
        /// Overriding classes should return a static readonly lists.
        /// </remarks>
        public abstract ScriptingImplementation[] SupportedScriptingBackends
        {
            get;
        }

        public ScriptingImplementation DefaultScriptingBackend
        {
            get
            {
                return SupportedScriptingBackends[0];
            }
        }

        public virtual ScriptingImplementation ScriptingBackend
        {
            get
            {
                return scriptingBackend;
            }
        }

        #region Overrides
        protected override LastPlayerSettings SetupPlayerSettings()
        {
            LastPlayerSettings returnSetting = base.SetupPlayerSettings();
            PlayerSettings.SetScriptingBackend(TargetGroup, ScriptingBackend);
            return returnSetting;
        }

        protected override BuildTargetGroup TargetGroup
        {
            get
            {
                return BuildTargetGroup.Standalone;
            }
        }

        protected override BuildOptions Options
        {
            get
            {
                BuildOptions options = base.Options;

                // Add compression options
                SetBuildOption(ref options, TargetGroup, compression);
                return options;
            }
        }
        #endregion
    }
}
