using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRaysTwoD : MonoBehaviour
{
    public bool GetSpritePixelColorUnderMousePointer(Vector3 testRay)
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(testRay)), out hit);
        SpriteRenderer spriteRenderer = hit.transform.GetComponent<SpriteRenderer>();
        Debug.Log(spriteRenderer);
        Camera cam = Camera.main;
        //Vector2 mousePos = Input.mousePosition;
        Vector2 viewportPos = cam.ScreenToViewportPoint(testRay);
        //Debug.Log(viewportPos);
        //if (viewportPos.x < 0.0f || viewportPos.x > 1.0f || viewportPos.y < 0.0f || viewportPos.y > 1.0f) return false; // out of viewport bounds
                                                                                                                        // Cast a ray from viewport point into world
        Ray ray = cam.ViewportPointToRay(viewportPos);

        // Check for intersection with sprite and get the color
        Debug.Log(ray);
        return IntersectsSprite(spriteRenderer, ray);
    }
    private bool IntersectsSprite(SpriteRenderer spriteRenderer, Ray ray)
    {
        if (spriteRenderer == null) return false;
        Sprite sprite = spriteRenderer.sprite;
        if (sprite == null) return false;
        Texture2D texture = sprite.texture;
        if (texture == null) return false;
        // Check atlas packing mode
        if (sprite.packed && sprite.packingMode == SpritePackingMode.Tight)
        {
            // Cannot use textureRect on tightly packed sprites
            Debug.LogError("SpritePackingMode.Tight atlas packing is not supported!");
            // TODO: support tightly packed sprites
            return false;
        }
        // Craete a plane so it has the same orientation as the sprite transform
        Plane plane = new Plane(transform.forward, transform.position);
        // Intersect the ray and the plane
        float rayIntersectDist; // the distance from the ray origin to the intersection point
        if (!plane.Raycast(ray, out rayIntersectDist)) return false; // no intersection
                                                                     // Convert world position to sprite position
                                                                     // worldToLocalMatrix.MultiplyPoint3x4 returns a value from based on the texture dimensions (+/- half texDimension / pixelsPerUnit) )
                                                                     // 0, 0 corresponds to the center of the TEXTURE ITSELF, not the center of the trimmed sprite textureRect
        Vector3 spritePos = spriteRenderer.worldToLocalMatrix.MultiplyPoint3x4(ray.origin + (ray.direction * rayIntersectDist));
        Rect textureRect = sprite.textureRect;
        float pixelsPerUnit = sprite.pixelsPerUnit;
        float halfRealTexWidth = texture.width * 0.5f; // use the real texture width here because center is based on this -- probably won't work right for atlases
        float halfRealTexHeight = texture.height * 0.5f;
        // Convert to pixel position, offsetting so 0,0 is in lower left instead of center
        int texPosX = (int)(spritePos.x * pixelsPerUnit + halfRealTexWidth);
        int texPosY = (int)(spritePos.y * pixelsPerUnit + halfRealTexHeight);
        // Check if pixel is within texture
        if (texPosX < 0 || texPosX < textureRect.x || texPosX >= Mathf.FloorToInt(textureRect.xMax)) return false; // out of bounds
        if (texPosY < 0 || texPosY < textureRect.y || texPosY >= Mathf.FloorToInt(textureRect.yMax)) return false; // out of bounds

        Debug.Log("b");
        Debug.Log(texture.GetPixel(texPosX, texPosY));
        if (texture.GetPixel(texPosX, texPosY).a == 1.0)
        {
            return true;
        }
        else
        {
            return false;
        }
            
    }
    //public bool DrawRay(Vector3 testRay)
    //{
    //Ray ray = Camera.main.ScreenPointToRay(testRay);

    //RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);

    //if (hit2D != null && hit2D.collider != null)
    //{
    //    Renderer rend = hit2D.transform.GetComponent<Renderer>();
    //    Texture2D tex = rend.material.mainTexture as Texture2D;
    //    Vector2 pixelUV = hit.textureCoord;

    //    pixelUV.x *= tex.width;
    //    pixelUV.y *= tex.height;

    //    if (tex.GetPixel((int)pixelUV.x, (int)pixelUV.y).a == 1.0)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
    //else
    //{
    //    return false;
    //}

    //RaycastHit hit;
    //if (!Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(testRay)), out hit))
    //{
    //    return false;
    //}

    ////test object is over the background and the ray hits something

    //Renderer rend = hit.transform.GetComponent<Renderer>();
    //MeshCollider meshCollider = hit.collider as MeshCollider;

    //if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
    //{
    //    return false;
    //}

    ////everything is ok, lets check the alpha value of the pixel
    //Texture2D tex = rend.material.mainTexture as Texture2D;
    //Vector2 pixelUV = hit.textureCoord;

    //pixelUV.x *= tex.width;
    //pixelUV.y *= tex.height;

    //if (tex.GetPixel((int)pixelUV.x, (int)pixelUV.y).a == 1.0)
    //{
    //    return true;
    //}
    //else
    //{
    //    return false;
    //}
    //}

    public bool Digging(Vector3 location, int radius)
    {
        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(location)), out hit))
        {
            return false;
        }

        //test object is over the background and the ray hits something

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return false;
        }

        //everything is ok, lets check the alpha value of the pixel
        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;

        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        if (tex.GetPixel((int)pixelUV.x, (int)pixelUV.y).a == 1.0)
        {
            //dig a circle
            Circle(tex, (int)pixelUV.x, (int)pixelUV.y, radius, Color.clear);
            tex.Apply();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Circle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        int x, y, px, py, nx, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);
                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);
            }
        }

    }

}
