namespace Game.Logic.World
{
    public abstract class ChunkEvent
    {
        ChunkEventProviderBase provider;

        internal void SetProvider(ChunkEventProviderBase provider)
        {
            this.provider = provider;
        }

        public void Publish()
        {
            provider.manager.Publish(this);
        }

        public void Pool()
        {
            provider.Pool(this);
        }

        public abstract void Execute();
    }
}
