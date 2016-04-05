using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class ScrollTexture : MonoBehaviour
{
    [SerializeField]
    Vector2 scrollSpeed = Vector2.one;

    Renderer cacheRenderer = null;
    Vector2 offset;

    public Renderer CachedRenderer
    {
        get
        {
            if(cacheRenderer == null)
            {
                cacheRenderer = GetComponent<Renderer>();
            }
            return cacheRenderer;
        }
    }

    void Update()
    {
        // Get texture offset
        offset = CachedRenderer.material.mainTextureOffset;

        // Change x
        offset.x += scrollSpeed.x * Time.deltaTime;
        while(offset.x > 1)
        {
            offset.x -= 1;
        }
        while(offset.x < -1)
        {
            offset.x += 1;
        }

        // Change y
        offset.y += scrollSpeed.y * Time.deltaTime;
        while (offset.y > 1)
        {
            offset.y -= 1;
        }
        while (offset.y < -1)
        {
            offset.y += 1;
        }

        // Update texture offset
        CachedRenderer.material.mainTextureOffset = offset;
    }
}
