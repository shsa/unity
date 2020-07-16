using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game
{
    [Serializable]
    public struct ChainItem
    {
        public float3 pos;
        public float4 rot;
    }

    [Serializable]
    public struct ChainNode : IComponentData
    {
        //public ChainItem[] queue;
    }

    [GenerateAuthoringComponent]
    public struct ChainNodeState : ISystemStateComponentData
    {
    }
}