
namespace SquircleUtil.Phoenyx
{
    public class PhoenyxMapMarkers : IMapMarkers
    {
        public List<PhoenyxNote> Notes { get; set; } = [];

        INote[] IMapMarkers.Notes => Notes.ToArray();
    }
}
