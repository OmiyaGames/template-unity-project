using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using TMPro;
using OmiyaGames.Translations;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="UpdateFontAssets.cs" company="Omiya Games">
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
    /// Creates a window that updates the asset font files.
    /// </summary>
    /// <seealso cref="TMPro.TMP_FontAsset"/>
    public partial class UpdateFontAssets : EditorWindow
    {
        public enum Action
        {
            Append,
            Overwrite
        }

        [System.Flags]
        public enum PresetCharacters
        {
            None = 0,
            LatinCharacters = 1 << 0,
            JapaneseKanasSymbolsAndRomaji = 1 << 1
        }

        static readonly Vector2 DefaultWindowSize = new Vector2(500f, 200f);
        static string[] presetCharacterNames = null;
        static int[] actionValues = null;
        static readonly StringBuilder builder = new StringBuilder();
        static readonly Dictionary<PresetCharacters, CharacterSet> PresetCharacterSets = new Dictionary<PresetCharacters, CharacterSet>()
        {
            {
                PresetCharacters.LatinCharacters,
                new CharacterSet("Latin Characters",
                    "\t\n\r !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ           ​‌‍‎‏‐‑‒–—―‖‗‘’‚‛“”„‟†‡•‣․‥…‧‪‫‬‭‮ ‰‱′″‴‵‶‷‸‹›※‼‽‾‿⁀⁁⁂⁃⁄⁅⁆⁇⁈⁉⁊⁋⁌⁍⁎⁏⁐⁑⁒⁓⁔⁕⁖⁗⁘⁙⁚⁛⁜⁝⁞ ⁠⁡⁢⁣⁤⁦⁧⁨⁩⁪⁫⁬⁭⁮⁯€™")
            }, {
                PresetCharacters.JapaneseKanasSymbolsAndRomaji,
                new CharacterSet("Japanese Kanas, Romaji, and Symbols",
                    "●➖　。「」ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔゕゖ゙゚゛゜ゝゞゟ゠ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶヷヸヹヺ・ーヽヾヿㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇺㇻㇼㇽㇾㇿ︙！＂＃＄％＆＇（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［＼］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝～｟｠｡｢｣､･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ￠￡￢￣￤￥￦￧￨￩￪￫￬￭￮")
            }
        };

        #region Helper Structs and Classes
        private struct CharacterSet
        {
            public readonly string name;
            public readonly string characters;

            public CharacterSet(string name, string characters)
            {
                this.name = name;
                this.characters = characters;
            }
        }
        #endregion

        public static void ShowPopUp(TranslationDictionaryEditor editor)
        {
            UpdateFontAssets window = GetWindow<UpdateFontAssets>(true, "Update Font Assets", true);
            window.Editor = editor;
            window.DictionaryToEdit = (TranslationDictionary)editor.serializedObject.targetObject;
            window.Setup();
            window.Show();
        }

        #region Properties
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

        PresetCharacters[] AddPresetCharacters
        {
            get;
            set;
        }

        DebugCharacterSets DebugFoldout
        {
            get;
            set;
        } = null;

        LanguageSets LanguageFoldout
        {
            get;
            set;
        } = null;

        FontSets FontFoldout
        {
            get;
            set;
        } = null;

        SortedSet<char> CurrentSet
        {
            get;
        } = new SortedSet<char>();

        public static string[] PresetCharacterNames
        {
            get
            {
                if (presetCharacterNames == null)
                {
                    int count = System.Enum.GetNames(typeof(PresetCharacters)).Length;
                    count -= 1;
                    presetCharacterNames = new string[count];
                    for (int i = 0; i < count; ++i)
                    {
                        PresetCharacters character = (PresetCharacters)(1 << i);
                        presetCharacterNames[i] = PresetCharacterSets[character].name;
                    }
                }
                return presetCharacterNames;
            }
        }

        public static string[] ActionNames
        {
            get;
        } = System.Enum.GetNames(typeof(Action));

        public static int[] ActionValues
        {
            get
            {
                // Setup action values
                if (actionValues == null)
                {
                    Action[] allActions = (Action[])System.Enum.GetValues(typeof(Action));
                    actionValues = new int[allActions.Length];
                    for (int i = 0; i < actionValues.Length; ++i)
                    {
                        actionValues[i] = (int)allActions[i];
                    }
                }
                return actionValues;
            }
        }
        #endregion

        private void Setup()
        {
            // Setup minimum window size
            minSize = DefaultWindowSize;

            // Setting up previews for each character sets
            DebugFoldout = new DebugCharacterSets(this, PresetCharacterSets);
            LanguageFoldout = new LanguageSets(this, Languages);
            FontFoldout = new FontSets(this, Languages, LanguageFoldout.LanguageToUpdate);
            FontFoldout.OnAfterButtonClicked += OnUpdateFontClicked;
            FontFoldout.UpdateFonts();
        }

        private void OnGUI()
        {
            // Draw the scroll view
            using (EditorGUILayout.ScrollViewScope scrollScope = new EditorGUILayout.ScrollViewScope(ScrollPosition, GUILayout.Height(position.height)))
            {
                // Draw the language checkboxes
                EditorGUILayout.Space();
                if (LanguageFoldout.Draw() == true)
                {
                    FontFoldout.UpdateFonts();
                }

                // Draw preview of preset characters
                EditorGUILayout.Space();
                DebugFoldout.Draw();

                // Draw fonts and their actions
                EditorGUILayout.Space();
                FontFoldout.Draw();

                // FIXME: draw fonts, and buttons correlating with them
                // Update scroll position
                ScrollPosition = scrollScope.scrollPosition;
            }
        }

        private void OnUpdateFontClicked(FontSets source, FontSets.OnClickEventArgs args)
        {
            // Setup variables
            FontAssetCreationSettings settings = args.Font.creationSettings;
            CurrentSet.Clear();

            // Start appending characters already in the settings
            if (args.ActionToTake == Action.Append)
            {
                // Add them to the hashset
                Add(CurrentSet, TMP_FontAsset.GetCharacters(args.Font));
            }

            // Add all the preset characters
            foreach(KeyValuePair<PresetCharacters, CharacterSet> pair in PresetCharacterSets)
            {
                // Check if this preset should be appended to the hashset
                if((args.PresetsToAdd & pair.Key) != 0)
                {
                    Add(CurrentSet, pair.Value.characters);
                }
            }

            // Go through the languages this font supports
            foreach (string language in args.Languages)
            {
                // Grab the language index
                int languageIndex = Languages[language];

                // Go through all the texts in the translation file
                foreach (TranslationDictionary.LanguageTextMap languageMap in DictionaryToEdit.AllTranslations.Values)
                {
                    // Add the text into the set
                    Add(CurrentSet, languageMap[languageIndex]);
                }
            }

            // Apply changes
            settings.characterSetSelectionMode = 7;
            settings.characterSequence = ToString(CurrentSet);
            args.Font.creationSettings = settings;

            // Open the window
            TMPro.EditorUtilities.TMPro_FontAssetCreatorWindow.ShowFontAtlasCreatorWindow(args.Font);
        }

        public static void Add(ISet<char> set, string add)
        {
            if ((set != null) && (string.IsNullOrEmpty(add) == false))
            {
                // Add them to the hashset
                foreach (char c in add)
                {
                    set.Add(c);
                }
            }
        }

        public static string ToString(ISet<char> set)
        {
            builder.Clear();
            foreach (char c in set)
            {
                builder.Append(c);
            }
            return builder.ToString();
        }
    }
}
