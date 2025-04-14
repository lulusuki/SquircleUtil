namespace SquircleUtil
{
    public interface IMapMetadata
    {
        public string ID { get; }

        public string AudioExtension { get; }

        public string Artist { get; }

        public string Title { get; }

        public IEnumerable<string> Mappers { get; }
        
        public int Difficulty { get; }
        
        public string DifficultyName { get; }

        public int NoteCount { get; }
    }
}
