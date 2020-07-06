using System.Collections;
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
        World world;
        RenderChunkProvider renderProvider;
        Material material;
        Mesh[] cubeMeshes;
        Plane[] planes;
        Geometry.Cube cube;
        public RenderViewSystem(Contexts contexts)
        {
            this.contexts = contexts;
            world = new World(1);
            renderProvider = new RenderChunkProvider(world, View.setup.viewSize);
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

            //cubeMeshes = Geometry.CreateCube(new Rect[] { R(1, 1), R(1, 3), R(0, 1), R(2, 1), R(1, 2), R(1, 0) });
            //cube = Geometry.CreateCube2(new Rect[] { R(1, 1), R(1, 3), R(0, 1), R(2, 1), R(1, 2), R(1, 0) });
            var r = new Rect(0, 0, 1, 1);
            cube = Geometry.CreateCube2(new Rect[] { r, r, r, r, r, r });
            planes = new Plane[6];
        }

        void RenderWorldSingle()
        {
            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            // Ordering: [0] = Left, [1] = Right, [2] = Down, [3] = Up, [4] = Near, [5] = Far
            GeometryUtility.CalculateFrustumPlanes(View.setup._camera, planes);
            float enter;

            int count = 0;

            UnityEngine.Profiling.Profiler.BeginSample("Calc planes");
            RectInt[] zz = new RectInt[World.depth];
            for (int z = 0; z < World.depth; z++)
            {
                var calcPos = new Vector3(View.setup._camera.transform.position.x, View.setup._camera.transform.position.y, z);
                // left
                var ray = new Ray(calcPos, Vector3.left);
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
                ray = new Ray(calcPos, Vector3.right);
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
                ray = new Ray(calcPos, Vector3.down);
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
                ray = new Ray(calcPos, Vector3.up);
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

            var playerPos = new Vector3(View.setup._camera.transform.position.x, View.setup._camera.transform.position.y, 0);
            var queue = new Queue<RenderChunkInfo>();
            var startChunkPos = ChunkPos.From(Vector3Int.FloorToInt(playerPos));
            minX = startChunkPos.x - View.setup.viewSize.x;
            maxX = startChunkPos.x + View.setup.viewSize.x;
            minY = startChunkPos.y - View.setup.viewSize.y;
            maxY = startChunkPos.y + View.setup.viewSize.y;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var renderChunk = renderProvider[new ChunkPos(x, y, 0)];
                    if (GeometryUtility.TestPlanesAABB(planes, renderChunk.bounds))
                    {
                        renderChunk.SetFrameIndex(Time.frameCount);
                        if (renderChunk.isCalculated)
                        {
                            queue.Enqueue(new RenderChunkInfo(renderChunk, Facing.North));
                        }
                        else
                        {
                            View.setup.StartCoroutine(renderChunk.CalcVisibility());
                        }
                    }
                }
            }

            while (queue.Count > 0 && queue.Count < 100)
            {
                var renderChunkInfo = queue.Dequeue();
                DrawBounds(renderChunkInfo.renderChunk.bounds);
                if (renderChunkInfo.renderChunk.mesh == null)
                {
                    View.setup.StartCoroutine(renderChunkInfo.renderChunk.CalcMesh());
                }
                else
                {
                    Graphics.DrawMesh(renderChunkInfo.renderChunk.mesh, Matrix4x4.identity, material, 0);
                }

                for (Facing facing = Facing.First; facing <= Facing.Last; facing++)
                {
                    var renderChunk = renderProvider[renderChunkInfo.renderChunk.chunk.position.Offset(facing)];
                    if (renderChunk != null)
                    {
                        if (renderChunk.isCalculated)
                        {
                            if (renderChunk != null && GeometryUtility.TestPlanesAABB(planes, renderChunk.bounds) && renderChunkInfo.renderChunk.IsVisible(facing) && renderChunk.SetFrameIndex(Time.frameCount))
                            {
                                queue.Enqueue(new RenderChunkInfo(renderChunk, facing));
                            }
                        }
                        else
                        {
                            View.setup.StartCoroutine(renderChunk.CalcVisibility());
                        }
                    }
                }
            }
        }

        public void Execute()
        {
             RenderWorldSingle();
        }

        void DrawBounds(Bounds b, float delay = 0)
        {
            // bottom
            var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
            var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
            var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
            var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

            Debug.DrawLine(p1, p2, Color.blue, delay);
            Debug.DrawLine(p2, p3, Color.red, delay);
            Debug.DrawLine(p3, p4, Color.yellow, delay);
            Debug.DrawLine(p4, p1, Color.magenta, delay);

            // top
            var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
            var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
            var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
            var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

            Debug.DrawLine(p5, p6, Color.blue, delay);
            Debug.DrawLine(p6, p7, Color.red, delay);
            Debug.DrawLine(p7, p8, Color.yellow, delay);
            Debug.DrawLine(p8, p5, Color.magenta, delay);

            // sides
            Debug.DrawLine(p1, p5, Color.white, delay);
            Debug.DrawLine(p2, p6, Color.gray, delay);
            Debug.DrawLine(p3, p7, Color.green, delay);
            Debug.DrawLine(p4, p8, Color.cyan, delay);
        }
    }
}
