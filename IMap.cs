namespace SquircleUtil
{
    public interface IMap
    {
        public byte[]? Audio { get; }

        public byte[]? Cover { get; }

        public byte[]? Video { get; }

        public IMapMarkers Markers { get; }

        public IMapMetadata Metadata { get; }
    }
}
