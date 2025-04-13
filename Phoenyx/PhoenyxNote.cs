namespace SquircleUtil.Phoenyx
{
    public class PhoenyxNote : INote
    {
        public int Index;

        public float X { get; set; }

        public float Y { get; set; }

        public int Millisecond { get; set; }

        public PhoenyxNote(int index, int millisecond, float x, float y)
        {
            Index = index;
            Millisecond = millisecond;
            X = x;
            Y = y;
        }
    }
}
