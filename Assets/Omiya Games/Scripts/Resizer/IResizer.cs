using UnityEngine;
using OmiyaGames.Settings;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IResizer.cs" company="Omiya Games">
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
    /// <date>6/5/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Parent class for resizing stuff based on <code>GameSettings</code>.
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
    /// <description>6/5/2018</description>
    /// <description>Taro</description>
    /// <description>Initial version</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="GameSettings"/>
    [DisallowMultipleComponent]
    public abstract class IResizer : MonoBehaviour
    {
        public delegate void ResizeMultiplierChanged(float lastMultiplier, float newMultiplier);
        public static event ResizeMultiplierChanged OnBeforeResizeMultiplierChanged;
        public static event ResizeMultiplierChanged OnAfterResizeMultiplierChanged;

        static GameSettings Settings
        {
            get
            {
                return Singleton.Get<GameSettings>();
            }
        }

        public static float ResizeMultiplier
        {
            get
            {
                return Settings.TextSizeMultiplier;
            }
            set
            {
                // Call before event
                OnBeforeResizeMultiplierChanged?.Invoke(Settings.TextSizeMultiplier, value);

                // Change the text size
                float lastMultiplier = Settings.TextSizeMultiplier;
                Settings.TextSizeMultiplier = value;

                // Call after event
                OnAfterResizeMultiplierChanged?.Invoke(lastMultiplier, Settings.TextSizeMultiplier);
            }
        }
        // FIXME: do something!
    }
}
