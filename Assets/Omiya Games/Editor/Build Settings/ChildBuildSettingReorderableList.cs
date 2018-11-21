#define TEST
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Builds;

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
    /// Property drawer for <code>CustomFileName</code>.
    /// </summary>
    public class ChildBuildSettingReorderableList
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

        public readonly BuildSettingCreator[] AllMethods;

        public ChildBuildSettingReorderableList(UnityEngine.Object target, SerializedProperty property, GUIContent label)
        {
            // Member Variable
            Target = target;
            Property = property;
            Label = label;

            // Setup List
            List = new ReorderableList(Property.serializedObject, Property, true, true, true, true);
            List.drawHeaderCallback = DrawBuildSettingListHeader;
            List.drawElementCallback = DrawBuildSettingListElement;
            List.onAddDropdownCallback = DrawBuildSettingListDropdown;
            List.onReorderCallback = OnBuildSettingListReorder;
            List.onRemoveCallback = OnBuildSettingListRemove;
            List.elementHeight = EditorUiUtility.GetHeight(2, 4f);

            // Setup all Methods
            AllMethods = new BuildSettingCreator[]
            {
                new BuildSettingCreator("Group of Platforms", () => { AddAndModify<GroupBuildSetting>("Group"); }),
                null,
                new BuildSettingCreator("Windows 64-bit", () => { CreateDesktopPlatformSettings<WindowsBuildSetting>("Windows 64-bit", IPlatformBuildSetting.Architecture.Build64Bit); }),
                new BuildSettingCreator("Windows 32-bit", () => { CreateDesktopPlatformSettings<WindowsBuildSetting>("Windows 32-bit", IPlatformBuildSetting.Architecture.Build32Bit); }),
                null,
                new BuildSettingCreator("Mac", () => { AddAndModify<MacBuildSetting>("Mac"); }),
                null,
                new BuildSettingCreator("Linux Universal", () => { CreateDesktopPlatformSettings<LinuxBuildSetting>("Linux", IPlatformBuildSetting.Architecture.BuildUniversal); }),
                new BuildSettingCreator("Linux 64-bit", () => { CreateDesktopPlatformSettings<LinuxBuildSetting>("Linux 64-bit", IPlatformBuildSetting.Architecture.Build64Bit); }),
                new BuildSettingCreator("Linux 32-bit", () => { CreateDesktopPlatformSettings<LinuxBuildSetting>("Linux 32-bit", IPlatformBuildSetting.Architecture.Build32Bit); }),
                null,
                new BuildSettingCreator("WebGL", () => { CreateWebGLSettings(); }),
                null,
                new BuildSettingCreator("iOS", () => { AddAndModify<IosBuildSetting>("iOS"); }),
                new BuildSettingCreator("Android", () => { AddAndModify<AndroidBuildSetting>("Android"); }),
                new BuildSettingCreator("UWP", () => { AddAndModify<UwpBuildSetting>("UWP"); }),
            };
        }

        private void CreateWebGLSettings()
        {
            SerializedProperty element = Add<WebGlBuildSetting>("WebGL");
            if (element != null)
            {
                element = element.FindPropertyRelative("fileName");
                if (element != null)
                {
                    element = element.FindPropertyRelative("asSlug");
                    if (element != null)
                    {
                        element.boolValue = true;
                    }
                }
            }

            // Apply the property
            ApplyModification();
        }

        private void CreateDesktopPlatformSettings<T>(string name, IPlatformBuildSetting.Architecture architecture) where T : IPlatformBuildSetting
        {
            SerializedProperty element = Add<T>(name);
            element = element.FindPropertyRelative("architecture");
            if (element != null)
            {
                element.enumValueIndex = (int)architecture;
            }

            // Apply the property
            ApplyModification();
        }

        #region Properties
        public UnityEngine.Object Target
        {
            get;
        }

        public SerializedProperty Property
        {
            get;
        }

        public ReorderableList List
        {
            get;
        }

        public GUIContent Label
        {
            get;
        }
        #endregion

        #region Helper Methods
        private void DrawBuildSettingListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, Label);
        }

        private void DrawBuildSettingListElement(Rect rect, int index, bool isActive, bool isFocused)
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

        private SerializedProperty Add<T>(string name) where T : IChildBuildSetting
        {
            SerializedProperty element = CreateNewElement();

            // Setup data field
            T instance = ScriptableObject.CreateInstance<T>();
            instance.Parent = Target as IBuildSetting;
            instance.name = name;

            // Create this asset
            AssetDatabase.AddObjectToAsset(instance, Target);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(instance));

            // Update the properties
            element.objectReferenceValue = instance;
            return element;
        }

        private void AddAndModify<T>(string name) where T : IChildBuildSetting
        {
            Add<T>(name);
            ApplyModification();
        }

        private void Duplicate<T>(string name, T original) where T : Object
        {
            SerializedProperty element = CreateNewElement();

            // Setup data field
            T instance = Object.Instantiate(original);
            instance.name = name;

            // Create this asset
            AssetDatabase.AddObjectToAsset(instance, Target);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(instance));

            // Update the properties
            element.objectReferenceValue = instance;

            // Apply the property
            ApplyModification();
        }

        private void ApplyModification()
        {
            List.serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        private SerializedProperty CreateNewElement()
        {
            int index = List.serializedProperty.arraySize;
            List.serializedProperty.arraySize++;
            List.index = index;
            return List.serializedProperty.GetArrayElementAtIndex(index);
        }

        private void DrawButtons(Rect rect, UnityEngine.Object serializedObject)
        {
            // Unindent
            bool originalEnabled = GUI.enabled;
            rect.x -= EditorUiUtility.IndentSpace;
            rect.width += EditorUiUtility.IndentSpace;

            // Update enabled stuff
            IBuildSetting setting = serializedObject as IBuildSetting;
            GUI.enabled = (setting != null);

            // Draw Edit Button
            rect.width -= (EditorUiUtility.VerticalMargin * 2);
            rect.width /= 3f;
            if (GUI.Button(rect, "Edit") == true)
            {
                // Select object
                Selection.activeObject = serializedObject;
            }

            // Draw Duplicate Button
            rect.x += EditorUiUtility.VerticalMargin;
            rect.x += rect.width;
            if (GUI.Button(rect, "Duplicate") == true)
            {
                // Duplicate object
                Duplicate(serializedObject.name + " (Clone)", serializedObject);
            }

            // Draw build button
            rect.x += EditorUiUtility.VerticalMargin;
            rect.x += rect.width;
            if (GUI.Button(rect, "Build") == true)
            {
                // Make a build
                Debug.Log(setting.Build());
            }
            GUI.enabled = originalEnabled;
        }

        private void OnBuildSettingListReorder(ReorderableList list)
        {
            for (int index = 0; index < list.count; ++index)
            {
                SerializedProperty element = Property.GetArrayElementAtIndex(index);
                AssetDatabase.MoveAsset(element.propertyPath, element.propertyPath);
            }
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Target));
        }

        private void OnBuildSettingListRemove(ReorderableList list)
        {
            // Grab the element
            SerializedProperty element = Property.GetArrayElementAtIndex(list.index);
            if (element.objectReferenceValue != null)
            {
                // Destroy the asset
                UnityEngine.Object.DestroyImmediate(element.objectReferenceValue, true);

                // Remove the element from the list
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }

            // If null, remove the entry
            ReorderableList.defaultBehaviours.DoRemoveButton(list);

            // Reimport
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Target));
        }
        #endregion
    }
}
