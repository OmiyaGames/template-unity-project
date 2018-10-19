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

        const float LanguageCheckBoxSetHeight = 50;
        const float PreviewPresetCharactersHeight = 150;
        static readonly Vector2 DefaultWindowSize = new Vector2(350f, 150f);
        static readonly string[] ActionNames = System.Enum.GetNames(typeof(Action));
        static int[] ActionValues = null;
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

        // FIXME: abstract this and split to 3 parts.
        private class DebugCharacterSets
        {
            readonly List<CharacterSet> allSets;
            readonly EditorWindow editor;
            readonly UnityEditor.AnimatedValues.AnimBool animation;
            Vector2 scrollPosition = Vector2.zero;

            public DebugCharacterSets(EditorWindow editor, Dictionary<PresetCharacters, CharacterSet> allCharacterLists)
            {
                // Setup variables
                SortedSet<char> currentSet;
                StringBuilder builder = new StringBuilder();

                // Setup member variables
                allSets = new List<CharacterSet>(allCharacterLists.Count);
                this.editor = editor;
                animation = new UnityEditor.AnimatedValues.AnimBool(false, editor.Repaint);

                // Go through params
                PresetCharacters[] allPresets = (PresetCharacters[])System.Enum.GetValues(typeof(PresetCharacters));
                CharacterSet set;
                foreach (PresetCharacters preset in allPresets)
                {
                    if (allCharacterLists.TryGetValue(preset, out set) == true)
                    {
                        // Setup variables
                        currentSet = new SortedSet<char>();

                        // Go through all characters
                        foreach (char c in set.characters)
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
                        allSets.Add(new CharacterSet(set.name, builder.ToString()));
                    }
                }
            }

            public void Draw()
            {
                // Draw fold out
                animation.target = EditorGUILayout.Foldout(animation.target, "Preview Preset Characters");

                // Animate expanding fold out
                using (EditorGUILayout.FadeGroupScope fadeScope = new EditorGUILayout.FadeGroupScope(animation.faded))
                {
                    // Confirm the content of fold out should be drawn
                    if (fadeScope.visible == true)
                    {
                        // Draw the scroll view
                        using (EditorGUILayout.ScrollViewScope scrollScope = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.Height(PreviewPresetCharactersHeight)))
                        {
                            // Give direction on what to do with the text areas below
                            EditorGUILayout.HelpBox("Feel free to copy the texts below", MessageType.Info);
                            foreach (CharacterSet set in allSets)
                            {
                                // Draw the language and their character set
                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField(set.name);
                                EditorGUILayout.TextArea(set.characters, EditorStyles.textArea);
                            }

                            // Update scroll position
                            scrollPosition = scrollScope.scrollPosition;
                        }
                    }
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
        #endregion

        public static void ShowPopUp(TranslationDictionaryEditor editor)
        {
            UpdateFontAssets window = GetWindow<UpdateFontAssets>(true, "Update Font Assets", true);
            window.Editor = editor;
            window.DictionaryToEdit = (TranslationDictionary)editor.serializedObject.targetObject;
            window.LanguageToUpdate = new bool[window.Languages.Count];
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

        bool[] LanguageToUpdate
        {
            get;
            set;
        }

        PresetCharacters[] AddPresetCharacters
        {
            get;
            set;
        }

        DebugCharacterSets DebugSets
        {
            get;
            set;
        } = null;

        Action UpdateAction
        {
            get;
            set;
        } = Action.Append;
        #endregion

        private void Setup()
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

            // Setting up previews for each character sets
            DebugSets = new DebugCharacterSets(this, PresetCharacterSets);

            // Setup action values
            if(ActionValues == null)
            {
                Action[] allActions = (Action[])System.Enum.GetValues(typeof(Action));
                ActionValues = new int[allActions.Length];
                for(int i = 0; i < ActionValues.Length; ++i)
                {
                    ActionValues[i] = (int)allActions[i];
                }
            }
        }

        private void OnGUI()
        {
            // Draw the action to take
            EditorGUILayout.HelpBox("Action determines whether characters from the translation files will append or overwrite characters already in the Font Asset's file.", MessageType.Info);
            UpdateAction = (Action)EditorGUILayout.IntPopup("Action", ((int)UpdateAction), ActionNames, ActionValues);

            // Draw the language checkboxes
            DrawLanguageCheckboxes();

            // Draw preview of preset characters
            DebugSets.Draw();

            // FIXME: draw fonts, and buttons correlating with them
        }

        private void DrawLanguageCheckboxes()
        {
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
    }
}
