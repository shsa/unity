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
        public PlayerPositionSystem(Contexts contexts) : base(contexts.game)
        {
            this.contexts = contexts;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            //return context.CreateCollector(GameMatcher.Position.Added());
            throw new System.NotImplementedException();
        }

        protected override bool Filter(GameEntity entity)
        {
            //return entity.isPlayer; 
            throw new System.NotImplementedException();
        }

        Hashtable chunks = new Hashtable();
        protected override void Execute(List<GameEntity> entities)
        {
            //var player = entities.SingleEntity();
            //var pos = player.position.Floor();
            //var chunkPos = new Vector2Int(pos.x / Game.chunkSize, pos.y / Game.chunkSize);
            //var minPos = new Vector2Int(pos.x - Game.window.x, pos.y - Game.window.y);
            //var maxPos = new Vector2Int(pos.x + Game.window.x, pos.y + Game.window.y);
            //var minChunkPos = new Vector2Int(minPos.x / Game.chunkSize, minPos.y / Game.chunkSize);
            //var maxChunkPos = new Vector2Int(maxPos.x / Game.chunkSize, maxPos.y / Game.chunkSize);

            //for (int j = minChunkPos.y; j < maxChunkPos.y; j++)
            //{
            //    for (int i = minChunkPos.x; i < maxChunkPos.x; i++)
            //    {

            //        //var chunkEntity = contexts.game.GetEntitiesWithChunkPosition(new Vector2Int(i, j)).FirstOrDefault();
            //        //chunkEntity.AddChunkPosition(new Vector2Int(0, 0));
            //    }
            //}
            throw new System.NotImplementedException();
        }
    }
}
