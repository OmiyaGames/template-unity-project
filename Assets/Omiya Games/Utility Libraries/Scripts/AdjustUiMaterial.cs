using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Community
{
    ///-----------------------------------------------------------------------
    /// <copyright file="AdjustUiMaterial.cs">
    /// Code by Julien-Lynge  from Unity Answers:
    /// https://answers.unity.com/questions/878667/world-space-canvas-on-top-of-everything.html
    /// </copyright>
    /// <author>Julien-Lynge </author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Changes a shared material's Z-testing property.
    /// </summary>
    public class AdjustUiMaterial : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;

        Material existingGlobalMat = null;
        Material updatedMaterial = null;

        private void Start()
        {
            if (updatedMaterial == null)
            {
                Image image = GetComponent<Image>();
                existingGlobalMat = image.materialForRendering;
                updatedMaterial = new Material(existingGlobalMat);
                updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
                image.material = updatedMaterial;
            }
        }

        private void OnDestroy()
        {
            if (updatedMaterial != null)
            {
                Image image = GetComponent<Image>();
                Destroy(updatedMaterial);
                image.material = existingGlobalMat;

                existingGlobalMat = null;
                updatedMaterial = null;
            }
        }
    }
}
