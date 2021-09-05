using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// https://www.redblobgames.com/grids/hexagons/

//[ExecuteInEditMode]
public class HexGrid : MonoBehaviour
{
    void Awake()
    {
        var grid = GetComponent<Grid>();
        var tilemap = GetComponent<Tilemap>();
    }

    bool mouseDown = false;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!mouseDown)
            {
                mouseDown = true;
                OnTouch();
            }
        }
        else
        {
            mouseDown = false;
        }
    }

    void OnTouch()
    {
        //var tilemap = GetComponent<Tilemap>();
        //Debug.Log(tilemap.WorldToCell(Input.mousePosition));

        var grid = GetComponent<Grid>();
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cell = grid.WorldToCell(worldPos);
        Debug.Log(cell);
    }
}
