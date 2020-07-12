using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Logic.World;

namespace Game.View.World
{
    public class RenderChunk
    {
        public bool calculating;
        public byte visibleFacing;
        public IWorldReader world { get; private set; }
        public IChunkReader chunk { get; private set; }
        public Bounds bounds { get; private set; }
        public int frameIndex { get; private set; }
        public bool isCalculated { get; private set; }
        public Mesh mesh { get; set; }
        public int empty;

        ChunkEventManager viewManager;
        ChunkEventManager coroutineManager;
        ChunkEventProvider<RenderCalcVisibilityEvent> calcVisibiltyProvider;
        ChunkEventProvider<RenderCalcVerticesEvent> calcVerticesProvider;
        public ChunkEventProvider<RenderCalcMeshEvent> calcMeshProvider;

        public bool[] data;
        public int visibilityVersion;
        public int meshVersion;

        public RenderChunk(IWorldReader world, IChunkReader chunk)
        {
            calculating = false;
            viewManager = new ChunkViewEventManager();
            coroutineManager = new ChunkCoroutineEventManager();
            calcVisibiltyProvider = new ChunkEventProvider<RenderCalcVisibilityEvent>(viewManager);
            calcVerticesProvider = new ChunkEventProvider<RenderCalcVerticesEvent>(viewManager);
            calcMeshProvider = new ChunkEventProvider<RenderCalcMeshEvent>(coroutineManager);

            this.world = world;
            this.chunk = chunk;
            visibilityVersion = 0;
            meshVersion = 0;
            bounds = chunk.position.bounds;
            empty = 4096;
            data = new bool[empty]; // 16 * 16 * 16
            mesh = new Mesh();
            visibleFacing = 0;
        }

        public void Update()
        {
            if (calculating)
            {
                return;
            }

            if (visibilityVersion != chunk.version)
            {
                var e = calcVisibiltyProvider.Create();
                e.chunk = chunk;
                e.render = this;
                e.Publish();
                calculating = true;
                return;
            }
            if (meshVersion != visibilityVersion)
            {
                var e = calcVerticesProvider.Create();
                e.world = world;
                e.chunk = chunk;
                e.render = this;
                e.Publish();
                calculating = true;
                return;
            }
        }

        public bool SetFrameIndex(int value)
        {
            if (frameIndex == value)
            {
                return false;
            }
            else
            {
                frameIndex = value;
                return true;
            }
        }

        public bool IsVisible(Facing facing)
        {
            return ((visibleFacing >> (int)facing) & 1) == 1;
        }
    }
}
