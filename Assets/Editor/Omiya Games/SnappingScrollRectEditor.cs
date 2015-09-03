using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using OmiyaGames;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor for <code>NestedScrollRect</code>.
    /// </summary>
    /// <seealso cref="NestedScrollRect"/>
    [CustomEditor(typeof(SnappingScrollRect), true)]
    [CanEditMultipleObjects]
    public class SnappingScrollRectEditor : ScrollRectEditor
    {
        SerializedProperty snapIfVelocityIsBelow;
        SerializedProperty numberOfSnappingPoints;

        protected override void OnEnable()
        {
            // Grab the contentToSnapTo property
            snapIfVelocityIsBelow = serializedObject.FindProperty("snapIfVelocityIsBelow");
            numberOfSnappingPoints = serializedObject.FindProperty("numberOfSnappingPoints");

            // Grab the rest of the properties
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            // Display the contentToSnapTo property
            serializedObject.Update();
            EditorGUILayout.PropertyField(snapIfVelocityIsBelow, true);
            EditorGUILayout.PropertyField(numberOfSnappingPoints, true);
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

            // Display the rest of the information
            base.OnInspectorGUI();
        }
    }
}
