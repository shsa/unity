using Game.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Material material;
    Material mat2;
    Texture2D tmpTexture;

    Contexts contexts;
    List<GameEntity> entities;
    Mesh[] cubeMeshes;

    // Start is called before the first frame update
    void Start()
    {
        var src = material.mainTexture;
        var w = src.width;
        var h = src.height;
        var rt = new RenderTexture(w, h, 0);
        var tt = new Texture2D(w, h, TextureFormat.ARGB32, false);
        Graphics.ConvertTexture(src, tt);
        Graphics.CopyTexture(tt, rt);
        RenderTexture.active = rt;
        tt.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tt.Apply();
        var pixels = tt.GetPixels();

        mat2 = new Material(material.shader);
        mat2.mainTexture = tt;

        contexts = new Contexts();
        entities = new List<GameEntity>();

        TestCreateCube();
    }

    void TestCreateEntities()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Destroy();
            }
            entities.Clear();

            for (int i = 0; i < 1000; i++)
            {
                var e = contexts.game.CreateEntity();
                entities.Add(e);
            }
        }
    }


    void TestCreateCube()
    {
        //var obj = new GameObject("TestCube");
        //obj.transform.SetParent(transform);
        //obj.transform.position = Vector3.zero;
        //obj.transform.localScale = Vector3.one;
        //var mf = obj.AddComponent<MeshFilter>();
        //mf.sharedMesh = Geometry.CreateCubeSide(new Rect(0, 0, 1, 1));
        //var mr = obj.AddComponent<MeshRenderer>();
        //mr.sharedMaterial = material;

        //var texture = new Texture2D(material.mainTexture.width, material.mainTexture.height, TextureFormat.RGBA32, false);
        //Graphics.ConvertTexture(material.mainTexture, texture);
        //var pixels = texture.GetPixels();
        //mat2 = new Material(material.shader);
        //mat2.mainTexture = texture;

        Rect R(int i, int j)
        {
            var w = material.mainTexture.width / 4f;
            var h = material.mainTexture.height / 3f;
            return new Rect(i * w / material.mainTexture.width, j * h / material.mainTexture.height, w / material.mainTexture.width, h / material.mainTexture.height);
        }

        cubeMeshes = Geometry.CreateCube(new Rect[] { R(1, 1), R(1, 3), R(0, 1), R(2, 1), R(1, 2), R(1, 0) });
    }

    void TestDrawCube()
    {
        for (int i = 0; i < 6; i++)
        {
            Graphics.DrawMesh(cubeMeshes[i], Vector3.zero, Quaternion.identity, mat2, 0);
        }
    }


    // Update is called once per frame
    void Update()
    {
        //TestCreateEntities();
        TestDrawCube();
    }
}
