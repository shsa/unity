using DefenceFactory.Game.Inventory;
using DefenceFactory.Game.World;
using DG.Tweening;
using Leopotam.Ecs.Types;
using UnityEngine;

namespace DefenceFactory
{
    public class InventoryService : MonoBehaviour, IInventoryService
    {
        public bool GetBlock(out Block block)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                block = Block.GetBlock(BlockType.Stone);
                return true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                block = Block.GetBlock(BlockType.Pipe);
                return true;
            }
            block = default;
            return false;
        }
    }
}
