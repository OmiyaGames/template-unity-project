using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using TMPro;
using OmiyaGames.Translations;

namespace OmiyaGames.UI.Translations
{
    ///-----------------------------------------------------------------------
    /// <copyright file="UpdateFontAssets.FoldoutSets.cs" company="Omiya Games">
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
    /// <date>10/21/2016</date>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Creates a window that updates the asset font files.
    /// </summary>
    /// <seealso cref="TMP_FontAsset"/>
    public partial class UpdateFontAssets
    {
        private abstract class FoldoutSet
        {
            private readonly UnityEditor.AnimatedValues.AnimBool animation;

            public FoldoutSet(EditorWindow editor, bool defaultIsShown)
            {
                animation = new UnityEditor.AnimatedValues.AnimBool(defaultIsShown, editor.Repaint);
            }

            protected abstract string FoldoutLabel
            {
                get;
            }

            protected abstract void DrawContent();

            public bool Draw()
            {
                // Draw fold out
                animation.target = EditorGUILayout.Foldout(animation.target, FoldoutLabel);

                // Animate expanding fold out
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel += 1;
                using (EditorGUILayout.FadeGroupScope fadeScope = new EditorGUILayout.FadeGroupScope(animation.faded))
                {
                    // Confirm the content of fold out should be drawn
                    if (fadeScope.visible == true)
                    {
                        DrawContent();
                    }
                }
                EditorGUI.indentLevel -= 1;
                return EditorGUI.EndChangeCheck();
            }

            protected static PresetCharacters GetDefaultPreset(SystemLanguage language)
            {
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
                        // Basing this list off of Wikipedia:
                        // https://en.wikipedia.org/wiki/List_of_languages_by_writing_system#Latin_script
                        return PresetCharacters.LatinCharacters;
                    case SystemLanguage.Japanese:
                        return PresetCharacters.JapaneseKanasSymbolsAndRomaji;
                    default:
                        return PresetCharacters.None;
                }
            }

            protected static bool IsLatinLanguage(SystemLanguage language)
            {
                return (GetDefaultPreset(language) == PresetCharacters.LatinCharacters);
            }
        }

        private class LanguageSets : FoldoutSet
        {
            private readonly SupportedLanguages languages;

            public LanguageSets(EditorWindow editor, SupportedLanguages languages) : base(editor, true)
            {
                this.languages = languages;
                LanguageToUpdate = new bool[languages.Count];

                // Check what languages to update
                for (int i = 0; i < languages.Count; ++i)
                {
                    LanguageToUpdate[i] = false;
                    SupportedLanguages.Language metadata = languages.GetLanguageMetaData(i);
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

            public bool[] LanguageToUpdate
            {
                get;
            }

            protected override string FoldoutLabel
            {
                get
                {
                    return "Languages With Updates";
                }
            }

            protected override void DrawContent()
            {
                // Give direction on what to do with the text areas below
                EditorGUILayout.HelpBox("Select Languages to Update", MessageType.Info);
                for (int i = 0; i < languages.Count; ++i)
                {
                    LanguageToUpdate[i] = EditorGUILayout.ToggleLeft(languages[i], LanguageToUpdate[i]);
                }
            }
        }

        private class DebugCharacterSets : FoldoutSet
        {
            readonly List<CharacterSet> allSets;

            public DebugCharacterSets(EditorWindow editor, Dictionary<PresetCharacters, CharacterSet> allCharacterLists) : base(editor, false)
            {
                // Setup variables
                SortedSet<char> currentSet;
                StringBuilder builder = new StringBuilder();

                // Setup member variables
                allSets = new List<CharacterSet>(allCharacterLists.Count);

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

            protected override string FoldoutLabel
            {
                get
                {
                    return "Preview Preset Characters";
                }
            }

            protected override void DrawContent()
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

        private class FontSets : FoldoutSet
        {
            public class OnClickEventArgs : System.EventArgs
            {
                public OnClickEventArgs(int capacity, TMP_FontAsset font, PresetCharacters characters) : base()
                {
                    Languages = new List<string>(capacity);
                    Font = font;
                    PresetsToAdd = characters;
                    ActionToTake = Action.Append;
                }

                public TMP_FontAsset Font
                {
                    get;
                }

                public List<string> Languages
                {
                    get;
                }

                public PresetCharacters PresetsToAdd
                {
                    get;
                    internal set;
                }

                public Action ActionToTake
                {
                    get;
                    internal set;
                }
            }

            public delegate void ButtonClicked(FontSets source, OnClickEventArgs args);
            private const string PrependInfo = "From languages: ";
            static readonly GUILayoutOption ActionsWidth = GUILayout.Width(80f);
            static readonly GUILayoutOption InfoWidth = GUILayout.Width(30f);
            static readonly GUILayoutOption ButtonsWidth = GUILayout.Width(120f);
            static readonly GUILayoutOption CommonHeight = GUILayout.Height(16.25f);
            static readonly GUILayoutOption InfoHeight = GUILayout.Height(16.75f);
            static readonly GUILayoutOption ButtonHeight = GUILayout.Height(15f);

            public event ButtonClicked OnAfterButtonClicked;
            private readonly SupportedLanguages languages;
            private readonly bool[] languageToUpdate;
            private readonly List<TMP_FontAsset> allFonts = new List<TMP_FontAsset>();
            private readonly Dictionary<TMP_FontAsset, OnClickEventArgs> allMetaData = new Dictionary<TMP_FontAsset, OnClickEventArgs>();

            public FontSets(UpdateFontAssets editor, SupportedLanguages languages, bool[] languageToUpdate) : base(editor, true)
            {
                this.languages = languages;
                this.languageToUpdate = languageToUpdate;
            }

            public void UpdateFonts()
            {
                // Setup variables
                SupportedLanguages.Language languageMetadata;
                PresetCharacters defaultPreset = PresetCharacters.None;
                OnClickEventArgs fontMetadata;

                // Reset the list
                allFonts.Clear();
                foreach (OnClickEventArgs data in allMetaData.Values)
                {
                    data.Languages.Clear();
                }

                // Go through all the languages
                for (int i = 0; i < languages.Count; ++i)
                {
                    // Check if we want to check this language
                    if (languageToUpdate[i] == false)
                    {
                        continue;
                    }

                    // Grab the metadata for this language
                    languageMetadata = languages.GetLanguageMetaData(i);
                    defaultPreset = PresetCharacters.None;
                    if (languageMetadata.IsSystemDefault == true)
                    {
                        defaultPreset = GetDefaultPreset(languageMetadata.LanguageMappedTo);
                    }

                    // If so, grab all the fonts
                    foreach (TMP_FontAsset font in languageMetadata.Fonts)
                    {
                        if (font != null)
                        {
                            if (allMetaData.TryGetValue(font, out fontMetadata) == true)
                            {
                                if (fontMetadata.Languages.Count == 0)
                                {
                                    // Indicate this is a unique new font
                                    allFonts.Add(font);
                                }

                                // Update the font's metadata
                                fontMetadata.Languages.Add(languages[i]);
                                fontMetadata.PresetsToAdd |= defaultPreset;
                            }
                            else
                            {
                                // Indicate this is a unique new font
                                allFonts.Add(font);

                                // Create a new font's metadata
                                fontMetadata = new OnClickEventArgs(languages.Count, font, defaultPreset);
                                fontMetadata.Languages.Add(languages[i]);
                                allMetaData.Add(font, fontMetadata);
                            }
                        }
                    }
                }
            }

            protected override string FoldoutLabel
            {
                get
                {
                    return "Add Presets on Fonts";
                }
            }

            protected override void DrawContent()
            {
                // Give direction on what to do with the text areas below
                EditorGUILayout.HelpBox("Configure if a font from a language should have preset fonts attached to it as well.", MessageType.Info);
                using (EditorGUILayout.HorizontalScope horizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    // Draw info column
                    DrawInfoColumn();

                    // Draw fonts column
                    DrawFontsColumn();

                    // Draw preset column
                    DrawPresetsColumn();

                    // Draw action column
                    DrawActionsColumn();

                    // Draw button column
                    DrawButtonsColumn();
                }
            }

            private void DrawButtonsColumn()
            {
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(ButtonsWidth))
                {
                    EditorGUILayout.LabelField("Button", EditorStyles.boldLabel, ButtonsWidth);
                    foreach (TMP_FontAsset font in allFonts)
                    {
                        if (GUILayout.Button("Update Font", ButtonHeight, ButtonsWidth) == true)
                        {
                            // Run the button
                            OnAfterButtonClicked?.Invoke(this, allMetaData[font]);
                        }
                    }
                }
            }

            private void DrawActionsColumn()
            {
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(ActionsWidth))
                {
                    EditorGUILayout.LabelField("Action", EditorStyles.boldLabel, ActionsWidth);
                    foreach (TMP_FontAsset font in allFonts)
                    {
                        int action = (int)allMetaData[font].ActionToTake;
                        action = EditorGUILayout.IntPopup(action, ActionNames, ActionValues, ActionsWidth, CommonHeight);
                        allMetaData[font].ActionToTake = (Action)action;
                    }
                }
            }

            private void DrawPresetsColumn()
            {
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Include Preset", EditorStyles.boldLabel);
                    foreach (TMP_FontAsset font in allFonts)
                    {
                        int mask = (int)allMetaData[font].PresetsToAdd;
                        mask = EditorGUILayout.MaskField(mask, PresetCharacterNames, CommonHeight);
                        allMetaData[font].PresetsToAdd = (PresetCharacters)mask;
                    }
                }
            }

            private void DrawInfoColumn()
            {
                StringBuilder builder = new StringBuilder();
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(InfoWidth))
                {
                    GUIContent tooltip = EditorGUIUtility.IconContent("_Help");
                    EditorGUILayout.LabelField("?", EditorStyles.boldLabel, InfoWidth);
                    foreach (TMP_FontAsset font in allFonts)
                    {
                        builder.Clear();
                        builder.Append(PrependInfo);
                        foreach (string language in allMetaData[font].Languages)
                        {
                            if (builder.Length > PrependInfo.Length)
                            {
                                builder.Append(", ");
                            }
                            builder.Append(language);
                        }
                        tooltip.tooltip = builder.ToString();

                        EditorGUILayout.LabelField(tooltip, InfoHeight, InfoWidth);
                    }
                }
            }

            private void DrawFontsColumn()
            {
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Font", EditorStyles.boldLabel);
                    GUI.enabled = false;
                    foreach (TMP_FontAsset font in allFonts)
                    {
                        EditorGUILayout.ObjectField(font, typeof(TMP_Asset), false, CommonHeight);
                    }
                    GUI.enabled = true;
                }
            }
        }
    }
}
