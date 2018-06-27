using UnityEngine;
using UnityEditor;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="DefaultNumberDrawer.cs" company="Omiya Games">
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
    /// Editor for <code>DefaultNumberAttribute</code>.
    /// </summary>
    /// <seealso cref="DefaultNumberAttribute"/>
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
    [CustomPropertyDrawer(typeof(DefaultNumberAttribute))]
    public class DefaultNumberDrawer : IDefaultDrawer
    {
        private bool isEnabled = false;
        private float sliderValue = 0;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // First get the attribute since it contains the range for the slider
            if (attribute is DefaultNumberAttribute)
            {
                DefaultNumberAttribute range = (DefaultNumberAttribute)attribute;

                // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    DisplayCheckboxAndControl(property, range, position, SetToDefaultFloat, DisplayFloatField, ref isEnabled, ref sliderValue);
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    DisplayCheckboxAndControl(property, range, position, SetToDefaultInt, DisplayIntField, ref isEnabled, ref sliderValue);
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Use DefaultNumber with float or int.");
                }
            }
        }

        static void DisplayFloatField(SerializedProperty property, DefaultNumberAttribute range, Rect position, ref float value)
        {
            value = LimitValue(range, EditorGUI.FloatField(position, value));
        }

        static void DisplayIntField(SerializedProperty property, DefaultNumberAttribute range, Rect position, ref float value)
        {
            value = LimitValue(range, EditorGUI.IntField(position, Mathf.RoundToInt(value)));
        }

        static void SetToDefaultFloat(SerializedProperty property, DefaultNumberAttribute range)
        {
            property.floatValue = range.defaultNumber;
        }

        static void SetToDefaultInt(SerializedProperty property, DefaultNumberAttribute range)
        {
            property.floatValue = Mathf.RoundToInt(range.defaultNumber);
        }

        static float LimitValue(DefaultNumberAttribute range, float value)
        {
            if ((range.numberRange == DefaultNumberAttribute.Range.GreaterThanOrEqualTo) && (value < range.startNumber))
            {
                value = range.startNumber;
            }
            else if ((range.numberRange == DefaultNumberAttribute.Range.LessThanOrEqualTo) && (value > range.startNumber))
            {
                value = range.startNumber;
            }

            return value;
        }
    }
}
