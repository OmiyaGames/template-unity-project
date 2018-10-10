using System.Collections.Generic;
using UnityEngine;
using System;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="LanguageTextPairEditor.cs" company="Omiya Games">
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
    /// <date>10/10/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Pair of key and list of <code>LanguageTextPair</code>.
    /// </summary>
    /// <seealso cref="LanguageTextPair"/>
    [Serializable]
    public struct TranslationCollection
    {
        public const byte DefaultNumberOfLanguages = 3;

        [SerializeField]
        string key;
        [SerializeField]
        List<LanguageTextPair> allTranslations;

        public TranslationCollection(string key)
        {
            this.key = key;
            this.allTranslations = new List<LanguageTextPair>(DefaultNumberOfLanguages);
        }

        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        public List<LanguageTextPair> AllTranslations
        {
            get
            {
                if (allTranslations == null)
                {
                    allTranslations = new List<LanguageTextPair>(DefaultNumberOfLanguages);
                }
                return allTranslations;
            }
        }
    }
}
