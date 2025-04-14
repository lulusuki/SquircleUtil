namespace SquircleUtil
{
    public interface IMapSet
    {
        public byte[]? Audio { get; }

        public byte[]? Cover { get; }

        public byte[]? Video { get; }

        public IList<string> ColorSet { get; }
        
        IMapSetMetadata Metadata { get; }

        IDictionary<string, IMap> Maps { get; }
    }
}