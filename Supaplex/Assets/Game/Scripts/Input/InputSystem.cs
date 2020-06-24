using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

namespace Game
{
    public class InputSystem : IExecuteSystem
    {
        Contexts contexts;
        public InputSystem(Contexts contexts)
        {
            this.contexts = contexts;
        }

        public void Execute()
        {
            var dx = Input.GetAxis("Horizontal");
            var dy = Input.GetAxis("Vertical");
            var player = contexts.game.playerEntity;
            contexts.game.playerEntity.ReplacePosition(new Vector2(player.position.value.x + dx, player.position.value.y + dy));
        }
    }
}
