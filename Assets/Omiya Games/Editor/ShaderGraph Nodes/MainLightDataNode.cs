using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

///-----------------------------------------------------------------------
/// <copyright file="MainLightDataNode.cs">
/// Source from
/// https://gist.github.com/bitinn/72922a0a69f6d723d94e94f2fc21230e
/// </copyright>
/// <author>David Frank (bitinn)</author>
/// <date>12/28/2018</date>
///-----------------------------------------------------------------------
/// <summary>
/// A custom node for Unity's ShaderGraph to capture lighting and use it
/// into the shader. Works as of Dec 2018, but the APIs might change!
/// </summary>
/// <remarks>
///  IMPORTANT:
///  - tested with LWRP and Shader Graph 4.6.0-preview ONLY
///  - likely to break in SG 5.x and beyond
///  - for HDRP, add your own keyword to detect environment
/// </remarks>
[Title("Custom", "Main Light Data")]
public class MainLightDataNode : CodeFunctionNode
{
    // shader code definition
    // handle shader graph editor environment where main light data isn't defined
    private static string shaderText = @"{
        #ifdef LIGHTWEIGHT_LIGHTING_INCLUDED
            Light mainLight = GetMainLight();
            Color = mainLight.color;
            Direction = mainLight.direction;
            Attenuation = mainLight.distanceAttenuation;
        #else
            Color = float3(1.0, 1.0, 1.0);
            Direction = float3(0.0, 1.0, 0.0);
            Attenuation = 1.0;
        #endif
    }";

    // disable own preview as no light data in shader graph editor
    public override bool hasPreview
    {
        get
        {
            return false;
        }
    }

    // declare node
    public MainLightDataNode()
    {
        name = "Main Light Data";
    }

    // reflection to shader function
    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod(
            "GetMainLightData",
            BindingFlags.Static | BindingFlags.NonPublic
        );
    }

    // shader function and port definition
    private static string GetMainLightData(
        [Slot(0, Binding.None)] out Vector3 Color,
        [Slot(1, Binding.None)] out Vector3 Direction,
        [Slot(2, Binding.None)] out Vector1 Attenuation
    )
    {
        // define vector3
        Direction = Vector3.zero;
        Color = Vector3.zero;

        // actual shader code
        return shaderText;
    }
}
