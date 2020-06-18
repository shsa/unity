using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var obj = GameObject.Find("arrow");
        var sr = obj.GetComponent<SpriteRenderer>();
        var point = sr.bounds.center;

        var pos = obj.transform.position;
        obj.transform.localPosition -= point;
        var q = Quaternion.Euler(0, 0, 1);
        obj.transform.localRotation *= q;
        obj.transform.localPosition += point;
    }
}
