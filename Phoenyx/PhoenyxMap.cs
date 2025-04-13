using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;

namespace SquircleUtil.Phoenyx
{
    public class PhoenyxMap : IMap
    {
        public byte[]? Audio { get; set; }

        public byte[]? Cover { get; set; }

        public byte[]? Video { get; set; }

        public PhoenyxMapMarkers Markers { get; set; } = new();

        public PhoenyxMapMetadata Metadata { get; set; } = new();

        IMapMarkers IMap.Markers { get => Markers; }

        IMapMetadata IMap.Metadata { get => Metadata; }

        public PhoenyxMap() {}

        public PhoenyxMap(IMap map)
        {
            Audio = map.Audio;
            Cover = map.Cover;
            Video = map.Video;
            Markers = (PhoenyxMapMarkers)map.Markers;
            Metadata = (PhoenyxMapMetadata)map.Metadata;
        }

        public PhoenyxMap(byte[]? audio, byte[]? cover, byte[]? video, PhoenyxMapMarkers markers, PhoenyxMapMetadata metadata)
        {
            Audio = audio;
            Cover = cover;
            Video = video;
            Markers = markers;
            Metadata = metadata;
        }

        public static void Encode(string file)
        {
            throw new NotImplementedException();
        }

        public static PhoenyxMap Decode(string file)
        {
            try
            {
                var folderContents = ZipFile.OpenRead(file);
                Stream metaStream = folderContents.GetEntry("metadata.json")!.Open();
                Stream objectsStream = folderContents.GetEntry("objects.phxmo")!.Open();

                byte[] metaBuffer = new byte[metaStream.Length];
                byte[] objectsBuffer = new byte[objectsStream.Length];
                byte[]? audioBuffer = null;
                byte[]? coverBuffer = null;
                byte[]? videoBuffer = null;

                metaStream.Read(metaBuffer, 0, (int)metaStream.Length);
                objectsStream.Read(objectsBuffer, 0, (int)objectsStream.Length);
                metaStream.Close();
                objectsStream.Close();

                var metadata = JsonConvert.DeserializeObject<PhoenyxMapMetadata>(Encoding.UTF8.GetString(metaBuffer))!;
                var objects = new FileParser(objectsBuffer);

                if (metadata.HasAudio)
                {
                    using (var stream = folderContents.GetEntry($"audio.{metadata.AudioExtension}")!.Open())
                    {
                        audioBuffer = new byte[stream.Length];
                        stream.Read(audioBuffer, 0, (int)stream.Length);
                    }
                }

                if (metadata.HasCover)
                {
                    using (var stream = folderContents.GetEntry($"cover.png")!.Open())
                    {
                        coverBuffer = new byte[stream.Length];
                        stream.Read(coverBuffer, 0, (int)stream.Length);
                    }
                }

                if (metadata.HasVideo)
                {
                    using (var stream = folderContents.GetEntry($"video.mp4")!.Open())
                    {
                        videoBuffer = new byte[stream.Length];
                        stream.Read(videoBuffer, 0, (int)stream.Length);
                    }
                }

                uint typeCount = objects.GetUInt32();
                uint noteCount = objects.GetUInt32();
                metadata.NoteCount = (int)noteCount;

                List<PhoenyxNote> notes = [];

                for (int i = 0; i < noteCount; i++)
                {
                    int ms = (int)objects.GetUInt32();
                    bool quantum = objects.GetBool();
                    float x;
                    float y;

                    if (quantum)
                    {
                        x = objects.GetFloat();
                        y = objects.GetFloat();
                    }
                    else
                    {
                        x = objects.Get(1)[0] - 1;
                        y = objects.Get(1)[0] - 1;
                    }

                    notes.Add(new(i, ms, x, y));
                }

                folderContents.Dispose();

                return new PhoenyxMap(audioBuffer, coverBuffer, videoBuffer, new PhoenyxMapMarkers() { Notes = notes }, metadata);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
