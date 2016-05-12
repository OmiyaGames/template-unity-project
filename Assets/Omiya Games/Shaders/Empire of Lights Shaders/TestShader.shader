Shader "Omiya Games/Test Shader"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("Light (RGB)", 2D) = "white" {}
		_ShadeTex ("Shade (RGB)", 2D) = "black" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf ToonRamp

			sampler2D _Ramp;

			// custom lighting function that uses a texture ramp based
			// on angle between light direction and normal
			#pragma lighting ToonRamp exclude_path:prepass
			inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
			{
				#ifndef USING_DIRECTIONAL_LIGHT
				lightDir = normalize(lightDir);
				#endif
	
				half d = dot (s.Normal, lightDir)*0.5 + 0.5;
				half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	
				half4 c;
				c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
				c.a = 0;
				return c;
			}


			sampler2D _MainTex;
			//sampler2D _ShadeTex;
			sampler2D _BumpMap;
			float4 _Color;

			struct Input
			{
				float2 uv_MainTex : TEXCOORD0;
				float2 uv_BumpMap;
			};

			void surf (Input IN, inout SurfaceOutput o)
			{
				//texcol.rgb = dot(texcol.rgb, float3(0.3, 0.59, 0.11));

				half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				o.Alpha = c.a;
			}
		ENDCG

	} 

	Fallback "Diffuse"
}
