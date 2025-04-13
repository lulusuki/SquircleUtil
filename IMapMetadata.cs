namespace SquircleUtil
{
    public interface IMapMetadata
    {
        public string ID { get; }

        public bool HasAudio { get; }

        public bool HasCover { get; }

        public bool HasVideo { get; }

        public string AudioExtension { get; }

        public string Artist { get; }

        public string RomanizedArtist { get; }

        public string Title { get; }

        public string RomanizedTitle { get; }

        public List<string> Mappers { get; }

        public List<int> MappersIds { get; }

        public int NoteCount { get; }
    }
}
