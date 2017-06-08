// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Omiya Games/Test Shader 5"
{
	Properties
	{
		//_Color("Blend Color", Color) = (1,1,1,1)

		_LightTex("Light", 2D) = "white" {}
		_ShadeTex("Shade", 2D) = "white" {}

		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Ramp("Toon Ramp", 2D) = "gray" {}
	}

	SubShader
	{
CGPROGRAM
//#include "UnityPBSLighting.cginc"
#pragma surface surf ToonRamp
		sampler2D _Ramp;
		sampler2D _BumpMap;

		struct Input
		{
			float2 uv_LightTex;
			float2 uv_BumpMap;
		};

		// custom lighting function that uses a texture ramp based
		// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
		inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
		{
#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
#endif

			half d = dot(s.Normal, lightDir)*0.5 + 0.5;
			half3 ramp = tex2D(_Ramp, float2(d, d)).rgb;

			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = 0;
			return c;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			// Calculate the normal
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			// Calculate the albedo
			o.Albedo = float3(1, 1, 1);
			o.Alpha = 1;
		}
ENDCG

		//save the white lighting out in a grab texture
		GrabPass{ "_LightingGrab" }

CGPROGRAM
#include "UnityPBSLighting.cginc"
#pragma surface surf NoLighting vertex:vert
		sampler2D _LightingGrab;

		sampler2D _LightTex;
		sampler2D _ShadeTex;

		struct Input
		{
			float2 uv_LightTex : TEXCOORD0;
			float2 uv_ShadeTex : TEXCOORD1;
			float4 grabUV;
		};

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		void vert(inout appdata_full v, out Input o)
		{
			float4 projVert = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
#else
			float scale = 1.0;
#endif
			o.uv_LightTex = v.texcoord;
			o.uv_ShadeTex = v.texcoord;

			o.grabUV.xy = (float2(projVert.x, projVert.y*scale) + projVert.w) * 0.5;
			//o.grabUV.xy = float2(projVert.x, projVert.y * scale);
			o.grabUV.zw = projVert.zw;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 lightColor = tex2Dproj(_LightingGrab, UNITY_PROJ_COORD(IN.grabUV));
			//half4 lightColor = half4(1, 1, 1, 1);
			half4 mainColor = tex2D(_LightTex, IN.uv_LightTex);
			half4 shadeColor = tex2D(_ShadeTex, IN.uv_ShadeTex);
			o.Albedo = lerp(shadeColor, mainColor, lightColor);
		}
ENDCG
	}
	Fallback "Diffuse"
}