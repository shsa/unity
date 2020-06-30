using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

namespace Game.Logic
{
    public class PlayerPositionSystem : ReactiveSystem<GameEntity>
    {
        Contexts contexts;
        IGroup<GameEntity> chunkEntities;
        public PlayerPositionSystem(Contexts contexts) : base(contexts.game)
        {
            this.contexts = contexts;
            this.chunkEntities = contexts.game.GetGroup(GameMatcher.Chunk);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Position.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isPlayer; 
        }

        protected override void Execute(List<GameEntity> entities)
        {
        }
    }
}
