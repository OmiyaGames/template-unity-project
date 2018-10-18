using UnityEngine;
using UnityEditor;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using OmiyaGames.Translations;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ImportCsvPopUp.cs" company="Omiya Games">
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
    /// <date>10/1/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Creates a window that imports CSV files.
    /// </summary>
    public class UpdateFontAssets : EditorWindow
    {
        const string LatinCharacters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ           ​‌‍‎‏‐‑‒–—―‖‗‘’‚‛“”„‟†‡•‣․‥…‧\n\r\t‪‫‬‭‮ ‰‱′″‴‵‶‷‸‹›※‼‽‾‿⁀⁁⁂⁃⁄⁅⁆⁇⁈⁉⁊⁋⁌⁍⁎⁏⁐⁑⁒⁓⁔⁕⁖⁗⁘⁙⁚⁛⁜⁝⁞ ⁠⁡⁢⁣⁤⁦⁧⁨⁩⁪⁫⁬⁭⁮⁯€™\n\t";
        const string JapaneseCharacters = LatinCharacters + "　ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔゕゖ゙゚゛゜ゝゞゟ゠ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺ・ーヽヾヿ･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇺㇻㇼㇽㇾㇿ「」。？！～￥｜,＜＞＝＋ー＿）（＊＆＾％＄＃＠！～｀｛｝｜￥’”ｌ＜＞・…︙➖１２３４５６７８９０!\" & '(),-./012347:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXY_abcdefghijklmnopqrstuvwxyz{}’“”●「」\n\t !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~‐‑‒–—―‖‗‘’‚‛“”„‟†‡•‣․‥…‧\n\r\t‪‫‬‭‮ ‰‱′″‴‵‶‷‸‹›※‼⁇⁈⁉⁊⁋‾‿⁀⁁⁂⁃⁎⁏⁐⁑⁒⁓⁔⁕⁖⁗⁘⁙⁚⁛⁜⁝⁞ ⁯€™⁠⁡⁢⁣⁤";
        static readonly Vector2 DefaultWindowSize = new Vector2(350f, 150f);

        DebugCharacterSets debugCharacterSets = null;

        public static void ShowPopUp(TranslationDictionaryEditor editor)
        {
            UpdateFontAssets window = GetWindow<UpdateFontAssets>(true, "Update Font Assets", true);
            window.Editor = editor;
            window.DictionaryToEdit = (TranslationDictionary)editor.serializedObject.targetObject;
            window.minSize = DefaultWindowSize;

            // FIXME: comment out the line below.
            window.debugCharacterSets = new DebugCharacterSets(LatinCharacters, JapaneseCharacters);
            window.Show();
        }

        TranslationDictionaryEditor Editor
        {
            get;
            set;
        } = null;

        TranslationDictionary DictionaryToEdit
        {
            get;
            set;
        } = null;

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Testing...", MessageType.Info);
            if(debugCharacterSets != null)
            {
                debugCharacterSets.Draw();
            }
        }

        private class DebugCharacterSets
        {
            readonly string[] allSets;

            public DebugCharacterSets(params string[] allCharacterLists)
            {
                // Setup variables
                SortedSet<char>[] previousSets = new SortedSet<char>[allCharacterLists.Length];
                SortedSet<char> currentSet;
                StringBuilder builder = new StringBuilder();
                // FIXME: get rid of this variable
                bool isCharacterAlreadyAdded = false;

                // Setup member variables
                allSets = new string[allCharacterLists.Length];

                // Go through params
                for (int i = 0; i < allSets.Length; ++i)
                {
                    // Setup variables
                    isCharacterAlreadyAdded = false;
                    currentSet = new SortedSet<char>();

                    // Go through all characters
                    foreach (char c in allCharacterLists[i])
                    {
                        // Make sure this character wasn't added in in a previous language
                        for(int check = 0; check < i; ++check)
                        {
                            if(previousSets[check].Contains(c) == true)
                            {
                                isCharacterAlreadyAdded = true;
                                break;
                            }
                        }

                        // If it wasn't, add this character into the set
                        if(isCharacterAlreadyAdded == false)
                        {
                            currentSet.Add(c);
                        }
                    }

                    // Go through all unique, sorted characters
                    foreach(char c in currentSet)
                    {
                        AddChar(c, builder);
                    }
                    builder.Clear();

                    // Plop this into allSets as a string
                    allSets[i] = builder.ToString();
                    previousSets[i] = currentSet;
                }
            }

            public void Draw()
            {
                foreach(string set in allSets)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.TextArea(set);
                }
            }

            private void AddChar(char c, StringBuilder builder)
            {
                switch (c)
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
        }
    }
}
