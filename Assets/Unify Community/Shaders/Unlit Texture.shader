// -----------------------------------------------------------------------
// Code by Jessy from Unify Community:
// http://wiki.unity3d.com/index.php/Texture_Only
// 
// Licensed under Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0):
// http://creativecommons.org/licenses/by-sa/3.0/
// -----------------------------------------------------------------------
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
