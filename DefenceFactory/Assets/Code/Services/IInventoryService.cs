using DefenceFactory.Game.World;
using Leopotam.Ecs.Types;

namespace DefenceFactory
{
    interface IInventoryService
    {
        bool GetBlock(out BlockType blockType);
    }
}
