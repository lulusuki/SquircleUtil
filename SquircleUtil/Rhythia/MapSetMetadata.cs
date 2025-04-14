
using Newtonsoft.Json;

namespace SquircleUtil.Rhythia
{
    [JsonObject]
    public class MapSetMetadata : IMapSetMetadata
    {
        [JsonProperty("version")]
        public int Version { get; set; } = 1;

        [JsonIgnore]
        public string ID { get; set; } = string.Empty;
        
        [JsonIgnore]
        public bool HasAudio { get; set; }

        [JsonIgnore]
        public bool HasCover { get; set; }

        [JsonIgnore]
        public bool HasVideo { get; set; }

        [JsonIgnore]
        public bool HasColorSet { get; set; }

        [JsonProperty("audioExtension")]
        public string AudioExtension { get; set; } = string.Empty;

        [JsonProperty("artist")]
        public string Artist { get; set; } = string.Empty;

        [JsonProperty("romanizedArtist")]
        public string RomanizedArtist { get; set; } = string.Empty;

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonProperty("romanizedTilte")]
        public string RomanizedTitle { get; set; } = string.Empty;

        [JsonProperty("difficulties")]
        public List<string> Difficulties { get; set; } = new();
    }
}