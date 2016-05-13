using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="WebLocationChecker.cs" company="Omiya Games">
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
    /// <date>4/21/2015</date>
    /// <author>Taro Omiya</author>
    /// <author>andyman</author>
    /// <author>jcx</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Original code by andyman from Github:<br/>
    /// https://gist.github.com/andyman/e58dea85cce23cccecff<br/>
    /// Extra modifications by jcx from Github:<br/>
    /// https://gist.github.com/jcx/93a3fc93531911add8a8<br/>
    /// Taro Omiya made a couple of changes as well.<br/>
    /// Add this script to an object in the first scene of your game.
    /// It doesn't do anything for non-webplayer builds. For webplayer
    /// builds, it checks the domain to make sure it contains at least
    /// one of the strings, or it will redirect the page to the proper
    /// URL for the game.
    /// </summary>
    public class WebLocationChecker : ISingletonScript
    {
        public enum State
        {
            NotUsed = -1,
            InProgress = 0,
            EncounteredError,
            DomainMatched,
            DomainDidntMatch
        }

        ///<summary>
        /// If it is a webplayer, then the domain must contain any
        /// one or more of these strings, or it will be redirected.
        /// This array is ignored if empty (i.e. no redirect will occur).
        ///</summary>
        [SerializeField]
        List<string> domainMustContain;

        ///<summary>
        /// (optional) or fetch the domain list from this URL
        ///</summary>
        [Header("Getting Domain List Remotely")]
        [SerializeField]
        string remoteDomainListUrl;
        ///<summary>
        /// (optional) or fetch the domain list from this URL
        ///</summary>
        [SerializeField]
        [Multiline]
        string[] splitRemoteDomainListUrlBy = new string[] { "\n", "," };
        ///<summary>
        /// (optional) game objects to deactivate while the domain checking is happening
        ///</summary>
        [SerializeField]
        GameObject[] waitObjects;

        ///<summary>
        /// If true, the game will force the webplayer to redirect to
        /// the URL below
        ///</summary>
        [Header("Redirect Options")]
        [SerializeField]
        bool forceRedirectIfDomainDoesntMatch = true;
        ///<summary>
        /// This is where to redirect the webplayer page if none of
        /// the strings in domainMustContain are found.
        ///</summary>
        [SerializeField]
        string redirectURL;

        State currentState = State.NotUsed;
        bool downloadedRemoteDomainList = false;
        string retrievedHostName = null;

        #region Properties
        public State CurrentState
        {
            get
            {
                return currentState;
            }
            private set
            {
                currentState = value;
            }
        }

        public bool IsRemoteDomainListSuccessfullyDownloaded
        {
            get
            {
                return downloadedRemoteDomainList;
            }
            private set
            {
                downloadedRemoteDomainList = value;
            }
        }

        public string RetrievedHostName
        {
            get
            {
                return retrievedHostName;
            }
        }

        public ReadOnlyCollection<string> DomainList
        {
            get
            {
                return domainMustContain.AsReadOnly();
            }
        }
        #endregion

        public override void SingletonAwake(Singleton instance)
        {
            if (Singleton.Instance.IsWebplayer == true)
            {
#if UNITY_EDITOR
                // Do a little bit of debugging
                StringBuilder buf = new StringBuilder();
                if (string.IsNullOrEmpty(remoteDomainListUrl) == false)
                {
                    // Print remote domain list URL
                    Debug.Log("Example URL to grab remote Domain List will look like:\n" + GenerateRemoteDomainList(buf));
                }
                if (string.IsNullOrEmpty(redirectURL) == false)
                {
                    // Print redirect javascript
                    Debug.Log("Redirect javascript will look like:\n" + GenerateRedirect(buf));
                }
#else
                // Shoot a coroutine if not in editor, and making Webplayer or WebGL
                StartCoroutine(CheckDomainList());
#endif
            }
        }

        public override void SceneAwake(Singleton instance)
        {
            // Do nothing
        }

        public void ForceRedirect()
        {
            ForceRedirect(new StringBuilder());
        }

        #region Helper Methods
        void ForceRedirect(StringBuilder buf)
        {
            if (string.IsNullOrEmpty(redirectURL) == false)
            {
                // Evaluate the javascript
                Application.ExternalEval(GenerateRedirect(buf));
            }
        }

        string GenerateRedirect(StringBuilder buf)
        {
            buf.Length = 0;
            buf.Append("window.top.location='");
            buf.Append(redirectURL);
            buf.Append("';");
            return buf.ToString();
        }

        IEnumerator DownloadRemoteDomainList(StringBuilder buf)
        {
            WWW www = new WWW(GenerateRemoteDomainList(buf));
            yield return www;

            // Check if there were any errors
            if (string.IsNullOrEmpty(www.error) == true)
            {
                // If none, split the text file we've downloaded, and add it to the list
                domainMustContain.AddRange(www.text.Split(splitRemoteDomainListUrlBy, StringSplitOptions.RemoveEmptyEntries));
                Utility.RemoveDuplicateEntries<string>(domainMustContain);
                IsRemoteDomainListSuccessfullyDownloaded = true;
            }
        }

        IEnumerator CheckDomainList()
        {
            // Update state
            CurrentState = State.InProgress;

            // Deactivate any objects
            int index = 0;
            for (index = 0; index < waitObjects.Length; ++index)
            {
                waitObjects[index].SetActive(false);
            }

            // Grab a domain list remotely
            StringBuilder buf = new StringBuilder();
            if (string.IsNullOrEmpty(remoteDomainListUrl) == false)
            {
                // Grab remote domain list
                yield return StartCoroutine(DownloadRemoteDomainList(buf));
            }

            // Make sure there's at least one domain we need to check
            if (domainMustContain.Count > 0)
            {
                // parse the page's address
                bool isErrorEncountered = false;
                if (IsHostMatchingListedDomain(domainMustContain, out isErrorEncountered, out retrievedHostName) == true)
                {
                    // Update state
                    CurrentState = State.DomainMatched;
                }
                else
                {
                    // Update state
                    if(isErrorEncountered == true)
                    {
                        CurrentState = State.EncounteredError;
                    }
                    else
                    {
                        CurrentState = State.DomainDidntMatch;
                    }

                    // Check if we should force redirecting the player
                    if (forceRedirectIfDomainDoesntMatch == true)
                    {
                        ForceRedirect(buf);
                    }
                }
            }
            else
            {
                // Update state
                CurrentState = State.NotUsed;
            }

            // Reactivate any objects
            for (index = 0; index < waitObjects.Length; ++index)
            {
                waitObjects[index].SetActive(true);
            }
        }

        string GenerateRemoteDomainList(StringBuilder buf)
        {
            buf.Length = 0;
            buf.Append(remoteDomainListUrl);
            buf.Append("?r=");
            buf.Append(UnityEngine.Random.value);
            return buf.ToString();
        }

        bool IsHostMatchingListedDomain(List<string> domainList, out bool encounteredError, out string retrievedHostName)
        {
            Uri uri;
            bool isTheCorrectHost = false;
            retrievedHostName = null;

            // Evaluate the URL
            encounteredError = true;
            if (Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out uri) == true)
            {
                // Indicate there were no errors
                encounteredError = false;
                retrievedHostName = uri.Host;

                // Check if the scheme isn't file (i.e. local file run on computer)
                if (uri.Scheme != "file")
                {
                    // Make sure host matches any one of the domains
                    for (int index = 0; index < domainList.Count; ++index)
                    {
                        if (retrievedHostName == domainList[index])
                        {
                            isTheCorrectHost = true;
                            break;
                        }
                    }
                }
                else
                {
                    // If this is a file run by a local computer, indicate domain matched
                    isTheCorrectHost = true;
                }
            }
            return isTheCorrectHost;
        }
        #endregion
    }
}
