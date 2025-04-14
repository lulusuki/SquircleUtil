using Newtonsoft.Json;

namespace SquircleUtil.Rhythia
{
    [JsonObject]
    public class MapMetadata : IRhythiaMapMetadata
    {
        [JsonIgnore]
        public string ID { get; set; } = string.Empty;

        [JsonProperty("audioExtension")]
        public string AudioExtension { get; set; } = string.Empty;

        [JsonProperty("artist")]
        public string Artist { get; set; } = string.Empty;

        [JsonProperty("romanizedArtist")]
        public string RomanizedArtist { get; set; } = string.Empty;

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("romanizedTitle")]
        public string RomanizedTitle { get; set; } = string.Empty;

        [JsonProperty("mappers")]
        public List<string> Mappers { get; set; } = [];

        [JsonProperty("mappersIds")]
        public List<int> MappersIds { get; set; } = [];
        
        [JsonProperty("noteCount")]
        public int NoteCount { get; set; }

        public int Difficulty { get; set; }

        public string DifficultyName { get; set; } = string.Empty;

        IEnumerable<string> IMapMetadata.Mappers => Mappers;
    }
}