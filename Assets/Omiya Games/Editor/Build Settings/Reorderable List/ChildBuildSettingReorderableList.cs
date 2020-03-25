//#define TEST
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Builds;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ChildBuildSettingReorderableList.cs" company="Omiya Games">
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
    /// <date>11/03/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper class to generate a reorderable list.
    /// </summary>
    public class ChildBuildSettingReorderableList : IReorderableList
    {
        public class BuildSettingCreator
        {
            public BuildSettingCreator(string name, GenericMenu.MenuFunction function)
            {
                DisplayName = name;
                Function = function;
            }

            public string DisplayName
            {
                get;
            }

            public GenericMenu.MenuFunction Function
            {
                get;
            }
        }

        public BuildSettingCreator[] AllMethods
        {
            get;
        }

        public ChildBuildSettingReorderableList(Object target, SerializedProperty property, GUIContent label) : base (target, property, label)
        {
            // Setup List
            List.onAddDropdownCallback = DrawBuildSettingListDropdown;
            List.elementHeight = EditorUiUtility.GetHeight(2, 4f);

            // Setup all Methods
            AllMethods = new BuildSettingCreator[]
            {
                #region Windows
                new BuildSettingCreator("Windows 64-bit", () => { CreateDesktopPlatformSettings<WindowsBuildSetting>("Windows 64-bit", IPlatformBuildSetting.Architecture.Build64Bit); }),
                new BuildSettingCreator("Windows 32-bit", () => { CreateDesktopPlatformSettings<WindowsBuildSetting>("Windows 32-bit", IPlatformBuildSetting.Architecture.Build32Bit); }),
                null,
                #endregion

                new BuildSettingCreator("Mac", () => { AddAndModify<MacBuildSetting>("Mac"); }),
                null,

                #region Linux
                new BuildSettingCreator("Linux Universal", () => { CreateDesktopPlatformSettings<LinuxBuildSetting>("Linux", IPlatformBuildSetting.Architecture.BuildUniversal); }),
                new BuildSettingCreator("Linux 64-bit", () => { CreateDesktopPlatformSettings<LinuxBuildSetting>("Linux 64-bit", IPlatformBuildSetting.Architecture.Build64Bit); }),
                new BuildSettingCreator("Linux 32-bit", () => { CreateDesktopPlatformSettings<LinuxBuildSetting>("Linux 32-bit", IPlatformBuildSetting.Architecture.Build32Bit); }),
                null,
                #endregion

                new BuildSettingCreator("WebGL", () => { CreateWebGLSettings(); }),
                null,

                // FIXME: add UWP support
                #region Mobile
                new BuildSettingCreator("iOS", () => { AddAndModify<IosBuildSetting>("iOS"); }),
                new BuildSettingCreator("Android", () => { AddAndModify<AndroidBuildSetting>("Android"); }),
                //new BuildSettingCreator("UWP", () => { AddAndModify<UwpBuildSetting>("UWP"); }),
                null,
                #endregion

                // FIXME: add Facebook support
                #region Facebook
                //new BuildSettingCreator("Facebook Gameroom, WebGL", () => { CreateWebGLSettings(); }),
                //new BuildSettingCreator("Facebook Gameroom, Windows 64-bit", () => { CreateDesktopPlatformSettings<WindowsBuildSetting>("Windows 64-bit", IPlatformBuildSetting.Architecture.Build64Bit); }),
                //new BuildSettingCreator("Facebook Gameroom,Windows 32-bit", () => { CreateDesktopPlatformSettings<WindowsBuildSetting>("Windows 32-bit", IPlatformBuildSetting.Architecture.Build32Bit); }),
                //null,
                #endregion

                new BuildSettingCreator("Group of Platforms", () => { AddAndModify<GroupBuildSetting>("Group"); }),
            };
        }

        protected override void DrawBuildSettingListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Get all the properties
            SerializedProperty element = Property.GetArrayElementAtIndex(index);

            // Calculate position
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorUiUtility.VerticalMargin;

            // Draw the object field
            bool originalEnabled = GUI.enabled;
#if !TEST
            GUI.enabled = false;
#endif
            element.objectReferenceValue = EditorGUI.ObjectField(rect, "", element.objectReferenceValue, typeof(IChildBuildSetting), false);
            GUI.enabled = originalEnabled;

            // Calculate position
            rect.y += rect.height;
            rect.y += EditorUiUtility.VerticalMargin;

            // Draw Edit buttons
            DrawButtons(rect, element.objectReferenceValue);
        }

        #region Helper Methods
        private void CreateWebGLSettings()
        {
            SerializedProperty element = Add<WebGlBuildSetting>("WebGL");
            if (element != null)
            {
                // Update file name to make it slug-based
                SerializedProperty childElement = element.FindPropertyRelative("fileName");
                if (childElement != null)
                {
                    childElement = childElement.FindPropertyRelative("asSlug");
                    if (childElement != null)
                    {
                        childElement.boolValue = true;
                    }
                }

                // Update template path to pass it what's already in the editor
                childElement = element.FindPropertyRelative("templatePath");
                if (childElement != null)
                {
                    childElement.stringValue = PlayerSettings.WebGL.template;
                }
            }

            // Apply the property
            ApplyModification();
        }

        private void CreateDesktopPlatformSettings<T>(string name, IPlatformBuildSetting.Architecture architecture) where T : IPlatformBuildSetting
        {
            SerializedProperty element = Add<T>(name);
            if (element.objectReferenceValue is IStandaloneBuildSetting)
            {
                IStandaloneBuildSetting setting = (IStandaloneBuildSetting)element.objectReferenceValue;
                setting.ArchitectureToBuild = architecture;
            }

            // Apply the property
            ApplyModification();
        }

        private void DrawBuildSettingListDropdown(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
            foreach (BuildSettingCreator value in AllMethods)
            {
                if (value != null)
                {
                    menu.AddItem(new GUIContent(value.DisplayName), false, value.Function);
                }
                else
                {
                    menu.AddSeparator("");
                }
            }
#if TEST
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Empty"), false, () =>
            {
                SerializedProperty element = CreateNewElement();
                element.objectReferenceValue = null;
                ApplyModification();
            });
#endif
            menu.ShowAsContext();
        }
        #endregion
    }
}
