using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Outside = 0x01,
    Empty   = 0x02,
    Cover   = 0x04,
    Filler  = 0x08,
    Border  = 0x10,
    Line    = 0x20
}

public class ItemMapInfo : MonoBehaviour
{
    public GameMap map;
    public Vector2Int position;
    public ItemType type;
    public int id;
}
