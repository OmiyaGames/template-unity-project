using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor for <code>NestedScrollRect</code>.
    /// </summary>
    /// <seealso cref="NestedScrollRect"/>
    [CustomEditor(typeof(NestedScrollRect), true)]
    [CanEditMultipleObjects]
    public class NestedScrollRectEditor : ScrollRectEditor
    {
        SerializedProperty parentScrollRectProperty;

        protected override void OnEnable()
        {
            // Grab the scroll rect property
            parentScrollRectProperty = serializedObject.FindProperty("parentScrollRect");

            // Grab the rest of the properties
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            // Check if a property was set
            if (parentScrollRectProperty.objectReferenceInstanceIDValue == 0)
            {
                // Display a warning
                EditorGUILayout.HelpBox("Parent Scroll Rect is not set.\nNested Scroll Rect will behave like a normal Scroll Rect.", MessageType.Warning);
            }

            // Display the parent scroll rect property
            serializedObject.Update();
            EditorGUILayout.PropertyField(parentScrollRectProperty, true);
            serializedObject.ApplyModifiedProperties();

            // Display the rest of the information
            base.OnInspectorGUI();
        }
    }
}
