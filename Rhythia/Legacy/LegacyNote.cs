namespace SquircleUtil.Rhythia.Legacy
{
    public class LegacyNote : INote
    {
        public int Millisecond { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public LegacyNote(int millisecond, float x, float y)
        {
            Millisecond = millisecond;
            X = x;
            Y = y;
        }
    }
}
