using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game
{
    public class SyncedGameObjectAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentObject(entity, new SyncedGameObject()
            {
                GameObject = gameObject,
            });

            dstManager.AddComponent<EntityAliveTag>(entity);
            dstManager.AddComponent<CopyTransformToGameObject>(entity);
        }
    }
}