namespace Game.Logic.World
{
    public sealed class ChunkGenerateEvent : ChunkEvent
    {
        public IWorldWriter world;
        public IChunkWriter chunk;
        public IWorldGenerator generator;
        public BlockPos pos = new BlockPos();

        public override void Execute()
        {
            generator.Generate(this);
        }
    }
}
