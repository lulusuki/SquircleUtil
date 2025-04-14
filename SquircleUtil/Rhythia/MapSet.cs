using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace SquircleUtil.Rhythia
{
    public class MapSet : IMapSet
    {
        public string? _path { get; private set; }

        public byte[]? Audio { get; set; }

        public byte[]? Cover { get; set; }

        public byte[]? Video { get; set; }

        public List<string> ColorSet { get; set; }

        public MapSetMetadata Metadata { get; set; }

        public Dictionary<string, Map> Maps { get; set; } = new();

        IMapSetMetadata IMapSet.Metadata => Metadata;
        
        IDictionary<string, IMap> IMapSet.Maps => (IDictionary<string, IMap>)Maps;

        IList<string> IMapSet.ColorSet => ColorSet;

        public static MapSet Convert(List<IMap> maps) 
        {
            if (maps.Count < 1)
                throw new("Maps list is empty");

            var invalidChars = Path.GetInvalidPathChars();
            string[] difficulties = [ ..maps.Select(x => x.Metadata.DifficultyName.Where(c => !invalidChars.Contains(c)).ToString() + "") ];
            var map = maps.First();
            var mapSet = new MapSet();
            var meta = map.Metadata;

            mapSet.Metadata.ID = meta.ID;
            mapSet.Metadata.AudioExtension = meta.AudioExtension;
            mapSet.Metadata.Artist = meta.Artist;
            mapSet.Metadata.Title = meta.Title;
            mapSet.Metadata.RomanizedTitle = meta.Title;
            mapSet.Metadata.Difficulties = difficulties.ToList();

            if (mapSet.Audio != null)
            {
                mapSet.Audio = map.Audio;
            }

            if (mapSet.Cover != null)
            {
                mapSet.Cover = map.Cover;
            }

            var video = map as IHasVideo;
            if (video != null && video.Video != null)
                mapSet.Video = video.Video;

            maps
                .Select((map, index) => new { map, index })
                .ToList()
                .ForEach((item) => mapSet.Maps.Add(difficulties[item.index], Map.Convert(item.map)));

            return mapSet;
        }

        public MapSet(string? path = null, byte[]? audio = null, byte[]? cover = null, byte[]? video = null, List<string>? colorSet = null, MapSetMetadata? metadata = null)
        {
            _path = path;
            Audio = audio;
            Cover = cover;
            Video = video;
            ColorSet = colorSet != null ? colorSet : new();
            Metadata = metadata != null ? metadata : new();
        }

        public Map GetMap(string difficulty)
        {
            if (Maps.ContainsKey(difficulty))
            {
                return Maps[difficulty];
            }

            var map = Map.Decode($"{_path}/{difficulty}", Metadata);

            Maps.Add(difficulty, map);

            return map;
        }

        public void Encode(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllText($"{path}/metadata.json", JsonConvert.SerializeObject(Metadata));

            if (Audio != null)
                File.WriteAllBytes($"{path}/audio.{(Metadata.AudioExtension != string.Empty ? Metadata.AudioExtension : "mp3")}", Audio);

            if (Cover != null)
                File.WriteAllBytes($"{path}/cover.png", Cover);

            if (Video != null)
                File.WriteAllBytes($"{path}/cover.png", Video);

            if (Metadata.HasColorSet)
                File.WriteAllText($"{path}/colorset.txt", string.Join("\n", ColorSet));

            foreach (string difficulty in Metadata.Difficulties)
            {
                GetMap(difficulty).Encode($"{path}/{difficulty}");
            }
        }

        public static MapSet Decode(string folder)
        {
            List<string> colorSet = new();

            try
            {
                var metaStream = File.OpenRead($"{folder}/metadata.json");

                byte[] metaBuffer = new byte[metaStream.Length];
                byte[]? audioBuffer = null;
                byte[]? coverBuffer = null;
                byte[]? videoBuffer = null;
                byte[]? colorsetBuffer = null;

                metaStream.Read(metaBuffer, 0, (int)metaStream.Length);
                metaStream.Close();

                var metadata = JsonConvert.DeserializeObject<MapSetMetadata>(Encoding.UTF8.GetString(metaBuffer))!;

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
                        metadata.HasVideo = true;
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

                return new(folder, audioBuffer, coverBuffer, videoBuffer, colorSet, metadata);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}