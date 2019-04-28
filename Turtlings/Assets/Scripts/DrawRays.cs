using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRays : MonoBehaviour {

    public GameObject digParticles;
    public bool DrawRay(Vector3 testRay)

    {
        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(testRay)), out hit))
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
            return true;
        }
        else
        {
            return false;
        }
    }

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
            //dig a square
            Destroy(Instantiate(digParticles, location, Quaternion.identity), 1); //particles
            DigSquare(tex, (int)pixelUV.x, (int)pixelUV.y, radius, Color.clear);
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
                if (y>d/2)
                {
                    //do nothing
                }
                else
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

    public bool Building(Vector3 location, int radius, string direction, int level)
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
            Square(tex, (int)pixelUV.x, (int)pixelUV.y, radius, Color.grey, direction, level);
            tex.Apply();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Square(Texture2D tex, int cx, int cy, int size, Color col, string direction, int level)
    {
        int x, y;
        
        //for (x = (cx-size*2); x <= (cx + size*2); x++)
        //{
        //    for (y = cy; y <= cy + (size / 2); y++)
        //    {
        //        tex.SetPixel(x, y, col);
        //    }
        //}
        if(level == 4)
        {
            if (direction == "right")
            {
                for (x = (cx + size); x <= (cx + size * 9); x++)
                {
                    for (y = cy; y <= cy + (size / 2); y++)
                    {
                        tex.SetPixel(x, y, col);
                    }
                }
            }
            else
            {
                //for (x = (cx - size * 7); x <= (cx + size); x++)
                for (x = (cx - size * 9); x <= (cx - size); x++)
                {
                    for (y = cy; y <= cy + (size / 2); y++)
                    {
                        tex.SetPixel(x, y, col);
                    }
                }
            }
        }
        else
        {
            if (direction == "right")
            {
                for (x = (cx - size); x <= (cx + size * 4); x++)
                {
                    for (y = cy; y <= cy + (size / 2); y++)
                    {
                        tex.SetPixel(x, y, col);
                    }
                }
            }
            else
            {
                for (x = (cx - size * 4); x <= (cx + size); x++)
                {
                    for (y = cy; y <= cy + (size / 2); y++)
                    {
                        tex.SetPixel(x, y, col);
                    }
                }
            }
        }
        
        
    }

    public void DigSquare(Texture2D tex, int cx, int cy, int size, Color col)
    {
        int x, y;

        //for (x = cx; x <= cx+(size*4); x++)
        //{
        //    for (y = cy; y <= cy+(size/2); y++)
        //    {
        //        tex.SetPixel(x, y, col);
        //    }
        //}
        for (x = (cx - size * 2); x <= (cx + size * 2); x++)
        {
            for (y = cy+20; y >= cy-(size-2); y--)
            {
                tex.SetPixel(x, y, col);
            }
        }
    }
}
