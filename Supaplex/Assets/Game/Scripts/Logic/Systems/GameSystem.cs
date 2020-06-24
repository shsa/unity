using Entitas;
using UnityEngine;

namespace Game.Logic
{
    public class GameSystem : IInitializeSystem, IExecuteSystem
    {
        Contexts contexts;

        public GameSystem(Contexts contexts)
        {
            this.contexts = contexts;
        }

        public void Initialize()
        {
            //var player = contexts.game.CreateEntity();
            //player.isPlayer = true;
            //player.AddPosition(Vector2.zero);
            throw new System.NotImplementedException();
        }

        public void Execute()
        {
        }
    }
}
