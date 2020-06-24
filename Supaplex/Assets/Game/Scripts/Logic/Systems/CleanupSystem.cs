using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Logic
{
    public class CleanupSystem : ICleanupSystem
    {
        Contexts contexts;
        IGroup<GameEntity> objectStates;
        public CleanupSystem(Contexts contexts)
        {
            this.contexts = contexts;
            objectStates = contexts.game.GetGroup(GameMatcher.ObjectState);
        }

        public void Cleanup()
        {
            foreach (var obj in objectStates.GetEntities())
            {
                obj.ReplaceObjectState(ObjectState.None);
            }
        }
    }
}