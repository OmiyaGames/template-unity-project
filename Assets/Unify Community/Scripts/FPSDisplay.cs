using UnityEngine;
using System.Collections;

///-----------------------------------------------------------------------
/// <copyright file="FPSDisplay.cs">
/// Code by Dave Hampson from Unify Community:
/// http://wiki.unity3d.com/index.php/FramesPerSecond
/// 
/// Licensed under Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0):
/// http://creativecommons.org/licenses/by-sa/3.0/
/// </copyright>
/// <author>Dave Hampson</author>
///-----------------------------------------------------------------------
/// <summary>
/// Displays the frame-rate in the upper-left hand corner of the screen.
/// </summary>
public class FPSDisplay : MonoBehaviour
{
    [SerializeField]
    Color textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

    float deltaTime = 0.0f;
    
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }
    
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        
        GUIStyle style = new GUIStyle();
        
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = textColor;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
