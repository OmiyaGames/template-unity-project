#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="UpdateTranslatedTextComponent.cs" company="Omiya Games">
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
        List<Text> oldTextsWithoutTranslations = new List<Text>();
        //[SerializeField]
        //List<Dropdown> oldDropdowns = new List<Dropdown>();
        //[SerializeField]
        //List<InputField> oldInputs = new List<InputField>();
        [SerializeField]
        List<TranslatedText> oldTranslations = new List<TranslatedText>();

        [Header("Replacement Info")]
        [SerializeField]
        TMP_FontAsset newFont = null;

        [ContextMenu("Find all old texts")]
        void FindAllOldText()
        {
            // Find texts
            if ((searchThrough != null) && (searchThrough.Length > 0))
            {
                // Clear all lists
                oldTextsWithoutTranslations.Clear();
                //oldDropdowns.Clear();
                //oldInputs.Clear();
                oldTranslations.Clear();

                // Go through all the texts
                foreach (GameObject search in searchThrough)
                {
                    RecursivelyFindOldTexts(search, recursivelyCheckChildren);
                }
            }
        }

        [ContextMenu("Upgrade texts")]
        void UpgradeTextComponents()
        {
            // Set the texts
            foreach (Text source in oldTextsWithoutTranslations)
            {
                ReplaceOldTextWithNew(source);
            }
            oldTextsWithoutTranslations.Clear();

            // Set the translations
            foreach (TranslatedText source in oldTranslations)
            {
                ReplaceOldTranslationWithNew(source);
            }
            oldTextsWithoutTranslations.Clear();
        }

        private void ReplaceOldTranslationWithNew(TranslatedText source)
        {
            // Setup variables
            GameObject parentObject = source.gameObject;
            Text sourceLabel = source.Label;
            string translationKey = source.TranslationKey;
            string fontKey = source.FontKey;

            // Destroy the old component
            DestroyImmediate(source);

            // Replace the label component
            ReplaceOldTextWithNew(sourceLabel);

            // Add the new text component
            TranslatedTextMeshPro newTranslation = parentObject.AddComponent<TranslatedTextMeshPro>();
            //TextMeshProResizer newResizer = parentObject.AddComponent<TextMeshProResizer>();

            // Copy the old translation properties into the new one
            newTranslation.TranslationKey = translationKey;
            newTranslation.FontKey = fontKey;
        }

        private void ReplaceOldTextWithNew(Text source)
        {
            // Grab all the old information first
            GameObject parentObject = source.gameObject;
            string text = source.text;
            FontStyle fontStyle = source.fontStyle;
            float fontSize = source.fontSize;
            float lineSpacing = source.lineSpacing;
            bool richText = source.supportRichText;
            TextAnchor alignment = source.alignment;
            HorizontalWrapMode horizontalMode = source.horizontalOverflow;
            VerticalWrapMode verticalMode = source.verticalOverflow;
            bool isAutoSizing = source.resizeTextForBestFit;
            float minFontSize = source.resizeTextMinSize;
            float maxFontSize = source.resizeTextMaxSize;
            Color fontColor = source.color;
            bool rayCastTarget = source.raycastTarget;

            // Grab transform information
            RectTransform transform = source.GetComponent<RectTransform>();
            Vector3 anchorPosition = transform.anchoredPosition3D;
            Vector2 anchorMax = transform.anchorMax;
            Vector2 anchorMin = transform.anchorMin;
            Vector2 offsetMax = transform.offsetMax;
            Vector2 offsetMin = transform.offsetMin;

            // Destroy the old component
            DestroyImmediate(source);

            // Add the new text component
            TextMeshProUGUI newText = parentObject.AddComponent<TextMeshProUGUI>();

            // Copy the old text properties into the new text
            newText.text = text;
            newText.font = newFont;
            newText.fontStyle = ConvertFontStyle(fontStyle);
            newText.fontSize = fontSize;
            newText.lineSpacing = lineSpacing;
            newText.richText = richText;
            newText.alignment = ConvertAlignment(alignment);

            // Setup word wrapping
            newText.enableWordWrapping = (horizontalMode == HorizontalWrapMode.Wrap);

            // Setup overflow
            TextOverflowModes overflowMode = TextOverflowModes.Overflow;
            if(verticalMode == VerticalWrapMode.Truncate)
            {
                overflowMode = TextOverflowModes.Truncate;
            }
            newText.overflowMode = overflowMode;

            // Setup the rest of the properties
            newText.enableAutoSizing = isAutoSizing;
            newText.fontSizeMin = minFontSize;
            newText.fontSizeMax = maxFontSize;
            newText.color = fontColor;
            newText.raycastTarget = rayCastTarget;

            // Revert transform information
            transform.anchoredPosition3D = anchorPosition;
            transform.anchorMax = anchorMax;
            transform.anchorMin = anchorMin;
            transform.offsetMax = offsetMax;
            transform.offsetMin = offsetMin;
        }

        public static TextAlignmentOptions ConvertAlignment(TextAnchor alignment)
        {
            switch (alignment)
            {
                case TextAnchor.UpperLeft:
                    return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperCenter:
                    return TextAlignmentOptions.Top;
                case TextAnchor.UpperRight:
                    return TextAlignmentOptions.TopRight;
                case TextAnchor.MiddleLeft:
                    return TextAlignmentOptions.Left;
                case TextAnchor.MiddleCenter:
                    return TextAlignmentOptions.Center;
                case TextAnchor.MiddleRight:
                    return TextAlignmentOptions.Right;
                case TextAnchor.LowerRight:
                    return TextAlignmentOptions.BottomRight;
                case TextAnchor.LowerCenter:
                    return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerLeft:
                default:
                    return TextAlignmentOptions.BottomLeft;
            }
        }

        public static FontStyles ConvertFontStyle(FontStyle fontStyle)
        {
            FontStyles finalStyle = FontStyles.Normal;
            switch (fontStyle)
            {
                case FontStyle.Bold:
                case FontStyle.BoldAndItalic:
                    finalStyle &= FontStyles.Bold;
                    break;
            }
            switch (fontStyle)
            {
                case FontStyle.Italic:
                case FontStyle.BoldAndItalic:
                    finalStyle &= FontStyles.Italic;
                    break;
            }

            return finalStyle;
        }

        void RecursivelyFindOldTexts(GameObject search, bool checkChildren)
        {
            if (search != null)
            {
                // Fill in the list
                //Dropdown dropDown = search.GetComponent<Dropdown>();
                //if(dropDown != null)
                //{
                //    oldDropdowns.Add(dropDown);
                //}
                //InputField input = search.GetComponent<InputField>();
                //if(input != null)
                //{
                //    oldInputs.Add(input);
                //}
                TranslatedText translation = search.GetComponent<TranslatedText>();
                if(translation != null)
                {
                    oldTranslations.Add(translation);
                }
                else
                {
                    Text text = search.GetComponent<Text>();
                    if (text != null)
                    {
                        oldTextsWithoutTranslations.Add(text);
                    }
                }

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
