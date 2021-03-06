﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Entitas;
using Game.Logic.World;
using Game.View.World;
using UnityEngine;

namespace Game.View
{
    public class RenderViewSystem : IExecuteSystem
    {
        Contexts contexts;
        IWorld world;
        RenderChunkProvider renderProvider;
        Material material;
        Material material2;
        Mesh[] cubeMeshes;
        Plane[] planes;
        Geometry.Cube cube;
        ChunkEventThread viewThread;
        Queue<ChunkEventBucket> coroutineQueue = new Queue<ChunkEventBucket>();

        public RenderViewSystem(Contexts contexts)
        {
            this.contexts = contexts;
            world = new WorldProvider(1);
            renderProvider = new RenderChunkProvider(world, View.setup.viewSize);

            var r = new Rect(0, 0, 1, 1);
            cube = Geometry.CreateCube2(new Rect[] { r, r, r, r, r, r });
            planes = new Plane[6];

            var blockCount = (int)Enum.GetValues(typeof(BlockType)).Cast<BlockType>().Max() + 1;
            Model.Create();
            material2 = MaterialProvider.material;

            viewThread = new ChunkEventThread();
            View.setup.StartCoroutine(UpdateQueue());
        }

        IEnumerator UpdateQueue()
        {
            var waiter = new WaitForEndOfFrame();
            while (true)
            {
                while (coroutineQueue.Count > 0)
                {
                    var bucket = coroutineQueue.Dequeue();
                    Execute(bucket);
                }
                yield return waiter;
            }
        }

        void Execute(ChunkEventBucket bucket)
        {
            while (bucket.Count > 0)
            {
                var e = bucket.Dequeue();
                e.Execute();
                e.Pool();
            }

            bucket.Pool();
        }

        void RenderWorldSingle()
        {
            lock (ChunkEventManager.viewHashSet)
            {
                foreach (var manager in ChunkEventManager.viewHashSet)
                {
                    while (manager.GetBucket(out var bucket))
                    {
                        viewThread.Enqueue(bucket);
                    }
                }
            }
            foreach (var manager in ChunkEventManager.coroutineHashSet)
            {
                while (manager.GetBucket(out var bucket))
                {
                    coroutineQueue.Enqueue(bucket);
                }
            }

            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            // Ordering: [0] = Left, [1] = Right, [2] = Down, [3] = Up, [4] = Near, [5] = Far
            GeometryUtility.CalculateFrustumPlanes(View.setup._camera, planes);
            float enter;

            UnityEngine.Profiling.Profiler.BeginSample("Calc planes");
            var depth = View.setup.viewSize.z * 16;
            RectInt[] zz = new RectInt[depth];
            for (int z = 0; z < depth; z++)
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
            var queue = new Queue<RenderChunk>();
            var startChunkPos = BlockPos.From(Vector3Int.FloorToInt(playerPos));
            minX = (startChunkPos.x >> 4) - View.setup.viewSize.x;
            maxX = (startChunkPos.x >> 4) + View.setup.viewSize.x;
            minY = (startChunkPos.y >> 4) - View.setup.viewSize.y;
            maxY = (startChunkPos.y >> 4) + View.setup.viewSize.y;
            var chunkPos = new BlockPos();
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    chunkPos.SetChunk(x, y, 0);
                    var renderChunk = renderProvider.GetChunk(chunkPos);
                    if (GeometryUtility.TestPlanesAABB(planes, renderChunk.bounds))
                    {
                        renderChunk.SetFrameIndex(Time.frameCount);
                        queue.Enqueue(renderChunk);
                    }
                    else
                    {
                        renderChunk.Update();
                    }
                }
            }

            while (queue.Count > 0 && queue.Count < 100)
            {
                var renderChunk = queue.Dequeue();
                renderChunk.Update();
                if (renderChunk.empty == 4096)
                {
                    DrawBounds(renderChunk.bounds, Color.white);
                }
                else
                {
                    DrawBounds(renderChunk.bounds, Color.red);
                }
                Graphics.DrawMesh(renderChunk.mesh, Matrix4x4.identity, material2, 0);

                for (Facing facing = Facing.First; facing <= Facing.Last; facing++)
                {
                    chunkPos.Set(renderChunk.chunk.position);
                    chunkPos.AddChunk(facing.GetVector());
                    var renderChunkOffset = renderProvider.GetChunk(startChunkPos, chunkPos);
                    if (renderChunkOffset != null)
                    {
                        renderChunkOffset.Update();
                        if (renderChunkOffset != null && GeometryUtility.TestPlanesAABB(planes, renderChunkOffset.bounds) && renderChunk.IsVisible(facing) && renderChunkOffset.SetFrameIndex(Time.frameCount))
                        {
                            queue.Enqueue(renderChunkOffset);
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

        void DrawBounds(Bounds b, Color color)
        {
            var delay = 0.0f;
            // bottom
            var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
            var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
            var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
            var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

            Debug.DrawLine(p1, p2, color, delay);
            Debug.DrawLine(p2, p3, color, delay);
            Debug.DrawLine(p3, p4, color, delay);
            Debug.DrawLine(p4, p1, color, delay);

            // top
            var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
            var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
            var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
            var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

            Debug.DrawLine(p5, p6, color, delay);
            Debug.DrawLine(p6, p7, color, delay);
            Debug.DrawLine(p7, p8, color, delay);
            Debug.DrawLine(p8, p5, color, delay);

            // sides
            Debug.DrawLine(p1, p5, color, delay);
            Debug.DrawLine(p2, p6, color, delay);
            Debug.DrawLine(p3, p7, color, delay);
            Debug.DrawLine(p4, p8, color, delay);
        }
    }
}
