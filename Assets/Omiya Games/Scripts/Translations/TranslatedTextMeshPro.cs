using UnityEngine;
using TMPro;

namespace OmiyaGames.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="TranslatedTextMeshPro.cs" company="Omiya Games">
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
    /// Set the translation on a <code>TextMeshProUGUI</code> component.
    /// </summary>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>6/1/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// <item>
    /// <description>9/11/2018</description>
    /// <description>Taro</description>
    /// <description>Abstracting script to <code>ITranslatedLabel</code></description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="TextMeshProUGUI"/>
    /// <seealso cref="ITranslatedLabel{LABEL, STYLE}"/>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class TranslatedTextMeshPro : ITranslatedLabel<TextMeshProUGUI, FontStyles>
    {
        /// <summary>
        /// Gets or sets the style of the label's font directly.
        /// Override to adjust the behavior of this script.
        /// </summary>
        public override FontStyles LabelFontStyle
        {
            get
            {
                return Label.fontStyle;
            }
            set
            {
                Label.fontStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the text of the label directly.
        /// Override to adjust the behavior of this script.
        /// </summary>
        protected override string LabelText
        {
            get
            {
                return Label.text;
            }
            set
            {
                Label.text = value;
            }
        }

        protected override void UpdateFont(TranslationManager.FontMap fontMap, string fontKey)
        {
            Label.font = fontMap.GetFontTextMeshPro(fontKey);
        }
    }
}
