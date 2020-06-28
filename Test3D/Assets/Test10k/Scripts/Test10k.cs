using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test10k : MonoBehaviour
{
	public GameObject cubePrefab;
	public float radius = 100;
    public int count = 10000;

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
        
    }
}
