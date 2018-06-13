using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="OmiyaGamesUtility.cs" company="Omiya Games">
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
    /// <date>6/12/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An enum indicating supported platforms. Can be multi-selected.
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
    /// <description>6/12/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    [System.Flags]
    public enum SupportedPlatforms {
        None = 0,

        Windows =   1 << 1,
        MacOS =     1 << 2,
        Linux =     1 << 3,
        Web =       1 << 4,
        iOS =       1 << 5,
        Android =   1 << 6,

        AllPlatforms = Windows | MacOS | Linux | Web | iOS | Android,
    }

    public static class SupportedPlatformsHelper
    {
        public static readonly string[] AllPlatformNames = new string[]
        {
            SupportedPlatforms.Windows.ToString(),
            SupportedPlatforms.MacOS.ToString(),
            SupportedPlatforms.Linux.ToString(),
            SupportedPlatforms.Web.ToString(),
            SupportedPlatforms.iOS.ToString(),
            SupportedPlatforms.Android.ToString(),
        };

        public static bool IsSupported(this SupportedPlatforms currentPlatforms, SupportedPlatforms singlePlatform)
        {
            return (currentPlatforms & singlePlatform) != 0;
        }

        public static bool IsSupported(this SupportedPlatforms currentPlatforms, RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return IsSupported(currentPlatforms, SupportedPlatforms.Windows);
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return IsSupported(currentPlatforms, SupportedPlatforms.MacOS);
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.LinuxPlayer:
                    return IsSupported(currentPlatforms, SupportedPlatforms.Linux);
                case RuntimePlatform.WebGLPlayer:
                    return IsSupported(currentPlatforms, SupportedPlatforms.Web);
                case RuntimePlatform.IPhonePlayer:
                    return IsSupported(currentPlatforms, SupportedPlatforms.iOS);
                case RuntimePlatform.Android:
                    return IsSupported(currentPlatforms, SupportedPlatforms.Android);
                default:
                    return false;
            }
        }
    }
}
