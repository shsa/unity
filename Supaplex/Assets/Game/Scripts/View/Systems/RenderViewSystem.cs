using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Entitas;
using Game.Logic;
using UnityEngine;

namespace Game.View
{
    public class RenderViewSystem : IExecuteSystem
    {
        Contexts contexts;
        Chunks chunks;
        Material material;
        Mesh[] cubeMeshes;
        public RenderViewSystem(Contexts contexts)
        {
            this.contexts = contexts;
            chunks = new Chunks(1);
            material = new Material(Shader.Find("Standard"));
            material.SetTexture("_MainTex", View.setup.wallTexture);
            material.SetColor("_Color", Color.white);

            var texture = View.setup.wallTexture;
            Rect R(int i, int j)
            {
                var w = texture.width / 4f;
                var h = texture.height / 3f;
                return new Rect(i * w / texture.width, j * h / texture.height, w / texture.width, h / texture.height);
            }

            cubeMeshes = Geometry.CreateCube(new Rect[] { R(1, 1), R(1, 3), R(0, 1), R(2, 1), R(1, 2), R(1, 0) });
            var p = new Plane();
        }

        public void Execute()
        {
            //var mm = contexts.game.GetEntities(GameMatcher.Position).Where(e => e.hasObjectType && e.objectType.value == ObjectType.Wall).Select(e => e.position.value);
            var mf = View.setup.wallPrefab.GetComponent<MeshFilter>();
            var mr = View.setup.wallPrefab.GetComponent<MeshRenderer>();

            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            var planes = GeometryUtility.CalculateFrustumPlanes(View.setup._camera);
            float enter = 0;

            int count = 0;

            UnityEngine.Profiling.Profiler.BeginSample("Calc planes");
            RectInt[] zz = new RectInt[Chunks.depth];
            for (int z = 0; z < Chunks.depth; z++)
            {
                var playerPos = new Vector3(View.setup._camera.transform.position.x, View.setup._camera.transform.position.y, z);
                // left
                var ray = new Ray(playerPos, Vector3.left);
                if (planes[0].Raycast(ray, out enter))
                {
                    var point = ray.GetPoint(enter).Floor();
                    if (planes[0].GetSide(point + new Vector3(0.5f, 0, -0.5f)))
                    {
                        minX = point.x - 1;
                    }
                    else
                    {
                        minX = point.x - 1;
                    }
                }
                // right
                ray = new Ray(playerPos, Vector3.right);
                if (planes[1].Raycast(ray, out enter))
                {
                    var point = ray.GetPoint(enter).Floor();
                    if (planes[1].GetSide(point + new Vector3(-0.5f, 0, -0.5f)))
                    {
                        maxX = point.x + 1;
                    }
                    else
                    {
                        maxX = point.x + 1;
                    }
                }
                // bottom
                ray = new Ray(playerPos, Vector3.down);
                if (planes[2].Raycast(ray, out enter))
                {
                    var point = ray.GetPoint(enter).Floor();
                    if (planes[2].GetSide(point + new Vector3(0, 0.5f, -0.5f)))
                    {
                        minY = point.y - 1;
                    }
                    else
                    {
                        minY = point.y - 1;
                    }
                }
                // top
                ray = new Ray(playerPos, Vector3.up);
                if (planes[3].Raycast(ray, out enter))
                {
                    var point = ray.GetPoint(enter).Floor();
                    if (planes[3].GetSide(point + new Vector3(0, -0.5f, -0.5f)))
                    {
                        maxY = point.y + 1;
                    }
                    else
                    {
                        maxY = point.y + 1;
                    }

                }
                zz[z] = new RectInt(minX, minY, maxX - minX, maxY - minY);
            }
            UnityEngine.Profiling.Profiler.EndSample();

            var hp = new Plane(Vector3.up, View.setup._camera.transform.position);
            var vp = new Plane(Vector3.right, View.setup._camera.transform.position);

            for (int z = 0; z < Chunks.depth; z++)
            {
                minX = zz[z].xMin;
                minY = zz[z].yMin;
                maxX = zz[z].xMax;
                maxY = zz[z].yMax;
                UnityEngine.Profiling.Profiler.BeginSample("for");
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        var pp = new Vector3Int(x, y, z);
                        using (var block = chunks[pp])
                        {
                            if (block.objectType == ObjectType.Wall)
                            {
                                if (chunks.IsEmpty(pp + new Vector3Int(0, 0, -1)))
                                {
                                    Graphics.DrawMesh(cubeMeshes[(int)CubeSide.Forward], pp, Quaternion.identity, material, 0);
                                }
                                UnityEngine.Profiling.Profiler.BeginSample("GetSide");
                                var b = hp.GetSide(pp);
                                UnityEngine.Profiling.Profiler.EndSample();
                                if (b)
                                {
                                    if (chunks.IsEmpty(pp + Vector3Int.down))
                                    {
                                        Graphics.DrawMesh(cubeMeshes[(int)CubeSide.Down], pp, Quaternion.identity, material, 0);
                                    }
                                }
                                else
                                {
                                    if (chunks.IsEmpty(pp + Vector3Int.up))
                                    {
                                        Graphics.DrawMesh(cubeMeshes[(int)CubeSide.Up], pp, Quaternion.identity, material, 0);
                                    }
                                }

                                UnityEngine.Profiling.Profiler.BeginSample("GetSide");
                                b = vp.GetSide(pp);
                                UnityEngine.Profiling.Profiler.EndSample();
                                if (b)
                                {
                                    if (chunks.IsEmpty(pp + Vector3Int.left))
                                    {
                                        Graphics.DrawMesh(cubeMeshes[(int)CubeSide.Left], pp, Quaternion.identity, material, 0);
                                    }
                                }
                                else
                                {
                                    if (chunks.IsEmpty(pp + Vector3Int.right))
                                    {
                                        Graphics.DrawMesh(cubeMeshes[(int)CubeSide.Right], pp, Quaternion.identity, material, 0);
                                    }
                                }
                                count++;
                            }
                        }
                    }
                }
                UnityEngine.Profiling.Profiler.EndSample();
            }
            Debug.Log(count);


            //Debug.Log(mm.Count());
            //foreach (var m in mm)
            //{
            //    Graphics.DrawMesh(mf.sharedMesh, m, Quaternion.identity, mr.sharedMaterial, 0);
            //}

            //var mpb = new MaterialPropertyBlock();
            //mpb.SetColor("_Color", Color.blue);
            //while (mm.Count() > 0)
            //{
            //    var t = mm.Take(1023).ToArray();
            //    Graphics.DrawMeshInstanced(mf.sharedMesh, 0, mr.sharedMaterial, t, t.Length, mpb);
            //    //foreach (var m in mm)
            //    //{
            //    //    Graphics.DrawMesh(mf.sharedMesh, m, mr.sharedMaterial, 0);
            //    //}
            //    mm = mm.Skip(1023);
            //}
        }
    }
}
