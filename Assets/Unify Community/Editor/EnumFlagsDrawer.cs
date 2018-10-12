using UnityEngine;
using UnityEditor;

namespace Community.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="EnumFlagsAttribute.cs">
    /// Code by Aqibsadiq from Unity Forums:
    /// https://forum.unity.com/threads/multiple-enum-select-from-inspector.184729/
    /// </copyright>
    /// <author>Aqibsadiq</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Makes an enum multi-selectable in the Unity editor with <code>[EnumFlags]</code>.
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            property.intValue = DisplayEnumFlags(position, property, label);

            EditorGUI.EndProperty();
        }

        public static int DisplayEnumFlags(Rect position, SerializedProperty property, GUIContent label)
        {
            return DisplayEnumFlags(position, property, label, property.enumNames);
        }

        public static int DisplayEnumFlags(Rect position, SerializedProperty property, GUIContent label, string[] enumNames)
        {
            return EditorGUI.MaskField(position, label, property.intValue, enumNames);
        }
    }
}
