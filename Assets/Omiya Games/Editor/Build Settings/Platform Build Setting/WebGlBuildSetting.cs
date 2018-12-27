using UnityEngine;
using UnityEditor;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="WebGlBuildSetting.cs" company="Omiya Games">
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
    /// <date>10/31/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Build settings for WebGL platform.
    /// </summary>
    public class WebGlBuildSetting : IPlatformBuildSetting
    {
        [System.Serializable]
        public struct WebLocationCheckerSettings
        {
            [SerializeField]
            CustomFileName fileName;
            [SerializeField]
            bool includeIndexHtml;
            [SerializeField]
            string[] acceptedDomains;

            public WebLocationCheckerSettings(bool includeIndexHtml = true)
            {
                // Setup member variables
                this.fileName = new CustomFileName();
                this.includeIndexHtml = includeIndexHtml;
                acceptedDomains = null;
            }

            public CustomFileName FileName
            {
                get
                {
                    return fileName;
                }
            }

            public bool IncludeIndexHtml
            {
                get
                {
                    return includeIndexHtml;
                }
            }

            public string[] AcceptedDomains
            {
                get
                {
                    return acceptedDomains;
                }
            }
        }

        // FIXME: work on setting up a drawer for this variable
        [SerializeField]
        protected WebLocationCheckerSettings[] webLocations;
        // FIXME: do more research on the Facebook builds
        //[SerializeField]
        //protected bool forFacebook = false;

        #region Overrides
        internal override int MaxNumberOfResults
        {
            get
            {
                return base.MaxNumberOfResults + webLocations.Length;
            }
        }

        protected override BuildTargetGroup TargetGroup
        {
            get
            {
                //if (forFacebook == true)
                //{
                //    return BuildTargetGroup.Facebook;
                //}
                return BuildTargetGroup.WebGL;
            }
        }

        protected override BuildTarget Target
        {
            get
            {
                return BuildTarget.WebGL;
            }
        }

        protected override void ArchiveBuild(BuildPlayersResult results)
        {
            //foreach(WebLocationCheckerSettings webLocation in webLocations)
            //{
            //    // FIXME: to generate the domains file
            //    // Then ZIP the folder that's generated
            //    throw new System.NotImplementedException();
            //}

            // Do the regular archive business
            base.ArchiveBuild(results);
        }
        #endregion
    }
}
