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
    public partial class SocialManager : ISingletonScript
    {
        public class LeaderboardWrapper
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

            public readonly ILeaderboard reference;
            IUserProfile[] allProfiles = null;
            State currentState = State.NothingLoaded;

            internal LeaderboardWrapper(string id, UserScope userScope, TimeScope timeScope, ushort startingRank, ushort numberOfRanks)
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
                this(key.id, key.userScope, key.timeScope, key.startingRank, key.numberOfRanks)
            {
            }

            public State CurrentState
            {
                get
                {
                    return currentState;
                }
            }

            public bool IsScoresLoaded
            {
                get
                {
                    bool returnFlag = false;
                    switch(CurrentState)
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

            public bool IsProfilesLoaded
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
            public IScore UserScore
            {
                get
                {
                    IScore returnScores = null;
                    if(IsScoresLoaded == true)
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
            public IUserProfile[] Profiles
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

            string DebugId
            {
                get
                {
                    return ("id " + reference.id + ", user " + reference.userScope + ", time " + reference.timeScope);

                }
            }

            public bool LoadLeaderboardAsync(bool forceLoadingNewScores = false)
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
                    // Indicate we're loading profiles this time
                    if (Debug.isDebugBuild == true)
                    {
                        Debug.Log("Scores loaded for: " + DebugId);
                    }
                    currentState = State.AttemptingToLoadProfiles;

                    // Create a list of user IDs
                    string[] allUserIds = new string[reference.scores.Length];
                    for(int index = 0; index < allUserIds.Length; ++index)
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
                    currentState = State.FailedToLoadScoresAndProfiles;
                }
            }

            void OnProfileLoaded(IUserProfile[] profiles)
            {
                if(profiles != null)
                {
                    // Set the profile array, and make sure it's the same length as the scores
                    allProfiles = profiles;
                    if(allProfiles.Length == reference.scores.Length)
                    {
                        // Indicate success
                        if (Debug.isDebugBuild == true)
                        {
                            Debug.Log("profiles loaded for: " + DebugId);
                        }
                        currentState = State.SuccessfullyLoadedScoresAndProfiles;
                    }
                }

                if(currentState != State.SuccessfullyLoadedScoresAndProfiles)
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
                    currentState = State.FailedToLoadOnlyProfiles;
                }
            }
        }
    }
}
