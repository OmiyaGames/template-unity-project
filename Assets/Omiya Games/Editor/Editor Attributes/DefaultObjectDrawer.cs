using UnityEngine;
using UnityEditor;

namespace OmiyaGames.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="DefaultObjectDrawer.cs" company="Omiya Games">
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
    /// Editor for <code>DefaultObjectAttribute</code>.
    /// </summary>
    /// <seealso cref="DefaultObjectAttribute"/>
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
    [CustomPropertyDrawer(typeof(DefaultObjectAttribute))]
    public class DefaultObjectDrawer : IDefaultDrawer
    {
        private bool isEnabled = false;
        private Object objectValue = null;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // First get the attribute since it contains the range for the slider
            if (attribute is DefaultObjectAttribute)
            {
                DefaultObjectAttribute range = (DefaultObjectAttribute)attribute;

                // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    DisplayCheckboxAndControl(property, range, position, SetToNull, DisplayObjectField, ref isEnabled, ref objectValue);
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Use DefaultObject with objects.");
                }
            }
        }

        static void DisplayObjectField(SerializedProperty property, DefaultObjectAttribute range, Rect position, ref Object value)
        {
            value = EditorGUI.ObjectField(position, value, property.objectReferenceValue.GetType(), true);
        }

        static void SetToNull(SerializedProperty property, DefaultObjectAttribute range)
        {
            property.objectReferenceValue = null;
        }
    }
}
