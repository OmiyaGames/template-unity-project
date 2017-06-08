using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames.Settings
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ISortedRecords.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2017 Omiya Games
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
    /// <date>5/28/2017</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Stores a sorted list of <see cref="IRecord{T}"/> with a limited capacity.
    /// Useful for keeping track of high scores.
    /// </summary>
    public abstract class ISortedRecords<T> : IEnumerable<IRecord<T>> where T : IComparable<T>
    {
        public const char ScoreDivider = '\n';

        private readonly IComparer<IRecord<T>> comparer;
        private readonly int maxCapacity;
        private readonly List<IRecord<T>> records;
        private readonly IRecord<T>.TryConvertOldRecord converter;

        #region Helper Classes
        public class DescendingOrder : Comparer<IRecord<T>>
        {
            public override int Compare(IRecord<T> left, IRecord<T> right)
            {
                return left.Record.CompareTo(right.Record);
            }
        }

        public class AsecendingOrder : Comparer<IRecord<T>>
        {
            public override int Compare(IRecord<T> left, IRecord<T> right)
            {
                return right.Record.CompareTo(left.Record);
            }
        }
        #endregion

        #region Constructors
        public ISortedRecords(int maxCapacity, bool isSortedInDescendingOrder, IRecord<T>.TryConvertOldRecord converter) : this(maxCapacity, null, converter)
        {
            if(isSortedInDescendingOrder == false)
            {
                comparer = new AsecendingOrder();
            }
        }

        public ISortedRecords(int maxCapacity, IComparer<IRecord<T>> comparer, IRecord<T>.TryConvertOldRecord converter)
        {
            // Setup member variables
            this.converter = converter;
            this.maxCapacity = maxCapacity;

            // Setup the list
            records = new List<IRecord<T>>(this.maxCapacity);

            // Check if a comparer is defined
            if (comparer != null)
            {
                // If so, use it
                this.comparer = comparer;
            }
            else
            {
                // If not, choose descending automatically
                this.comparer = new DescendingOrder();
            }
        }
        #endregion

        #region Properties
        public IComparer<IRecord<T>> Comparer
        {
            get
            {
                return comparer;
            }
        }

        public int MaxCapacity
        {
            get
            {
                return maxCapacity;
            }
        }

        public int Count
        {
            get
            {
                return records.Count;
            }
        }

        public IRecord<T>.TryConvertOldRecord Converter
        {
            get
            {
                return converter;
            }
        }

        public IRecord<T> this[int index]
        {
            get
            {
                return records[index];
            }
        }

        public IRecord<T> TopRecord
        {
            get
            {
                IRecord<T> returnRecord = null;
                if(Count > 0)
                {
                    returnRecord = this[0];
                }
                return returnRecord;
            }
        }
        #endregion

        public abstract int AddRecord(T record, string name, out IRecord<T> newRecord);
        protected abstract IRecord<T> ParseRecord(string recordEntry, int appVersion, IRecord<T>.TryConvertOldRecord converter);

        /// <summary>
        /// Checks to see if the record exceeds any values held in this list.
        /// If so, adds the record into the list, and returns it's rank placement.
        /// </summary>
        /// <param name="newRecord">Record to add to the list.</param>
        /// <returns>The new record's index in the list, or -1 if it was not added.</returns>
        public int AddRecord(IRecord<T> newRecord)
        {
            // By default, set the rank to an invalid value
            int returnRank = -1;

            // Check if there are less records than the max capacity allows
            if (Count < MaxCapacity)
            {
                // If so, default to the last index of the list;
                // this record is guaranteed to be added to the list
                returnRank = Count;
            }

            // Go through the loop
            for (int index = 0; index < Count; ++index)
            {
                // Check if this record is greater than a previously-stored record
                if (comparer.Compare(newRecord, this[index]) >= 0)
                {
                    // If so, return this rank
                    returnRank = index;
                    break;
                }
            }

            // Make sure the high score belongs to the list
            if (returnRank >= 0)
            {
                // If it does, check if we need to trim excess
                while (Count >= maxCapacity)
                {
                    records.RemoveAt(Count - 1);
                }

                // Insert the score
                records.Insert(returnRank, newRecord);
            }
            return returnRank;
        }

        /// <summary>
        /// Retrieves records from a string recalled from settings.
        /// This method will clear anything that's in the list.
        /// </summary>
        /// <param name="pastRecords">String from settings</param>
        /// <param name="parser">Delegate that parses a string into a record</param>
        /// <returns>True if all records has been successfully parsed and added into the list.</returns>
        public bool RetrieveRecords(string pastRecords, int appVersion)
        {
            // Make sure there are records to read
            bool returnFlag = false;
            if (string.IsNullOrEmpty(pastRecords) == false)
            {
                // Attempt to split the records
                string[] highScoresArray = pastRecords.Split(ScoreDivider);

                // Clear the list
                Clear();

                // Add elements to the list
                for (int i = 0; i < highScoresArray.Length; ++i)
                {
                    records.Add(ParseRecord(highScoresArray[i], appVersion, Converter));
                }
                returnFlag = true;
            }
            return returnFlag;
        }

        /// <summary>
        /// Clears all records from the list
        /// </summary>
        public void Clear()
        {
            records.Clear();
        }

        public IEnumerator<IRecord<T>> GetEnumerator()
        {
            return records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return records.GetEnumerator();
        }

        /// <summary>
        /// Returns a string to store in settings.
        /// </summary>
        /// <returns>Returns a string to store in settings.</returns>
        public override string ToString()
        {
            // Go through each record
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < Count; ++index)
            {
                // If this is NOT the first element...
                if (index > 0)
                {
                    // Append the divider
                    builder.Append(ScoreDivider);
                }

                // Append the record into the string
                builder.Append(this[index]);
            }
            return builder.ToString();
        }
    }
}
