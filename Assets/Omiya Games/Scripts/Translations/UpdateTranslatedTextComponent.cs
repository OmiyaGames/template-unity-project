#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AudioFinder.cs" company="Omiya Games">
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
    /// <date>6/1/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A helper scripts to search through Text and TranslatedText, and replace them with TextMeshPro.
    /// Run it by attaching this script to a GameObject, then using the right-click context menu.
    /// </summary>
    /// <seealso cref="TranslatedText"/>
    /// <seealso cref="AmbientMusic"/>
    /// <seealso cref="AudioMixerReference"/>
    [DisallowMultipleComponent]
    public class UpdateTranslatedTextComponent : MonoBehaviour
    {
        [Header("Search through...")]
        [SerializeField]
        GameObject[] searchThrough;
        [SerializeField]
        bool recursivelyCheckChildren = true;

        [Header("Text Components")]
        [SerializeField]
        List<Text> oldTexts = new List<Text>();
        [SerializeField]
        List<Dropdown> oldDropdowns = new List<Dropdown>();
        [SerializeField]
        List<InputField> oldInputs = new List<InputField>();
        [SerializeField]
        List<TranslatedText> oldTranslations = new List<TranslatedText>();

        [Header("Replacement Info")]
        [SerializeField]
        TMP_FontAsset newFont = null;

        [ContextMenu("Find all old texts")]
        void FindAllAudioSources()
        {
            // Find texts
            if ((searchThrough != null) && (searchThrough.Length > 0))
            {
                // Clear all lists
                oldTexts.Clear();
                oldDropdowns.Clear();
                oldInputs.Clear();
                oldTranslations.Clear();

                // Go through all the texts
                foreach (GameObject search in searchThrough)
                {
                    RecursivelyFindOldTexts(search, recursivelyCheckChildren);
                }
            }
        }

        [ContextMenu("Upgrade texts")]
        void SetMixerGroup()
        {
            // Set the texts
            foreach (Text source in oldTexts)
            {
                ReplaceOldTextWithNew(source);
            }
            oldTexts.Clear();

            // Set the translations
            foreach (TranslatedText source in oldTranslations)
            {
                ReplaceOldTranslationWithNew(source);
            }
            oldTexts.Clear();
        }

        private void ReplaceOldTranslationWithNew(TranslatedText source)
        {
            // Add the new text component
            TranslatedTextMeshPro newTranslation = source.gameObject.AddComponent<TranslatedTextMeshPro>();

            // Copy the old translation properties into the new one
            newTranslation.TranslationKey = source.TranslationKey;
            newTranslation.FontKey = source.FontKey;

            // Destroy the old component
            DestroyImmediate(source);
        }

        private void ReplaceOldTextWithNew(Text source)
        {
            // Add the new text component
            TMP_Text newText = source.gameObject.AddComponent<TMP_Text>();

            // FIXME: Copy the old text properties into the new text
            newText.font = newFont;

            // Destroy the old component
            DestroyImmediate(source);
        }

        void RecursivelyFindOldTexts(GameObject search, bool checkChildren)
        {
            if (search != null)
            {
                // Fill in the list
                oldTexts.AddRange(search.GetComponents<Text>());
                oldDropdowns.AddRange(search.GetComponents<Dropdown>());
                oldInputs.AddRange(search.GetComponents<InputField>());
                oldTranslations.AddRange(search.GetComponents<TranslatedText>());

                // Look for children
                if ((checkChildren == true) && (search.transform.childCount > 0))
                {
                    Transform child;
                    for (int index = 0; index < search.transform.childCount; ++index)
                    {
                        child = search.transform.GetChild(index);
                        RecursivelyFindOldTexts(child.gameObject, checkChildren);
                    }
                }
            }
        }
    }
}
#endif
