using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Game.View
{
    public class PlayerPositionViewSystem : ReactiveSystem<GameEntity>
    {
        Contexts contexts;
        public PlayerPositionViewSystem(Contexts contexts) : base(contexts.game)
        {
            this.contexts = contexts;
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
            var playerEntity = contexts.game.playerEntity;
            var playerObject = View.setup.player;
            playerObject.transform.localPosition = new Vector3(playerEntity.position.value.x, playerEntity.position.value.y, playerObject.transform.localPosition.z);
        }
    }
}
