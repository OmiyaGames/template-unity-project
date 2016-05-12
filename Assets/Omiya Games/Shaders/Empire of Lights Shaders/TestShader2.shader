Shader "Omiya Games/Test Shader 2"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("Light (RGB)", 2D) = "white" {}
		_ShadeTex ("Shade (RGB)", 2D) = "black" {}
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_BumpMap("Bumpmap", 2D) = "bump" {}
		//_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" "PerformanceChecks" = "False" }
		LOD 200
		
		CGPROGRAM
			#include "UnityPBSLighting.cginc"
			#pragma surface surf Standard
			//#pragma surface surf ToonRamp

			//sampler2D _Ramp;

			//// custom lighting function that uses a texture ramp based
			//// on angle between light direction and normal
			//#pragma lighting ToonRamp exclude_path:prepass
			//inline half4 LightingToonRamp (SurfaceOutputStandard s, half3 lightDir, half atten)
			//{
			//	#ifndef USING_DIRECTIONAL_LIGHT
			//	lightDir = normalize(lightDir);
			//	#endif
	
			//	half d = dot (s.Normal, lightDir)*0.5 + 0.5;
			//	half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	
			//	half4 c;
			//	c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			//	c.a = 0;
			//	return c;
			//}

			sampler2D _MainTex;
			sampler2D _ShadeTex;
			sampler2D _BumpMap;
			float4 _Color;
			half _Metallic;
			half _Glossiness;

			struct Input
			{
				float2 uv_MainTex : TEXCOORD0;
				float2 uv_ShadeTex : TEXCOORD0;
				float2 uv_BumpMap;
			};

			void surf (Input IN, inout SurfaceOutputStandard o)
			{
				// Set the Metallic & Glossiness
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;

				// Calculate the normal
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

				// Calculate the albedo
				half4 highlight = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				half4 shade = tex2D(_ShadeTex, IN.uv_ShadeTex) * _Color;
				//o.Albedo = highlight.rgb;
				//o.Alpha = highlight.a;
				o.Albedo = shade.rgb;
				o.Alpha = shade.a;
			}
		ENDCG

	} 

	Fallback "Diffuse"
}
