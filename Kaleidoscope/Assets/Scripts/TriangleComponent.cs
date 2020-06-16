using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public enum TriangleType
{
    Up,
    Down
}

public enum TriangleDir
{
    Up,
    Down,
    Left,
    Right
}

public class TriangleComponent : MonoBehaviour
{
    public Vector3Int index;
    public Vector2 center;
    public Vector3Int rotation;
}
