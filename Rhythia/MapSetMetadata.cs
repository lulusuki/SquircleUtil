
using Newtonsoft.Json;

namespace SquircleUtil.Rhythia
{
    [JsonObject]
    public class MapSetMetadata : IMapSetMetadata
    {
        public int Version { get; set; } = 1;

        [JsonIgnore]
        public string ID { get; set; } = string.Empty;

        public bool HasAudio { get; set; }

        public bool HasCover { get; set; }

        public bool HasVideo { get; set; }

        public bool HasColorSet { get; set; }

        public string AudioExtension { get; set; } = string.Empty;

        public string Artist { get; set; } = string.Empty;

        public string RomanizedArtist { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string RomanizedTitle { get; set; } = string.Empty;

        public List<string> Difficulties { get; set; } = new();
    }
}