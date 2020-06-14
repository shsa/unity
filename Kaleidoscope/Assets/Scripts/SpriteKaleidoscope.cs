using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SpriteKaleidoscope : MonoBehaviour
{
    Texture2D _texture;
    public Texture2D texture;
    public int Size = 4;
    int _pixelsPerUnit = 100;
    public int pixelsPerUnit = 100;

    Sprite _sprite;
    /// <summary>
    ///     a
    ///   /   \
    ///  c ___ b
    /// </summary>
    Vector2 a;
    Vector2 b;
    Vector2 c;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        Build();
    }

    void Build()
    {
        _texture = texture;
        _pixelsPerUnit = pixelsPerUnit;

        var t = new Texture2D(texture.width, texture.height);
        Graphics.ConvertTexture(texture, t);
        var center = new Vector2(texture.width * 0.5f, texture.height * 0.5f);
        a = new Vector2(0.0f, center.y);
        b = Quaternion.Euler(0.0f, 0.0f, 120.0f) * a;
        c = Quaternion.Euler(0.0f, 0.0f, 120.0f) * b;
        a += center;
        b += center;
        c += center;
        var l = (b - a).magnitude;

        _sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        _sprite.OverrideGeometry(new Vector2[]
        {
                a, b, c
        }, new ushort[] {
                0, 1, 2
        });

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        center = new Vector2(_sprite.rect.width * 0.5f / pixelsPerUnit, _sprite.rect.height * 0.5f / pixelsPerUnit);
        a = new Vector2(0.0f, center.y);
        b = Quaternion.Euler(0.0f, 0.0f, 120.0f) * a;
        c = Quaternion.Euler(0.0f, 0.0f, 120.0f) * b;

        var obj = new GameObject("t");
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = _sprite;
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        var obj1 = Object.Instantiate(obj, transform);
        obj1.transform.localPosition = Quaternion.Euler(0, 0, -60) * a;
        var r = new Vector3(0, -180, -60);
        
        obj1.transform.localRotation = Quaternion.Euler(r);
        obj1.transform.localScale = Vector3.one;
        obj1.name = "t";

        obj1 = Object.Instantiate(obj, transform);
        obj1.transform.localPosition = Quaternion.Euler(0, 0, -60) * a;
        r += new Vector3(0, -180, -60);
        obj1.transform.localRotation = Quaternion.Euler(r);
        obj1.transform.localScale = Vector3.one;
        obj1.name = "t";
    }

    void Update()
    {
        var changed = texture != null && _texture != texture;
        changed &= _pixelsPerUnit == pixelsPerUnit;
        changed = true;
        if (changed)
        {
        }
    }
}
