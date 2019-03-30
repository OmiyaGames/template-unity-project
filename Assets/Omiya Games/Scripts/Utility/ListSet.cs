using System;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ListSet.cs" company="Omiya Games">
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
    /// <date>1/23/2019</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A list where all the elements are unique (i.e. a set).
    /// Note that due to its nature, <code>null</code>
    /// cannot be added into this collection.
    /// </summary>
    /// <seealso cref="HashSet{T}"/>
    /// <seealso cref="List{T}"/>
    /// <seealso cref="Dictionary{TKey, TValue}"/>
    public class ListSet<T> : ISet<T>, IList<T>
    {
        /// <summary>
        /// The list itself.
        /// </summary>
        private readonly List<T> list;
        /// <summary>
        /// The dictionary mapping from an item to an index.
        /// Used to verify whether an item already exists in the list.
        /// </summary>
        private readonly Dictionary<T, int> itemToIndexMap;

        #region Constructors
        public ListSet()
        {
            itemToIndexMap = new Dictionary<T, int>();
            list = new List<T>();
        }

        public ListSet(int capacity)
        {
            itemToIndexMap = new Dictionary<T, int>(capacity);
            list = new List<T>(capacity);
        }

        public ListSet(IEqualityComparer<T> comparer)
        {
            itemToIndexMap = new Dictionary<T, int>(comparer);
            list = new List<T>();
        }

        public ListSet(int capacity, IEqualityComparer<T> comparer)
        {
            itemToIndexMap = new Dictionary<T, int>(capacity, comparer);
            list = new List<T>(capacity);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets an element in the list. For the setter,
        /// if an element already in the list is inserted,
        /// an exception will be thrown.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => list[index];
            set
            {
                // First confirm the value and index is valid
                if (index < 0)
                {
                    throw new IndexOutOfRangeException("Index cannot be negative.");
                }
                else if (index >= Count)
                {
                    throw new IndexOutOfRangeException("Index cannot be greater than or equal to list size.");
                }
                else if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                else if (Contains(value) == true)
                {
                    throw new ArgumentException("Cannot insert an item already in the list.", "value");
                }
                else
                {
                    // Update the map with the new item
                    itemToIndexMap.Remove(list[index]);
                    itemToIndexMap.Add(value, index);

                    // Update the list with the new item
                    list[index] = value;
                }
            }
        }

        public int Count => list.Count;

        public bool IsReadOnly => ((IList<T>)list).IsReadOnly;
        #endregion

        public System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly()
        {
            return list.AsReadOnly();
        }

        public void Clear()
        {
            // Clear both lists
            itemToIndexMap.Clear();
            list.Clear();
        }

        public bool Contains(T item)
        {
            // Check contains on the dictionary
            return itemToIndexMap.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // Copy from the list
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            // Grab the enumerator from the list (thus preserving insertion order)
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Grab the enumerator from the list (thus preserving insertion order)
            return list.GetEnumerator();
        }

        /// <summary>
        /// Gets the index of an item, or -1 if it isn't in the list.
        /// </summary>
        /// <param name="item">The element to search through the list.</param>
        /// <returns>Index of an item, or -1 if it isn't in the list.</returns>
        public int IndexOf(T item)
        {
            // Check if the dictionary contains the index
            int returnIndex;
            if (itemToIndexMap.TryGetValue(item, out returnIndex) == false)
            {
                // If not, return -1
                returnIndex = -1;
            }
            return returnIndex;
        }

        public bool Add(T item)
        {
            // Make sure the item isn't null,
            // and already in the list.
            bool returnFlag = ((item != null) && (Contains(item) == false));
            if (returnFlag == true)
            {
                AddHelper(item);
            }
            return returnFlag;
        }

        void ICollection<T>.Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            else if (Contains(item) == true)
            {
                throw new ArgumentException("Cannot add elements already in the list.", "item");
            }
            else
            {
                AddHelper(item);
            }
        }

        public bool Insert(int index, T item)
        {
            bool returnFlag = ((index >= 0) && (index <= Count) && (item != null) && (Contains(item) == false));
            if (returnFlag == true)
            {
                InsertHelper(index, item);
            }
            return returnFlag;

        }

        void IList<T>.Insert(int index, T item)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException("Index cannot be negative.");
            }
            else if (index > Count)
            {
                throw new IndexOutOfRangeException("Index cannot be greater than list size.");
            }
            else if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            else if (Contains(item) == true)
            {
                throw new ArgumentException("Cannot insert elements already in a list.", "item");
            }
            else
            {
                InsertHelper(index, item);
            }
        }

        public bool Remove(T item)
        {
            // Check the index to remove from.
            // We'll need it to figure out which index to update the list.
            int index = IndexOf(item);
            if (index >= 0)
            {
                // Just let RemoveAt handle it at this point
                RemoveAt(index);
            }
            return (index >= 0);
        }

        public void RemoveAt(int index)
        {
            // Check if index is valid
            if (index < 0)
            {
                throw new IndexOutOfRangeException("Index cannot be negative.");
            }
            else if (index >= Count)
            {
                throw new IndexOutOfRangeException("Index cannot be greater than or equal to list size.");
            }

            // Remove the item from the dictionary
            // before it's removed from the list.
            itemToIndexMap.Remove(list[index]);

            // Remove the item from the list
            list.RemoveAt(index);

            // Udpate the list
            UpdateMapFromIndex(index);
        }

        #region Unimplemented Interface Methods
        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented!
        /// </summary>
        [Obsolete("Not implemented!", true)]
        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Adds an item into the list and dictionary.
        /// Using a helper to avoid ambiguity between
        /// interface methods.
        /// </summary>
        private void AddHelper(T item)
        {
            // Add the item into the dictionary first.
            // We need to preserve the old list size, as
            // that will be the index the item will be added to.
            itemToIndexMap.Add(item, list.Count);

            // Add the item into the list last
            list.Add(item);
        }

        /// <summary>
        /// Inserts an item into the list and dictionary.
        /// Using a helper to avoid ambiguity between
        /// interface methods.
        /// </summary>
        private void InsertHelper(int index, T item)
        {
            // Add the item into the dictionary first.
            itemToIndexMap.Add(item, index);

            // Add the item into the list last
            list.Insert(index, item);

            // Update the list from index + 1 up
            UpdateMapFromIndex(index + 1);
        }

        /// <summary>
        /// Synchronizes the mapping from item-to-index,
        /// starting at the provided index.
        /// </summary>
        /// <param name="startIndex"></param>
        private void UpdateMapFromIndex(int startIndex)
        {
            // Go through the list
            for (int index = startIndex; index < list.Count; ++index)
            {
                // Update the map
                itemToIndexMap[list[index]] = index;
            }
        }
        #endregion
    }
}
