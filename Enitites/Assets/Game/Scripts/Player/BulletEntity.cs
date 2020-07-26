using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Game
{
    /// <summary>
    /// Handles player shooting
    /// </summary>
    public class BulletEntity : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Entity entity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            this.entity = entity;
            dstManager.AddComponent<CopyTransformToGameObject>(entity);
            dstManager.SetComponentData(entity, new Translation { Value = transform.position });
            dstManager.SetComponentData(entity, new Rotation { Value = transform.rotation });
        }
    }
}