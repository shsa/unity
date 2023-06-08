using Assets.Scripts.World;
using Assets.Scripts.World.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    World world;
    void Start()
    {
        world = new World();
    }

    Dictionary<string, GameObject> tiles = new Dictionary<string, GameObject>();

    string GetTilePath(string key)
    {
        if (key == "ground")
        {
            return "Prefabs/Grass 01";
        }
        return "Prefabs/Water 01";
    }

    GameObject GetTile(string key)
    {
        if (!tiles.TryGetValue(key, out GameObject tile))
        {
            tile = Resources.Load<GameObject>(GetTilePath(key));
            tiles.Add(key, tile);

        }

        return Instantiate<GameObject>(tile);
    }


    public int width = 100;
    public int height = 100;

    Dictionary<Vector2Int, GameObject> map = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> pipes = new Dictionary<Vector2Int, GameObject>();

    bool isRightMouseDown = false;
    bool isLeftMouseDown = false;
    Vector3 startMousePosition;
    Vector3 startPosition;

    // Update is called once per frame
    void Update()
    {
        UpdateInput();

        var x = Mathf.FloorToInt(transform.position.x);
        var z = Mathf.FloorToInt(transform.position.z);

        var newMap = new Dictionary<Vector2Int, GameObject>();
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var posX = x + i - width / 2;
                var posZ = z + j - height / 2;
                var pos = new Vector2Int(posX, posZ);
                if (map.TryGetValue(pos, out var obj))
                {
                    map.Remove(pos);
                }
                else
                {
                    obj = GetTile("ground");
                    obj.transform.position = new Vector3(posX, 0, posZ);
                }
                newMap.Add(pos, obj);
            }
        }

        foreach (var obj in map.Values)
        {
            Destroy(obj);
        }

        map = newMap;
    }


    void UpdateNavigation()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Input.mouseScrollDelta.y * 0.5f, transform.position.z);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            isRightMouseDown = true;
            startMousePosition = Input.mousePosition;
            startPosition = transform.position;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            isRightMouseDown = false;
        }

        if (isRightMouseDown)
        {
            startMousePosition.z = transform.position.y;
            var point0 = Camera.main.ScreenToWorldPoint(startMousePosition);
            var mousePos = Input.mousePosition;
            mousePos.z = transform.position.y;
            var point1 = Camera.main.ScreenToWorldPoint(mousePos);
            var worldOffset = point1 - point0;
            transform.position = startPosition - worldOffset;
        }
    }


    void PutItem(ItemEnum itemIndex)
    {
        var item = Item.GetClass(itemIndex);
        var mousePos = Input.mousePosition;
        mousePos.z = transform.position.y;
        var point = Camera.main.ScreenToWorldPoint(mousePos);
        var pos = new Vector2Int(Mathf.FloorToInt(point.x + 0.5f), Mathf.FloorToInt(point.z + 0.5f));
        DirectionEnum side = DirectionEnum.Top;
        if (Mathf.Abs(pos.x - point.x) > Mathf.Abs(pos.y - point.z))
        {
            if ((pos.x - point.x) > 0)
            {
                side = DirectionEnum.Left;
            }
            else
            {
                side = DirectionEnum.Right;
            }
        }
        else
        {
            if ((pos.y - point.z) > 0)
            {
                side = DirectionEnum.Bottom;
            }
        }
        //dir = dir.GetOpposite();

        if (world[pos.x, pos.y].item == ItemEnum.None)
        {
            world[pos.x, pos.y] = new ChunkValue(itemIndex, side);
        }

        if (!pipes.TryGetValue(pos, out var gameObject))
        {
            gameObject = Instantiate(item.GetGameObject(side));
            gameObject.transform.position = new Vector3(pos.x, 0.5f, pos.y);
            pipes.Add(pos, gameObject);
        }
    }

    void UpdateInput()
    {
        UpdateNavigation();

        if (gameUI.SelectedItem == ItemEnum.None)
        {
            return;
        }

        if (gameUI.SelectedItem == ItemEnum.Pipe)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                isLeftMouseDown = true;
                PutItem(gameUI.SelectedItem);
            }
        }
    }
}
