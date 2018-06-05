using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LevelSelectMenu.cs" company="Omiya Games">
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
    /// <date>6/5/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A script for opening links in a TextMeshPro link.
    /// </summary>
    public class WebLinkOpener : MonoBehaviour
    {
        [System.Serializable]
        public struct WebLink
        {
            [SerializeField]
            string linkId;
            [SerializeField]
            string urlToOpen;

            public string LinkId
            {
                get
                {
                    return linkId;
                }
            }

            public string UrlToOpen
            {
                get
                {
                    return urlToOpen;
                }
            }
        }

        [SerializeField]
        WebLink[] links;

        readonly Dictionary<string, string> idToUrlCache = new Dictionary<string, string>();

        Dictionary<string, string> IdToUrlCache
        {
            get
            {
                if(idToUrlCache.Count != links.Length)
                {
                    idToUrlCache.Clear();
                    foreach(WebLink pair in links)
                    {
                        idToUrlCache.Add(pair.LinkId, pair.UrlToOpen);
                    }
                }
                return idToUrlCache;
            }
        }

        /// <summary>
        /// Event for when link is clicked.
        /// </summary>
        /// <param name="linkId"></param>
        /// <param name="linkText"></param>
        /// <param name="linkIndex"></param>
        public void OnWebsiteLinkClicked(string linkId, string linkText, int linkIndex)
        {
            string url;
            if ((IdToUrlCache.TryGetValue(linkId, out url) == true) && (string.IsNullOrEmpty(url) == false))
            {
                Application.OpenURL(url);
            }
        }
    }
}
