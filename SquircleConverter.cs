namespace SquircleUtil
{
    public class SquircleConverter
    {
        public static SquircleFormat GetFormat(string file)
        {
            var extension = Path.GetExtension(file);

            switch (extension)
            {
                case nameof(SquircleFormat.RHYM):
                    return SquircleFormat.RHYM;
                case nameof(SquircleFormat.SSPM):
                    return SquircleFormat.SSPM;
                case nameof(SquircleFormat.PHXM):
                    return SquircleFormat.PHXM;
                default:
                    throw new ("Unsupported file type");
            }
        }

        public static T Convert<T>(IMap map)
            where T : IMap
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes to file on selected encoding type
        /// </summary>
        public static void Encode(string path, IMap map, SquircleFormat format = SquircleFormat.RHYM)
        {
            switch (format)
            {
                default:
                    return;
            }
        }
    }
}
