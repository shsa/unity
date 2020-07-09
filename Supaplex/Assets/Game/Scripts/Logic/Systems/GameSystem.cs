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
        }

        IEnumerator DoEvents(EventChunk e)
        {
            while (e.Raise())
            {
            }
            e.Pool();
            yield return e;
        }

        public void Execute()
        {
            var e = Events.blockPlaced.Dequeue();
            while (e != null)
            {
                forCoroutines.StartCoroutine(DoEvents(e));
                e = Events.blockPlaced.Dequeue();
            }
        }
    }
}
