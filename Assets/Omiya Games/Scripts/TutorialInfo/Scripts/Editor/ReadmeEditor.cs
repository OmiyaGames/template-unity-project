using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="Readme.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2019 Omiya Games
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
    /// <date>4/18/2019</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// ReadMe <see cref="ScriptableObject"/> largely copied from Unity's own.
    /// </summary>
    /// <seealso cref="Readme"/>
    [CustomEditor(typeof(Readme))]
    [InitializeOnLoad]
    public class ReadmeEditor : Editor
    {
        private const int BodyTextSize = 13;
        private const int TitleTextSize = 26;
        private const int HeadingTextSize = 18;
        static string kShowedReadmeSessionStateName = "ReadmeEditor.showedReadme";

        static float kSpace = 16f;

        static ReadmeEditor()
        {
            EditorApplication.delayCall += SelectReadmeAutomatically;
        }

        static void SelectReadmeAutomatically()
        {
            if (!EditorPrefs.GetBool(kShowedReadmeSessionStateName, false))
            {
                var readme = SelectReadme();
                EditorPrefs.SetBool(kShowedReadmeSessionStateName, true);

                if (readme && !readme.LoadedLayout)
                {
                    LoadLayout();
                    readme.LoadedLayout = true;
                }
            }
        }

        static void LoadLayout()
        {
            var assembly = typeof(EditorApplication).Assembly;
            var windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
            var method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, new object[] { Path.Combine(Application.dataPath, "TutorialInfo/Layout.wlt"), false });
        }

        [MenuItem("Tutorial/Show Tutorial Instructions")]
        static Readme SelectReadme()
        {
            var ids = AssetDatabase.FindAssets("Readme t:Readme");
            if (ids.Length == 1)
            {
                var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

                Selection.objects = new UnityEngine.Object[] { readmeObject };

                return (Readme)readmeObject;
            }
            else
            {
                Debug.Log("Couldn't find a readme");
                return null;
            }
        }

        protected override void OnHeaderGUI()
        {
            var readme = (Readme)target;
            Init();

            var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

            GUILayout.BeginHorizontal("In BigTitle");
            {
                GUILayout.Label(readme.Icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                GUILayout.Label(readme.Title, TitleStyle);
            }
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            var readme = (Readme)target;
            Init();

            // Check if edit mode is on
            if (readme.EditMode == false)
            {
                // Hiding the edit mode button for now
                //readme.EditMode = GUILayout.Toggle(readme.EditMode, "Edit README");

                // Go through all sections
                foreach (var section in readme.Sections)
                {
                    if (!string.IsNullOrEmpty(section.Heading))
                    {
                        GUILayout.Label(section.Heading, HeadingStyle);
                    }
                    if (!string.IsNullOrEmpty(section.Text))
                    {
                        GUILayout.Label(section.Text, BodyStyle);
                    }
                    if (!string.IsNullOrEmpty(section.LinkText))
                    {
                        if (LinkLabel(new GUIContent(section.LinkText)))
                        {
                            Application.OpenURL(section.Url);
                        }
                    }
                    if ((section.TextList != null) && (section.TextList.Length > 0))
                    {
                        GUILayout.Space(kSpace);
                        System.Text.StringBuilder builder = new System.Text.StringBuilder();
                        foreach(string text in section.TextList)
                        {
                            builder.Clear();
                            builder.Append(" • ");
                            builder.Append(text);
                            GUILayout.Label(builder.ToString(), BodyStyle);
                        }
                    }
                    GUILayout.Space(kSpace);
                }
            }
            else
            {
                base.OnInspectorGUI();
            }
        }


        bool m_Initialized;

        GUIStyle LinkStyle { get { return m_LinkStyle; } }
        [SerializeField] GUIStyle m_LinkStyle;

        GUIStyle TitleStyle { get { return m_TitleStyle; } }
        [SerializeField] GUIStyle m_TitleStyle;

        GUIStyle HeadingStyle { get { return m_HeadingStyle; } }
        [SerializeField] GUIStyle m_HeadingStyle;

        GUIStyle BodyStyle { get { return m_BodyStyle; } }
        [SerializeField] GUIStyle m_BodyStyle;

        void Init()
        {
            if (m_Initialized)
                return;
            m_BodyStyle = new GUIStyle(EditorStyles.label);
            m_BodyStyle.wordWrap = true;
            m_BodyStyle.fontSize = BodyTextSize;

            m_TitleStyle = new GUIStyle(m_BodyStyle);
            m_TitleStyle.fontSize = TitleTextSize;

            m_HeadingStyle = new GUIStyle(m_BodyStyle);
            m_HeadingStyle.fontSize = HeadingTextSize;

            m_LinkStyle = new GUIStyle(m_BodyStyle);
            m_LinkStyle.wordWrap = false;
            // Match selection color which works nicely for both light and dark skins
            m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
            m_LinkStyle.stretchWidth = false;

            m_Initialized = true;
        }

        bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

            Handles.BeginGUI();
            Handles.color = LinkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            return GUI.Button(position, label, LinkStyle);
        }
    }
}
