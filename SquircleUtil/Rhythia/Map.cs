using System.Text;
using Newtonsoft.Json;

namespace SquircleUtil.Rhythia
{
    public class Map : IRhythiaMap
    {
        public MapSetMetadata MapSetMetadata { get; private set; } = new();

        public byte[]? Audio { get; set; }

        public byte[]? Cover { get; set; }

        public byte[]? Video { get; set; }

        public MapObjects Objects { get; set; }

        public List<string> ColorSet { get; set; }

        public MapMetadata Metadata { get; set; }

        IMapObjects IMap.Objects => Objects;

        IMapMetadata IMap.Metadata => Metadata;

        IList<string> IMap.ColorSet => ColorSet;

        internal static Map Convert(IMap map, MapSetMetadata metadata)
        {
            var _map = new Map(metadata);

            var notes = map.Objects.Notes.ToList();
            notes.Sort();

            var lastDelta = 0;

            _map.Metadata.Mappers = map.Metadata.Mappers.ToList();
            _map.Metadata.NoteCount = notes.Count;
            _map.Objects.NoteFields = 3;

            foreach (INote note in notes)
            {
                _map.Objects.Notes.Add(new Note(note.Millisecond, note.X, note.Y));
                _map.Objects.NoteList.Add(note.Millisecond - lastDelta);
                lastDelta = note.Millisecond;
                _map.Objects.NoteList.Add(note.X);
                _map.Objects.NoteList.Add(note.Y);
            }

            return _map;
        }

        public static Map Convert(IMap map)
        {
            var _map = new Map();
            var notes = map.Objects.Notes.ToList();
            notes.Sort();
            var lastDelta = 0;

            _map.Metadata.Artist = map.Metadata.Artist;
            _map.Metadata.Title = map.Metadata.Title;
            _map.Metadata.Mappers = map.Metadata.Mappers.ToList();
            _map.Metadata.NoteCount = notes.Count;
            _map.Objects.NoteFields = 3;


            if (map.Audio != null)
                _map.Audio = map.Audio;
                _map.Metadata.AudioExtension = map.Metadata.AudioExtension;
            if (map.Cover != null)
                _map.Cover = map.Cover;
            var video = map as IHasVideo;
            if (video != null && video.Video != null)
                _map.Video = video.Video;
            

            foreach (INote note in notes)
            {
                _map.Objects.Notes.Add(new Note(note.Millisecond, note.X, note.Y));
                _map.Objects.NoteList.Add(note.Millisecond - lastDelta);
                lastDelta = note.Millisecond;
                _map.Objects.NoteList.Add(note.X);
                _map.Objects.NoteList.Add(note.Y);
            }

            return _map;
        }

        public Map(MapSetMetadata? mapSetMetadata = null, byte[]? audio = null, byte[]? cover = null, byte[]? video = null, MapObjects? markers = null, List<string>? colorSet = null, MapMetadata? metadata = null)
        {
            MapSetMetadata = mapSetMetadata != null ? mapSetMetadata : new();
            Audio = audio;
            Cover = cover;
            Video = video;
            Objects = markers != null ? markers : new();
            ColorSet = colorSet != null ? colorSet : new();
            Metadata = metadata != null ? metadata : new();
        }

        public void Encode(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            File.WriteAllText($"{path}/metadata.json", JsonConvert.SerializeObject(Metadata));
            File.WriteAllText($"{path}/object.json", JsonConvert.SerializeObject(Objects));

            if (Audio != null)
                File.WriteAllBytes($"{path}/audio.{(Metadata.AudioExtension != string.Empty ? Metadata.AudioExtension : "mp3")}", Audio);

            if (Cover != null)
                File.WriteAllBytes($"{path}/cover.png", Cover);

            if (Video != null)
                File.WriteAllBytes($"{path}/cover.png", Video);

            if (ColorSet != null)
                File.WriteAllText($"{path}/colorset.txt", string.Join("\n", ColorSet));
        }

        public static Map Decode(string folder)
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
                var mapMarkers = JsonConvert.DeserializeObject<MapObjects>(Encoding.UTF8.GetString(objectsBuffer))!;


                if (metadata.AudioExtension != string.Empty && File.Exists($"{folder}/audio.{metadata.AudioExtension}"))
                {
                    using (var stream = File.OpenRead($"{folder}/audio.{metadata.AudioExtension}"))
                    {
                        audioBuffer = new byte[stream.Length];
                        stream.Read(audioBuffer, 0, (int)stream.Length);
                    }
                }

                if (File.Exists($"{folder}/cover.png"))
                {
                    using (var stream = File.OpenRead($"{folder}/cover.png"))
                    {
                        coverBuffer = new byte[stream.Length];
                        stream.Read(coverBuffer, 0, (int)stream.Length);
                    }
                }

                if (File.Exists($"{folder}/video.mp4"))
                {
                    using (var stream = File.OpenRead($"{folder}/audio.{metadata.AudioExtension}"))
                    {
                        videoBuffer = new byte[stream.Length];
                        stream.Read(videoBuffer, 0, (int)stream.Length);
                    }
                }

                if (File.Exists($"{folder}/colorset.txt"))
                {
                    using (var stream = File.OpenRead($"{folder}/colorset.txt"))
                    {
                        colorsetBuffer = new byte[stream.Length];
                        stream.Read(colorsetBuffer, 0, (int)stream.Length);
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

                return new Map(null, audioBuffer, coverBuffer, videoBuffer, mapMarkers, colorSet, metadata);
            }
            catch (Exception)
            {
                throw;
            }
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
                var mapMarkers = JsonConvert.DeserializeObject<MapObjects>(Encoding.UTF8.GetString(objectsBuffer))!;

            
                if (metadata.AudioExtension != string.Empty && File.Exists($"{folder}/audio.{metadata.AudioExtension}"))
                {
                    using (var stream = File.OpenRead($"{folder}/audio.{metadata.AudioExtension}"))
                    {
                        audioBuffer = new byte[stream.Length];
                        stream.Read(audioBuffer, 0, (int)stream.Length);
                    }
                }

                if (File.Exists($"{folder}/cover.png"))
                {
                    using (var stream = File.OpenRead($"{folder}/cover.png"))
                    {
                        coverBuffer = new byte[stream.Length];
                        stream.Read(coverBuffer, 0, (int)stream.Length);
                    }
                }

                if (File.Exists($"{folder}/video.mp4"))
                {
                    using (var stream = File.OpenRead($"{folder}/audio.{metadata.AudioExtension}"))
                    {
                        videoBuffer = new byte[stream.Length];
                        stream.Read(videoBuffer, 0, (int)stream.Length);
                    }
                }

                if (File.Exists($"{folder}/colorset.txt"))
                {
                    using (var stream = File.OpenRead($"{folder}/colorset.txt"))
                    {
                        colorsetBuffer = new byte[stream.Length];
                        stream.Read(colorsetBuffer, 0, (int)stream.Length);
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