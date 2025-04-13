using System.Text;
using Newtonsoft.Json;

namespace SquircleUtil.Rhythia
{
    public class MapSet : IMapSet
    {
        public string? Path { get; private set; }

        public byte[]? Audio { get; set; }

        public byte[]? Cover { get; set; }

        public byte[]? Video { get; set; }

        public List<string> ColorSet { get; set; }

        public MapSetMetadata Metadata { get; set; }
        
        public Dictionary<string, Map> Maps { get; set; } = new();

        public static MapSet Convert(IMap map, string difficultyName)
        {
            var mapSet = new MapSet();
            mapSet.Metadata = new();
            var mapSetMetadata = mapSet.Metadata;
            var meta = map.Metadata;
            
            mapSetMetadata.ID = meta.ID;
            mapSetMetadata.HasAudio = meta.HasAudio;
            mapSetMetadata.HasCover = meta.HasCover;
            mapSetMetadata.HasVideo = meta.HasVideo;
            mapSetMetadata.AudioExtension = meta.AudioExtension;
            mapSetMetadata.Artist = meta.Artist;
            mapSetMetadata.RomanizedArtist = meta.RomanizedArtist;
            mapSetMetadata.Title = meta.Title;
            mapSetMetadata.RomanizedTitle = meta.Title;
            mapSetMetadata.Difficulties = [ difficultyName ];

            if (meta.HasAudio)
            {
                mapSet.Audio = map.Audio;
            }

            if (meta.HasCover)
            {
                mapSet.Cover = map.Cover;
            }

            if (meta.HasVideo)
            {
                mapSet.Video = map.Video;
            }

            mapSet.Maps.Add(difficultyName, Map.Convert(map, mapSetMetadata));

            return mapSet;
        }

        public MapSet(string? path = null, byte[]? audio = null, byte[]? cover = null, byte[]? video = null, List<string>? colorSet = null, MapSetMetadata? metadata = null)
        {
            Path = path;
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

            var map = Map.Decode($"{Path}/{difficulty}", Metadata);

            Maps.Add(difficulty, map);

            return map;
        }

        public void Encode(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllText($"{path}/metadata.json", JsonConvert.SerializeObject(Metadata));

            if (Audio != null)
                File.WriteAllBytes($"{path}/audio.{(Metadata.AudioExtension != string.Empty ? Metadata.AudioExtension : "mp4")}", Audio);

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
            } catch (Exception)
            {
                throw;
            }
        }
    }
}