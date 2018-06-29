using UnityEngine;
using UnityEditor;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="IDefaultDrawer.cs" company="Omiya Games">
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
    /// <date>6/26/2018</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Helper class that unifies common methods in a few drawers.
    /// </summary>
    /// <seealso cref="DefaultRangeDrawer"/>
    /// <seealso cref="DefaultObjectDrawer"/>
    /// <seealso cref="DefaultNumberDrawer"/>
    /// <remarks>
    /// Revision History:
    /// <list type="table">
    /// <listheader>
    /// <description>Date</description>
    /// <description>Name</description>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <description>6/26/2018</description>
    /// <description>Taro</description>
    /// <description>Initial verison</description>
    /// </item>
    /// </list>
    /// </remarks>
    public abstract class IDefaultDrawer : PropertyDrawer
    {
        const int LabelWidth = 4;
        const int GapBetweenCheckboxAndSlider = 4;
        const int CheckboxSize = 16;

        protected delegate void DisplayControl<ATTRIBUTE, VALUE>(SerializedProperty property, ATTRIBUTE range, Rect position, ref VALUE value) where ATTRIBUTE : PropertyAttribute;
        protected delegate void SetToDefault<ATTRIBUTE>(SerializedProperty property, ATTRIBUTE range) where ATTRIBUTE : PropertyAttribute;

        /// <summary>
        /// Taken from https://bitbucket.org/Unity-Technologies/ui/src/2017.3/UnityEditor.UI/UI/LayoutElementEditor.cs
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        protected static void DisplayCheckboxAndControl<ATTRIBUTE, VALUE>(SerializedProperty property, ATTRIBUTE range, Rect position, SetToDefault<ATTRIBUTE> setToDefault, DisplayControl<ATTRIBUTE, VALUE> slider, ref bool show, ref VALUE value) where ATTRIBUTE : PropertyAttribute
        {
            Rect toggleRect, objectFieldRect;
            SetupPositioning(property, position, out toggleRect, out objectFieldRect);

            // Checkbox
            EditorGUI.BeginChangeCheck();
            show = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, show);
            if ((EditorGUI.EndChangeCheck() == true) && (show == false))
            {
                // Set the property to the default value
                setToDefault(property, range);
            }
            else if (!property.hasMultipleDifferentValues && (show == true))
            {
                // Small invisible label area for drag zone functionality
                float originalWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = LabelWidth;

                // Display the slider
                slider(property, range, objectFieldRect, ref value);
                EditorGUIUtility.labelWidth = originalWidth;
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Taken from https://bitbucket.org/Unity-Technologies/ui/src/2017.3/UnityEditor.UI/UI/LayoutElementEditor.cs
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        protected static void SetupPositioning(SerializedProperty property, Rect position, out Rect toggleRect, out Rect fieldRect)
        {
            // Label
            GUIContent label = EditorGUI.BeginProperty(position, null, property);

            // Rects
            Rect fieldPosition = EditorGUI.PrefixLabel(position, label);

            toggleRect = fieldPosition;
            toggleRect.width = CheckboxSize;

            fieldRect = fieldPosition;
            fieldRect.xMin += CheckboxSize + GapBetweenCheckboxAndSlider;
        }
    }
}
