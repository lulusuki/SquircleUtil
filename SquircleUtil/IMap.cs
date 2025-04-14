namespace SquircleUtil
{
    public interface IMap
    {
        public byte[]? Audio { get; }

        public byte[]? Cover { get; }

        public IList<string> ColorSet { get; }

        public IMapObjects Objects { get; }

        public IMapMetadata Metadata { get; }
    }
}
