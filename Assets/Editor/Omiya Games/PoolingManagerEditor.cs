using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(PoolingManager))]
public class PoolingManagerEditor : Editor
{
	private ReorderableList objectsToPreloadList;
	private SerializedProperty objectsToPreload;

	public void OnEnable()
	{
		objectsToPreload = serializedObject.FindProperty("objectsToPreload");

		objectsToPreloadList = new ReorderableList(serializedObject, objectsToPreload, true, true, true, true);
		objectsToPreloadList.drawHeaderCallback = DrawObjectsToPreloadListHeader;
		objectsToPreloadList.drawElementCallback = DrawObjectsToPreloadListElement;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		objectsToPreloadList.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}

	private void DrawObjectsToPreloadListHeader(Rect rect)
	{
		EditorGUI.LabelField(rect, "Preloaded Objects");
	}

	private void DrawObjectsToPreloadListElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		SerializedProperty element = objectsToPreloadList.serializedProperty.GetArrayElementAtIndex(index);
		rect.y += 2;
		rect.height = EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(rect, element, GUIContent.none);
	}
}
