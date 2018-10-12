// -----------------------------------------------------------------------
// Sadly, can't figure out where this shader is from
// -----------------------------------------------------------------------
Shader "Unify Community/Texture With Transparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = ""
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {    // iPhone 3GS and later
            GLSLPROGRAM
            varying mediump vec2 uv;
            
            #ifdef VERTEX
            void main()
            {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                uv = gl_MultiTexCoord0.xy;
            }
            #endif
            
            #ifdef FRAGMENT
            uniform lowp sampler2D _MainTex;
            void main()
            {
                gl_FragColor = texture2D(_MainTex, uv);
            }
            #endif
            ENDGLSL
        }
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {    // pre-3GS devices, including the September 2009 8GB iPod touch
            SetTexture[_MainTex]
        }
    }
}
