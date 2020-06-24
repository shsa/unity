using Entitas;

namespace Logic
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
        }

        public void Execute()
        {
        }
    }
}
