
namespace SquircleUtil.Rhythia.Legacy
{
    public class LegacyMapMarkers : IMapObjects
    {
        public List<LegacyNote> Notes { get; set; } = [];

        IEnumerable<INote> IMapObjects.Notes => Notes;
    }
}
