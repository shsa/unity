using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Game.View
{
    public class PositionViewSystem : ReactiveSystem<GameEntity>
    {
        Contexts contexts;
        public PositionViewSystem(Contexts contexts) : base(contexts.game)
        {
            this.contexts = contexts;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Position.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            //return entity.hasView;
            return entity.hasObjectType;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var gameEntity in entities)
            {
                //if (gameEntity.objectType.value == BlockType.Wall)
                {
                    //gameEntity.view.value.transform.localPosition = gameEntity.position.value;
                    //gameEntity.AddMatrix(Matrix4x4.Translate(gameEntity.position.value));
                }
            }
        }
    }
}
