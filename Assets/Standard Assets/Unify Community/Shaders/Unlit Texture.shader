Shader "Unify Community/Unlit Texture" {
 
Properties {
    _MainTex ("Texture", 2D) = ""
}
 
SubShader {
    Pass {
        // iPhone 3GS and later
        GLSLPROGRAM
        varying mediump vec2 uv;
     
        #ifdef VERTEX
        void main() {
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
        }
        #endif
     
        #ifdef FRAGMENT
        uniform lowp sampler2D _MainTex;
        void main() {
            gl_FragColor = texture2D(_MainTex, uv);
        }
        #endif        
        ENDGLSL
    }
}
 
SubShader {
        Pass {
            // pre-3GS devices, including the September 2009 8GB iPod touch
        SetTexture[_MainTex]
    }
}
 
}