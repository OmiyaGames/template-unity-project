using UnityEngine;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections.Generic;

// Import other packages specific to device
#if GOOGLE_PLAY_GAMES
// importing Google Play Games here
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SocialManagerWrapperClasses.cs" company="Omiya Games">
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
    /// <date>4/14/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Singleton file used to authenticate with various social platforms.
    /// It also provides a social platform agnostic interface to report to 
    /// leaderboards and achievements.
    /// <code>Singleton</code>'s events.
    /// </summary>
    /// <seealso cref="UnityEngine.SocialPlatforms"/>
    public class SocialManager : ISingletonScript
    {
        public enum LogInState
        {
            NotConnected,
            MiddleOfAuthenticating,
            AuthenticationSuccess
        }

        public enum ServiceType
        {
            None = -1,
            AppleGameCenter = 0,
            GooglePlayGames,
            AmazonGameCircle,
            //XboxLive,
            //Newgrounds,
            //GameJolt,
            //Kongregate,
            NumberOfServiceTypes
        }

#if UNITY_EDITOR
        class DebugSequence
        {
            uint index = 0;
            readonly float[] secondsSequence = new float[] { 15f, 0.1f, 0.2f };
            readonly bool[] successSequence = new bool[] { true, true, false };

            public void Next()
            {
                ++index;
            }

            public float WaitForSeconds
            {
                get
                {
                    return secondsSequence[index % secondsSequence.Length];
                }
            }

            public bool ReportedSuccess
            {
                get
                {
                    return successSequence[index % successSequence.Length];
                }
            }
        }

        readonly DebugSequence sequence = new DebugSequence();
#endif

        [SerializeField]
        bool authenticateOnStartUp = true;
        [SerializeField]
        bool showAchievementBannerOnStartUp = true;
        [SerializeField]
        int numberOfAuthenticationRetries = 3;

        [Header("Leaderboard IDs")]
        [Tooltip("The first leaderboard listed will be considered the default leaderboard to report to.")]
        [SerializeField]
        ServiceSpecificIds[] leaderboardIds;

        [Header("Achievement IDs")]
        [SerializeField]
        ServiceSpecificIds[] achievementIds;

        readonly Dictionary<LeaderboardKey, ILeaderboardWrapper> allLeaderboards = new Dictionary<LeaderboardKey, ILeaderboardWrapper>();
        readonly Dictionary<string, ServiceSpecificIds> leaderboardIdsMap = new Dictionary<string, ServiceSpecificIds>();
        readonly Dictionary<string, ServiceSpecificIds> achievementIdsMap = new Dictionary<string, ServiceSpecificIds>();
        readonly System.Text.StringBuilder warningBuilder = new System.Text.StringBuilder();
        LogInState authenticationState = LogInState.NotConnected;
        Action<float> onUpdate = null;
        Action<bool> onAchievementReported = null, onLeaderboardReported = null;

        #region Properties
        public LogInState CurrentLogInState
        {
            get
            {
                LogInState returnState = authenticationState;
                if (authenticationState == LogInState.AuthenticationSuccess)
                {
                    if (Social.localUser.authenticated == true)
                    {
                        returnState = LogInState.AuthenticationSuccess;
                    }
                    else
                    {
                        returnState = LogInState.NotConnected;
                    }
                }
                return returnState;
            }
            private set
            {
                authenticationState = value;
            }
        }

        public ServiceSpecificIds DefaultLeaderboardIds
        {
            get
            {
                ServiceSpecificIds returnId = null;
                if (leaderboardIds.Length > 0)
                {
                    returnId = leaderboardIds[0];
                }
                return returnId;
            }
        }

        public static bool IsSupported
        {
            get
            {
                return (CurrentService != ServiceType.None);
            }
        }

        public static ServiceType CurrentService
        {
            get
            {
                // TODO: as more services are implemented in SetupPlatformSpecificServices(),
                // add more supported services below here
#if (UNITY_EDITOR || DEMO)
                // In the editor or demo, don't allow any leaderboard services
                return ServiceType.None;
#elif GOOGLE_PLAY_GAMES
                return ServiceType.GooglePlayGames;
#elif UNITY_IOS
                return ServiceType.AppleGameCenter;
#else
                return ServiceType.None;
#endif
            }
        }

        static void SetupPlatformSpecificServices()
        {
            // Check if we need to do anything special based on platform
#if GOOGLE_PLAY_GAMES
            // Activate the Google Play Games platform
            PlayGamesPlatform.DebugLogEnabled = Debug.isDebugBuild;
            PlayGamesPlatform.Activate();
#endif
        }
        #endregion

        public bool LogInAsync(bool forceLogin = false)
        {
            bool returnIsAttemptingAuthentication = false;

            if ((IsSupported == true) && ((CurrentLogInState == LogInState.NotConnected) || (forceLogin == true)))
            {
                // Setup any platform specific quirks
                SetupPlatformSpecificServices();

                // Check if this manager supports this specific platform
                CurrentLogInState = LogInState.NotConnected;

                // Setup all flags
                returnIsAttemptingAuthentication = true;
                CurrentLogInState = LogInState.MiddleOfAuthenticating;

                // Start authentication process
                if (Debug.isDebugBuild == true)
                {
                    Debug.Log("Starting authentication!");
                }
                Social.localUser.Authenticate(OnAuthenticationComplete);
            }
            return returnIsAttemptingAuthentication;
        }

        public ServiceSpecificIds GetLeaderboardIds(string key)
        {
            return GetIds(key, leaderboardIdsMap, leaderboardIds);
        }

        public ServiceSpecificIds GetAchievementIds(string key)
        {
            return GetIds(key, achievementIdsMap, achievementIds);
        }

        public bool ReportAchievementProgressAsync(double record, string achievementKey, Action<bool> onResultsReceived = null)
        {
            bool returnIsAttemptingReport = false;
            ServiceSpecificIds requestedAchievementIds = null;

            // Clamp record
            if (record < 0)
            {
                record = 0;
            }
            else if (record > 100)
            {
                record = 100;
            }

            // Grab an achievement
            if (string.IsNullOrEmpty(achievementKey) == false)
            {
                requestedAchievementIds = GetAchievementIds(achievementKey);
            }

            // Make sure all the parameters are correct
            if (IsArgumentValid<double>("Achievement", requestedAchievementIds, achievementKey, record, onResultsReceived) == true)
            {
                // Grab the action to report to
                if (onResultsReceived == null)
                {
                    if (onAchievementReported == null)
                    {
                        onAchievementReported = new Action<bool>(OnAchievementReported);
                    }
                    onResultsReceived = onAchievementReported;
                }

                // Report score
                Social.ReportProgress(requestedAchievementIds.Id, record, onResultsReceived);

                // Indicate attempt succeeded
                returnIsAttemptingReport = true;

                // Show debugging messages
                if (Debug.isDebugBuild == true)
                {
                    lock (warningBuilder)
                    {
                        warningBuilder.Length = 0;
                        warningBuilder.Append("Reporting Achievement with key ");
                        warningBuilder.Append(achievementKey);
                        warningBuilder.Append(" and progress ");
                        warningBuilder.Append(record);
                        Debug.Log(warningBuilder.ToString());
                    }
                }
            }
            return returnIsAttemptingReport;
        }

        public bool RevealHiddenAchievementAsync(string achievementKey, Action<bool> onResultsReceived = null)
        {
            return ReportAchievementProgressAsync(0, achievementKey, onResultsReceived);
        }

        public bool UnlockAchievementAsync(string achievementKey, Action<bool> onResultsReceived = null)
        {
            return ReportAchievementProgressAsync(100, achievementKey, onResultsReceived);
        }

        public bool ReportLeaderboardRecordAsync(long record, string leaderboardKey = null, Action<bool> onResultsReceived = null)
        {
            bool returnIsAttemptingReport = false;
            ServiceSpecificIds requestedLeaderboardIds = DefaultLeaderboardIds;
            if (string.IsNullOrEmpty(leaderboardKey) == false)
            {
                requestedLeaderboardIds = GetLeaderboardIds(leaderboardKey);
            }

            // Make sure all the parameters are correct
            if (IsArgumentValid<long>("Leaderboard", requestedLeaderboardIds, leaderboardKey, record, onResultsReceived) == true)
            {
                // Grab the action to report to
                if (onResultsReceived == null)
                {
                    if (onLeaderboardReported == null)
                    {
                        onLeaderboardReported = new Action<bool>(OnLeaderboardReported);
                    }
                    onResultsReceived = onLeaderboardReported;
                }

                // Report score
                Social.ReportScore(record, requestedLeaderboardIds.Id, onResultsReceived);

                // Indicate attempt succeeded
                returnIsAttemptingReport = true;

                // Show debugging messages
                if (Debug.isDebugBuild == true)
                {
                    lock (warningBuilder)
                    {
                        warningBuilder.Length = 0;
                        warningBuilder.Append("Reporting Leaderboard with key ");
                        warningBuilder.Append(leaderboardKey);
                        warningBuilder.Append(" and score ");
                        warningBuilder.Append(record);
                        Debug.Log(warningBuilder.ToString());
                    }
                }
            }
            return returnIsAttemptingReport;
        }

        public ILeaderboardWrapper GetLeaderboard(string leaderboardKey = null,
            UserScope userScope = UserScope.Global, TimeScope timeScope = TimeScope.AllTime,
            ushort startingRank = 0, ushort numberOfRanks = 0, bool retrieveScore = true)
        {
            ILeaderboardWrapper returnWrapper = null;
            ServiceSpecificIds leaderboardIds = DefaultLeaderboardIds;
            if (string.IsNullOrEmpty(leaderboardKey) == false)
            {
                leaderboardIds = GetLeaderboardIds(leaderboardKey);
            }

            // Check if the ID is provided
            if (leaderboardIds == null)
            {
                if (Debug.isDebugBuild == true)
                {
                    Debug.LogWarning("No Leaderboard with key " + leaderboardKey + " found");
                }
            }
            else if (string.IsNullOrEmpty(leaderboardIds.Id) == true)
            {
                if (Debug.isDebugBuild == true)
                {
                    Debug.LogWarning("No Leaderboard ID provided");
                }
            }
            else
            {
                // Find a LeaderboardWrapper from the dictionary
                LeaderboardKey key = new LeaderboardKey(leaderboardIds.Id, userScope, timeScope, startingRank, numberOfRanks);
                if (allLeaderboards.TryGetValue(key, out returnWrapper) == false)
                {
                    // Create a new wrapper, and add it to the dictionary
                    returnWrapper = new LeaderboardWrapper(key);
                    allLeaderboards.Add(key, returnWrapper);

                    // Check if we should retrive scores for this leaderboard
                    if ((retrieveScore == true) && (CurrentLogInState == LogInState.AuthenticationSuccess))
                    {
                        returnWrapper.LoadLeaderboardAsync();
                    }
                }
            }
            return returnWrapper;
        }

        #region Singleton Overrides
        public override void SingletonAwake(Singleton instance)
        {
            // Setup flag
            CurrentLogInState = LogInState.NotConnected;

            // Setup Banner
            SetupAchievementBanner(showAchievementBannerOnStartUp);

            // If this script is enabled, start authentication
            if ((authenticateOnStartUp == true) && (IsSupported == true))
            {
                // Sign to Singleton's update function
                OnDestroy();
                onUpdate = new System.Action<float>(OnEveryFrame);
                instance.OnUpdate += onUpdate;
            }
        }

        public override void SceneAwake(Singleton instance)
        {
        }
        #endregion

        #region Event Listeners
        void OnDestroy()
        {
            if (onUpdate != null)
            {
                Singleton.Instance.OnUpdate -= onUpdate;
                onUpdate = null;
            }
        }

        void OnEveryFrame(float deltaTime)
        {
            if (CurrentLogInState == LogInState.NotConnected)
            {
                if (numberOfAuthenticationRetries > 0)
                {
                    // Login
                    LogInAsync(true);

                    // Decrement the number of attempts to authenticate
                    --numberOfAuthenticationRetries;
                }
                else
                {
                    // We've went through all the retries
                    // Stop attempting to authenticate
                    OnDestroy();
                }
            }
        }

        // This function gets called when Authenticate completes
        // Note that if the operation is successful, Social.localUser will contain data from the server. 
        void OnAuthenticationComplete(bool success)
        {
            // Check if authentication succeeded
            if (success == true)
            {
                // Stop attempting to authenticate
                if (authenticateOnStartUp == true)
                {
                    OnDestroy();
                }

                // Indicate success
                CurrentLogInState = LogInState.AuthenticationSuccess;
                if (Debug.isDebugBuild == true)
                {
                    Debug.Log("Authentication success!");
                }
            }
            else
            {
                // Indicate failure
                CurrentLogInState = LogInState.NotConnected;
                if (Debug.isDebugBuild == true)
                {
                    Debug.Log("Failed to authenticate");
                }
            }
        }

        void OnLeaderboardReported(bool success)
        {
            // Check if report succeeded
            if (Debug.isDebugBuild == true)
            {
                if (success == true)
                {
                    Debug.Log("Successfully recorded to a leaderboard!");
                }
                else
                {
                    Debug.Log("Failed to record to a leaderboard...");
                }
            }
        }

        void OnAchievementReported(bool success)
        {
            // Check if report succeeded
            if (Debug.isDebugBuild == true)
            {
                if (success == true)
                {
                    Debug.Log("Successfully recorded to an achivement!");
                }
                else
                {
                    Debug.Log("Failed to record to an achievement...");
                }
            }
        }
        #endregion

        #region Helpers
        static void SetupAchievementBanner(bool isVisible)
        {
#if !UNITY_EDITOR && UNITY_IOS
            GameCenterPlatform.ShowDefaultAchievementCompletionBanner(isVisible);
#endif
        }

        ServiceSpecificIds GetIds(string key, Dictionary<string, ServiceSpecificIds> dictionary, ServiceSpecificIds[] list)
        {
            ServiceSpecificIds returnId = null;

            // First, check if the leaderboard dictionary needs to be populated
            if (dictionary.Count != list.Length)
            {
                dictionary.Clear();
                for (int index = 0; index < list.Length; ++index)
                {
                    dictionary.Add(list[index].Key, list[index]);
                }
            }

            // Next, see if we can grab a leaderboard
            if (dictionary.TryGetValue(key, out returnId) == false)
            {
                returnId = null;
            }
            return returnId;
        }

        bool IsArgumentValid<T>(string type, ServiceSpecificIds ids, string achievementKey, T record, Action<bool> debug)
        {
            bool returnFlag = true;

#if UNITY_EDITOR
            // Return false
            returnFlag = false;

            // Pretend to run the results, anyway
            StartCoroutine(DebugUnsupportedPlatforms<T>(achievementKey, record, debug));
#else
            if (ids == null)
            {
                // Return false
                returnFlag = false;

                // Generate warning
                lock(warningBuilder)
                {
                    warningBuilder.Length = 0;
                    warningBuilder.Append("No ");
                    warningBuilder.Append(type);
                    warningBuilder.Append(" with key ");
                    warningBuilder.Append(achievementKey);
                    warningBuilder.Append("found!");
                    Debug.LogWarning(warningBuilder.ToString());
                }
            }
            else if (string.IsNullOrEmpty(ids.Id) == true)
            {
                // Return false
                returnFlag = false;

                // Generate warning
                lock (warningBuilder)
                {
                    warningBuilder.Length = 0;
                    warningBuilder.Append("No ");
                    warningBuilder.Append(type);
                    warningBuilder.Append(" ID provided");
                    Debug.LogWarning(warningBuilder.ToString());
                }
            }
            else if (CurrentLogInState != LogInState.AuthenticationSuccess)
            {
                // Return false
                returnFlag = false;

                // Generate warning
                Debug.LogWarning("User is not authenticated yet");
            }
#endif
            return returnFlag;
        }

#if UNITY_EDITOR
        System.Collections.IEnumerator DebugUnsupportedPlatforms<T>(string key, T record, Action<bool> debug)
        {
            sequence.Next();
            float seconds = sequence.WaitForSeconds;
            bool reportSuccess = sequence.ReportedSuccess;

            // Wait
            yield return new WaitForSeconds(seconds);

            // Print info
            if (Debug.isDebugBuild == true)
            {
                lock (warningBuilder)
                {
                    warningBuilder.Length = 0;
                    warningBuilder.Append("Waited ");
                    warningBuilder.Append(seconds);
                    warningBuilder.Append(" seconds, pretending to report to ");
                    warningBuilder.Append(key);
                    warningBuilder.Append(" with record ");
                    warningBuilder.Append(record);
                    warningBuilder.Append(" and success ");
                    warningBuilder.Append(reportSuccess);
                    Debug.Log(warningBuilder.ToString());
                }
            }
            if (debug != null)
            {
                debug(reportSuccess);
            }
        }
#endif
        #endregion
    }
}
