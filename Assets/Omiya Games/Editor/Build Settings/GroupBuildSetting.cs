using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="GroupBuildSetting.cs" company="Omiya Games">
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
    /// <date>10/29/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A list of <code>IChildBuildSettings</code> to run sequentially.
    /// </summary>
    public class GroupBuildSetting : IChildBuildSetting
    {
        [SerializeField]
        List<IChildBuildSetting> allSettings = new List<IChildBuildSetting>();

        #region Overrides
        internal override int MaxNumberOfResults
        {
            get
            {
                // Indicate this group drops 2 statuses
                int returnNumber = 2;

                // Increment the max by all the other settings
                foreach(IBuildSetting setting in allSettings)
                {
                    returnNumber += setting.MaxNumberOfResults;
                }
                return returnNumber;
            }
        }

        public override IBuildSetting Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                base.Parent = value;
                foreach (IChildBuildSetting setting in allSettings)
                {
                    setting.Parent = this;
                }
            }
        }

        protected override void BuildBaseOnSettings(RootBuildSetting root, BuildPlayersResult results)
        {
            // Indicate group build started
            using (new GroupBuildScope(results, this))
            {
                // Build the list of settings
                BuildGroup(root, allSettings, results);
            }
        }
        #endregion

        public void Add(IChildBuildSetting addSetting)
        {
            AddSetting(this, allSettings, addSetting);
        }

        public IChildBuildSetting Remove(int index)
        {
            return RemoveSetting(allSettings, index);
        }
    }
}
