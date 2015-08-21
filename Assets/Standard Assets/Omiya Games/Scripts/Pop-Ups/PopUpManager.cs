using UnityEngine;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="PopUpManager.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2015 Omiya Games
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
        [Header("Stats")]
        [Range(1, 5)]
        [SerializeField]
        int maxNumberOfDialogs = 3;
        [SerializeField]
        float moveLerpSpeed = 10f;

        // FIXME: handle animating
        [Header("Required Components")]
        [SerializeField]
        PopUpDialog dialogInstace;
        [SerializeField]
        string dialogName = "Dialog ({0})";

        ulong uniqueId = 0;
        PopUpDialog[] allDialogs = null;

        // FIXME: create a queue of texts and their corresponding ID
        // FIXME: create a stack of dialogs and their corresponding ID

        void Awake()
        {
            allDialogs = new PopUpDialog[maxNumberOfDialogs + 1];
            allDialogs[0] = dialogInstace;
            dialogInstace.name = string.Format(dialogName, 0);

            GameObject clone = null;
            for(int index = 1; index < allDialogs.Length; ++index)
            {
                // Clone dialog
                clone = Instantiate<GameObject>(dialogInstace.gameObject);

                // Setup the dialog properly
                clone.transform.SetParent(transform, false);
                clone.name = string.Format(dialogName, index);

                // Grab the script from the dialog
                allDialogs[index] = clone.GetComponent<PopUpDialog>();
            }
        }

        public ulong ShowNewDialog(string text)
        {
            // FIXME: for now, only using one dialog

            // Update dialog text
            allDialogs[0].Label.text = text;

            // Set the dialog's unique ID
            allDialogs[0].ID = uniqueId++;

            // Show the dialog
            allDialogs[0].Show();

            // Return this dialog's ID
            return allDialogs[0].ID;
        }

        public void HideDialog(ulong id)
        {
            // FIXME: for now, only using one dialog

            // Update dialog text
            allDialogs[0].Hide();
        }

        public void HideAllDialogs()
        {
            // FIXME: for now, only using one dialog

            // Update dialog text
            allDialogs[0].Hide();
        }
    }
}
