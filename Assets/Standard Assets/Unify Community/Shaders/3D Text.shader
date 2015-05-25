// -----------------------------------------------------------------------
// Code by Eric Haines from Unify Community:
// http://wiki.unity3d.com/index.php?title=3DText
// 
// Licensed under Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0):
// http://creativecommons.org/licenses/by-sa/3.0/
// -----------------------------------------------------------------------
Shader "Unify Community/3D Text" { 
Properties { 
   _MainTex ("Font Texture", 2D) = "white" {} 
   _Color ("Text Color", Color) = (1,1,1,1) 
} 

SubShader { 
   Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" } 
   Lighting Off Cull Off ZWrite Off Fog { Mode Off } 
   Blend SrcAlpha OneMinusSrcAlpha 
   Pass { 
      Color [_Color] 
      SetTexture [_MainTex] { 
         combine primary, texture * primary 
      } 
   } 
} 
}
