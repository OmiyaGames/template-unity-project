using UnityEngine;

namespace OmiyaGames.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IChildBuildSetting.cs" company="Omiya Games">
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
    /// Assets holding settings for creating builds.
    /// </summary>
    public abstract class IChildBuildSetting : IBuildSetting
    {
        [SerializeField]
        private IBuildSetting parentSetting = null;

        public RootBuildSetting RootSetting
        {
            get
            {
                // Grab the parent
                IBuildSetting currentParent = Parent;

                // Check if it's not null
                while (currentParent != null)
                {
                    if (currentParent is RootBuildSetting)
                    {
                        // Check if the parent is already a root
                        // If so, return this parent
                        return (RootBuildSetting)currentParent;
                    }
                    else if(currentParent is IChildBuildSetting)
                    {
                        // Check if parent is also a child.
                        // If so, loop again, this time with the parent's parent.
                        currentParent = ((IChildBuildSetting)currentParent).Parent;
                    }
                    else
                    {
                        // Otherwise, break out of the loop
                        break;
                    }
                }

                // Nothing found, return null
                return null;
            }
        }

        public virtual IBuildSetting Parent
        {
            get
            {
                return parentSetting;
            }
            set
            {
                parentSetting = value;
            }
        }
    }
}
