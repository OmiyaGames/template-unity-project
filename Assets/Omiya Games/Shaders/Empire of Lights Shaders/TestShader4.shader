// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Omiya Games/Test Shader 4"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Light (RGB)", 2D) = "white" {}
		[NoScaleOffset] _ShadeTex("Shade (RGB)", 2D) = "black" {}
		_LightCutoff("Light Cutoff", Range(0.0, 1.0)) = 0.7
		_ShadeCutoff("Shade Cutoff", Range(0.0, 1.0)) = 0.6
		_Ambience("Ambience Strength", Range(0.0, 1.0)) = 0.1
	}

		SubShader
	{
		Pass
		{
			// indicate that our pass is the "base" pass in forward
			// rendering pipeline. It gets ambient and main directional
			// light data set up; light direction in _WorldSpaceLightPos0
			// and color in _LightColor0
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc" // for UnityObjectToWorldNormal
#include "Lighting.cginc" // For shadows
			
			// compile shader into multiple variants, with and without shadows
			// (we don't care about any lightmaps yet, so skip these variants)
#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			// shadow helper functions and macros
#include "AutoLight.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				SHADOW_COORDS(1) // put shadows data into TEXCOORD1
				fixed3 diff : COLOR0;
				fixed3 ambient : COLOR1;
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0.rgb;
				o.ambient = ShadeSH9(half4(worldNormal, 1));
				// compute shadows data
				TRANSFER_SHADOW(o)
				return o;
			}

			float getGrayscale(fixed3 diff, half lightCutoff, half shadeCutoff)
			{
				float returnScale = (diff.r * 0.3) + (diff.g * 0.59) + (diff.b * 0.11);
				returnScale /= (lightCutoff - shadeCutoff);
				returnScale -= shadeCutoff;
				if (returnScale > 1)
				{
					returnScale = 1;
				}
				else if (returnScale < 0)
				{
					returnScale = 0;
				}
				return returnScale;
			}

			sampler2D _MainTex;
			sampler2D _ShadeTex;
			half _LightCutoff;
			half _ShadeCutoff;
			half _Ambience;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				// sample textures
				fixed4 highlight = tex2D(_MainTex, i.uv);
				fixed4 shade = tex2D(_ShadeTex, i.uv);

				// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
				fixed shadow = SHADOW_ATTENUATION(i);
				// darken light's illumination with shadow, keep ambient intact
				fixed3 lighting = (i.diff * shadow) + (i.ambient * _Ambience);

				// Compute grayscale
				float grayscale = getGrayscale(lighting, _LightCutoff, _ShadeCutoff);

				// Blend the texture colors
				return (highlight * grayscale) + (shade * (1 - grayscale));
			}
		ENDCG
		}

			// shadow caster rendering pass, implemented manually
			// using macros from UnityCG.cginc
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
					return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
Fallback "Diffuse"
}
