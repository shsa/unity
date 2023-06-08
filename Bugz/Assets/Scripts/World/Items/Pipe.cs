using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.World.Items
{
    public class Pipe : Item
    {

        public override GameObject GetGameObject(byte meta)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Pipe/Pipe");
            return GameObject.Instantiate(prefab);
        }
    }
}
