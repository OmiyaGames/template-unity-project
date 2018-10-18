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
        const float LanguageCheckBoxSetHeight = 100;
        static readonly Vector2 DefaultWindowSize = new Vector2(350f, 150f);
        const string LatinCharacters = "\t\n\r !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ           ​‌‍‎‏‐‑‒–—―‖‗‘’‚‛“”„‟†‡•‣․‥…‧‪‫‬‭‮ ‰‱′″‴‵‶‷‸‹›※‼‽‾‿⁀⁁⁂⁃⁄⁅⁆⁇⁈⁉⁊⁋⁌⁍⁎⁏⁐⁑⁒⁓⁔⁕⁖⁗⁘⁙⁚⁛⁜⁝⁞ ⁠⁡⁢⁣⁤⁦⁧⁨⁩⁪⁫⁬⁭⁮⁯€™";
        const string JapaneseCharacters = LatinCharacters + "●➖　。「」ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔゕゖ゙゚゛゜ゝゞゟ゠ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺ・ーヽヾヿㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇺㇻㇼㇽㇾㇿ︙！＃＄％＆（）＊＋０１２３４５６７８９＜＝＞？＠＾＿｀ｌ｛｜｝～･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ￥";

        DebugCharacterSets debugCharacterSets = null;

        public static void ShowPopUp(TranslationDictionaryEditor editor)
        {
            UpdateFontAssets window = GetWindow<UpdateFontAssets>(true, "Update Font Assets", true);
            window.Editor = editor;
            window.DictionaryToEdit = (TranslationDictionary)editor.serializedObject.targetObject;
            window.LanguageToUpdate = new bool[window.Languages.Count];
            window.SetupDefaults();

            // Debugging line below: useful for filtering unique characters and sorting them in order
            // to later paste into this code.
            //window.debugCharacterSets = new DebugCharacterSets(LatinCharacters, JapaneseCharacters);
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

        Vector2 ScrollPosition
        {
            get;
            set;
        } = Vector2.zero;

        SupportedLanguages Languages
        {
            get
            {
                return DictionaryToEdit.SupportedLanguages;
            }
        }

        bool[] LanguageToUpdate
        {
            get;
            set;
        }

        private void SetupDefaults()
        {
            // Setup minimum window size
            minSize = DefaultWindowSize;

            // Check what languages to update
            for (int i = 0; i < Languages.Count; ++i)
            {
                LanguageToUpdate[i] = false;
                SupportedLanguages.Language metadata = Languages.GetLanguageMetaData(i);
                if (metadata.IsSystemDefault == false)
                {
                    LanguageToUpdate[i] = true;
                }
                else if (IsLatinLanguage(metadata.LanguageMappedTo) == false)
                {
                    LanguageToUpdate[i] = true;
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Testing...", MessageType.Info);
            if (debugCharacterSets != null)
            {
                debugCharacterSets.Draw();
            }

            using (EditorGUILayout.ScrollViewScope scope = new EditorGUILayout.ScrollViewScope(ScrollPosition, GUILayout.Height(LanguageCheckBoxSetHeight)))
            {
                for (int i = 0; i < Languages.Count; ++i)
                {
                    LanguageToUpdate[i] = EditorGUILayout.ToggleLeft(Languages[i], LanguageToUpdate[i]);
                }

                // Update scroll position
                ScrollPosition = scope.scrollPosition;
            }

        }

        private bool IsLatinLanguage(SystemLanguage language)
        {
            // Basing this list off of Wikipedia:
            // https://en.wikipedia.org/wiki/List_of_languages_by_writing_system#Latin_script
            Debug.Log(language);
            switch (language)
            {
                case SystemLanguage.Afrikaans:
                case SystemLanguage.Basque:
                case SystemLanguage.Catalan:
                case SystemLanguage.Danish:
                case SystemLanguage.Dutch:
                case SystemLanguage.English:
                case SystemLanguage.Estonian:
                case SystemLanguage.Faroese:
                case SystemLanguage.Finnish:
                case SystemLanguage.French:
                case SystemLanguage.German:
                case SystemLanguage.Hungarian:
                case SystemLanguage.Icelandic:
                case SystemLanguage.Indonesian:
                case SystemLanguage.Italian:
                case SystemLanguage.Latvian:
                case SystemLanguage.Lithuanian:
                case SystemLanguage.Norwegian:
                case SystemLanguage.Polish:
                case SystemLanguage.Portuguese:
                case SystemLanguage.Romanian:
                case SystemLanguage.Slovak:
                case SystemLanguage.Slovenian:
                case SystemLanguage.Spanish:
                case SystemLanguage.Swedish:
                case SystemLanguage.Turkish:
                case SystemLanguage.Vietnamese:
                    return true;
                default:
                    return false;
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

                // Setup member variables
                allSets = new string[allCharacterLists.Length];

                // Go through params
                for (int i = 0; i < allCharacterLists.Length; ++i)
                {
                    // Setup variables
                    currentSet = new SortedSet<char>();

                    // Go through all characters
                    foreach (char c in allCharacterLists[i])
                    {
                        currentSet.Add(c);
                    }

                    // Go through all unique, sorted characters
                    builder.Clear();
                    foreach (char c in currentSet)
                    {
                        AddChar(c, builder);
                    }

                    // Plop this into allSets as a string
                    allSets[i] = builder.ToString();
                    previousSets[i] = currentSet;
                }
            }

            public void Draw()
            {
                foreach (string set in allSets)
                {
                    EditorGUILayout.HelpBox("Copy texts below", MessageType.Info);
                    EditorGUILayout.Space();
                    EditorGUILayout.TextArea(set, EditorStyles.textArea);
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
