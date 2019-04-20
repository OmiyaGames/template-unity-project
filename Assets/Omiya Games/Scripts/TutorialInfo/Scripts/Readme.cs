using System;
using UnityEngine;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="Readme.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2019 Omiya Games
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
    /// <date>4/18/2019</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// ReadMe <see cref="ScriptableObject"/> largely copied from Unity's own.
    /// </summary>
    public class Readme : ScriptableObject
    {
        [SerializeField]
        private bool editMode;
        [SerializeField]
        private Texture2D icon;
        [SerializeField]
        private string title;
        [SerializeField]
        private Section[] sections;
        [SerializeField]
        private bool loadedLayout;

        [Serializable]
        public class Section
        {
            [SerializeField]
            private string heading;
            [SerializeField]
            private string text;
            [SerializeField]
            private string linkText;
            [SerializeField]
            private string url;
            [SerializeField]
            private string[] textList;

            public string Heading
            {
                get => heading;
                set => heading = value;
            }

            public string Text
            {
                get => text;
                set => text = value;
            }

            public string LinkText
            {
                get => linkText;
                set => linkText = value;
            }

            public string Url
            {
                get => url;
                set => url = value;
            }

            public string[] TextList
            {
                get => textList;
                set => textList = value;
            }
        }

        public Texture2D Icon
        {
            get => icon;
            set => icon = value;
        }

        public string Title
        {
            get => title;
            set => title = value;
        }

        public Section[] Sections
        {
            get => sections;
            set => sections = value;
        }

        public bool LoadedLayout
        {
            get => loadedLayout;
            set => loadedLayout = value;
        }

        public bool EditMode
        {
            get => editMode;
            set => editMode = value;
        }
    }
}
