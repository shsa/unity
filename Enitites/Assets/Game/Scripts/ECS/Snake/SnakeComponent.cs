﻿using Unity.Entities;
using Unity.Mathematics;

namespace Game
{
    [GenerateAuthoringComponent]
    public struct Snake : IComponentData
    {
        public float time;
    }
}