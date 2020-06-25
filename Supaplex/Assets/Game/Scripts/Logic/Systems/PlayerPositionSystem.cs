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
            var player = contexts.game.playerEntity;
            var playerPos = player.position.Floor();
            var minPos = new Vector2Int(playerPos.x - Game.window.x, playerPos.y - Game.window.y);
            var maxPos = new Vector2Int(playerPos.x + Game.window.x, playerPos.y + Game.window.y);
            var minChunkPos = new Vector2Int(minPos.x / Game.chunkSize - 1, minPos.y / Game.chunkSize - 1);
            var maxChunkPos = new Vector2Int(maxPos.x / Game.chunkSize, maxPos.y / Game.chunkSize);

            for (int j = minChunkPos.y; j <= maxChunkPos.y; j++)
            {
                for (int i = minChunkPos.x; i <= maxChunkPos.x; i++)
                {
                    var chunkPos = new Vector2Int(i, j);
                    var chunk = Game.chunks[chunkPos];
                    if (chunk == null)
                    {
                        chunk = Game.chunks.CreateChunk(chunkPos);
                        foreach (var pos in chunk.GetKeys())
                        {
                            var obj = contexts.game.CreateEntity();
                            obj.AddObjectType(chunk[pos]);
                            obj.AddObjectState(ObjectState.Init);
                            obj.AddPosition(pos);
                            obj.AddChunkPosition(chunkPos);
                        }
                    }
                    chunk.time = Time.time;
                }
            }

            // cleanup chunks
            foreach (var chunk in Game.chunks.GetUpdateEnumerator())
            {
                if (chunk.time == Time.time)
                {
                    Game.chunks.Add(chunk);
                }
                else
                {
                    var list = contexts.game.GetEntitiesWithChunkPosition(chunk.position);
                    foreach (var listEntity in list)
                    {
                        listEntity.isDestroyed = true;
                    }
                    chunk.Destroy();
                }
            }
        }
    }
}
