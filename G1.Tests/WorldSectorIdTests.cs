using G1.Server.Agents;

namespace G1.Tests;

public class WorldSectorIdTests
{
    [Fact]
    public void WorldSectorId_ToRawAndBack()
    {
        var initial = new WorldSectorId(1, 2, 3, 4);
        var expected = new WorldSectorId(initial.Raw);

        Assert.Equal(initial.SystemId, expected.SystemId);
        Assert.Equal(initial.X, expected.X);
        Assert.Equal(initial.Y, expected.Y);
        Assert.Equal(initial.Z, expected.Z);
    }
    [Fact]
    public void WorldSectorId_ToString()
    {
        var initial = new WorldSectorId(1, 2, 3, 4);
        var expected = new WorldSectorId(initial.Raw);
        Assert.Equal(initial.ToString(), expected.ToString());
        Assert.Equal(initial.Raw.ToString(), expected.Raw.ToString());
    }
}