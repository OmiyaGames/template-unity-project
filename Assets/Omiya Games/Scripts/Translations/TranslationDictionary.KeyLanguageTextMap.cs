using System;
using System.Collections;
using System.Collections.Generic;

///-----------------------------------------------------------------------
/// <copyright file="TranslationDictionary.KeyLanguageTextMap.cs" company="Omiya Games">
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
        public class KeyLanguageTextMap : IEnumerable<KeyValuePair<string, LanguageTextMap>>
        {
            readonly IDictionary<string, LanguageTextMap> dictionary;

            public KeyLanguageTextMap(TranslationDictionary parent)
            {
                dictionary = new SortedDictionary<string, LanguageTextMap>();
                Parent = parent;
            }

            public LanguageTextMap this[string translationKey]
            {
                get
                {
                    return dictionary[translationKey];
                }
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

            public int Count
            {
                get
                {
                    return dictionary.Count;
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    return dictionary.Keys;
                }
            }

            public ICollection<LanguageTextMap> Values
            {
                get
                {
                    return dictionary.Values;
                }
            }

            public bool ContainsKey(string translationKey)
            {
                return dictionary.ContainsKey(translationKey);
            }

            public bool TryGetValue(string translationKey, out LanguageTextMap textCollection)
            {
                return dictionary.TryGetValue(translationKey, out textCollection);
            }

            /// <summary>
            /// Creates a new <code>TranslationTextCollection</code> for the argument,
            /// <code>translationKey</code>; or null if the collection already contains the key.
            /// </summary>
            /// <param name="translationKey"></param>
            /// <returns>
            /// A new <code>TranslationTextCollection</code>,
            /// or null if the collection already has the key.
            /// </returns>
            public LanguageTextMap Add(string translationKey)
            {
                // Make sure the key hasn't been added already
                LanguageTextMap returnCollection = null;
                if(ContainsKey(translationKey) == false)
                {
                    // If not, create a new collection
                    returnCollection = new LanguageTextMap(this);

                    // Add it to this dictionary
                    dictionary.Add(translationKey, returnCollection);

                    // Indicate the serialization no longer matches what's contained in the dictionary
                    IsSerialized = false;
                }
                return returnCollection;
            }

            public LanguageTextMap Remove(string translationKey)
            {
                // Make sure the dictionar contains the key
                LanguageTextMap returnCollection = null;
                if (dictionary.TryGetValue(translationKey, out returnCollection) == true)
                {
                    // If so, remove it from the dictionary
                    dictionary.Remove(translationKey);

                    // Indicate the serialization no longer matches what's contained in the dictionary
                    IsSerialized = false;
                }
                else
                {
                    // Otherwise, return null
                    returnCollection = null;
                }
                return returnCollection;
            }

            public void Clear()
            {
                // Check the size of the dictionary
                if(Count > 0)
                {
                    // Clear the dictionary
                    dictionary.Clear();

                    // Indicate the serialization no longer matches what's contained in the dictionary
                    IsSerialized = false;
                }
            }

            public IEnumerator<KeyValuePair<string, LanguageTextMap>> GetEnumerator()
            {
                return dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return dictionary.GetEnumerator();
            }
        }

        /// <summary>
        /// An interface to the translations in <code>TranslationDictionary</code>.
        /// </summary>
        /// <seealso cref="TranslationDictionary"/>
        public class LanguageTextMap : IEnumerable<KeyValuePair<int, string>>
        {
            readonly IDictionary<int, string> dictionary;

            public LanguageTextMap(KeyLanguageTextMap parent)
            {
                dictionary = new SortedDictionary<int, string>();
                Parent = parent;
            }

            public string this[int languageIndex]
            {
                get
                {
                    // Check if the language index is valid
                    if (SupportedLanguages.Contains(languageIndex) == false)
                    {
                        // Indicate if it isn't
                        throw new ArgumentOutOfRangeException("languageIndex");
                    }

                    // Grab a text based off of the index
                    string returnText = null;
                    if (dictionary.TryGetValue(languageIndex, out returnText) == false)
                    {
                        // If none found, return null
                        returnText = null;
                    }
                    return returnText;
                }
                set
                {
                    // Check if the language index is valid
                    if (SupportedLanguages.Contains(languageIndex) == false)
                    {
                        // Indicate if it isn't
                        throw new ArgumentOutOfRangeException("languageIndex");
                    }

                    // Check if the dictionary already contains a text
                    if (dictionary.ContainsKey(languageIndex) == false)
                    {
                        // Add this text
                        dictionary.Add(languageIndex, value);

                        // Indicate the serialization no longer matches what's contained in the dictionary
                        IsSerialized = false;
                    }
                    else if (dictionary[languageIndex] != value)
                    {
                        // Overwrite this text
                        dictionary[languageIndex] = value;

                        // Indicate the serialization no longer matches what's contained in the dictionary
                        IsSerialized = false;
                    }
                }
            }

            public string this[string languageName]
            {
                get
                {
                    // Check if the language index is valid
                    if (SupportedLanguages.Contains(languageName) == false)
                    {
                        // Indicate if it isn't
                        throw new ArgumentOutOfRangeException("languageName");
                    }

                    // Use the above getter
                    return this[SupportedLanguages[languageName]];
                }
                set
                {
                    // Check if the language index is valid
                    if (SupportedLanguages.Contains(languageName) == false)
                    {
                        // Indicate if it isn't
                        throw new ArgumentOutOfRangeException("languageName");
                    }

                    // Use the above setter
                    this[SupportedLanguages[languageName]] = value;
                }
            }

            public KeyLanguageTextMap Parent
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

            public SupportedLanguages SupportedLanguages
            {
                get
                {
                    return Parent.Parent.SupportedLanguages;
                }
            }

            public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
            {
                return dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return dictionary.GetEnumerator();
            }
        }
    }
}
