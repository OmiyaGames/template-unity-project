using UnityEngine;
using UnityEngine.UI;
using OmiyaGames.Translations;

namespace Project
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MenuTest.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2019 Omiya Games
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
    /// <date>6/1/2019</date>
    ///-----------------------------------------------------------------------
    /// <summary>A simple test script: swaps the materials on various models</summary>
    [ExecuteInEditMode]
    public class ShaderTest : MonoBehaviour
    {
        /// <summary>
        /// A quick set of materials
        /// </summary>
        [System.Serializable]
        public struct SwapMaterials
        {
            [SerializeField]
            private TranslatedString shaderName;
            [SerializeField]
            private Material ethanMaterial;
            [SerializeField]
            private Material objectMaterial;
            [SerializeField]
            private Material floorMaterial;

            public TranslatedString ShaderName
            {
                get => shaderName;
            }

            public Material EthanMaterial
            {
                get => ethanMaterial;
            }

            public Material ObjectMaterial
            {
                get => objectMaterial;
            }

            public Material FloorMaterial
            {
                get => floorMaterial;
            }
        }

        [Header("Materials")]
        [SerializeField]
        private SwapMaterials[] allMaterials;

        [Header("Renderers")]
        [SerializeField]
        private Renderer floorRenderer;
        [SerializeField]
        private Renderer[] objectRenderers;
        [SerializeField]
        private Renderer[] ethanRenderers;

        [Header("UI")]
        [SerializeField]
        private Toggle shaderCheckbox;

#if UNITY_EDITOR
        [Header("Test Material")]
        [SerializeField]
        private int testMaterial = 0;
        private int lastMaterial = 0;

        private void Update()
        {
            // First, clamp the test material
            testMaterial = Mathf.Clamp(testMaterial, 0, (allMaterials.Length - 1));

            // Check if this is a different index than the last frame
            if (testMaterial != lastMaterial)
            {
                // Switch to the new material
                OnChangeMaterialToggled(testMaterial);

                // Update the latest index
                lastMaterial = testMaterial;
            }
        }
#endif

        private void Start()
        {
            // DON'T run the start function if we're not playing
            if (Application.isPlaying == false)
            {
                return;
            }

            Transform parent = shaderCheckbox.transform.parent;
            Toggle currentCheckbox = shaderCheckbox;
            for (int index = 0; index < allMaterials.Length; ++index)
            {
                // Setup the checkbox's label
                TranslatedTextMeshPro label = currentCheckbox.GetComponentInChildren<TranslatedTextMeshPro>();
                label.SetTranslationKey(allMaterials[index].ShaderName);

                // Setup the checkbox's name
                currentCheckbox.name = allMaterials[index].ShaderName.ToString();
                int materialIndex = index;
                currentCheckbox.onValueChanged.AddListener((bool toggle) =>
                {
                    if (toggle == true)
                    {
                        OnChangeMaterialToggled(materialIndex);
                    }
                });

                // Check if this is NOT the last element
                if (index < (allMaterials.Length - 1))
                {
                    // Clone the element
                    GameObject clone = Instantiate(shaderCheckbox.gameObject, parent);
                    clone.transform.localRotation = Quaternion.identity;
                    clone.transform.localScale = Vector3.one;

                    // Grab the checkbox
                    currentCheckbox = clone.GetComponent<Toggle>();
                }
            }

            // Turn on the first checkbox
            shaderCheckbox.isOn = true;
        }

        public void OnChangeMaterialToggled(int index)
        {
            if (floorRenderer != null)
            {
                floorRenderer.sharedMaterial = allMaterials[index].FloorMaterial;
            }
            foreach (Renderer renderer in objectRenderers)
            {
                if (renderer != null)
                {
                    renderer.sharedMaterial = allMaterials[index].ObjectMaterial;
                }
            }
            foreach (Renderer renderer in ethanRenderers)
            {
                if (renderer != null)
                {
                    renderer.sharedMaterial = allMaterials[index].EthanMaterial;
                }
            }
        }
    }
}
