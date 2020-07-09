namespace Game.Logic.World
{
    public static class Events
    {
        public static readonly EventProvider<BlockPlacedEvent> blockPlaced = new EventProvider<BlockPlacedEvent>();
    }
}
