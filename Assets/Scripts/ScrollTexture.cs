using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class ScrollTexture : MonoBehaviour
{
	public float scrollSpeed = -0.75f;
	void FixedUpdate()
	{
		Vector2 offset = GetComponent<Renderer>().material.mainTextureOffset;
		offset.x += scrollSpeed * Time.deltaTime;
		while(offset.x > 1)
		{
			offset.x -= 1;
		}
		while(offset.x < -1)
		{
			offset.x += 1;
		}
		GetComponent<Renderer>().material.mainTextureOffset = offset;
	}
}
