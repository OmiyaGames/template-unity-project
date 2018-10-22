#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using OmiyaGames.Translations;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="FindUntranslatedLabelField.cs" company="Omiya Games">
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
    /// <date>10/11/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A helper scripts to search through <code>ITranslatedLabel</code>s and
    /// fixing their fields.
    /// </summary>
    /// <seealso cref="TranslatedText"/>
    /// <seealso cref="AmbientMusic"/>
    /// <seealso cref="AudioMixerReference"/>
    [DisallowMultipleComponent]
    public class FindUntranslatedLabelField : MonoBehaviour
    {
        [Header("Search through...")]
        [SerializeField]
        GameObject[] searchThrough;
        [SerializeField]
        bool recursivelyCheckChildren = true;

        [Header("Text Components")]
        [SerializeField]
        List<TranslatedText> labels1 = new List<TranslatedText>();
        [SerializeField]
        List<TranslatedTextMesh> labels2 = new List<TranslatedTextMesh>();
        [SerializeField]
        List<TranslatedTextMeshPro> labels3 = new List<TranslatedTextMeshPro>();

        [ContextMenu("Find all texts")]
        void FindAllOldText()
        {
            // Find texts
            if ((searchThrough != null) && (searchThrough.Length > 0))
            {
                // Clear all lists
                labels1.Clear();
                labels2.Clear();
                labels3.Clear();

                // Go through all the texts
                foreach (GameObject search in searchThrough)
                {
                    RecursivelyFindLabels(search, recursivelyCheckChildren);
                }
            }
        }

        void RecursivelyFindLabels(GameObject search, bool checkChildren)
        {
            if (search != null)
            {
                TranslatedText translation1 = search.GetComponent<TranslatedText>();
                if ((translation1 != null) && (IsTranslating(translation1) == false))
                {
                    labels1.Add(translation1);
                }
                TranslatedTextMesh translation2 = search.GetComponent<TranslatedTextMesh>();
                if ((translation2 != null) && (IsTranslating(translation2) == false))
                {
                    labels2.Add(translation2);
                }
                TranslatedTextMeshPro translation3 = search.GetComponent<TranslatedTextMeshPro>();
                if ((translation3 != null) && (IsTranslating(translation3) == false))
                {
                    labels3.Add(translation3);
                }

                // Look for children
                if ((checkChildren == true) && (search.transform.childCount > 0))
                {
                    Transform child;
                    for (int index = 0; index < search.transform.childCount; ++index)
                    {
                        child = search.transform.GetChild(index);
                        RecursivelyFindLabels(child.gameObject, checkChildren);
                    }
                }
            }
        }

        private bool IsTranslating<L, S>(ITranslatedLabel<L, S> label) where L : Component
        {
            bool returnFlag = false;
            if((label != null) && (string.IsNullOrEmpty(label.TranslationKey) == false) && (label.Dictionary != null))
            {
                returnFlag = label.Dictionary.AllTranslations.ContainsKey(label.TranslationKey);
            }
            return returnFlag;
        }
    }
}
#endif
