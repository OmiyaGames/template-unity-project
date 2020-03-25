using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEditor;
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
                if (isEntering == true)
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

        public class GroupBuildScope : System.IDisposable
        {
            private readonly BuildPlayersResult result;
            private readonly IBuildSetting setting;

            public GroupBuildScope(BuildPlayersResult result, IBuildSetting setting)
            {
                this.result = result;
                this.setting = setting;
                result.AddGroupReport(true, setting);
            }

            public void Dispose()
            {
                result.AddGroupReport(false, setting);
            }
        }
        #endregion

        private readonly List<IReport> allReports;
        private readonly List<GroupBuildSetting> allEmbeddedGroups;
        private readonly StringBuilder builder = new StringBuilder();
        private string rootFolderName;
        private string folderNameCache = null;
        string[] defaultScenesCache = null;

        public BuildPlayersResult(RootBuildSetting root, IBuildSetting info)
        {
            allReports = new List<IReport>(info.MaxNumberOfResults);
            allEmbeddedGroups = new List<GroupBuildSetting>(info.MaxNumberOfResults);

            //StringBuilder builder = new StringBuilder();
            //CustomFileName.RemoveDiacritics(builder, PlayerSettings.productName);
            //AppName = builder.ToString();
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

        public string FolderName
        {
            get
            {
                if (folderNameCache == null)
                {
                    builder.Clear();
                    builder.Append(rootFolderName);
                    foreach (GroupBuildSetting setting in allEmbeddedGroups)
                    {
                        builder.Append(Helpers.PathDivider);
                        builder.Append(setting.FolderName);
                    }
                    folderNameCache = builder.ToString();
                }
                return folderNameCache;
            }
        }

        //public string AppName
        //{
        //    get;
        //}

        /// <summary>
        /// Helper property that retrieve all the scenes from the build settings.
        /// </summary>
        public string[] DefaultScenes
        {
            get
            {
                if (defaultScenesCache == null)
                {
                    // Grab all enabled scenes
                    List<string> EditorScenes = new List<string>();
                    foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                    {
                        if (scene.enabled == true)
                        {
                            EditorScenes.Add(scene.path);
                        }
                    }
                    defaultScenesCache = EditorScenes.ToArray();
                }
                return defaultScenesCache;
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
                rootFolderName = root.GetPathPreview(builder, Helpers.PathDivider);
            }
        }

        public void AddReport(BuildReport report, IBuildSetting source)
        {
            allReports.Add(new ChildReport(report, source));
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
            if (isError == true)
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
            switch (option)
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
            foreach (string sentence in sentences)
            {
                builder.Append(sentence);
            }
            return builder.ToString();
        }

        public string ConcatenateFolders(string path, params string[] folders)
        {
            if ((folders != null) && (folders.Length > 0))
            {
                builder.Clear();
                builder.Append(path);
                foreach (string folder in folders)
                {
                    if (string.IsNullOrEmpty(folder) == false)
                    {
                        builder.Append(Helpers.PathDivider);
                        builder.Append(folder);
                    }
                }
                path = builder.ToString();
            }
            return path;
        }

        public override string ToString()
        {
            int indentLevel = 0;
            builder.Clear();
            foreach (IReport report in allReports)
            {
                // Add newline if this isn't the first report
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                // Add indentation
                AppendIndentation(ref indentLevel, report);

                // Add report's message
                AppendMessage(report, builder, ref indentLevel);
            }
            return builder.ToString();
        }

        #region Helper Methods
        private int AppendIndentation(ref int indentLevel, IReport report)
        {
            int indent;
            // Adjust indentation
            if (report.State == Status.ExitGroup)
            {
                indentLevel -= 1;
            }

            // Add indentations, if any
            for (indent = 0; indent < indentLevel; ++indent)
            {
                builder.Append("  ");
            }

            // Adjust indentation
            if (report.State == Status.EnterGroup)
            {
                indentLevel += 1;
            }

            return indent;
        }

        private void AddGroupReport(bool isEntering, IBuildSetting source)
        {
            allReports.Add(new GroupReport(isEntering, source));

            // Check if source is a Group
            if (source is GroupBuildSetting)
            {
                // Check whether to stack the group
                GroupBuildSetting groupSetting = (GroupBuildSetting)source;
                if (groupSetting.IsInEmbeddedFolder == true)
                {
                    if (isEntering == false)
                    {
                        if (allEmbeddedGroups.Count > 0)
                        {
                            allEmbeddedGroups.RemoveAt(allEmbeddedGroups.Count - 1);
                        }
                        folderNameCache = null;
                    }
                    else
                    {
                        allEmbeddedGroups.Add(groupSetting);
                        if (folderNameCache != null)
                        {
                            folderNameCache = ConcatenateFolders(folderNameCache, groupSetting.FolderName);
                        }
                    }
                }
            }
        }

        private static void AppendMessage(IReport report, StringBuilder builder, ref int indentLevel)
        {
            switch (report.State)
            {
                case Status.EnterGroup:
                    builder.Append("Entering group: ");
                    break;
                case Status.ExitGroup:
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
