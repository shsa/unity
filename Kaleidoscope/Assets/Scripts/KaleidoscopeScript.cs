using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaleidoscopeScript : MonoBehaviour
{
    Rect rect;
    Rect textureRect;
    Vector2 dir;
    // Start is called before the first frame update
    void Start()
    {
        var sk = GetComponent<SpriteKaleidoscope>();
        rect = new Rect(0, 0, 300, 300);
        textureRect = new Rect(0, 0, sk.texture.width, sk.texture.height);
        sk.SetRect(rect);
        dir = new Vector2(0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if ((rect.xMax + dir.x) > textureRect.xMax)
        {
            dir = new Vector2(-dir.x, dir.y);
        }
        if ((rect.yMax + dir.y) > textureRect.yMax)
        {
            dir = new Vector2(dir.x, -dir.y);
        }
        if ((rect.xMin + dir.x) < textureRect.xMin)
        {
            dir = new Vector2(-dir.x, dir.y);
        }
        if ((rect.yMin + dir.y) < textureRect.yMin)
        {
            dir = new Vector2(dir.x, -dir.y);
        }
        rect = new Rect(rect.x + dir.x, rect.y + dir.y, rect.width, rect.height);
        var sk = GetComponent<SpriteKaleidoscope>();
        sk.SetRect(rect);
    }
}
