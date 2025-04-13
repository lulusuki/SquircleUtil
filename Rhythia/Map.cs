using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace SquircleUtil.Rhythia
{
    public class Map : IMap
    {
        public MapSetMetadata MapSetMetadata { get; private set; } = new();

        public byte[]? Audio { get; set; }

        public byte[]? Cover { get; set; }

        public byte[]? Video { get; set; }

        public MapMarkers Markers { get; set; }

        public List<string> ColorSet { get; set; }

        public MapMetadata Metadata { get; set; }

        IMapMarkers IMap.Markers => Markers;

        IMapMetadata IMap.Metadata => Metadata;

        internal static Map Convert(IMap map, MapSetMetadata metadata)
        {
            var _map = new Map(metadata);
            var lastDelta = 0;
            
            _map.Metadata.Mappers = map.Metadata.Mappers;
            _map.Metadata.MappersIds = map.Metadata.MappersIds;
            _map.Markers.NoteFields = 3;

            foreach (INote note in map.Markers.Notes)
            {
                _map.Markers.Notes.Add(new Note(note.Millisecond, note.X, note.Y));
                _map.Markers.NoteList.Add(note.Millisecond - lastDelta);
                _map.Markers.NoteList.Add(note.X);
                _map.Markers.NoteList.Add(note.Y);
            }

            return _map;
        }

        public Map(MapSetMetadata mapSetMetadata, byte[]? audio = null, byte[]? cover = null, byte[]? video = null, MapMarkers? markers = null, List<string>? colorSet = null, MapMetadata? metadata = null)
        {
            MapSetMetadata = mapSetMetadata;
            Audio = audio;
            Cover = cover;
            Video = video;
            Markers = markers != null ? markers : new();
            ColorSet = colorSet != null ? colorSet : new();
            Metadata = metadata != null ? metadata : new();
        }

        public void Encode(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            File.WriteAllText($"{path}/metadata.json", JsonConvert.SerializeObject(Metadata));
            File.WriteAllText($"{path}/objects.json", JsonConvert.SerializeObject(Markers));

            if (Audio != null)
                File.WriteAllBytes($"{path}/audio.{Metadata.AudioExtension}", Audio);

            if (Cover != null)
                File.WriteAllBytes($"{path}/cover.png", Cover);

            if (Video != null)
                File.WriteAllBytes($"{path}/cover.png", Video);

            if (Metadata.HasColorSet)
                File.WriteAllText($"{path}/colorset.txt", string.Join("\n", ColorSet));
        }

        public static Map Decode(string folder, MapSetMetadata mapSetMetadata)
        {
            List<string> colorSet = new();

            try
            {
                Stream metaStream = File.OpenRead($"{folder}/metadata.json");
                Stream objectsStream = File.OpenRead($"{folder}/object.json");

                byte[]? metaBuffer = new byte[metaStream.Length];
                byte[]? objectsBuffer = new byte[objectsStream.Length];
                byte[]? audioBuffer = null;
                byte[]? coverBuffer = null;
                byte[]? videoBuffer = null;
                byte[]? colorsetBuffer = null;

                metaStream.Read(metaBuffer, 0, (int)metaStream.Length);
                objectsStream.Read(objectsBuffer, 0, (int)objectsStream.Length);
                metaStream.Close();
                objectsStream.Close();

                var metadata = JsonConvert.DeserializeObject<MapMetadata>(Encoding.UTF8.GetString(metaBuffer))!;
                var mapMarkers = JsonConvert.DeserializeObject<MapMarkers>(Encoding.UTF8.GetString(objectsBuffer))!;


                metadata.ID = mapSetMetadata.ID;

            
                if (metadata.AudioExtension != string.Empty && File.Exists($"{folder}/audio.{metadata.AudioExtension}"))
                {
                    using (var stream = File.OpenRead($"{folder}/audio.{metadata.AudioExtension}"))
                    {
                        audioBuffer = new byte[stream.Length];
                        stream.Read(audioBuffer, 0, (int)stream.Length);
                        metadata.HasAudio = true;
                    }
                }

                if (File.Exists($"{folder}/cover.png"))
                {
                    using (var stream = File.OpenRead($"{folder}/cover.png"))
                    {
                        coverBuffer = new byte[stream.Length];
                        stream.Read(coverBuffer, 0, (int)stream.Length);
                        metadata.HasCover = true;
                    }
                }

                if (File.Exists($"{folder}/video.mp4"))
                {
                    using (var stream = File.OpenRead($"{folder}/audio.{metadata.AudioExtension}"))
                    {
                        videoBuffer = new byte[stream.Length];
                        stream.Read(videoBuffer, 0, (int)stream.Length);
                        metadata.HasVideo= true;
                    }
                }

                if (File.Exists($"{folder}/colorset.txt"))
                {
                    using (var stream = File.OpenRead($"{folder}/colorset.txt"))
                    {
                        colorsetBuffer = new byte[stream.Length];
                        stream.Read(colorsetBuffer, 0, (int)stream.Length);
                        metadata.HasColorSet = true;
                        colorSet.AddRange(Encoding.UTF8.GetString(colorsetBuffer).Split("\n"));
                    }
                }
                
                List<Note> notes = new();

                int lastDelta = 0;

                var noteList = mapMarkers.NoteList;
                
                for (int i = 0; i < mapMarkers.NoteList.Count; i += mapMarkers.NoteFields)
                {
                    
                    var note = new Note((int)noteList[i] + lastDelta, noteList[i + 1], noteList[i + 2]);
                    lastDelta = note.Millisecond;

                    notes.Add(note);
                }

                return new Map(mapSetMetadata, audioBuffer, coverBuffer, videoBuffer, mapMarkers, colorSet, metadata);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}