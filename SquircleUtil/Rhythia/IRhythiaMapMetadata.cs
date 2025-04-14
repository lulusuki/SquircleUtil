namespace SquircleUtil.Rhythia
{
    public interface IRhythiaMapMetadata : IMapMetadata
    {
        public string RomanizedArtist { get; }

        public string RomanizedTitle { get; }

        public List<int> MappersIds { get; }
    }
}