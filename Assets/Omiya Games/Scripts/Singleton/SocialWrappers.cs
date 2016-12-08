using UnityEngine;
using UnityEngine.SocialPlatforms;

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
    /// File containing wrapper code for certain Social-related functionalities.
    /// <code>Singleton</code>'s events.
    /// </summary>
    /// <seealso cref="SocialManager"/>
    /// <seealso cref="UnityEngine.SocialPlatforms"/>
    public abstract class ILeaderboardWrapper
    {
        public enum State
        {
            NothingLoaded,
            AttemptingToLoadScores,
            AttemptingToLoadProfiles,
            SuccessfullyLoadedScoresAndProfiles,
            FailedToLoadScoresAndProfiles,
            FailedToLoadOnlyProfiles
        }

        State currentState = State.NothingLoaded;

        public State CurrentState
        {
            get
            {
                return currentState;
            }
            protected set
            {
                currentState = value;
            }
        }

        public string DebugId
        {
            get
            {
                return ("id " + Reference.id + ", user " + Reference.userScope + ", time " + Reference.timeScope);
            }
        }

        public abstract ILeaderboard Reference
        {
            get;
        }

        public abstract bool IsScoresLoaded
        {
            get;
        }

        public abstract bool IsProfilesLoaded
        {
            get;
        }

        /// <summary>
        /// Gets the local user's score.
        /// </summary>
        /// <value>The list of scores.  If null, no scores has been loaded yet.</value>
        public abstract IScore UserScore
        {
            get;
        }

        /// <summary>
        /// Gets the scores.
        /// </summary>
        /// <value>The list of scores.  If null, no scores has been loaded yet.</value>
        public abstract IScore[] Scores
        {
            get;
        }

        /// <summary>
        /// Gets the profiles.
        /// </summary>
        /// <value>The list of user profiles.  If null, no profiles has been loaded yet.</value>
        public abstract IUserProfile[] Profiles
        {
            get;
        }

        public bool LoadLeaderboardAsync()
        {
            return LoadLeaderboardAsync(false);
        }

        public abstract bool LoadLeaderboardAsync(bool forceLoadingNewScores);
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

    internal class LeaderboardWrapper : ILeaderboardWrapper
    {
        readonly ILeaderboard reference;
        IUserProfile[] allProfiles = null;

        internal LeaderboardWrapper(string id, UserScope userScope, TimeScope timeScope, ushort startingRank, ushort numberOfRanks)
        {
            // Setup Leaderboard
            reference = Social.CreateLeaderboard();
            reference.id = id;
            reference.userScope = userScope;
            reference.timeScope = timeScope;

            // Update the range
            Range newRange = reference.range;
            if (startingRank > 0)
            {
                newRange.from = startingRank;
            }
            if (numberOfRanks > 0)
            {
                newRange.count = numberOfRanks;
            }
            reference.range = newRange;
        }

        internal LeaderboardWrapper(LeaderboardKey key) :
            this(key.id, key.userScope, key.timeScope, key.startingRank, key.numberOfRanks)
        {
        }

        public override ILeaderboard Reference
        {
            get
            {
                return reference;
            }
        }

        public override bool IsScoresLoaded
        {
            get
            {
                bool returnFlag = false;
                switch (CurrentState)
                {
                    case State.AttemptingToLoadProfiles:
                    case State.FailedToLoadOnlyProfiles:
                    case State.SuccessfullyLoadedScoresAndProfiles:
                        returnFlag = true;
                        break;
                }
                return returnFlag;
            }
        }

        public override bool IsProfilesLoaded
        {
            get
            {
                bool returnFlag = false;
                switch (CurrentState)
                {
                    case State.SuccessfullyLoadedScoresAndProfiles:
                        returnFlag = true;
                        break;
                }
                return returnFlag;
            }
        }

        /// <summary>
        /// Gets the local user's score.
        /// </summary>
        /// <value>The list of scores.  If null, no scores has been loaded yet.</value>
        public override IScore UserScore
        {
            get
            {
                IScore returnScores = null;
                if (IsScoresLoaded == true)
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
        public override IScore[] Scores
        {
            get
            {
                IScore[] returnScores = null;
                if (IsScoresLoaded == true)
                {
                    returnScores = reference.scores;
                }
                return returnScores;
            }
        }

        /// <summary>
        /// Gets the profiles.
        /// </summary>
        /// <value>The list of user profiles.  If null, no profiles has been loaded yet.</value>
        public override IUserProfile[] Profiles
        {
            get
            {
                IUserProfile[] returnProfiles = null;
                if (IsProfilesLoaded == true)
                {
                    returnProfiles = allProfiles;
                }
                return returnProfiles;
            }
        }

        public override bool LoadLeaderboardAsync(bool forceLoadingNewScores)
        {
            bool returnFlag = false;

            // Check if the leaderboard is created, and scores haven't been loaded yet (or we want to force the score loading)
            if ((CurrentState == State.NothingLoaded) || (forceLoadingNewScores == true))
            {
                // Setup flags
                returnFlag = true;
                CurrentState = State.AttemptingToLoadScores;

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
            if (isSuccess == true)
            {
                // Indicate we're loading profiles this time
                if (Debug.isDebugBuild == true)
                {
                    Debug.Log("Scores loaded for: " + DebugId);
                }
                CurrentState = State.AttemptingToLoadProfiles;

                // Create a list of user IDs
                string[] allUserIds = new string[reference.scores.Length];
                for (int index = 0; index < allUserIds.Length; ++index)
                {
                    allUserIds[index] = reference.scores[index].userID;
                }

                // Load all the user profiles
                Social.LoadUsers(allUserIds, OnProfileLoaded);
            }
            else
            {
                // Indicate scores failed to load
                if (Debug.isDebugBuild == true)
                {
                    Debug.Log("Failed to loads scores for: " + DebugId);
                }
                CurrentState = State.FailedToLoadScoresAndProfiles;
            }
        }

        void OnProfileLoaded(IUserProfile[] profiles)
        {
            if (profiles != null)
            {
                // Set the profile array, and make sure it's the same length as the scores
                allProfiles = profiles;
                if (allProfiles.Length == reference.scores.Length)
                {
                    // Indicate success
                    if (Debug.isDebugBuild == true)
                    {
                        Debug.Log("profiles loaded for: " + DebugId);
                    }
                    CurrentState = State.SuccessfullyLoadedScoresAndProfiles;
                }
            }

            if (CurrentState != State.SuccessfullyLoadedScoresAndProfiles)
            {
                // Indicate failure
                if (Debug.isDebugBuild == true)
                {
                    if (profiles == null)
                    {
                        Debug.Log("Profiles failed to load for: " + DebugId);
                    }
                    else
                    {
                        Debug.Log("Length of profiles array did not match scores array for: " + DebugId);
                    }
                }
                CurrentState = State.FailedToLoadOnlyProfiles;
            }
        }
    }

    [System.Serializable]
    public class ServiceSpecificIds
    {
        [SerializeField]
        [Tooltip("Common key to reference")]
        string key;

#pragma warning disable 0414
        [SerializeField]
        [Tooltip("ID for iOS and Mac devices")]
        string AppleGameCenter;
        [SerializeField]
        [Tooltip("ID for Android devices (requires GOOGLE_PLAY_GAMES macro defined)")]
        string GooglePlayGames;
        //// TODO: add support for Amazon
        //[SerializeField]
        //[Tooltip("ID for Amazon devices (requires AMAZON_GAME_CIRCLE macro defined)")]
        //string AmazonGameCircle;
        //// TODO: add support for Xbox Live
        //[SerializeField]
        //[Tooltip("ID for Windows 10 and Xbox devices")]
        //string XboxLive;
        //// TODO: add support for Newgrounds
        //[SerializeField]
        //[Tooltip("ID for Newgrounds Web Portal (requires NEWGROUNDS macro defined)")]
        //string Newgrounds;
        //// TODO: add support for GameJolt
        //[SerializeField]
        //[Tooltip("ID for GameJolt Web Portal (requires GAMEJOLT macro defined)")]
        //string GameJolt;
        //// TODO: add support for Kongregate
        //[SerializeField]
        //[Tooltip("ID for Kongregate Web Portal (requires KONGREGATE macro defined)")]
        //string Kongregate;
#pragma warning restore 0414

        public ServiceSpecificIds(string key)
        {
            this.key = key;
        }

        public string Key
        {
            get
            {
                return key;
            }
        }

        public string Id
        {
            get
            {
                switch (SocialManager.CurrentService)
                {
                    case SocialManager.ServiceType.AppleGameCenter:
                        return AppleGameCenter;
                    case SocialManager.ServiceType.GooglePlayGames:
                        return GooglePlayGames;
                    //case SocialManager.ServiceType.AmazonGameCircle:
                    //    return AmazonGameCircle;
                    //case ServiceType.XboxLive:
                    //    return XboxLive;
                    //case ServiceType.Newgrounds:
                    //    return Newgrounds;
                    //case ServiceType.GameJolt:
                    //    return GameJolt;
                    //case ServiceType.Kongregate:
                    //    return Kongregate;
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
