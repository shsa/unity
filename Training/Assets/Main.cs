using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    int index = 0;
    public GameObject red;
    public GameObject blue;

    // Start is called before the first frame update
    void Start()
    {
        test4();
    }

    void test1()
    {
        var newObject = Instantiate(red);
        newObject.transform.position = new Vector3(0, 0, 0);

        for (int i = -9; i < 10; i++)
        {
            for (int j = -9; j < 10; j++)
            {

                for (int k = -9; k < 10; k++)
                {
                    Debug.Log(j);
                    var obj = Instantiate(red);
                    obj.transform.position = new Vector3(i, j, k);
                    obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                }
            }
        }
    }

    void test2()

    {
        for(int i = -100; i < 100; i++)
        {
            var obj = Instantiate(red);
            obj.transform.position = new Vector3(5, 0, 0);
            obj.transform.RotateAround(Vector3.zero, Vector3.up, i * 18);
            obj.transform.Translate(new Vector3(0, i, 0));
        }
    }
    void test3()
    {
        for (int i = -50; i < 50; i++)
        {
            var obj = Instantiate(red);
            obj.transform.position = new Vector3(5, 0, 0);
            obj.transform.RotateAround(Vector3.zero, Vector3.up, i * 18);
            obj.transform.Translate(new Vector3(0, i, 0));

            obj = Instantiate(blue);
            obj.transform.position = new Vector3(5, 1, 0);
            obj.transform.RotateAround(Vector3.zero, Vector3.up, i * -18);
            obj.transform.Translate(new Vector3(0, i, 0));
        }
    }
    void test4()
    {
        for( int i = -5; i < 5; i++)
        {
            var obj = Instantiate(red);
            obj.transform.position = new Vector3(i, 0, 5);
            obj = Instantiate(red);
            obj.transform.position = new Vector3(i, 0, -5);
            obj = Instantiate(red);
            obj.transform.position = new Vector3(5, 0, i);
            obj = Instantiate(red);
            obj.transform.position = new Vector3(-5, 0, i);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
