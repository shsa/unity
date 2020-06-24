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
            var pos = player.position.Floor();
            var minPos = new Vector2Int(pos.x - Game.window.x, pos.y - Game.window.y);
            var maxPos = new Vector2Int(pos.x + Game.window.x, pos.y + Game.window.y);
            var minChunkPos = new Vector2Int(minPos.x / Game.chunkSize - 1, minPos.y / Game.chunkSize - 1);
            var maxChunkPos = new Vector2Int(maxPos.x / Game.chunkSize, maxPos.y / Game.chunkSize);

            for (int j = minChunkPos.y; j <= maxChunkPos.y; j++)
            {
                for (int i = minChunkPos.x; i <= maxChunkPos.x; i++)
                {
                    var chunkPos = new Vector2Int(i, j);
                    var chunkEntity = contexts.game.GetEntitiesWithChunk(chunkPos).SingleOrDefault();
                    if (chunkEntity == null)
                    {
                        chunkEntity = contexts.game.CreateEntity();
                        var newChunk = new Chunk();
                        chunkEntity.AddChunk(chunkPos, newChunk);
                        NoiseS3D.seed = 0;
                        for (int y = 0; y < Game.chunkSize; y++)
                        {
                            for (int x = 0; x < Game.chunkSize; x++)
                            {
                                var p = new Vector2(chunkPos.x * Game.chunkSize + x, chunkPos.y * Game.chunkSize + y);
                                var z = NoiseS3D.Noise(p.x, p.y);
                                if (z > -0.3)
                                {
                                    var item = contexts.game.CreateEntity();
                                    item.AddObjectType(ObjectType.Wall);
                                    item.AddObjectState(ObjectState.Init);
                                    item.AddPosition(p);
                                    item.AddChunkPosition(chunkPos);
                                }
                            }
                        }
                    }
                    var chunk = chunkEntity.chunk.value;
                    chunk.time = Time.time;
                }
            }

            foreach (var chunkEntity in chunkEntities)
            {
                if (chunkEntity.chunk.value.time != Time.time)
                {
                    chunkEntity.isDestroyed = true;
                    var list = contexts.game.GetEntitiesWithChunkPosition(chunkEntity.chunk.position);
                    foreach (var listEntity in list)
                    {
                        listEntity.isDestroyed = true;
                    }
                }
            }
        }
    }
}
