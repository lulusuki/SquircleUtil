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
    }
}