using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Game.Logic
{
    public class CleanupSystem : ICleanupSystem
    {
        Contexts contexts;
        IGroup<GameEntity> objectStates;
        IGroup<GameEntity> destroyedEntities;
        public CleanupSystem(Contexts contexts)
        {
            this.contexts = contexts;
            objectStates = contexts.game.GetGroup(GameMatcher.ObjectState);
            destroyedEntities = contexts.game.GetGroup(GameMatcher.Destroyed);
        }

        public void Cleanup()
        {
            foreach (var obj in objectStates.GetEntities())
            {
                obj.ReplaceObjectState(ObjectState.None);
            }

            foreach (var obj in destroyedEntities.GetEntities())
            {
                obj.Destroy();
            }
        }
    }
}