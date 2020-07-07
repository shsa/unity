using UnityEngine;
using Game.Logic.World;

namespace Game.Logic
{
    public static class Game
    {
        public static int chunkSize = 4;
        /// <summary>
        /// pos = Player position:
        /// var minPos = new Vector2Int(pos.x - Game.window.x, pos.y - Game.window.y);
        /// var maxPos = new Vector2Int(pos.x + Game.window.x, pos.y + Game.window.y);
        /// </summary>
        public static Vector2Int window = new Vector2Int(8, 8);
        public static Level level;

        public static IWorld chunks;
    }
}
