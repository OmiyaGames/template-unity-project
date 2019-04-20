using UnityEngine;
using UnityEditor;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ScenePathDrawer.cs" company="Omiya Games">
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
    /// <date>11/01/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Editor for <code>ScenePathAttribute</code>.
    /// </summary>
    /// <seealso cref="ScenePathAttribute"/>
    [CustomPropertyDrawer(typeof(ScenePathAttribute))]
    public class ScenePathDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // First get the attribute since it contains the range for the slider
            if (attribute is ScenePathAttribute)
            {
                // Draw the property with a string field and a button.
                if (property.propertyType == SerializedPropertyType.String)
                {
                    // Label
                    EditorGUI.BeginProperty(position, label, property);

                    DrawSceneAssetField(position, property, label);

                    // Show text field
                    EditorGUI.EndProperty();
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Use ScenePath attribute with a string.");
                }
            }
        }

        public static void DrawSceneAssetField(Rect position, SerializedProperty property, GUIContent label = null)
        {
            // Grab the old scene
            SceneAsset oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

            // Prompt for a scene asset
            EditorGUI.BeginChangeCheck();
            SceneAsset newScene = null;
            if (label != null)
            {
                newScene = (SceneAsset)EditorGUI.ObjectField(position, label, oldScene, typeof(SceneAsset), false);
            }
            else
            {
                newScene = (SceneAsset)EditorGUI.ObjectField(position, oldScene, typeof(SceneAsset), false);
            }

            // Check if this field has any changes
            if (EditorGUI.EndChangeCheck())
            {
                // If so, grab the path of the scene
                property.stringValue = AssetDatabase.GetAssetPath(newScene);
            }
        }
    }
}
