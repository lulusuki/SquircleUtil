using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SquircleUtil.Phoenyx
{
    [JsonObject]
    public class PhoenyxMapMetadata : IMapMetadata
    {
        public string ID { get; set; } = string.Empty;

        public bool HasAudio { get; set; } = false;

        public bool HasCover { get; set; } = false;

        public bool HasVideo { get; set; } = false;

        [JsonProperty(PropertyName = "AudioExt")]
        public string AudioExtension { get; set; } = string.Empty;

        public string Artist { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public List<string> Mappers { get; set; } = [];

        public int Difficulty { get; set; } = 0;

        public string DifficultyName { get; set; } = string.Empty;

        public int NoteCount { get; set; }

        IEnumerable<string> IMapMetadata.Mappers => Mappers;
    }
}
