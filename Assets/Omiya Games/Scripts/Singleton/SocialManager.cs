using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;

// Import other packages specific to device
#if GOOGLE_PLAY_GAMES
// importing Google Play Games here
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace OmiyaGames
{
    public class SocialManager : ISingletonScript
    {
        public enum LogInState
        {
            NotConnected,
            MiddleOfAuthenticating,
            AuthenticationSuccess
        }

        public enum ScoreReportState
        {
            NoReport,
            MiddleOfReportingScore,
            ScoreReportSucceeded,
            ScoreReportFailed
        }

        public enum ServiceType
        {
            None = -1,
            AppleGameCenter = 0,
            GooglePlayGames,
            AmazonGameCircle,
            XboxLive,
            Newgrounds,
            GameJolt,
            Kongregate,
            NumberOfServiceTypes
        }

        public class LeaderboardWrapper
        {
            public enum State
            {
                NothingLoaded,
                AttemptingToLoadScores,
                ScoresLoaded,
                FailedToLoadScores
            }

            public readonly ILeaderboard reference;
            State currentState = State.NothingLoaded;

            internal LeaderboardWrapper(string id,
                UserScope userScope, TimeScope timeScope, 
                ushort startingRank, ushort numberOfRanks)
            {
                // Setup Leaderboard
                reference = Social.CreateLeaderboard();
                reference.id = id;
                reference.userScope = userScope;
                reference.timeScope = timeScope;

                // Update the range
                Range newRange = reference.range;
                if(startingRank > 0)
                {
                    newRange.from = startingRank;
                }
                if(numberOfRanks > 0)
                {
                    newRange.count = numberOfRanks;
                }
                reference.range = newRange;
            }

            internal LeaderboardWrapper(LeaderboardKey key) :
                this(key.id, key.userScope, key.timeScope,
                key.startingRank, key.numberOfRanks)
            {
            }

            public State CurrentState
            {
                get
                {
                    return currentState;
                }
            }

            /// <summary>
            /// Gets the local user's score.
            /// </summary>
            /// <value>The list of scores.  If null, no scores has been loaded yet.</value>
            public IScore UserScore
            {
                get
                {
                    IScore returnScores = null;
                    if(CurrentState == State.ScoresLoaded)
                    {
                        returnScores = reference.localUserScore;
                    }
                    return returnScores;
                }
            }

            /// <summary>
            /// Gets the scores.
            /// </summary>
            /// <value>The list of scores.  If null, no scores has been loaded yet.</value>
            public IScore[] Scores
            {
                get
                {
                    IScore[] returnScores = null;
                    if(CurrentState == State.ScoresLoaded)
                    {
                        returnScores = reference.scores;
                    }
                    return returnScores;
                }
            }

            string DebugId
            {
                get
                {
                    return ("id " + reference.id + ", user " + reference.userScope + ", time " + reference.timeScope);

                }
            }

            public bool LoadScoresAsync(bool forceLoadingNewScores = false)
            {
                bool returnFlag = false;

                // Check if the leaderboard is created, and scores haven't been loaded yet (or we want to force the score loading)
                if((CurrentState == State.NothingLoaded) || (forceLoadingNewScores == true))
                {
                    // Setup flags
                    returnFlag = true;
                    currentState = State.AttemptingToLoadScores;

                    // Start loading in scores
                    if (Debug.isDebugBuild == true)
                    {
                        Debug.Log("Loading Scores for: " + DebugId);
                    }
                    reference.LoadScores(OnScoresLoaded);
                }
                return returnFlag;
            }

            void OnScoresLoaded(bool isSuccess)
            {
                if(isSuccess == true)
                {
                    currentState = State.ScoresLoaded;
                    if (Debug.isDebugBuild == true)
                    {
                        Debug.Log("Scores loaded for: " + DebugId);
                    }
                }
                else
                {
                    currentState = State.FailedToLoadScores;
                    if (Debug.isDebugBuild == true)
                    {
                        Debug.Log("Failed to loads scores for: " + DebugId);
                    }
                }
            }
        }

        internal struct LeaderboardKey
        {
            public readonly string id;
            public readonly UserScope userScope;
            public readonly TimeScope timeScope;
            public readonly ushort startingRank;
            public readonly ushort numberOfRanks;

            public LeaderboardKey(string id, UserScope userScope, TimeScope timeScope, ushort startingRank, ushort numberOfRanks)
            {
                this.id = id;
                this.userScope = userScope;
                this.timeScope = timeScope;
                this.startingRank = startingRank;
                this.numberOfRanks = numberOfRanks;
            }
        }

        [SerializeField]
        bool authenticateOnStartUp = true;
        [SerializeField]
        int numberOfAuthenticationRetries = 3;

#pragma warning disable 0414
        [Header("Leaderboard IDs")]
        // TODO: add support for Xbox Live
        [SerializeField]
        [Tooltip("Leaderboard ID for Windows 10 and Xbox devices")]
        string defaultXboxLiveLeaderboardId = "";
        [SerializeField]
        [Tooltip("Leaderboard ID for iOS and Mac devices")]
        string defaultAppleGameCenterLeaderboardId = "";
        [SerializeField]
        [Tooltip("Leaderboard ID for Android devices")]
        string defaultGooglePlayGamesLeaderboardId = "";
        // TODO: add support for Amazon
        [SerializeField]
        [Tooltip("Leaderboard ID for Amazon devices (requires AMAZON macro defined)")]
        string defaultAmazonGameCircleLeaderboardId = "";
        // TODO: add support for Newgrounds
        [SerializeField]
        [Tooltip("Leaderboard ID for Newgrounds Web Portal (requires NEWGROUNDS macro defined)")]
        string defaultNewgroundsLeaderboardId = "";
        // TODO: add support for GameJolt
        [SerializeField]
        [Tooltip("Leaderboard ID for GameJolt Web Portal (requires GAMEJOLT macro defined)")]
        string defaultGameJoltLeaderboardId = "";
        // TODO: add support for Kongregate
        [SerializeField]
        [Tooltip("Leaderboard ID for Kongregate Web Portal (requires KONGREGATE macro defined)")]
        string defaultKongregateLeaderboardId = "";
#pragma warning restore 0414

        readonly Dictionary<LeaderboardKey, LeaderboardWrapper> allLeaderboards = new Dictionary<LeaderboardKey, LeaderboardWrapper>();
        LogInState authenticationState = LogInState.NotConnected;
        ScoreReportState scoreReportState = ScoreReportState.NoReport;
        System.Action<float> onUpdate = null;
        int currentNumberOfAuthenticationRetries = 0;

        #region Properties
        public LogInState CurrentLogInState
        {
            get
            {
                LogInState returnState = authenticationState;
                if(authenticationState == LogInState.AuthenticationSuccess)
                {
                    if(Social.localUser.authenticated == true)
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

        public ScoreReportState CurrentScoreReportState
        {
            get
            {
                return scoreReportState;
            }
            private set
            {
                scoreReportState = value;
            }
        }

        public string DefaultLeaderboardId
        {
            get
            {
                switch(CurrentService)
                {
                    case ServiceType.AppleGameCenter:
                        return defaultAppleGameCenterLeaderboardId;
                    case ServiceType.GooglePlayGames:
                        return defaultGooglePlayGamesLeaderboardId;
                    case ServiceType.AmazonGameCircle:
                        return defaultAmazonGameCircleLeaderboardId;
                    case ServiceType.XboxLive:
                        return defaultXboxLiveLeaderboardId;
                    case ServiceType.Newgrounds:
                        return defaultNewgroundsLeaderboardId;
                    case ServiceType.GameJolt:
                        return defaultGameJoltLeaderboardId;
                    case ServiceType.Kongregate:
                        return defaultKongregateLeaderboardId;
                    default:
                        return string.Empty;
                }
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
#if GOOGLE_PLAY_GAMES
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

            if((IsSupported == true) && ((CurrentLogInState == LogInState.NotConnected) || (forceLogin == true)))
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

        public bool ReportScoreAsync(long score, string leaderboardId = null)
        {
            bool returnIsAttemptingReport = false;

            // Check if the leaderboard ID is empty
            if (string.IsNullOrEmpty(leaderboardId) == true)
            {
                // Replace this parameter with default leaderboard ID
                leaderboardId = DefaultLeaderboardId;
            }

            // Make sure all the parameters are correct
            if(CurrentScoreReportState == ScoreReportState.MiddleOfReportingScore)
            {
                if(Debug.isDebugBuild == true)
                {
                    Debug.LogWarning("Still in the middle of reporting the last score");
                }
            }
            else if (string.IsNullOrEmpty(leaderboardId) == true)
            {
                if(Debug.isDebugBuild == true)
                {
                    Debug.LogWarning("No Leaderboard ID provided");
                }
            }
            else if(CurrentLogInState != LogInState.AuthenticationSuccess)
            {
                if(Debug.isDebugBuild == true)
                {
                    Debug.LogWarning("Local use is not authenticated yet");
                }
            }
            else
            {
                returnIsAttemptingReport = true;
                CurrentScoreReportState = ScoreReportState.MiddleOfReportingScore;
                Social.ReportScore(score, leaderboardId, OnScoresReported);
            }
            return returnIsAttemptingReport;
        }

        public LeaderboardWrapper GetLeaderboard(string leaderboardId = null,
            UserScope? userScope = null, TimeScope timeScope = TimeScope.AllTime,
            ushort startingRank = 0, ushort numberOfRanks = 0, bool retrieveScore = true)
        {
            LeaderboardWrapper returnWrapper = null;

            // Check if the leaderboard ID is provided
            if (string.IsNullOrEmpty(leaderboardId) == true)
            {
                // If not, replace this parameter with default leaderboard ID
                leaderboardId = DefaultLeaderboardId;
            }

            // Check if the user scope is provided
            if (userScope.HasValue == false)
            {
                // If not, replace this parameter with default leaderboard ID
                userScope = Singleton.Get<GameSettings>().LeaderboardUserScope;
            }

            // Check if the ID is provided
            if (string.IsNullOrEmpty(leaderboardId) == true)
            {
                Debug.LogWarning("No Leaderboard ID provided");
            }
            else
            {
                // Find a LeaderboardWrapper from the dictionary
                LeaderboardKey key = new LeaderboardKey(leaderboardId, userScope.Value, timeScope, startingRank, numberOfRanks);
                if(allLeaderboards.TryGetValue(key, out returnWrapper) == false)
                {
                    // Create a new wrapper, and add it to the dictionary
                    returnWrapper = new LeaderboardWrapper(key);
                    allLeaderboards.Add(key, returnWrapper);

                    // Check if we should retrive scores for this leaderboard
                    if((retrieveScore == true) && (CurrentLogInState == LogInState.AuthenticationSuccess))
                    {
                        returnWrapper.LoadScoresAsync();
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
            CurrentScoreReportState = ScoreReportState.NoReport;
            currentNumberOfAuthenticationRetries = 0;

            // If this script is enabled, start authentication
            if (authenticateOnStartUp == true)
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
            if(onUpdate != null)
            {
                Singleton.Instance.OnUpdate -= onUpdate;
                onUpdate = null;
            }
        }

        void OnEveryFrame(float deltaTime)
        {
            switch(CurrentLogInState)
            {
                case LogInState.NotConnected:
                {
                    if(currentNumberOfAuthenticationRetries < numberOfAuthenticationRetries)
                    {
                        // Increment the number of attempts to authenticate
                        ++currentNumberOfAuthenticationRetries;

                        // Login (this will change the CurrentLoginState)
                        LogInAsync(true);
                    }
                    else
                    {
                        // We've went through all the retries
                        // Stop attempting to authenticate
                        OnDestroy();
                    }
                    break;
                }
                case LogInState.AuthenticationSuccess:
                {
                    if(currentNumberOfAuthenticationRetries > 0)
                    {
                        // Revert the number of authentication back to 0
                        currentNumberOfAuthenticationRetries = 0;
                    }
                    break;
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
                if(authenticateOnStartUp == true)
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

        void OnScoresReported(bool success)
        {
            // Check if report succeeded
            if(success == true)
            {
                CurrentScoreReportState = ScoreReportState.ScoreReportSucceeded;
                if (Debug.isDebugBuild == true)
                {
                    Debug.Log("Succeeded reporting score!");
                }
            }
            else
            {
                CurrentScoreReportState = ScoreReportState.ScoreReportFailed;
                if (Debug.isDebugBuild == true)
                {
                    Debug.Log("Failed to report score");
                }
            }
        }
        #endregion
    }
}
