using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using OmiyaGames;

namespace UnityEditor.UI
{
    ///-----------------------------------------------------------------------
    /// <copyright file="SoundEffectEditor.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
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
    /// <date>5/25/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// An editor to make it easier to edit <code>SoundEffect</code> scripts.
    /// </summary>
    /// <seealso cref="SoundEffect"/>
    [CustomEditor(typeof(SoundEffect), true)]
    [CanEditMultipleObjects]
    public class SoundEffectEditor : Editor
    {
        const float VerticalMargin = 2;
        static readonly GUILayoutOption SliderLabelWidth = GUILayout.MinWidth(90);
        static readonly GUILayoutOption SliderTextFieldWidth = GUILayout.MinWidth(20);

        SerializedProperty mutatePitch;
        SerializedProperty pitchMutationRange;
        
        SerializedProperty mutateVolume;
        SerializedProperty volumeMutationRange;

        SerializedProperty clipVariations;
        ReorderableList clipVariationList;

        protected void OnEnable()
        {
            // Grab every field
            mutatePitch = serializedObject.FindProperty("mutatePitch");
            pitchMutationRange = serializedObject.FindProperty("pitchMutationRange");
            mutateVolume = serializedObject.FindProperty("mutateVolume");
            volumeMutationRange = serializedObject.FindProperty("volumeMutationRange");
            clipVariations = serializedObject.FindProperty("clipVariations");

            // Construct a list for the clip variations
            clipVariationList = new ReorderableList(serializedObject, clipVariations, true, true, true, true);
            clipVariationList.drawHeaderCallback = DrawObjectsToPreloadListHeader;
            clipVariationList.drawElementCallback = DrawObjectsToPreloadListElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Start the Mutate Pitch group
            mutatePitch.boolValue = EditorGUILayout.BeginToggleGroup("Mutate Pitch", mutatePitch.boolValue);
            if(mutatePitch.boolValue == true)
            {
                DisplayRangeSlider("Pitch Range", pitchMutationRange, SoundEffect.MinPitch, SoundEffect.MaxPitch);
            }
            EditorGUILayout.EndToggleGroup();

            // Start the Mutate Volume group
            mutateVolume.boolValue = EditorGUILayout.BeginToggleGroup("Mutate Volume", mutateVolume.boolValue);
            if (mutateVolume.boolValue == true)
            {
                DisplayRangeSlider("Volume Range", volumeMutationRange, SoundEffect.MinVolume, SoundEffect.MaxVolume);
            }
            EditorGUILayout.EndToggleGroup();

            // Start the audio clip variations list
            clipVariationList.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayRangeSlider(string label, SerializedProperty rangeProperty, float min, float max)
        {
            // First, start a horizontal group
            EditorGUILayout.BeginHorizontal();
            Vector2 range = rangeProperty.vector2Value;

            // Draw the label
            EditorGUILayout.LabelField(label, SliderLabelWidth);

            // Draw the min field
            range.x = EditorGUILayout.FloatField(range.x, SliderTextFieldWidth);

            // Display the min-max slider
            EditorGUILayout.MinMaxSlider(ref range.x, ref range.y, min, max/*, SliderSliderWidth*/);

            // Draw the max field
            range.y = EditorGUILayout.FloatField(range.y, SliderTextFieldWidth);

            // End the horizontal group
            rangeProperty.vector2Value = range;
            EditorGUILayout.EndHorizontal();
        }

        void DrawObjectsToPreloadListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Clip Variations");
        }

        void DrawObjectsToPreloadListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = clipVariationList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += VerticalMargin;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }
    }
}