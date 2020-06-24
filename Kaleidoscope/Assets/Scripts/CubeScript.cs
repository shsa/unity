using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public PhysicMaterial material;

    // Start is called before the first frame update
    void Start()
    {
        var size = new Vector3(5, 5, 5);
        var h = 1f;

        var box = transform.Find("Box");
        var b = box.gameObject.AddComponent<BoxCollider>();
        b.size = new Vector3(size.x, h, size.z);
        b.center = new Vector3(0, -(size.y + h) * 0.5f , 0);
        b.material = material;

        b = box.gameObject.AddComponent<BoxCollider>();
        b.size = new Vector3(size.x, h, size.z);
        b.center = new Vector3(0, (size.y + h) * 0.5f, 0);
        b.material = material;

        b = box.gameObject.AddComponent<BoxCollider>();
        b.size = new Vector3(h, size.y, size.z);
        b.center = new Vector3(-(size.x + h) * 0.5f, 0, 0);
        b.material = material;

        b = box.gameObject.AddComponent<BoxCollider>();
        b.size = new Vector3(h, size.y, size.z);
        b.center = new Vector3((size.x + h) * 0.5f, 0, 0);
        b.material = material;

        b = box.gameObject.AddComponent<BoxCollider>();
        b.size = new Vector3(size.x, size.y, h);
        b.center = new Vector3(0, 0, -(size.x + h) * 0.5f);
        b.material = material;

        b = box.gameObject.AddComponent<BoxCollider>();
        b.size = new Vector3(size.x, size.y, h);
        b.center = new Vector3(0, 0, (size.x + h) * 0.5f);
        b.material = material;


        var list = new List<Transform>();
        list.Add(transform.Find("Cube"));
        list.Add(transform.Find("Cube1"));
        list.Add(transform.Find("Sphere"));
        list.Add(transform.Find("Capsule"));

        var newList = new List<Transform>();
        foreach (var t in list)
        {
            var rb = t.gameObject.AddComponent<Rigidbody>();
            var c = t.GetComponent<Collider>();
            c.material = material;

            for (int i = 0; i < 3; i++)
            {
                var newT = Instantiate(t.gameObject).transform;
                newT.SetParent(transform);
                newT.localScale = newT.localScale;
                newT.localPosition = Random.insideUnitSphere * size.x * 0.5f;
                newList.Add(newT);
            }
        }
        //box.rotation = Quaternion.Euler(0, 0, 45);

        StartCoroutine(RotateBox(box, newList));
    }

    IEnumerator RotateBox(Transform box, List<Transform> list)
    {
        var obj = new WaitForSeconds(3);
        while (true)
        {
            //box.rotation *= Quaternion.Euler(0, 0, 90);
            foreach (var t in list)
            {
                var rb = t.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.up * 10 + Random.insideUnitSphere, ForceMode.Impulse);
            }

            yield return obj;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
