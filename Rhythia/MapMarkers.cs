

using Newtonsoft.Json;

namespace SquircleUtil.Rhythia
{
    [JsonObject]
    public class MapMarkers : IMapMarkers
    {
        public int TimingFields { get; set; }

        public int NoteFields { get; set; }

        public List<int> TimingList { get; set; } = [];

        [JsonIgnore]
        public List<Note> Notes { get; set; } = [];

        public List<float> NoteList { get; set; } = [];

        INote[] IMapMarkers.Notes => Notes.ToArray();
    }
}
