using UnityEngine;
using UnityEngine.Serialization;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GameSettings.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
    /// <date>9/22/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A struct that returns a link specific to a platform.
    /// Useful as a <code>SerializedField</code> set in the inspector.
    /// </summary>
    [System.Serializable]
    public struct PlatformSpecificLink
    {
        public enum SupportedPlatforms
        {
            WebDefault,
            NativeIos,
            NativeAndroid,
            NativeAmazon,
            NativeWin10,
            WebiOS,
            WebAndroid,
            WebAmazon,
            WebWin10
        }

        [Header("Web Links")]
        [Tooltip("The default link, in case the rest of the fields are not covered")]
        [FormerlySerializedAsAttribute("webLink")]
        [SerializeField]
        string defaultWebLink;
#pragma warning disable 0414
        [SerializeField]
        string iosWebLink;
        [SerializeField]
        string androidWebLink;
        [SerializeField]
        string amazonWebLink;
        [SerializeField]
        string windows10WebLink;

        [Header("Native Store Links")]
        [FormerlySerializedAsAttribute("iosLink")]
        [SerializeField]
        string iosNativeLink;
        [FormerlySerializedAsAttribute("androidLink")]
        [SerializeField]
        string androidNativeLink;
        [FormerlySerializedAsAttribute("amazonLink")]
        [SerializeField]
        string amazonNativeLink;
        [FormerlySerializedAsAttribute("windows10Link")]
        [SerializeField]
        string windows10NativeLink;
#pragma warning restore 0414

        public static SupportedPlatforms Platform
        {
            get
            {
#if UNITY_IOS
                return SupportedPlatforms.NativeIos;
#elif AMAZON
                return SupportedPlatforms.NativeAmazon;
#elif UNITY_ANDROID
                return SupportedPlatforms.NativeAndroid;
#elif UNITY_WSA
                return SupportedPlatforms.NativeWin10;
#else
                return SupportedPlatforms.WebDefault;
#endif
            }
        }

        public string WebLink
        {
            get
            {
                return defaultWebLink;
            }
        }

        public string PlatformLink
        {
            get
            {
                // Return the URL
                return GetPlatformLink(Platform);
            }
        }

        public string GetPlatformLink(SupportedPlatforms platform)
        {
            // Check the platform
            string returnUrl = null;
            switch(platform)
            {
                case SupportedPlatforms.NativeIos:
                    returnUrl = iosNativeLink;
                    break;
                case SupportedPlatforms.WebiOS:
                    returnUrl = iosWebLink;
                    break;
                case SupportedPlatforms.NativeAmazon:
                    returnUrl = amazonNativeLink;
                    break;
                case SupportedPlatforms.WebAmazon:
                    returnUrl = amazonWebLink;
                    break;
                case SupportedPlatforms.NativeAndroid:
                    returnUrl = androidNativeLink;
                    if (string.IsNullOrEmpty(returnUrl) == true)
                    {
                        returnUrl = amazonNativeLink;
                    }
                    break;
                case SupportedPlatforms.WebAndroid:
                    returnUrl = androidWebLink;
                    if (string.IsNullOrEmpty(returnUrl) == true)
                    {
                        returnUrl = amazonWebLink;
                    }
                    break;
                case SupportedPlatforms.NativeWin10:
                    returnUrl = windows10NativeLink;
                    break;
                case SupportedPlatforms.WebWin10:
                    returnUrl = windows10WebLink;
                    break;
            }

            // Check if the string is empty
            if (string.IsNullOrEmpty(returnUrl) == true)
            {
                // Replace with a web URL
                returnUrl = WebLink;
            }

            // Return the URL
            return returnUrl;
        }
    }
}
