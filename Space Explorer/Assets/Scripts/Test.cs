using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Test : MonoBehaviour
{
    Texture2D texture;
    Rect window;
    int cellSize = 64;
    public Camera offscreenCamera;
    public Material lineMaterial;
    public int radius = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        window = new Rect(0, 0, 512, 512);

        var cols = (int)window.width / cellSize;
        var rows = (int)window.height / cellSize;
        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                CreateCell(x, y, cols, rows);
            }
        }

        ScaleOrthographic();

        StartCoroutine(SavePng());
    }

    IEnumerator SavePng()
    {
        yield return new WaitForEndOfFrame();

        var size = (int)window.width;

        var rt = new RenderTexture(512, 512, 0);
        offscreenCamera.targetTexture = rt;

        offscreenCamera.Render();
        // Read screen contents into the texture
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        tex.ReadPixels(window, 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        RenderTexture.active = null;
        Destroy(rt);
        Destroy(tex);


        offscreenCamera.targetTexture = null;

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
    }

    void CreateCell(int x, int y, int cols, int rows)
    {
        var obj = new GameObject($"({x}, {y})");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(cols / 2 - x - 0.5f, rows / 2 - y - 0.5f);
        var lr = obj.AddComponent<LineRenderer>();

        DrawCircle(lr, 0.5f, 0.02f);
    }

    void DrawCircle(LineRenderer lineRenderer, float r, float width)
    {
        float theta_scale = 0.1f;
        
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.useWorldSpace = false;
        var size = Mathf.CeilToInt((2 * Mathf.PI) / theta_scale) + 2;
        lineRenderer.positionCount = size;

        var theta = 0f;
        for (int i = 0; i < size; i++)
        {
            float x = r * Mathf.Cos(theta);
            float y = r * Mathf.Sin(theta);

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));

            theta += theta_scale;
        }
    }

    void Render()
    {
        var rt = new RenderTexture((int)window.width, (int)window.height, 0);
        //offscreenCamera.targetTexture = rt;

        texture = new Texture2D((int)window.width, (int)window.height, TextureFormat.RGB24, false);

        //RenderTexture currentRT = RenderTexture.active;
        //// Set target texture as active render texture. 			
        //RenderTexture.active = offscreenCamera.targetTexture;

        // Render to texture
        offscreenCamera.Render();
        // Read offscreen texture 			
        //Texture2D offscreenTexture = new Texture2D(100, 100, TextureFormat.RGB24, false);
        //offscreenTexture.ReadPixels(new Rect(0, 0, 100, 100), 0, 0, false);
        //offscreenTexture.Apply();

        //RenderTexture.active = rt;
        
        texture.ReadPixels(window, 0, 0, false);
        texture.Apply();
        // Reset previous render texture. 			
        RenderTexture.active = null;
        Destroy(rt);
        // Delete textures. 			
        //UnityEngine.Object.Destroy(offscreenTexture);

        offscreenCamera.targetTexture = null;

        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 64);
        var sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
    }

    IEnumerator UpdateTexture()
    {
        var obj = new WaitForEndOfFrame();
        yield return obj;
        //while (true)
        {
            // https://gist.githubusercontent.com/danielbierwirth/10965844fecc38243007f0cd21843d90/raw/06282b4660149fb716a5c08c604ed6fb4750f5d5/OffscreenRendering.cs%2520
            RenderTexture currentRT = RenderTexture.active;
            // Set target texture as active render texture. 			
            RenderTexture.active = offscreenCamera.targetTexture;

            //DrawCircle();

            // Render to texture
            offscreenCamera.Render();
            // Read offscreen texture 			
            //Texture2D offscreenTexture = new Texture2D(100, 100, TextureFormat.RGB24, false);
            //offscreenTexture.ReadPixels(new Rect(0, 0, 100, 100), 0, 0, false);
            //offscreenTexture.Apply();

            texture.ReadPixels(window, 0, 0, false);
            texture.Apply();
            // Reset previous render texture. 			
            RenderTexture.active = currentRT;

            // Delete textures. 			
            //UnityEngine.Object.Destroy(offscreenTexture);

            offscreenCamera.targetTexture = null;

            yield return obj;
        }
    }



    void DrawCircle()
    {
        float theta_scale = 0.1f;  // Circle resolution
        //var l = Mathf.PI * radius;
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        //lineRenderer.startColor = Color.white;
        //lineRenderer.endColor = Color.white;
        //lineRenderer.startWidth = 0.05f;
        //lineRenderer.endWidth = 0.05f;
        var size = Mathf.CeilToInt((2 * Mathf.PI) / theta_scale) + 2;
        lineRenderer.positionCount = size;

        var theta = 0f;
        for (int i = 0; i < size; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);

            Vector3 pos = new Vector3(x, y, 0);
            lineRenderer.SetPosition(i, pos);

            theta += theta_scale;
        }
    }

    void Scale()
    {
        var camera = offscreenCamera;
        Vector3[] frustumCorners = new Vector3[4];
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        var worldSpaceCorners = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            worldSpaceCorners[i] = camera.transform.TransformVector(frustumCorners[i]);
            worldSpaceCorners[i].z = -camera.transform.position.z;
            Debug.DrawRay(camera.transform.position, worldSpaceCorners[i], Color.blue);
        }

        var min_x = worldSpaceCorners[0].x;
        var max_x = worldSpaceCorners[0].x;

        var min_y = worldSpaceCorners[0].y;
        var max_y = worldSpaceCorners[0].y;

        for (var i = 1; i < 4; i++)
        {
            min_x = Mathf.Min(min_x, worldSpaceCorners[i].x);
            max_x = Mathf.Max(max_x, worldSpaceCorners[i].x);
            min_y = Mathf.Min(min_y, worldSpaceCorners[i].y);
            max_y = Mathf.Max(max_y, worldSpaceCorners[i].y);
        }

        void DrawRay(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawRay(start, end - start, color);
        }

        var z = 0;
        DrawRay(new Vector3(min_x, min_y, z), new Vector3(max_x, min_y, z), Color.red);
        DrawRay(new Vector3(min_x, max_y, z), new Vector3(max_x, max_y, z), Color.yellow);
        DrawRay(new Vector3(min_x, min_y, z), new Vector3(min_x, max_y, z), Color.green);
        DrawRay(new Vector3(max_x, min_y, z), new Vector3(max_x, max_y, z), Color.cyan);


        var w = max_x - min_x;
        var h = max_y - min_y;
        var cols = (int)window.width / cellSize;
        var rows = (int)window.height / cellSize;
        var size = Mathf.Min(w, h);

        var k = size / (cols);
        transform.localScale = new Vector3(k, k, 1);
    }

    void ScaleOrthographic()
    {
        var camera = offscreenCamera;
        float verticalHeightSeen = camera.orthographicSize * 2.0f;
        var fw = verticalHeightSeen * camera.aspect;
        var fh = verticalHeightSeen;

        var max_x = fw / 2.0f;
        var min_x = -max_x;

        var max_y = fh / 2.0f;
        var min_y = -max_y;

        void DrawRay(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawRay(start, end - start, color);
        }

        var z = 0;
        DrawRay(new Vector3(min_x, min_y, z), new Vector3(max_x, min_y, z), Color.red);
        DrawRay(new Vector3(min_x, max_y, z), new Vector3(max_x, max_y, z), Color.yellow);
        DrawRay(new Vector3(min_x, min_y, z), new Vector3(min_x, max_y, z), Color.green);
        DrawRay(new Vector3(max_x, min_y, z), new Vector3(max_x, max_y, z), Color.cyan);


        var w = max_x - min_x;
        var h = max_y - min_y;
        var cols = (int)window.width / cellSize;
        var rows = (int)window.height / cellSize;
        var size = Mathf.Min(w, h);

        var k = size / (cols);
        transform.localScale = new Vector3(k, k, 1);
    }

    // Update is called once per frame
    void Update()
    {
        ScaleOrthographic();
    }
}
