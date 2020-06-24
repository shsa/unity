using System;
using System.Collections.Generic;
using Entitas;

namespace View
{
    public class PositionViewSystem : ReactiveSystem<GameEntity>
    {
        Contexts contexts;
        public PositionViewSystem(Contexts contexts) : base(contexts.game)
        {
            this.contexts = contexts;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Position.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var gameEntity in entities)
            {
                if (gameEntity.objectState.value == ObjectState.Init)
                {
                    //gameEntity.view.value.transform.localPosition = gameEntity.position.value.ToVector3();
                }
            }
        }
    }
}
