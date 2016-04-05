// -----------------------------------------------------------------------
// Code by Jessy from Unity Answers:
// http://answers.unity3d.com/questions/47385/looking-for-a-flat-shaderno-info-with-just-a-color.html
// -----------------------------------------------------------------------
Shader "Unify Community/Unlit Color"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1)
    }
    SubShader
    {
        Color [_Color]
        Pass
        {
        }
    }
}
