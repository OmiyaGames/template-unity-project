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
        readonly ThreadSafeInt currentStep;
        readonly ThreadSafeInt numberOfSteps;

        public ProgressReport() : this(1) { }

        public ProgressReport(int numberOfSteps)
        {
            if (numberOfSteps < 1)
            {
                throw new System.ArgumentException("Argument \"numberOfSteps\" cannot be less than 1.");
            }
            currentStep = new ThreadSafeInt(0);
            this.numberOfSteps = new ThreadSafeInt(numberOfSteps);
        }

        public int CurrentStep
        {
            get
            {
                return currentStep.Value;
            }
            set
            {
                currentStep.Value = UnityEngine.Mathf.Clamp(value, 0, NumberOfSteps);
            }
        }

        public int NumberOfSteps
        {
            get
            {
                return numberOfSteps.Value;
            }
        }

        public void Reset()
        {
            CurrentStep = 0;
        }

        public void Reset(int newNumberOfSteps)
        {
            if (newNumberOfSteps < 1)
            {
                throw new System.ArgumentException("Argument \"newNumberOfSteps\" cannot be less than 1.");
            }
            Reset();
            numberOfSteps.Value = newNumberOfSteps;
        }

        public void IncrementCurrentStep()
        {
            if (CurrentStep < NumberOfSteps)
            {
                currentStep.Increment();
            }
        }
    }
}
