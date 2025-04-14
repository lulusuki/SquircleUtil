namespace SquircleUtil
{
    public interface INote : IComparable<INote>
    {
        public int Millisecond { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        
    }
}