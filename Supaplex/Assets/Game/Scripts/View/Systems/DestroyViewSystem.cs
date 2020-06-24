using System;
using System.Collections.Generic;
using Entitas;

namespace Game.View
{
    public class DestroyViewSystem : ReactiveSystem<GameEntity>
    {
        Contexts contexts;
        public DestroyViewSystem(Contexts contexts) : base(contexts.game)
        {
            this.contexts = contexts;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Destroyed.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var gameEntity in entities)
            {
                CreateView.Pool(gameEntity.view.value);
            }
        }
    }
}
