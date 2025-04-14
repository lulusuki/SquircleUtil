namespace SquircleUtil.Phoenyx
{
    public interface IPhoenyxMapMetadata : IMapMetadata
    {
        public bool HasAudio { get; }

        public bool HasCover { get; }

        public bool HasVideo { get; }

        public bool HasColorSet { get; }
    }
}