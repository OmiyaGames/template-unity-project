using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Build.Reporting;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="BuildPlayersResult.cs" company="Omiya Games">
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
    /// List of results from a series of builds.
    /// </summary>
    public class BuildPlayersResult
    {
        public enum Status
        {
            EnterGroup,
            ExitGroup,
            Success,
            Cancelled,
            Error
        }

        #region Helper Classes
        public abstract class IReport
        {
            public IReport(IBuildSetting source)
            {
                Source = source;
            }

            public IBuildSetting Source
            {
                get;
            }

            public Status State
            {
                get;
                protected set;
            }
        }

        public class GroupReport : IReport
        {
            public GroupReport(bool isEntering, IBuildSetting source) : base(source)
            {
                State = Status.ExitGroup;
                if(isEntering == true)
                {
                    State = Status.EnterGroup;
                }
            }
        }

        public class ChildReport : IReport
        {
            public ChildReport(BuildReport report, IBuildSetting source) : base(source)
            {
                switch (report.summary.result)
                {
                    case BuildResult.Succeeded:
                        State = Status.Success;
                        break;
                    case BuildResult.Cancelled:
                        State = Status.Cancelled;
                        break;
                    default:
                        State = Status.Error;
                        break;
                }

                // Set property
                Report = report;
            }

            public BuildReport Report
            {
                get;
            }
        }

        public class PostBuildReport : IReport
        {
            public PostBuildReport(Status state, string message, IBuildSetting source) : base(source)
            {
                State = state;
                Message = message;
            }

            public string Message
            {
                get;
            }
        }
        #endregion

        private readonly List<IReport> allReports;
        private readonly StringBuilder builder = new StringBuilder();

        public BuildPlayersResult(RootBuildSetting root, IBuildSetting info)
        {
            allReports = new List<IReport>(info.MaxNumberOfResults);
            Setup(root);
        }

        #region Properties
        public bool IsAllBuildsCancelled
        {
            get;
            set;
        } = false;

        public RootBuildSetting.BuildProgression OnBuildFailed
        {
            get;
            set;
        } = RootBuildSetting.BuildProgression.AskWhetherToContinue;

        public RootBuildSetting.BuildProgression OnBuildCancelled
        {
            get;
            set;
        } = RootBuildSetting.BuildProgression.AskWhetherToContinue;

        public ReadOnlyCollection<IReport> AllReports
        {
            get
            {
                return allReports.AsReadOnly();
            }
        }

        public IReport LastReport
        {
            get
            {
                IReport returnReport = null;
                if (allReports.Count > 0)
                {
                    returnReport = allReports[allReports.Count - 1];
                }
                return returnReport;
            }
        }
        #endregion

        /// <summary>
        /// Sets the initial values for OnBuildCancelled and OnBuildFailed.
        /// </summary>
        /// <param name="root"></param>
        public void Setup(RootBuildSetting root)
        {
            if (root != null)
            {
                OnBuildCancelled = root.OnBuildCancelled;
                OnBuildFailed = root.OnBuildFailed;
            }
        }

        public void AddReport(BuildReport report, IBuildSetting source)
        {
            allReports.Add(new ChildReport(report, source));
        }

        public void AddGroupReport(bool isEntering, IBuildSetting source)
        {
            allReports.Add(new GroupReport(isEntering, source));
        }

        public void AddPostBuildReport(Status state, string message, IBuildSetting source)
        {
            allReports.Add(new PostBuildReport(state, message, source));
        }

        public RootBuildSetting.BuildProgression DisplayBuildProgressionDialog(bool isError)
        {
            // Setup the dialog strings
            string title, promptMeAgain;
            builder.Clear();
            if(isError == true)
            {
                title = "Build Failed";
                promptMeAgain = "Resume, But Ask Me On The Next Error";
                builder.Append("The following build failed: ");
            }
            else
            {
                title = "Build Cancelled";
                promptMeAgain = "Resume, But Ask Me On The Next Cancellation";
                builder.Append("The following build was cancelled: ");
            }
            builder.AppendLine(LastReport.Source.name);
            builder.Append("Do you want to resume the rest of the builds?");

            // Prompt the user what they want to do
            int option = UnityEditor.EditorUtility.DisplayDialogComplex(
                title, builder.ToString(),
                "Stop All", promptMeAgain, "Resume, And Don't Ask Me Again");

            // Evaluate options
            switch(option)
            {
                case 0:
                    return RootBuildSetting.BuildProgression.HaltImmediately;
                case 2:
                    return RootBuildSetting.BuildProgression.IgnoreAndResumeBuilding;
                default:
                case 1:
                    return RootBuildSetting.BuildProgression.AskWhetherToContinue;
            }
        }

        public string Concatenate(params string[] sentences)
        {
            builder.Clear();
            foreach(string sentence in sentences)
            {
                builder.Append(sentence);
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            int indentLevel = 0, indent;
            builder.Clear();
            foreach(IReport report in allReports)
            {
                // Add newline if this isn't the first report
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                // Add indentations, if any
                for (indent = 0; indent < indentLevel; ++indent)
                {
                    builder.Append("  ");
                }

                // Add report's message
                AppendMessage(report, builder, ref indentLevel);
            }
            return base.ToString();
        }

        #region Helper Methods
        private static void AppendMessage(IReport report, StringBuilder builder, ref int indentLevel)
        {
            switch (report.State)
            {
                case Status.EnterGroup:
                    indentLevel += 1;
                    builder.Append("Entering group: ");
                    break;
                case Status.ExitGroup:
                    indentLevel -= 1;
                    builder.Append("Exiting group: ");
                    break;
                case Status.Success:
                    builder.Append("Successfully built: ");
                    break;
                case Status.Cancelled:
                    builder.Append("Cancelled build: ");
                    break;
                case Status.Error:
                    builder.Append("Failed to build: ");
                    break;
            }
            builder.Append(report.Source.name);
        }
        #endregion
    }
}
