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
        TMPro.TextMeshProUGUI infoLabel;

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
                infoLabel.text = BuildTestMessage(new StringBuilder(), webChecker);
            }
            else
            {
                infoLabel.text = ForWebGlMessage;
            }
        }

        public static string BuildTestMessage(StringBuilder builder, WebLocationChecker webChecker)
        {
            builder.AppendLine("Information according to the WebLocationChecker:");

            // Indicate the object's state
            int bulletNumber = 1;
            builder.Append(bulletNumber);
            builder.AppendLine(") the WebLocationChecker state is:");
            builder.AppendLine(webChecker.CurrentState.ToString());

            // Indicate the current domain information
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") this game's domain is:");
            builder.AppendLine(webChecker.RetrievedHostName);

            // List entries from the default domain list
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") the default domain list is:");
            int index = 0;
            for (; index < webChecker.DefaultDomainList.Length; ++index)
            {
                builder.Append("- ");
                builder.AppendLine(webChecker.DefaultDomainList[index]);
            }

            // Check if there's a download URL to list
            if (string.IsNullOrEmpty(webChecker.DownloadDomainsUrl) == false)
            {
                // Print that URL
                ++bulletNumber;
                builder.Append(bulletNumber);
                builder.AppendLine(") downloaded a list of domains from:");
                builder.AppendLine(webChecker.DownloadDomainsUrl);

                // Check if there are any downloaded domains
                if (webChecker.DownloadedDomainList != null)
                {
                    ++bulletNumber;
                    builder.Append(bulletNumber);
                    builder.AppendLine(") downloaded the following domains:");
                    for (index = 0; index < webChecker.DownloadedDomainList.Length; ++index)
                    {
                        builder.Append("- ");
                        builder.AppendLine(webChecker.DownloadedDomainList[index]);
                    }
                }
                else
                {
                    ++bulletNumber;
                    builder.Append(bulletNumber);
                    builder.AppendLine(") downloading that list failed, however. The reason:");
                    builder.AppendLine(webChecker.DownloadErrorMessage);
                }
            }

            // Show unique list of domains
            ++bulletNumber;
            builder.Append(bulletNumber);
            builder.AppendLine(") together, the full domain list is as follows:");
            foreach (string domain in webChecker.AllUniqueDomains.Keys)
            {
                builder.Append("- ");
                builder.AppendLine(domain);
            }

            // Show any errors
            if (string.IsNullOrEmpty(webChecker.DownloadErrorMessage) == false)
            {
                ++bulletNumber;
                builder.Append(bulletNumber);
                builder.AppendLine(") Errors messages:");
                builder.AppendLine(webChecker.DownloadErrorMessage);
            }

            // Return URL
            return builder.ToString();
        }
    }
}
