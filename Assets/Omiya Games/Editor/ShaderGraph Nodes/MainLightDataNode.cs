/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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

[Title("Custom", "Main Light")]
public class MainLightNode : CodeFunctionNode
{
    public override bool hasPreview { get { return false; } }

    //None of this is mine. It was created by @CiroContns on twitter. He uses an older version of shadergraph, so all I did was update it. This will eventually become outdatded.
    private static string functionBodyForReals = @"{
            Light mainLight = GetMainLight();
            Color = mainLight.color;
            Direction = mainLight.direction;
            float4 shadowCoord;
            #ifdef LIGHTWEIGHT_SHADOWS_INCLUDED
            #if SHADOWS_SCREEN
                float4 clipPos = TransformWorldToHClip(WorldPos);
                shadowCoord = ComputeScreenPos(clipPos);
            #else
                shadowCoord = TransformWorldToShadowCoord(WorldPos);
            #endif
            #endif
                Attenuation = MainLightRealtimeShadow(shadowCoord);
        }";
    private static string functionBodyPreview = @"{
            Color = 1;
            Direction = float3(-0.5, .5, 0.5);
            Attenuation = 1;
        }";

    private static bool isPreview;

    private static string functionBody
    {
        get
        {
            if (isPreview)
                return functionBodyPreview;
            else
                return functionBodyForReals;
        }
    }


    public MainLightNode()
    {
        name = "Main Light";
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("CustomFunction", BindingFlags.Static | BindingFlags.NonPublic);
    }


    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        isPreview = generationMode == GenerationMode.Preview;

        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }

    private static string CustomFunction(
    [Slot(0, Binding.None)] out Vector3 Direction,
    [Slot(1, Binding.None)] out Vector1 Attenuation,
    [Slot(2, Binding.None)] out Vector3 Color,
    [Slot(3, Binding.WorldSpacePosition)] Vector3 WorldPos)
    {
        Direction = Vector3.zero;
        Color = Vector3.zero;

        return functionBody;
    }
}
*/
