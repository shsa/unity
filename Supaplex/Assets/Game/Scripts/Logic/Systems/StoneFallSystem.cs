using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Logic
{
    public class StoneFallSystem : ReactiveSystem<GameEntity>
    {
        Contexts contexts;
        public StoneFallSystem(Contexts contexts) : base(contexts.game)
        {
            this.contexts = contexts;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Position.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasObjectType 
                && entity.objectType.value == ObjectType.Stone 
                && !contexts.HasObject(entity.position.value + Vector2Int.up, ObjectType.Stone);
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var gameEntity in entities)
            {
                var pos = gameEntity.position.value;
                var to = pos + Vector2Int.down;
                if (contexts.IsEmpty(pos + Vector2Int.left) && contexts.IsEmpty(to))
                {
                    contexts.Move(pos, to);
                    continue;
                }
                to = pos + new Vector2Int(-1, -1);
                if (contexts.IsEmpty(pos + Vector2Int.left) && contexts.IsEmpty(to))
                {
                    contexts.Move(pos, to);
                    continue;
                }
                to = pos + new Vector2Int(1, -1);
                if (contexts.IsEmpty(pos + Vector2Int.right) && contexts.IsEmpty(to))
                {
                    contexts.Move(pos, to);
                    continue;
                }
            }
        }
    }
}
