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

        public int CompareTo(INote? obj)
        {
            if (obj == null) return 1;
            
            if (Millisecond == obj.Millisecond) return 0;
            
            return Millisecond > obj.Millisecond ? 1 : -1;
        }
    }
}