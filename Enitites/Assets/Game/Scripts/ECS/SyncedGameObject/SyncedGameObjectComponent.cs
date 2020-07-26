using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game
{
    public class SyncedGameObject : ISystemStateComponentData
    {
        public GameObject GameObject;
    }

    public struct EntityAliveTag : IComponentData
    {
    }
}