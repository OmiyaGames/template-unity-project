using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(MainMenuLayoutGroup), true)]
    [CanEditMultipleObjects]
    public class MainMenuLayoutGroupEditor : Editor
    {
        SerializedProperty m_Padding;
        SerializedProperty m_Spacing;
        SerializedProperty m_StartCorner;
        SerializedProperty m_StartAxis;
        SerializedProperty m_ChildAlignment;
        SerializedProperty m_Constraint;
        SerializedProperty m_ConstraintCount;

        protected virtual void OnEnable()
        {
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_StartCorner = serializedObject.FindProperty("m_StartCorner");
            m_StartAxis = serializedObject.FindProperty("m_StartAxis");
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
            m_Constraint = serializedObject.FindProperty("m_menuConstraint");
            m_ConstraintCount = serializedObject.FindProperty("m_ConstraintCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_StartCorner, true);
            EditorGUILayout.PropertyField(m_StartAxis, true);
            EditorGUILayout.PropertyField(m_ChildAlignment, true);
            EditorGUILayout.PropertyField(m_Constraint, true);
            EditorGUILayout.PropertyField(m_ConstraintCount, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
