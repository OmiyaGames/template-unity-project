using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

namespace OmiyaGames.Web
{
    ///-----------------------------------------------------------------------
    /// <copyright file="WebLocationInfo.cs" company="Omiya Games">
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
    /// <date>5/15/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Provides debugging information for the WebGL build, and its host information.
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
    /// <description>>6/13/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public class WebLocationInfo : MonoBehaviour
    {
        const string ForWebGlMessage = "This menu is meant to provide information for WebGL builds.";
        const string LoadingMessage = "Loading web information...";

        [SerializeField]
        Text infoLabel;

        // Use this for initialization
        IEnumerator Start()
        {
            // Grab the web checker
            WebLocationChecker webChecker = null;
            if (Singleton.Instance.IsWebApp == true)
            {
                webChecker = Singleton.Get<WebLocationChecker>();
            }

            // Grab information about webChecker
            if(webChecker != null)
            {
                // Print that we're loading
                infoLabel.text = LoadingMessage;

                // Wait until the WebLocationChecker is done
                while(webChecker.CurrentState == WebLocationChecker.State.InProgress)
                {
                    yield return null;
                }

                // Update the reason for this dialog to appear
                infoLabel.text = Utility.BuildTestMessage(new StringBuilder(), webChecker);
            }
            else
            {
                infoLabel.text = ForWebGlMessage;
            }
        }
    }
}
