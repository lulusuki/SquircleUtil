using SquircleUtil.Rhythia;

namespace SquircleUtil.Tests;

public class Rhythia
{
    [Fact]
    public void Properly_Parsed()
    {
        var map = Map.Decode("C:/Users/Faded/Desktop/SquircleUtil/rhym_map");
        Assert.NotNull(map);
    }
}