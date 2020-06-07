using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SpriteKaleidoscope : MonoBehaviour
{
    public Texture2D texture;
    public int Size = 4;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        var sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.FullRect);
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("update");
    }
}
