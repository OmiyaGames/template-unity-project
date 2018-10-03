namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ProgressReport.cs" company="Omiya Games">
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
    /// <date>10/2/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper method to report progress reports.  Thread-safe.
    /// </summary>
    /// <seealso cref="ThreadSafeInt"/>
    public class ProgressReport
    {
        readonly ThreadSafeLong currentStep;
        readonly ThreadSafeLong totalSteps;

        public ProgressReport() : this(1) { }

        public ProgressReport(int totalSteps)
        {
            if (totalSteps < 1)
            {
                throw new System.ArgumentException("Argument \"numberOfSteps\" cannot be less than 1.");
            }
            currentStep = new ThreadSafeLong(0);
            this.totalSteps = new ThreadSafeLong(totalSteps);
        }

        public long CurrentStep
        {
            get
            {
                return currentStep.Value;
            }
            set
            {
                // Don't change the finalValue if value is below 0
                long finalValue = 0;
                if (value > 0)
                {
                    // Grab the number of steps only once
                    finalValue = TotalSteps;

                    // Don't change the finalValue if the value is above NumberOfSteps
                    if (value < finalValue)
                    {
                        finalValue = value;
                    }
                }
                currentStep.Value = finalValue;
            }
        }

        public long TotalSteps
        {
            get
            {
                return totalSteps.Value;
            }
        }

        public float ProgressPercent
        {
            get
            {
                float returnPercent = CurrentStep;
                returnPercent /= TotalSteps;
                return returnPercent;
            }
        }

        public void Reset()
        {
            currentStep.Value = 0;
        }

        /// <summary>
        /// Resets current step, then sets the total steps
        /// </summary>
        /// <param name="newTotalSteps"></param>
        public void SetTotalSteps(long newTotalSteps)
        {
            if (newTotalSteps < 1)
            {
                throw new System.ArgumentException("Argument \"newNumberOfSteps\" cannot be less than 1.");
            }
            Reset();
            totalSteps.Value = newTotalSteps;
        }

        public void IncrementCurrentStep()
        {
            if (CurrentStep < TotalSteps)
            {
                currentStep.Increment();
            }
        }
    }
}
