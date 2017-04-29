using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
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
    /// <date>5/15/2016</date>
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
        public const string RemoteDomainListHeader = "Remote Domain List";
        public enum State
        {
            NotUsed = -1,
            InProgress = 0,
            EncounteredError,
            DomainMatched,
            DomainDidntMatch
        }

        public enum DownloadedFileType
        {
            Text,
            AcceptedDomainListAssetBundle
        }

        static readonly string[] AlwaysSplitDomainsBy = new string[] { "\r\n", "\n" };

        ///<summary>
        /// If it is a webplayer, then the domain must contain any
        /// one or more of these strings, or it will be redirected.
        /// This array is ignored if empty (i.e. no redirect will occur).
        ///</summary>
        [SerializeField]
        string[] domainMustContain;

        ///<summary>
        /// [optional] The URL to fetch a list of domains
        ///</summary>
        [Header(RemoteDomainListHeader)]
        [SerializeField]
        [Tooltip("[optional] The URL to fetch a list of domains")]
        string remoteDomainListUrl;
        /// <summary>
        /// The file type of this list of domains
        /// </summary>
        [SerializeField]
        [Tooltip("[optional] The file type of this list of domains")]
        DownloadedFileType remoteListFileType = DownloadedFileType.AcceptedDomainListAssetBundle;
        ///<summary>
        /// [optional] The list of strings the text list of domains will be split by.
        /// Note that the text list will already be divided by newlines.
        ///</summary>
        [SerializeField]
        [Tooltip("[optional] The list of strings the text list of domains will be split by.\nNote that the text list will already be divided by newlines.")]
        string[] splitRemoteDomainListUrlBy = new string[] { "," };
        ///<summary>
        /// [Optional] GameObjects to deactivate while the domain checking is happening
        ///</summary>
        [SerializeField]
        [Tooltip("[Optional] GameObjects to deactivate while the domain checking is happening")]
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
        string retrievedHostName = null;
        string downloadDomainsUrl = null;
        string downloadErrorMessage = null;
        string[] downloadedDomainList = null;
        string[] cachedSplitString = null;
        readonly Dictionary<string, Regex> allUniqueDomains = new Dictionary<string, Regex>();

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

        public bool IsDomainListSuccessfullyDownloaded
        {
            get
            {
                return ((DownloadedDomainList != null) && (string.IsNullOrEmpty(DownloadDomainsUrl) == false));
            }
        }

        public string RetrievedHostName
        {
            get
            {
                return retrievedHostName;
            }
        }

        public string[] DefaultDomainList
        {
            get
            {
                return domainMustContain;
            }
        }

        public string[] DownloadedDomainList
        {
            get
            {
                return downloadedDomainList;
            }
        }

        public string DownloadDomainsUrl
        {
            get
            {
                return downloadDomainsUrl;
            }
        }

        public Dictionary<string, Regex> AllUniqueDomains
        {
            get
            {
                return allUniqueDomains;
            }
        }

        public string[] SplitString
        {
            get
            {
                if(cachedSplitString == null)
                {
                    // Setup variables
                    List<string> allSplits = new List<string>();
                    HashSet<string> allUniqueSplits = new HashSet<string>();

                    // Add these two arrays into the hashset
                    AddString(AlwaysSplitDomainsBy, allSplits, allUniqueSplits);
                    AddString(splitRemoteDomainListUrlBy, allSplits, allUniqueSplits);

                    // Convert the list into an array
                    cachedSplitString = allSplits.ToArray();
                }
                return cachedSplitString;
            }
        }

        public string DownloadErrorMessage
        {
            get
            {
                return downloadErrorMessage;
            }
            private set
            {
                downloadErrorMessage = value;
            }
        }
        #endregion

        public override void SingletonAwake(Singleton instance)
        {
            if (Singleton.Instance.IsWebplayer == true)
            {
                // Shoot a coroutine if not in editor, and making Webplayer or WebGL
                StartCoroutine(CheckDomainList());
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

        #region Helper Static Methods
        static State GetNewState(StringBuilder buf, Dictionary<string, Regex> allUniqueDomains, out string retrievedHostName)
        {
            State newState = State.NotUsed;
            retrievedHostName = null;
            if (allUniqueDomains.Count > 0)
            {
                // parse the page's address
                bool isErrorEncountered = false;
                newState = State.DomainDidntMatch;
                if (IsHostMatchingListedDomain(allUniqueDomains, out isErrorEncountered, out retrievedHostName) == true)
                {
                    // Update state
                    newState = State.DomainMatched;
                }
                else if (isErrorEncountered == true)
                {
                    newState = State.EncounteredError;
                }
            }
            return newState;
        }

        static void PopulateAllUniqueDomains(StringBuilder buf, Dictionary<string, Regex> allUniqueDomains, params string[][] allDomains)
        {
            // Setup variables
            int listIndex = 0, stringIndex = 0;

            // Clear the dictionary
            allUniqueDomains.Clear();

            if(allDomains != null)
            {
                // Go through all the domains
                for (; listIndex < allDomains.Length; ++listIndex)
                {
                    if(allDomains[listIndex] != null)
                    {
                        // Go through all strings in the list
                        for (stringIndex = 0; stringIndex < allDomains[listIndex].Length; ++stringIndex)
                        {
                            // Add the entry and its regular expression equivalent
                            if(string.IsNullOrEmpty(allDomains[listIndex][stringIndex]) == false)
                            {
                                allUniqueDomains.Add(allDomains[listIndex][stringIndex], ConvertToWildCardAcceptingRegex(buf, allDomains[listIndex][stringIndex]));
                            }
                        }
                    }
                }
            }
        }

        static bool IsHostMatchingListedDomain(Dictionary<string, Regex> domainList, out bool encounteredError, out string retrievedHostName)
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
                isTheCorrectHost = true;
                if (uri.Scheme != "file")
                {
                    // Make sure host matches any one of the domains
                    isTheCorrectHost = false;
                    foreach (Regex expression in domainList.Values)
                    {
                        if (expression.IsMatch(retrievedHostName) == true)
                        {
                            isTheCorrectHost = true;
                            break;
                        }
                    }
                }
            }
            return isTheCorrectHost;
        }

        static Regex ConvertToWildCardAcceptingRegex(StringBuilder buf, string domainString)
        {
            // Reset StringBuilder
            buf.Length = 0;

            // Add forced start characters
            buf.Append('^');

            // Escape all Regex Expression
            domainString = Regex.Escape(domainString);

            // Replace ? and * with equivalent symbols
            buf.Append(domainString.Replace("\\?", ".").Replace("\\*", ".*"));

            // Add forced end characters
            buf.Append('$');

            // Create a new Regex
            return new Regex(buf.ToString(), RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        static string[] ConvertToDomainList(string text, string[] splitBy)
        {
            string[] returnDomainList = text.Split(splitBy, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < returnDomainList.Length; ++index)
            {
                // Trim out any empty spaces in each string
                returnDomainList[index] = returnDomainList[index].Trim();
            }
            return returnDomainList;
        }

        static string[] ConvertToDomainList(DomainList domainList)
        {
            string[] returnDomainList = null;
            if ((domainList != null) && (domainList.Count > 0))
            {
                // Create a new string array
                returnDomainList = new string[domainList.Count];

                // Copy each element from the domainList
                for (int index = 0; index < returnDomainList.Length; ++index)
                {
                    // Trim out any empty spaces in each string
                    returnDomainList[index] = domainList[index].Trim();
                }
            }
            return returnDomainList;
        }

        static bool IsDomainInvalid(State state)
        {
            bool returnState = false;
            switch (state)
            {
                case State.EncounteredError:
                case State.DomainDidntMatch:
                    returnState = true;
                    break;
            }
            return returnState;
        }

        static void AddString(string[] toAddArray, List<string> listToAddTo, HashSet<string> setToAddTo)
        {
            if (toAddArray != null)
            {
                string toAdd;
                for (int index = 0; index < toAddArray.Length; ++index)
                {
                    toAdd = toAddArray[index];
                    if ((string.IsNullOrEmpty(toAdd) == false) && (setToAddTo.Contains(toAdd) == false))
                    {
                        listToAddTo.Add(toAdd);
                        setToAddTo.Add(toAdd);
                    }
                }
            }
        }
        #endregion

        #region Helper Local Methods
        IEnumerator DownloadRemoteDomainList(StringBuilder buf, string remoteDomainUrl)
        {
            // Start downloading the remote file (never cache this file)
            using (WWW www = new WWW(remoteDomainListUrl))
            {
                yield return www;

                // Check if there were any errors
                DownloadErrorMessage = www.error;
                if (string.IsNullOrEmpty(DownloadErrorMessage) == true)
                {
                    // If none, check what type this downloaded file is
                    if(remoteListFileType == DownloadedFileType.Text)
                    {
                        // If text, split the text file we've downloaded, and add it to the list
                        downloadedDomainList = ConvertToDomainList(www.text, SplitString);
                    }
                    else if(www.assetBundle != null)
                    {
                        // If asset bundle, convert it into a list
                        downloadedDomainList = ConvertToDomainList(Utility.GetDomainList(www.assetBundle));
                    }
                }
            }
        }

        IEnumerator CheckDomainList()
        {
            // Setup variables
            StringBuilder buf = new StringBuilder();
            downloadedDomainList = null;
            downloadDomainsUrl = null;
            DownloadErrorMessage = null;

            // Update state
            CurrentState = State.InProgress;

            // Deactivate any objects
            SetWaitObjectActive(false);

            // Grab a domain list remotely
            if (string.IsNullOrEmpty(remoteDomainListUrl) == false)
            {
                // Grab remote domain list
                downloadDomainsUrl = GenerateRemoteDomainList(buf);
                yield return StartCoroutine(DownloadRemoteDomainList(buf, downloadDomainsUrl));
            }

            // Setup hashset
            PopulateAllUniqueDomains(buf, AllUniqueDomains, DefaultDomainList, DownloadedDomainList);

            // Make sure there's at least one domain we need to check
            CurrentState = GetNewState(buf, AllUniqueDomains, out retrievedHostName);

            // Reactivate any objects
            SetWaitObjectActive(true);

            // Check if we should force redirecting the player
            if ((forceRedirectIfDomainDoesntMatch == true) && (IsDomainInvalid(CurrentState) == true))
            {
                ForceRedirect(buf);
            }
        }

        void ForceRedirect(StringBuilder buf)
        {
            if (string.IsNullOrEmpty(redirectURL) == false)
            {
                // Create a redirect javascript command
                buf.Length = 0;
                buf.Append("window.top.location='");
                buf.Append(redirectURL);
                buf.Append("';");

                // Evaluate the javascript
                Application.ExternalEval(buf.ToString());
            }
        }

        string GenerateRemoteDomainList(StringBuilder buf)
        {
            buf.Length = 0;
            buf.Append(remoteDomainListUrl);
            buf.Append("?r=");
            buf.Append(UnityEngine.Random.Range(0, int.MaxValue));
            return buf.ToString();
        }

        void SetWaitObjectActive(bool state)
        {
            for (int index = 0; index < waitObjects.Length; ++index)
            {
                waitObjects[index].SetActive(state);
            }
        }
        #endregion
    }
}
