
namespace SquircleUtil.Rhythia.Legacy
{
    public class LegacyMap : IMap
    {

        public byte[]? Audio { get; set; }
        public byte[]? Cover { get; set; }
        public byte[]? Video { get; set; }

        public LegacyMapMarkers Markers { get; set; } = new LegacyMapMarkers();

        public LegacyMapMetadata Metadata { get; set; } = new LegacyMapMetadata();

        IMapMarkers IMap.Markers => Markers;

        IMapMetadata IMap.Metadata => Metadata;

        public LegacyMap(byte[]? audio, byte[]? cover, byte[]? video, LegacyMapMarkers markers, LegacyMapMetadata metadata)
        {
            Audio = audio;
            Cover = cover;
            Video = video;
            Markers = markers;
            Metadata = metadata;
        }

        public static LegacyMap Decode(string file)
        {
            var parser = new FileParser(file);

            try
            {
                if (parser.GetString(4) != "SS+m")
                    throw new("Incorrect file signature");

                ushort version = parser.GetUInt16(); // SSPM version

                if (version == 1)
                {
                    return SSPMV1(parser);
                }
                else if (version == 2)
                {
                    return SSPMV2(parser);
                }
                else throw new("Invalid SSPM version");

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static LegacyMap SSPMV1(FileParser parser)
        {
            throw new NotImplementedException();
        }

        private static LegacyMap SSPMV2(FileParser parser)
        {
            var metadata = new LegacyMapMetadata();

            try
            {
                parser.Skip(4); // reserved
                parser.Skip(20);    // hash

                uint mapLength = parser.GetUInt32();
                uint noteCount = parser.GetUInt32();

                metadata.NoteCount = (int)noteCount;

                parser.Skip(4); // marker count

                metadata.Difficulty = parser.Get(1)[0];

                parser.Skip(2); // map rating

                metadata.HasAudio = parser.GetBool();
                metadata.HasCover = parser.GetBool();

                parser.Skip(1); // 1mod

                ulong customDataOffset = parser.GetUInt64();
                ulong customDataLength = parser.GetUInt64();

                ulong audioByteOffset = parser.GetUInt64();
                ulong audioByteLength = parser.GetUInt64();

                ulong coverByteOffset = parser.GetUInt64();
                ulong coverByteLength = parser.GetUInt64();

                parser.Skip(16);    // marker definitions offset & marker definitions length

                ulong markerByteOffset = parser.GetUInt64();

                parser.Skip(8); // marker byte length (can just use notecount)

                uint mapIdLength = parser.GetUInt16();
                string id = parser.GetString((int)mapIdLength);

                uint mapNameLength = parser.GetUInt16();
                string[] mapName = parser.GetString((int)mapNameLength).Split(" - ", 2);

                if (mapName.Length == 1)
                {
                    metadata.Title = mapName[0].Trim();
                }
                else
                {
                    metadata.Artist = mapName[0].Trim();
                    metadata.Title = mapName[1].Trim();
                }

                uint songNameLength = parser.GetUInt16();

                parser.Skip((int)songNameLength);   // why is this different?

                uint mapperCount = parser.GetUInt16();
                string[] mappers = new string[mapperCount];

                for (int i = 0; i < mapperCount; i++)
                {
                    uint mapperNameLength = parser.GetUInt16();

                    mappers[i] = parser.GetString((int)mapperNameLength);
                }

                metadata.Mappers = mappers.ToList();

                byte[]? audioBuffer = null;
                byte[]? coverBuffer = null;

                parser.Seek((int)customDataOffset);
                parser.Skip(2); // skip number of fields, only care about diff name

                if (parser.GetString(parser.GetUInt16()) == "difficulty_name")
                {
                    int length = 0;

                    switch (parser.Get(1)[0])
                    {
                        case 9:
                            length = parser.GetUInt16();
                            break;
                        case 11:
                            length = (int)parser.GetUInt32();
                            break;
                    }

                    metadata.DifficultyName = parser.GetString(length);
                }

                if (metadata.HasAudio)
                {
                    parser.Seek((int)audioByteOffset);
                    audioBuffer = parser.Get((int)audioByteLength);
                }

                if (metadata.HasCover)
                {
                    parser.Seek((int)coverByteOffset);
                    coverBuffer = parser.Get((int)coverByteLength);
                }

                parser.Seek((int)markerByteOffset);

                List<LegacyNote> notes = [];

                for (int i = 0; i < noteCount; i++)
                {
                    int millisecond = (int)parser.GetUInt32();

                    parser.Skip(1); // marker type, always note

                    bool isQuantum = parser.GetBool();
                    float x;
                    float y;

                    if (isQuantum)
                    {
                        x = parser.GetFloat();
                        y = parser.GetFloat();
                    }
                    else
                    {
                        x = parser.Get(1)[0];
                        y = parser.Get(1)[0];
                    }

                    notes.Add(new(millisecond, x - 1, -y + 1));
                }

                return new LegacyMap(audioBuffer, coverBuffer, null, new LegacyMapMarkers() { Notes = notes }, metadata);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
