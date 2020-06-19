using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaleidoscopeScript : MonoBehaviour
{
    Rect rect, oldRect;
    Rect textureRect;
    Vector2 dir;
    // Start is called before the first frame update
    void Start()
    {
        var ka = GetComponent<KaleidoscopeAnimator>();
        var ks = GetComponent<KaleidoscopeSprite>();
        //ks.texture = dstTexture;
        rect = new Rect(0, 0, 100, 100);
        oldRect = rect;
        textureRect = new Rect(0, 0, 100, 100);
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
        rect.position = rect.position + dir;
        if ((rect.position - oldRect.position).sqrMagnitude >= 1)
        {
            //var sk = GetComponent<KaleidoscopeSprite>();
            //var cc = srcTexture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            //dstTexture.SetPixels(cc);
            //dstTexture.Apply();
            //oldRect = rect;
        }
    }
}
