using DefenceFactory.Game.Inventory;
using DefenceFactory.Game.World;
using DG.Tweening;
using Leopotam.Ecs.Types;
using UnityEngine;

namespace DefenceFactory
{
    public class InventoryService : MonoBehaviour, IInventoryService
    {
        public bool GetBlock(out BlockType blockType)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                blockType = BlockType.Stone;
                return true;
            }
            blockType = BlockType.None;
            return false;
        }
    }
}
