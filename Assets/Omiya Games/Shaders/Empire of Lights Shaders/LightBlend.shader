// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Omiya Games/LightBlend"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BlendTex("Texture", 2D) = "white" {}
	}

		SubShader
	{
		CGPROGRAM
#pragma surface surf LightPass     

	struct Input
	{
		float2 uv_MainTex;
	};

	float4 LightingLightPass(SurfaceOutput s, half3 lightDir, half atten) {
		//Do a toon-like lighting pass, make the edge between black and white sort of crisp
		half NdotL = dot(s.Normal, lightDir);
		half NdotLSharp = smoothstep(0, 0.4f, NdotL);

		return float4(1,1,1,1) * NdotLSharp * atten * 2;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = float4(0,0,0,0);
	}
	ENDCG

		//save the white lighting out in a grab texture      
		GrabPass{ "_LightingGrab" }

		CGPROGRAM
#pragma surface surf CustomLambert vertex:vert
#pragma debug      
		sampler2D _LightingGrab;

	sampler2D _MainTex;
	sampler2D _BlendTex;

	struct Input {
		float2 uv_MainTex : TEXCOORD0;
		float2 uv_BlendTex : TEXCOORD1;
		float4 grabUV;
	};

	//I couldn't get this to work with regular lambert lighting, I have no idea why.  The texture flickered like crazy
	float4 LightingCustomLambert(SurfaceOutput s, fixed3 lightDir, fixed atten) {
		half NdotL = dot(s.Normal, lightDir);
		return float4(s.Albedo * _LightColor0.rgb * NdotL * atten * 2, 1);
	}

	void vert(inout appdata_full v, out Input o) {
		float4 projVert = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
#else
		float scale = 1.0;
#endif 
		o.uv_MainTex = v.texcoord;
		o.uv_BlendTex = v.texcoord;
		o.grabUV.xy = (float2(projVert.x, projVert.y*scale) + projVert.w) * 0.5;
		o.grabUV.zw = projVert.zw;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		half4 lightColor = tex2Dproj(_LightingGrab, UNITY_PROJ_COORD(IN.grabUV));
		half4 mainColor = tex2D(_MainTex, IN.uv_MainTex);
		half4 blendColor = tex2D(_BlendTex, IN.uv_BlendTex);
		o.Albedo = lerp(mainColor, blendColor, lightColor);
	}
	ENDCG
	}
		Fallback "Diffuse"
}