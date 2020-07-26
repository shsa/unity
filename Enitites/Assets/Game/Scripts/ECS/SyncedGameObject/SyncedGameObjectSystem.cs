using Unity.Entities;
using UnityEngine;

namespace Game
{
    public class DestroySyncedGameObjectSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<EntityAliveTag>()
                .ForEach((Entity entity, SyncedGameObject syncedGameObject) =>
            {
                GameObject.Destroy(syncedGameObject.GameObject);
                EntityManager.RemoveComponent<SyncedGameObject>(entity);
            });
        }
    }
}