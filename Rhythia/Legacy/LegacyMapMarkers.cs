namespace SquircleUtil.Rhythia.Legacy
{
    public class LegacyMapMarkers : IMapMarkers
    {
        public List<LegacyNote> Notes { get; set; } = [];

        INote[] IMapMarkers.Notes => [.. Notes];
    }
}
