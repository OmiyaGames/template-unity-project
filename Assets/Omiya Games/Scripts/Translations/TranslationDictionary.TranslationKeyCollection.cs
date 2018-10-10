using System;
using System.Collections;
using System.Collections.Generic;

///-----------------------------------------------------------------------
/// <copyright file="TranslationDictionary.TranslationKeyCollection.cs" company="Omiya Games">
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
namespace OmiyaGames.Translations
{
    public partial class TranslationDictionary
    {
        /// <summary>
        /// An interface to the translations in <code>TranslationDictionary</code>.
        /// </summary>
        /// <seealso cref="TranslationDictionary"/>
        public class TranslationKeyCollection : IEnumerable<KeyValuePair<string, TranslationTextCollection>>
        {
            readonly IDictionary<string, TranslationTextCollection> dictionary;

            public TranslationKeyCollection(TranslationDictionary parent)
            {
                dictionary = new SortedDictionary<string, TranslationTextCollection>();
                Parent = parent;
            }

            public TranslationDictionary Parent
            {
                get;
            }

            public bool IsSerialized
            {
                get
                {
                    return Parent.IsAllTranslationsSerialized;
                }
                private set
                {
                    Parent.IsAllTranslationsSerialized = value;
                }
            }

            public TranslationTextCollection this[string translationKey]
            {
                get
                {
                    return dictionary[translationKey];
                }
            }

            public bool ContainsKey(string translationKey)
            {
                return dictionary.ContainsKey(translationKey);
            }

            public IEnumerator<KeyValuePair<string, TranslationTextCollection>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// An interface to the translations in <code>TranslationDictionary</code>.
        /// </summary>
        /// <seealso cref="TranslationDictionary"/>
        public class TranslationTextCollection : IEnumerable<KeyValuePair<int, string>>
        {
            readonly IDictionary<int, string> dictionary;

            public TranslationTextCollection(TranslationKeyCollection parent)
            {
                dictionary = new SortedDictionary<int, string>();
                Parent = parent;
            }

            public TranslationKeyCollection Parent
            {
                get;
            }

            public bool IsSerialized
            {
                get
                {
                    return Parent.Parent.IsAllTranslationsSerialized;
                }
                private set
                {
                    Parent.Parent.IsAllTranslationsSerialized = value;
                }
            }

            public string this[int key]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
