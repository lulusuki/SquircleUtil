

namespace SquircleUtil.Phoenyx
{
    public class PhoenyxMapObjects : IPhoenyxMapObjects
    {
        public List<PhoenyxNote> Notes { get; set; } = [];

        IEnumerable<INote> IMapObjects.Notes => Notes;
    }
}
