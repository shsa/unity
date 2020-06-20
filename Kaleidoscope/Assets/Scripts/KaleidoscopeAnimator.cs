using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KaleidoscopeAnimator : MonoBehaviour
{
    public KaleidoscopeSprite kaleidoscopeSprite = null;
    public string sceneName;
    public Rect window = new Rect(0, 0, 100, 100);
    public float interval = 0.1f;

    Camera offscreenCamera;

    // Start is called before the first frame update
    void Start()
    {
        kaleidoscopeSprite.texture = new Texture2D((int)window.width, (int)window.height, TextureFormat.RGB24, false);

        var scene = SceneManager.GetSceneByName(sceneName);
        if (scene != null)
        {
            if (scene.isLoaded)
            {
                LoadScene_completed(null);
            }
            else
            {
                var oper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                oper.completed += LoadScene_completed;
            }
        }
    }

    private void LoadScene_completed(AsyncOperation obj)
    {
        var offscreenScene = SceneManager.GetSceneByName(sceneName);
        //offscreenScene.isSubScene = true;
        offscreenCamera = null;
        foreach (var rootObject in offscreenScene.GetRootGameObjects())
        {
            if (offscreenCamera == null)
            {
                offscreenCamera = rootObject.GetComponent<Camera>();
            }
            rootObject.transform.position -= new Vector3(0, 0, 1000);
        }
        if (offscreenCamera != null)
        {
            offscreenCamera.targetTexture = new RenderTexture(kaleidoscopeSprite.texture.width, kaleidoscopeSprite.texture.height, 0);
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

            kaleidoscopeSprite.texture.ReadPixels(window, 0, 0, false);
            kaleidoscopeSprite.texture.Apply();
            // Reset previous render texture. 			
            RenderTexture.active = currentRT;

            // Delete textures. 			
            //UnityEngine.Object.Destroy(offscreenTexture);

            yield return obj;
        }
    }
}
