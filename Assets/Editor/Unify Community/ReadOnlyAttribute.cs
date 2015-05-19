using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Copied from It3ration at:
/// http://answers.unity3d.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute
{

}

/// <summary>
/// Copied from It3ration at:
/// http://answers.unity3d.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
