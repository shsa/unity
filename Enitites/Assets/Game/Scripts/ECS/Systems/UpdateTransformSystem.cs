using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

namespace Game
{
    public sealed class TransformConversion : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((UnityEngine.Transform transform) =>
            {
                //Convert
            });
        }
    }

    public sealed class UpdateTransformSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            //Entities.ForEach((ref UnityEngine.Transform transform, in Translation translation) =>
            //{
            //    transform.position = translation.Value;
            //});
        }
    }
}