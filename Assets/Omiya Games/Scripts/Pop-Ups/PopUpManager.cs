using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames.Menu
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PopUpManager.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
    /// <date>8/21/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// TODO: documentation
    /// </summary>
    public class PopUpManager : MonoBehaviour
    {
        public const ulong InvalidId = 0;

        private class DescendingOrder : IComparer<ulong>
        {
            public int Compare(ulong x, ulong y)
            {
                if(x == y)
                {
                    return 0;
                }
                else if(x > y)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        [Header("Stats")]
        [Range(1, 5)]
        [SerializeField]
        int maxNumberOfDialogs = 3;
        [SerializeField]
        float moveLerpSpeed = 10f;
        [SerializeField]
        float padding = 10f;
        [SerializeField]
        float spacing = 10f;

        // FIXME: handle animating
        [Header("Required Components")]
        [SerializeField]
        PopUpDialog dialogInstace;
        [SerializeField]
        string dialogName = "Dialog ({0})";

        int index = 0;
        ulong uniqueId = (InvalidId + 1);
        PopUpDialog[] allDialogs = null;
        Vector2 startingPosition, targetPosition;
        System.Action<float> animateDialogEvent = null;
        bool repositionDialogs = false;

        // A queue of all texts logged, along with their associated ID
        readonly SortedDictionary<ulong, string> allLoggedTexts = new SortedDictionary<ulong, string>(new DescendingOrder());

        // Lists of visible and hidden dialogs
        List<PopUpDialog> visibleDialogs = null;
        Queue<PopUpDialog> hiddenDialogs = null;

        public int MaximumNumberOfDialogs
        {
            get
            {
                return maxNumberOfDialogs;
            }
        }

        ulong NextId
        {
            get
            {
                ulong returnId = uniqueId;
                ++uniqueId;
                if(uniqueId == ulong.MaxValue)
                {
                    uniqueId = (InvalidId + 1);
                }
                return returnId;
            }
        }

        #region MonoBehavior Events
        void Start()
        {
            // Reset flags
            repositionDialogs = false;

            // Calculate the starting dialog position
            startingPosition.x = 0;
            startingPosition.y = -padding;
            targetPosition = startingPosition;

            // Create the dialog array
            allDialogs = new PopUpDialog[maxNumberOfDialogs + 1];
            SetupDialogArray(allDialogs);

            // Setup dynamic dialog lists
            allLoggedTexts.Clear();
            visibleDialogs = new List<PopUpDialog>(allDialogs.Length);
            hiddenDialogs = new Queue<PopUpDialog>(allDialogs.Length);
            for(index = 0; index < allDialogs.Length; ++index)
            {
                hiddenDialogs.Enqueue(allDialogs[index]);
            }

            // Bind to the real-time update event
            if (animateDialogEvent == null)
            {
                animateDialogEvent = new System.Action<float>(AnimateDialogs);
                Singleton.Instance.OnRealTimeUpdate += animateDialogEvent;
            }
        }

        void OnDestroy()
        {
            // Un-bind from the real-time update event
            if (animateDialogEvent != null)
            {
                Singleton.Instance.OnRealTimeUpdate -= animateDialogEvent;
                animateDialogEvent = null;
            }
        }
        #endregion

        public ulong ShowNewDialog(string text)
        {
            ulong returnId = 0;
            if ((allDialogs != null) && (allDialogs.Length > 0) && (hiddenDialogs.Count > 0))
            {
                // Find a new dialog
                PopUpDialog newDialog = hiddenDialogs.Dequeue();

                // Set the dialog's unique ID
                newDialog.ID = NextId;

                // Update dialog text
                newDialog.Label.text = text;
                allLoggedTexts.Add(newDialog.ID, text);

                // Update the dialog's transform
                newDialog.CachedTransform.SetAsLastSibling();
                newDialog.CachedTransform.anchoredPosition = startingPosition;
                newDialog.TargetAnchorPosition = startingPosition;

                // Show the dialog
                newDialog.Show();
                visibleDialogs.Insert(0, newDialog);

                // Check to see if we reached the max number of dialogs
                if(visibleDialogs.Count > maxNumberOfDialogs)
                {
                    // If so, hide the last dialog
                    RemoveLastVisibleDialog(false);
                }

                // Return this dialog's ID
                returnId = newDialog.ID;

                // Indicate dialogs need to be re-positioned
                repositionDialogs = true;
            }
            return returnId;
        }

        public void RemoveDialog(ulong id)
        {
            // Check to see if there's a text to remove
            if (allLoggedTexts.ContainsKey(id) == true)
            {
                // If so, remove the entry
                allLoggedTexts.Remove(id);
            }

            if (visibleDialogs.Count > 0)
            {
                // Grab the current last dialog
                float bottomPosition = YPositionBelow(visibleDialogs[visibleDialogs.Count - 1]);

                // Search for the pop-up dialog in the displayed dialog list
                for (index = 0; index < visibleDialogs.Count; ++index)
                {
                    // Check if the ID matches
                    if (visibleDialogs[index].ID == id)
                    {
                        // Run the hide animation
                        HideDialog(visibleDialogs[index]);

                        // Remove the dialog from the list
                        visibleDialogs.RemoveAt(index);
                        break;
                    }
                }

                // Append dialog
                AppendDialogs(bottomPosition);
            }
        }

        public ulong RemoveLastVisibleDialog()
        {
            return RemoveLastVisibleDialog(true);
        }

        public void RemoveAllDialogs()
        {
            // Clear out all the texts
            allLoggedTexts.Clear();

            // Check to see if there's any dialogs to clear
            if (visibleDialogs.Count > 0)
            {
                // Move all the visible dialogs to hidden list
                for (index = 0; index < visibleDialogs.Count; ++index)
                {
                    // Animate to move back to the top of the screen
                    visibleDialogs[index].TargetAnchorPosition = startingPosition;

                    // Run the hide animation
                    HideDialog(visibleDialogs[index]);
                }

                // Clear out the visible dialog list
                visibleDialogs.Clear();
            }
        }

        #region Helper Methods
        void AnimateDialogs(float deltaTime)
        {
            if (repositionDialogs == true)
            {
                // By default, indicate we don't need to reposition anything
                repositionDialogs = false;

                // Go through all the visible dialogs
                targetPosition.y = startingPosition.y;
                for (index = 0; index < visibleDialogs.Count; ++index)
                {
                    // Update dialog position and highlight state
                    visibleDialogs[index].TargetAnchorPosition = targetPosition;
                    visibleDialogs[index].Highlight = (index == 0);

                    // Check to see if the height of this dialog has been properly calculated
                    if (visibleDialogs[index].Height > 0)
                    {
                        // If so, increment position
                        targetPosition.y -= visibleDialogs[index].Height;
                        targetPosition.y -= spacing;
                    }
                    else
                    {
                        // Indicate we still need to re-calculate positioninng of all the visible dialogs
                        repositionDialogs = true;
                    }
                }
            }

            // Animate all dialogs
            for (index = 0; index < allDialogs.Length; ++index)
            {
                allDialogs[index].UpdateAnchorPosition(deltaTime, moveLerpSpeed);
            }
        }

        void SetupDialogArray(PopUpDialog[] dialogArray)
        {
            // Populate the first entry
            dialogArray[0] = dialogInstace;
            dialogInstace.name = string.Format(dialogName, 0);

            // Populate the rest
            GameObject clone = null;
            for (index = 1; index < dialogArray.Length; ++index)
            {
                // Clone dialog
                clone = Instantiate<GameObject>(dialogInstace.gameObject);

                // Setup the dialog properly
                clone.transform.SetParent(transform, false);
                clone.name = string.Format(dialogName, index);

                // Grab the script from the dialog
                dialogArray[index] = clone.GetComponent<PopUpDialog>();
            }
        }

        void HideDialog(PopUpDialog removeDialog)
        {
            // Hide the dialog
            removeDialog.Hide();

            // Push it back to the hidden dialog list
            hiddenDialogs.Enqueue(removeDialog);

            // Indicate dialogs need to be re-positioned
            repositionDialogs = true;
        }

        void AppendDialogs(float yPosition)
        {
            // Check to see if there's more texts to display
            if ((allDialogs != null) && (allDialogs.Length > 0) && (allLoggedTexts.Count > visibleDialogs.Count))
            {
                // Calculate position
                targetPosition.y = yPosition;

                // Append dialogs
                while ((allLoggedTexts.Count > visibleDialogs.Count) && (visibleDialogs.Count < maxNumberOfDialogs) && (hiddenDialogs.Count > 0))
                {
                    // Find the string to display
                    index = 0;
                    foreach(KeyValuePair<ulong, string> info in allLoggedTexts)
                    {
                        if(index >= visibleDialogs.Count)
                        {
                            // Find a new dialog
                            PopUpDialog newDialog = hiddenDialogs.Dequeue();

                            // Set the dialog's unique ID
                            newDialog.ID = info.Key;

                            // Update dialog text
                            newDialog.Label.text = info.Value;

                            // Update the dialog's transform
                            newDialog.CachedTransform.SetAsFirstSibling();
                            newDialog.CachedTransform.anchoredPosition = targetPosition;
                            newDialog.TargetAnchorPosition = targetPosition;

                            // Show the dialog
                            newDialog.Show();
                            visibleDialogs.Add(newDialog);

                            // Indicate dialogs need to be re-positioned
                            repositionDialogs = true;
                            break;
                        }
                        ++index;
                    }

                    // This shouldn't happen, but in case we don't find the string to display, stop appending dialogs
                    if (index >= allLoggedTexts.Count)
                    {
                        break;
                    }
                }
            }
        }

        ulong RemoveLastVisibleDialog(bool removeText)
        {
            ulong returnId = 0;
            if (visibleDialogs.Count > 0)
            {
                // Check to see if there's a text to remove
                index = visibleDialogs.Count - 1;
                returnId = visibleDialogs[index].ID;
                if ((removeText == true) && (allLoggedTexts.ContainsKey(returnId) == true))
                {
                    // If so, remove the entry
                    allLoggedTexts.Remove(returnId);
                }

                // Run the hide animation on the last element
                HideDialog(visibleDialogs[index]);

                // Remove the last element
                float bottomPosition = YPositionBelow(visibleDialogs[index]);
                visibleDialogs.RemoveAt(index);

                // Append any left over dialog
                AppendDialogs(bottomPosition);
            }
            return returnId;
        }

        float YPositionBelow(PopUpDialog dialog)
        {
            float returnYPos = startingPosition.y;
            if (dialog != null)
            {
                returnYPos = dialog.TargetAnchorPosition.y;
                if (dialog.Height > 0)
                {
                    returnYPos -= dialog.Height;
                    returnYPos -= spacing;
                }
            }
            return returnYPos;
        }
        #endregion
    }
}
