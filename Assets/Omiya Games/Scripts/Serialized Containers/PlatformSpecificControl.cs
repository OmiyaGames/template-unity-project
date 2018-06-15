using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PlatformSpecificControl.cs" company="Omiya Games">
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
    /// <date>6/15/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A control that comes with specifications on what platform is supported.
    /// </summary>
    /// <seealso cref="SupportPlatforms"/>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>6/15/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [System.Serializable]
    public class PlatformSpecificControl<C> where C : Component
    {
        [SerializeField]
        C component;
        [SerializeField]
        SupportedPlatforms enabledFor;

        public C Component
        {
            get
            {
                return component;
            }
        }

        /// <summary>
        /// Platforms this control supports
        /// </summary>
        public SupportedPlatforms EnabledFor
        {
            get
            {
                return enabledFor;
            }
        }

        /// <summary>
        /// Activates the control if supported by the current platform.
        /// If not, deactivates it.
        /// </summary>
        public void Setup()
        {
            Component.gameObject.SetActive(EnabledFor.IsThisBuildSupported());
        }
    }

    /// <summary>
    /// A button that comes with specifications on what platform is supported.
    /// </summary>
    /// <seealso cref="PlatformSpecificControl<C>"/>
    /// <seealso cref="OptionsListButtonEditor"/>
    [System.Serializable]
    public class PlatformSpecificButton : PlatformSpecificControl<Button> { }
}
