using UnityEngine;
using UnityEditor;

namespace Community.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="ReadOnlyAttribute.cs">
    /// Code by andyman from Unity Answers:
    /// http://answers.unity3d.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
    /// </copyright>
    /// <author>It3ration</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Makes a field read-only in the Unity editor with <code>[ReadOnly]</code>.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
