using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class SpriteKaleidoscope : MonoBehaviour
{
    static Dictionary<Vector3Int, TriangleComponent> cache = new Dictionary<Vector3Int, TriangleComponent>();

    Texture2D _texture;
    public Texture2D texture;
    public int Size = 4;
    int _pixelsPerUnit = 100;
    public int pixelsPerUnit = 100;
    public int radius = 3;

    Sprite _sprite;
    /// <summary>
    ///     a
    ///   /   \
    ///  c ___ b
    /// </summary>
    Vector2 a;
    Vector2 b;
    Vector2 c;
    int idx = 0;

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
        var rect = new Rect(0, 0, texture.width / 2, texture.height / 2);
        var t = texture;
        var center = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
        a = new Vector2(0.0f, Mathf.Min(center.x, center.y));
        b = Quaternion.Euler(0.0f, 0.0f, 120.0f) * a;
        c = Quaternion.Euler(0.0f, 0.0f, 120.0f) * b;
        a += center;
        b += center;
        c += center;
        var l = (b - a).magnitude;

        //_sprite = Sprite.Create(t, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
        _sprite = Sprite.Create(t, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
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

        center = new Vector2(rect.width * 0.5f / pixelsPerUnit, rect.height * 0.5f / pixelsPerUnit);
        a = new Vector2(0.0f, Mathf.Min(center.x, center.y));
        b = Quaternion.Euler(0.0f, 0.0f, 120.0f) * a;
        c = Quaternion.Euler(0.0f, 0.0f, 120.0f) * b;

        var obj = new GameObject("t");
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = _sprite;
        obj.transform.SetParent(transform);
        var tri = obj.AddComponent<TriangleComponent>();
        tri.center = Vector2.zero - a;
        tri.rotation = Vector3Int.zero;
        tri.index = Vector3Int.zero;
        cache.Add(tri.index, tri);
        idx = 0;
        UpdateObject(tri);

        var stack = new Stack<TriangleComponent>();
        stack.Push(tri);

        var sqrH = (b - a).sqrMagnitude * 0.75f; // h = a * sqrt(3) / 2 => h^2 = a^2 * 3 / 4

        var sqrRadius = radius * sqrH;
        var coll = GetComponent<CircleCollider2D>();
        coll.radius = Mathf.Sqrt(sqrRadius);
        CreateObject(stack, Vector2.zero, sqrRadius);
        //tri = CreateObject(stack, tri, Vector2Int.left, Vector2.zero, sqrRadius);
        //tri = CreateObject(stack, tri, Vector2Int.left, Vector2.zero, sqrRadius);
        //tri = CreateObject(stack, tri, Vector2Int.left, Vector2.zero, sqrRadius);
        //tri = CreateObject(stack, tri, Vector2Int.up, Vector2.zero, sqrRadius);
    }

    void UpdateObject(TriangleComponent tri)
    {
        // https://youclever.org/book/ravnostoronnij-treugolnik-1
        var len = (a - b).magnitude * 1.2f;
        var h = len * Mathf.Sqrt(3) * 0.5f; // // h = a * sqrt(3) / 2
        var r = len * Mathf.Sqrt(3) / 6; // // r = a * sqrt(3) / 6
        var pos = new Vector2(
            tri.index.x * len + (tri.index.y + tri.index.z) * len * 0.5f,
            tri.index.y * h + tri.index.z * r) - a;
        var zMod = (tri.index.x - (tri.index.y + tri.index.z)) % 3;
        var rZ = zMod * -120;
        var rX = tri.index.z * -180f;
        var rotation = new Vector3(
            rX, 
            0, 
            rZ
            );
        CreateArrow(pos, rotation, tri.name);

        tri.transform.localScale = Vector3.one;
        //tri.transform.localPosition = tri.center;
        //tri.transform.localRotation = Quaternion.Euler(tri.rotation);
        tri.transform.localPosition = pos;
        tri.transform.localRotation = Quaternion.Euler(rotation);
    }

    void CreateObject(Stack<TriangleComponent> stack, Vector2 center, float sqrRadius)
    {
        while (stack.Count > 0)
        {
            var tri = stack.Pop();
            CreateObject(stack, tri, Vector2Int.up, center, sqrRadius);
            CreateObject(stack, tri, Vector2Int.down, center, sqrRadius);
            CreateObject(stack, tri, Vector2Int.left, center, sqrRadius);
            CreateObject(stack, tri, Vector2Int.right, center, sqrRadius);
        }
    }

    TriangleComponent CreateObject(Stack<TriangleComponent> stack, TriangleComponent start, Vector2Int dir, Vector2 center, float sqrRadius)
    {
        var scale = 1.2f;
        var tri0 = start.GetComponent<TriangleComponent>();
        var top = a * scale;
        var c = Vector2.zero;
        var r = Vector3Int.zero;
        var index = tri0.index;
        idx++;
        var name = "t " + idx.ToString() + " ";
        if (tri0.index.z == 0)
        {
            if (dir == Vector2Int.left)
            {
                c = tri0.center + (Vector2)(Quaternion.Euler(0, 0, 60) * top);
                r = new Vector3Int(tri0.rotation.x, tri0.rotation.y - 180, tri0.rotation.z + 60);
                index = new Vector3Int(index.x - 1, index.y, 1);
                name += "left";
            }
            else
            if (dir == Vector2Int.right)
            {
                c = tri0.center + (Vector2)(Quaternion.Euler(0, 0, -60) * top);
                r = new Vector3Int(tri0.rotation.x, tri0.rotation.y - 180, tri0.rotation.z - 60);
                index = new Vector3Int(index.x, index.y, 1);
                name += "right";
            }
            else
            if (dir == Vector2Int.down)
            {
                c = tri0.center + (Vector2)(Quaternion.Euler(0, 0, 180) * top);
                r = new Vector3Int(tri0.rotation.x - 180, tri0.rotation.y, tri0.rotation.z);
                index = new Vector3Int(index.x, index.y - 1, 1);
                name += "down";
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (dir == Vector2Int.left)
            {
                c = tri0.center + (Vector2)(Quaternion.Euler(0, 0, 120) * top);
                r = new Vector3Int(tri0.rotation.x, tri0.rotation.y - 180, tri0.rotation.z + 60);
                index = new Vector3Int(index.x, index.y, 0);
                name += "left";
            }
            else
            if (dir == Vector2Int.right)
            {
                c = tri0.center + (Vector2)(Quaternion.Euler(0, 0, -120) * top);
                r = new Vector3Int(tri0.rotation.x, tri0.rotation.y - 180, tri0.rotation.z - 60);
                index = new Vector3Int(index.x + 1, index.y, 0);
                name += "right";
            }
            else
            if (dir == Vector2Int.up)
            {
                c = tri0.center + top;
                r = new Vector3Int(tri0.rotation.x - 180, tri0.rotation.y, tri0.rotation.z);
                index = new Vector3Int(index.x, index.y + 1, 0);
                name += "up";
            }
            else
            {
                return null;
            }
        }

        if (cache.TryGetValue(index, out var tri))
        {
            return null;
        }

        var q = Quaternion.Euler(r);
        Vector2 a0 = q * top;
        Vector2 b0 = Quaternion.Euler(0, 0, 120) * a0;
        Vector2 c0 = Quaternion.Euler(0, 0, 120) * b0;

        if ((c + a0 - center).sqrMagnitude > sqrRadius && (c + b0 - center).sqrMagnitude > sqrRadius && (c + c0 - center).sqrMagnitude > sqrRadius)
        {
            return null;
        }

        var obj = new GameObject(name);
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = _sprite;
        obj.transform.SetParent(transform);
        tri = obj.AddComponent<TriangleComponent>();
        tri.index = index;
        tri.center = c;
        tri.rotation = r;

        UpdateObject(tri);

        cache.Add(index, tri);
        stack.Push(tri);

        return tri;
    }

    void CreateObject(Vector3Int pos)
    {

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
