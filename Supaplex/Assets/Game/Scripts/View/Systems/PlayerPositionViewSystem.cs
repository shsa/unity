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
            var player = contexts.game.playerEntity;
            Camera.main.transform.localPosition = new Vector3(player.position.value.x, player.position.value.y, Camera.main.transform.localPosition.z);
        }
    }
}
