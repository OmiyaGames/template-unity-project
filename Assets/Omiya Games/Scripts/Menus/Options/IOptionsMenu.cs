using UnityEngine;
using UnityEngine.UI;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IOptionsMenu.cs" company="Omiya Games">
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
    /// <date>6/29/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A Helper script for other options menu.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>6/29/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DisallowMultipleComponent]
    public abstract class IOptionsMenu : IMenu
    {
        [Header("Common Options Controls")]
        [SerializeField]
        MenuNavigator navigator;

        public static void SetupDividers(GameObject[] allDividers, params SupportedPlatforms[] allSupportFlags)
        {
            // Make sure all arguments are populated
            if ((allDividers != null) && (allDividers.Length > 0) && (allSupportFlags != null) && (allSupportFlags.Length > 0))
            {
                // Throw errors if the number of elements are not what's expected
                if (allDividers.Length != (allSupportFlags.Length - 1))
                {
                    throw new System.ArgumentException("Expected argument allDividers' length to be one less than the length of allSupportedFlags.");
                }

                // Deactivate all dividers
                for (int index = 0; index < allDividers.Length; ++index)
                {
                    allDividers[index].SetActive(false);
                }

                // Loop through all the sections
                int numVisibleControlSets = 0;
                for (int index = 0; ((index < allSupportFlags.Length) && ((index - 1) < allDividers.Length)); ++index)
                {
                    // Check if this control set should be made visible
                    if (allSupportFlags[index].IsThisBuildSupported() == true)
                    {
                        // Increment the number of visible controls
                        ++numVisibleControlSets;

                        // If there are more than one controls visible...
                        if (numVisibleControlSets > 1)
                        {
                            // Make the divider *above* this control visible
                            allDividers[index - 1].SetActive(true);
                        }
                    }
                }
            }
        }

        public override MenuNavigator Navigator
        {
            get
            {
                return navigator;
            }
        }
    }
}
