namespace SquircleUtil.Rhythia
{
    public class Note : INote
    {
        public int Millisecond { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public Note(int millisecond, float x, float y)
        {
            Millisecond = millisecond;
            X = x;
            Y = y;
        }
    }
}