using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class KaleidoscopeSprite : MonoBehaviour
{
    static Dictionary<Vector3Int, TriangleComponent> cache = new Dictionary<Vector3Int, TriangleComponent>();

    public int radius = 3;

    /// <summary>
    ///     a
    ///   /   \
    ///  c ___ b
    /// </summary>
    Vector2 a;
    Vector2 b;
    Vector2 c;
    int idx = 0;
    float len;
    float h; // h = len * sqrt(3) / 2
    float r; // r = len * sqrt(3) / 6 => r = h / 3

    Texture2D _texture;
    public Texture2D texture {
        get {
            return _texture;
        }
        set {
            if (_texture != value)
            {
                _texture = value;
                CreateSprite();
            }
        }
    }

    [SerializeField]
    int _pixelsPerUnit = 100;
    public int pixelsPerUnit {
        get {
            return _pixelsPerUnit;
        }
        set {
            _pixelsPerUnit = value;
            CreateSprite();
        }
    }

    [SerializeField]
    Vector3 _rotation = Vector3.zero;
    public Vector3 rotation {
        get {
            return _rotation;
        }
        set {
            if (_rotation != value)
            {
                _rotation = value;
                CreateSprite();
            }
        }
    }

    Sprite sprite;

    void CreateSprite()
    {
        Build();

        var rect = new Rect(0, 0, texture.width, texture.height);
        sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
        UpdateSprite();

        var center = rect.center / pixelsPerUnit;
        a = new Vector2(0.0f, Mathf.Min(center.x, center.y));
        b = Quaternion.Euler(0.0f, 0.0f, 120.0f) * a;
        c = Quaternion.Euler(0.0f, 0.0f, 120.0f) * b;

        // https://youclever.org/book/ravnostoronnij-treugolnik-1
        //len = (a - b).magnitude * 1.5f;
        len = (a - b).magnitude;
        h = len * Mathf.Sqrt(3) * 0.5f; // h = a * sqrt(3) / 2
        r = h / 3; // r = a * sqrt(3) / 6

        foreach (var tri in cache.Values)
        {
            tri.transform.localPosition = CalcCenter(tri.index);
            var sr = tri.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }
    }

    void UpdateSprite()
    {
        var center = new Vector2(sprite.rect.width * 0.5f, sprite.rect.height * 0.5f);
        a = Quaternion.Euler(-rotation) * new Vector2(0.0f, Mathf.Min(center.x, center.y));
        b = Quaternion.Euler(0.0f, 0.0f, 120.0f) * a;
        c = Quaternion.Euler(0.0f, 0.0f, 120.0f) * b;
        a += center;
        b += center;
        c += center;

        sprite.OverrideGeometry(new Vector2[]
        {
                a, b, c
        }, new ushort[] {
                0, 1, 2
        });

        foreach (var tri in cache.Values)
        {
            var r = CalcRotation(tri.index);
            tri.transform.localEulerAngles = rotation + r;
        }
    }

    void Build()
    {
        if (cache.Count > 0)
        {
            return;
        }

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        CreateObjects(radius);
        //tri = CreateObject(stack, tri, Vector2Int.left, Vector2.zero, sqrRadius);
        //tri = CreateObject(stack, tri, Vector2Int.left, Vector2.zero, sqrRadius);
        //tri = CreateObject(stack, tri, Vector2Int.left, Vector2.zero, sqrRadius);
        //tri = CreateObject(stack, tri, Vector2Int.up, Vector2.zero, sqrRadius);
    }

    void UpdateTexture()
    {
    }

    Vector2 CalcCenter(Vector3Int index)
    {
        return new Vector2(
            index.x * len + (index.y + index.z) * len * 0.5f,
            index.y * h + index.z * r) - a;
    }

    Vector3 CalcRotation(Vector3Int index)
    {
        var zMod = (index.x - (index.y + index.z)) % 3;
        var rZ = zMod * -120;
        var rX = index.z * -180f;
        return new Vector3(rX, 0, rZ);
    }

    /// <summary>
    /// directions:
    ///       0               0
    ///       |               |
    ///       a         5 -c --- b- 1
    ///  5 -/   \- 1     4 -\   /-2
    /// 4 -c ___ b- 2         a
    ///       |               |
    ///       3               3
    /// </summary>
    Vector3Int CalcIndex(Vector3Int index, int dir)
    {
        if (index.z == 0)
        {
            switch (dir)
            {
                case 0: return new Vector3Int(index.x - 1, index.y + 1, 1);
                case 1: return new Vector3Int(index.x, index.y, 1);
                case 2: return new Vector3Int(index.x + 1, index.y - 1, 1);
                case 3: return new Vector3Int(index.x, index.y - 1, 1);
                case 4: return new Vector3Int(index.x - 1, index.y - 1, 1);
                case 5: return new Vector3Int(index.x - 1, index.y, 1);
            }
        }
        else
        {
            switch (dir)
            {
                case 0: return new Vector3Int(index.x, index.y + 1, 0);
                case 1: return new Vector3Int(index.x + 1, index.y + 1, 0);
                case 2: return new Vector3Int(index.x + 1, index.y, 0);
                case 3: return new Vector3Int(index.x + 1, index.y - 1, 0);
                case 4: return new Vector3Int(index.x, index.y, 0);
                case 5: return new Vector3Int(index.x - 1, index.y + 1, 0);
            }
        }
        return index;
    }

    void CreateObjects(int radius)
    {
        var level = new HashSet<Vector3Int>();
        var r = 0;
        {
            var index = new Vector3Int(0, 0, 0);
            level.Add(index);
            index = CalcIndex(index, 5);
            level.Add(index);
            index = CalcIndex(index, 0);
            level.Add(index);
            index = CalcIndex(index, 1);
            level.Add(index);
            index = CalcIndex(index, 2);
            level.Add(index);
            index = CalcIndex(index, 3);
            level.Add(index);

            CreateObjects(level, r);
            r++;
        }
        while (r < radius)
        {
            var nextLevel = new HashSet<Vector3Int>();
            foreach (var index in level)
            {
                for (int i = 0; i < 6; i++)
                {
                    var newIndex = CalcIndex(index, i);
                    if (!cache.ContainsKey(newIndex))
                    {
                        nextLevel.Add(newIndex);
                    }
                }
            }
            CreateObjects(nextLevel, r);
            level = nextLevel;
            r++;
        }
    }

    void CreateObjects(HashSet<Vector3Int> indices, int level)
    {
        foreach (var index in indices)
        {
            idx++;
            var name = "t " + idx.ToString() + " ";
            var obj = new GameObject(name);
            obj.AddComponent<SpriteRenderer>();
            obj.transform.SetParent(transform);
            var tri = obj.AddComponent<TriangleComponent>();
            tri.index = index;
            tri.level = level;

            var rotation = CalcRotation(tri.index);
            tri.transform.localScale = Vector3.one;
            tri.transform.localRotation = Quaternion.Euler(rotation);

            cache.Add(index, tri);
        }
    }

    void CreateDot(Vector2 pos, string name)
    {
        var t = Resources.Load<GameObject>("dot");
        var obj = Object.Instantiate(t, transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = pos;
        obj.name = "dot " + name;
    }

    void CreateArrow(Vector2 pos, Vector3 rotation, string name)
    {
        var t = Resources.Load<GameObject>("arrow");
        var obj = Object.Instantiate(t, transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = pos;
        obj.transform.localRotation = Quaternion.Euler(rotation);
        obj.name = "arrow " + name;
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
