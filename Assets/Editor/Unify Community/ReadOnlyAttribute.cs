using UnityEngine;
using UnityEditor;
using System.Collections;

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
public class ReadOnlyAttribute : PropertyAttribute
{
}

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
}
