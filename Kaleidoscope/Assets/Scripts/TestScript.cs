using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    Scene offscreenScene;
    Texture2D texture;
    Camera offscreenCamera;

    public string sceneName;
    public Rect window;
    public float interval = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D((int)window.width, (int)window.height, TextureFormat.RGB24, false);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.height / 10);
        var sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        offscreenScene = SceneManager.GetSceneByName(sceneName);
        if (offscreenScene == null)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            offscreenScene = SceneManager.GetSceneByName(sceneName);
        }

        //offscreenScene.isSubScene = true;
        offscreenCamera = null;
        foreach (var obj in offscreenScene.GetRootGameObjects())
        {
            if (offscreenCamera == null)
            {
                offscreenCamera = obj.GetComponent<Camera>();
            }
            obj.transform.position -= new Vector3(0, 0, 1000);
        }
        if (offscreenCamera != null)
        {
            offscreenCamera.targetTexture = new RenderTexture(texture.width, texture.height, 0);
            StartCoroutine(UpdateTexture());
        }
    }

    IEnumerator UpdateTexture()
    {
        var obj = new WaitForSeconds(interval);
        while (true)
        {
            // https://gist.githubusercontent.com/danielbierwirth/10965844fecc38243007f0cd21843d90/raw/06282b4660149fb716a5c08c604ed6fb4750f5d5/OffscreenRendering.cs%2520
            RenderTexture currentRT = RenderTexture.active;
            // Set target texture as active render texture. 			
            RenderTexture.active = offscreenCamera.targetTexture;
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

            yield return obj;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (offscreenCamera != null)
        if (false)
        {
            // https://gist.githubusercontent.com/danielbierwirth/10965844fecc38243007f0cd21843d90/raw/06282b4660149fb716a5c08c604ed6fb4750f5d5/OffscreenRendering.cs%2520
            RenderTexture currentRT = RenderTexture.active;
            // Set target texture as active render texture. 			
            RenderTexture.active = offscreenCamera.targetTexture;
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
        }
    }
}
