using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test10k : MonoBehaviour
{
	public GameObject cubePrefab;
	public float radius = 100;
    public int count = 10000;
    public GameObject _camera;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10000; i++)
		{
			var cube = Instantiate(cubePrefab, Random.insideUnitSphere * radius, Quaternion.identity, transform);
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            _camera.transform.Rotate(Vector3.left, v);
            _camera.transform.Rotate(Vector3.up, h);
        }
        else
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            _camera.transform.Translate(Vector3.forward * v * 0.25f);
            _camera.transform.Translate(Vector3.left * h * 0.25f);
        }
    }
}
