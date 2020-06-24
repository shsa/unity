using UnityEngine;

public static class Helpers
{
    public static Vector3 ToVector3(this Vector2Int v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static int Floor(this float x)
    {
        return x > 0 ? (int)x : (int)x - 1;
    }

    public static Vector2Int Floor(this Vector2 v)
    {
        return new Vector2Int(Floor(v.x), Floor(v.y));
    }
}
