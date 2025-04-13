using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SquircleUtil.Rhythia
{
    [JsonObject]
    public class MapMarkers : IMapMarkers
    {
        [JsonProperty("timingFields")]
        public int TimingFields { get; set; }

        [JsonProperty("noteFields")]
        public int NoteFields { get; set; }

        [JsonProperty("timingList")]
        public List<int> TimingList { get; set; } = [];

        [JsonIgnore]
        public List<Note> Notes { get; set; } = [];

        [JsonProperty("noteList")]
        public List<float> NoteList { get; set; } = [];

        INote[] IMapMarkers.Notes => Notes.ToArray();
    }
}
