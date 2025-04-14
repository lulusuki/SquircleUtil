using SquircleUtil.Rhythia;

namespace SquircleUtil.Tests;

public class Rhythia
{
    private string mapsFolder = "../../../../maps";

    [Fact]
    public void Properly_Parsed()
    {
        var map = Map.Decode($"{mapsFolder}/rhym_map");

        Assert.NotNull(map);
        Assert.NotNull(map.Audio);

        var metadata = map.Metadata;

        Assert.True(metadata.Artist == "STOMACH BOOK");
        Assert.True(metadata.RomanizedArtist == "STOMACH BOOK");
        Assert.True(metadata.Title == "FUKOUNA GIRL");
        Assert.True(metadata.RomanizedTitle == "FUKOUNA GIRL");
        Assert.True(metadata.AudioExtension == "mp3");
        Assert.True(metadata.Mappers[0] == "Zackmixing");
        Assert.True(metadata.MappersIds[0] == 5219);
        Assert.True(metadata.NoteCount == 2969);
        Assert.True(metadata.DifficultyName == "FUKOUNA");

        IMap imap = map;
        Assert.True(imap.Metadata.Mappers.First() == "Zackmixing");
    }
}