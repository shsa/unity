using DefenceFactory.Game.World;
using Leopotam.Ecs;
using System;
using Unity.Collections;
using UnityEngine;

namespace DefenceFactory.Ecs
{
    sealed class DestroySystem : IEcsDestroySystem
    {
        readonly GameWorld _gameWorld = default;

        public void Destroy()
        {
            _gameWorld.Dispose();
        }
    }
}
