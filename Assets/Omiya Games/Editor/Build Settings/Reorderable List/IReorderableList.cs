using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames.Builds;
using OmiyaGames.Common.Editor;

namespace OmiyaGames.UI.Builds
{
    ///-----------------------------------------------------------------------
    /// <copyright file="HostArchiveSettingReorderableList.cs" company="Omiya Games">
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
    /// <date>2/12/2019</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper class to generate a reorderable list.
    /// </summary>
    public abstract class IReorderableList
    {
        protected IReorderableList(Object target, SerializedProperty property, GUIContent label)
        {
            // Member Variable
            Target = target;
            Property = property;
            Label = label;

            // Setup List
            List = new ReorderableList(Property.serializedObject, Property, true, true, true, true);
            List.drawHeaderCallback = DrawBuildSettingListHeader;
            List.drawElementCallback = DrawBuildSettingListElement;
            List.onReorderCallback = OnBuildSettingListReorder;
            List.onRemoveCallback = OnBuildSettingListRemove;
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

        protected abstract void DrawBuildSettingListElement(Rect rect, int index, bool isActive, bool isFocused);

        protected void DrawButtons(Rect rect, UnityEngine.Object serializedObject)
        {
            // Unindent
            bool originalEnabled = GUI.enabled;
            rect.x -= EditorHelpers.IndentSpace;
            rect.width += EditorHelpers.IndentSpace;

            // Update enabled stuff
            IBuildSetting setting = serializedObject as IBuildSetting;
            GUI.enabled = (setting != null);

            // Draw Edit Button
            rect.width -= (EditorHelpers.VerticalMargin * 2);
            rect.width /= 3f;
            if (GUI.Button(rect, "Edit") == true)
            {
                // Select object
                Selection.activeObject = serializedObject;
            }

            // Draw Duplicate Button
            rect.x += EditorHelpers.VerticalMargin;
            rect.x += rect.width;
            if (GUI.Button(rect, "Duplicate") == true)
            {
                // Duplicate object
                Duplicate(serializedObject.name + " (Clone)", serializedObject);
            }

            // Draw build button
            rect.x += EditorHelpers.VerticalMargin;
            rect.x += rect.width;
            if (GUI.Button(rect, "Build") == true)
            {
                // Make a build
                Debug.Log(setting.Build());
            }
            GUI.enabled = originalEnabled;
        }

        protected SerializedProperty Add<T>(string name) where T : IChildBuildSetting
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

        protected void AddAndModify<T>(string name) where T : IChildBuildSetting
        {
            Add<T>(name);
            ApplyModification();
        }

        protected void ApplyModification()
        {
            List.serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        #region Helper Methods
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

        private SerializedProperty CreateNewElement()
        {
            int index = List.serializedProperty.arraySize;
            List.serializedProperty.arraySize++;
            List.index = index;
            return List.serializedProperty.GetArrayElementAtIndex(index);
        }

        private void DrawBuildSettingListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, Label);
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
