using Entitas;
using UnityEngine;
using Game.Logic.World;
using System.Collections;
using System.Diagnostics.Tracing;

namespace Game.Logic
{
    public class GameSystem : IInitializeSystem, IExecuteSystem
    {
        Contexts contexts;
        MonoBehaviour forCoroutines;
        ChunkEventThread[] eventThreads;

        public GameSystem(Contexts contexts, MonoBehaviour forCoroutines)
        {
            this.contexts = contexts;
            this.forCoroutines = forCoroutines;
        }

        public void Initialize()
        {
            var player = contexts.game.CreateEntity();
            player.isPlayer = true;
            player.AddPosition(Vector2.zero);
            Game.chunks = new WorldProvider(0);

            eventThreads = new ChunkEventThread[2];
            for (int i = 0; i < eventThreads.Length; i++)
            {
                eventThreads[i] = new ChunkEventThread();
            }
        }

        public void Execute()
        {
            lock (ChunkEventManager.logicHashSet)
            {
                foreach (var manager in ChunkEventManager.logicHashSet)
                {
                    while (manager.GetBucket(out var bucket))
                    {
                        var thread = eventThreads[0];
                        for (int i = 1; i < eventThreads.Length; i++)
                        {
                            if (thread.Count() > eventThreads[i].Count())
                            {
                                thread = eventThreads[i];
                            }
                        }
                        thread.Enqueue(bucket);
                    }
                }
            }

            //var e = EventManager.blockPlaced.Dequeue();
            //while (e != null)
            //{
            //    forCoroutines.StartCoroutine(DoEvents(e));
            //    e = EventManager.blockPlaced.Dequeue();
            //}
        }
    }
}
