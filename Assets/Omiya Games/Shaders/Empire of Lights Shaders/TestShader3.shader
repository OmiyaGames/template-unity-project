Shader "Omiya Games/Test Shader 3"
{
	Properties
	{
		_Color("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		
		_MainTex("Light (RGB)", 2D) = "white" {}
		_ShadeTex("Shade (RGB)", 2D) = "black" {}
		
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" "PerformanceChecks" = "False" }
		LOD 200

CGPROGRAM
#include "UnityPBSLighting.cginc"
#pragma surface surf NoLighting
		fixed4 LightingNoLighting(SurfaceOutputStandard s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;

			//half d = dot(s.Normal, lightDir) * 0.5 + 0.5;
			half d = dot(s.Normal, lightDir) * 0.5 + 0.5;
			if (d > 0.9)
			{
				c.rgb = s.Albedo;
			}
			else
			{
				c.rgb = s.Emission;
			}
			c.a = s.Alpha;
			return c;
		}

//#pragma surface surf ToonRamp
//		sampler2D _Ramp;
//		inline half4 LightingToonRamp (SurfaceOutputStandard s, half3 lightDir, half atten)
//		{
//			#ifndef USING_DIRECTIONAL_LIGHT
//				lightDir = normalize(lightDir);
//			#endif
//
//			// Calculate the shading based on the ramp
//			half d = dot(s.Normal, lightDir) * 0.5 + 0.5;
//			half3 ramp = tex2D (_Ramp, float2(d, d)).rgb;
//
//			// Calculate the grayscale value that emission will be affecting
//			half4 shading;
//			shading.rgb = ramp * (atten * 2);
//			shading.a = 0;
//
//			float grayscale = (shading.r * 0.3) + (shading.g * 0.59) + (shading.b * 0.11);
//
//			half4 c;
//			if (grayscale > 0.5)
//			{
//				c.rgb = s.Albedo;
//			}
//			else
//			{
//				s.Albedo = s.Emission;
//				c.rgb = s.Emission;
//			}
//			// *dot(s.Emission, float3(grayscale, grayscale, grayscale));
//			c.a = 0;
//			return c;
//		}

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

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Set the Metallic & Glossiness
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			// Calculate the normal
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			// Calculate the albedo
			half4 highlight = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = highlight.rgb;
			o.Alpha = highlight.a;

			// Calculate the shade
			half4 shade = tex2D(_ShadeTex, IN.uv_ShadeTex);
			o.Emission = shade.rgb;

			// Test code for just a solid color
			//o.Albedo = _Color;
			//o.Alpha = _Color;
		}
ENDCG
	}
Fallback "Diffuse"
}
