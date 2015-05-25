using UnityEngine;

///-----------------------------------------------------------------------
/// <copyright file="HSBColor.cs">
/// Code by Jonathan Czeck from Unify Community:
/// http://wiki.unity3d.com/index.php/HSBColor
/// 
/// Licensed under Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0):
/// http://creativecommons.org/licenses/by-sa/3.0/
/// </copyright>
/// <author>Jonathan Czeck</author>
///-----------------------------------------------------------------------
/// <summary>
/// Displays the frame-rate in the upper-left hand corner of the screen.
/// </summary>
[System.Serializable]
public struct HSBColor
{
	[Range(0f, 1f)]
	[SerializeField]
    float hue;
	[Range(0f, 1f)]
	[SerializeField]
	float saturation;
	[Range(0f, 1f)]
	[SerializeField]
	float brightness;
	[Range(0f, 1f)]
	[SerializeField]
	float alpha;

	#region Properties
	public float Hue
	{
		get
		{
			return hue;
		}
		set
		{
			hue = Mathf.Clamp01(value);
		}
	}

	public float Saturation
	{
		get
		{
			return saturation;
		}
		set
		{
			saturation = Mathf.Clamp01(value);
		}
	}

	public float Brightness
	{
		get
		{
			return brightness;
		}
		set
		{
			brightness = Mathf.Clamp01(value);
		}
	}

	public float Alpha
	{
		get
		{
			return alpha;
		}
		set
		{
			alpha = Mathf.Clamp01(value);
		}
	}
    #endregion

    public HSBColor(float h, float s, float b, float a)
    {
        this.hue = h;
        this.saturation = s;
        this.brightness = b;
        this.alpha = a;
    }
    
    public HSBColor(float h, float s, float b)
    {
        this.hue = h;
        this.saturation = s;
        this.brightness = b;
        this.alpha = 1f;
    }
    
    public HSBColor(Color col)
    {
        HSBColor temp = FromColor(col);
        hue = temp.hue;
        saturation = temp.saturation;
        brightness = temp.brightness;
        alpha = temp.alpha;
    }
    
    public static HSBColor FromColor(Color color)
    {
        HSBColor ret = new HSBColor(0f, 0f, 0f, color.a);
        
        float r = color.r;
        float g = color.g;
        float b = color.b;
        
        float max = Mathf.Max(r, Mathf.Max(g, b));
        
        if (max <= 0)
        {
            return ret;
        }
        
        float min = Mathf.Min(r, Mathf.Min(g, b));
        float dif = max - min;
        
        if (max > min)
        {
            if (g == max)
            {
                ret.hue = (b - r) / dif * 60f + 120f;
            }
            else if (b == max)
            {
                ret.hue = (r - g) / dif * 60f + 240f;
            }
            else if (b > g)
            {
                ret.hue = (g - b) / dif * 60f + 360f;
            }
            else
            {
                ret.hue = (g - b) / dif * 60f;
            }
            if (ret.hue < 0)
            {
                ret.hue = ret.hue + 360f;
            }
        }
        else
        {
            ret.hue = 0;
        }
        
        ret.hue *= 1f / 360f;
        ret.saturation = (dif / max) * 1f;
        ret.brightness = max;
        
        return ret;
    }
    
    public static Color ToColor(HSBColor hsbColor)
    {
        float red = hsbColor.brightness;
        float green = hsbColor.brightness;
        float blue = hsbColor.brightness;
        if (hsbColor.saturation != 0)
        {
            float max = hsbColor.brightness;
            float dif = hsbColor.brightness * hsbColor.saturation;
            float min = hsbColor.brightness - dif;
            
            float h = hsbColor.hue * 360f;
            
            if (h < 60f)
            {
                red = max;
                green = h * dif / 60f + min;
                blue = min;
            }
            else if (h < 120f)
            {
                red = -(h - 120f) * dif / 60f + min;
                green = max;
                blue = min;
            }
            else if (h < 180f)
            {
                red = min;
                green = max;
                blue = (h - 120f) * dif / 60f + min;
            }
            else if (h < 240f)
            {
                red = min;
                green = -(h - 240f) * dif / 60f + min;
                blue = max;
            }
            else if (h < 300f)
            {
                red = (h - 240f) * dif / 60f + min;
                green = min;
                blue = max;
            }
            else if (h <= 360f)
            {
                red = max;
                green = min;
                blue = -(h - 360f) * dif / 60 + min;
            }
            else
            {
                red = 0;
                green = 0;
                blue = 0;
            }
        }
        
        return new Color(Mathf.Clamp01(red), Mathf.Clamp01(green), Mathf.Clamp01(blue), hsbColor.alpha);
    }
    
    public Color ToColor()
    {
        return ToColor(this);
    }
    
    public override string ToString()
    {
        return "H:" + hue + " S:" + saturation + " B:" + brightness;
    }
    
    public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
    {
        float h,s;
        
        //check special case black (color.b==0): interpolate neither hue nor saturation!
        //check special case grey (color.s==0): don't interpolate hue!
        if(a.brightness==0){
            h=b.hue;
            s=b.saturation;
        }else if(b.brightness==0){
            h=a.hue;
            s=a.saturation;
        }else{
            if(a.saturation==0){
                h=b.hue;
            }else if(b.saturation==0){
                h=a.hue;
            }else{
                // works around bug with LerpAngle
                float angle = Mathf.LerpAngle(a.hue * 360f, b.hue * 360f, t);
                while (angle < 0f)
                    angle += 360f;
                while (angle > 360f)
                    angle -= 360f;
                h=angle/360f;
            }
            s=Mathf.Lerp(a.saturation,b.saturation,t);
        }
        return new HSBColor(h, s, Mathf.Lerp(a.brightness, b.brightness, t), Mathf.Lerp(a.alpha, b.alpha, t));
    }
}
