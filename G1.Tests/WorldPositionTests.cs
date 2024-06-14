using G1.Server.Agents;

namespace G1.Tests;

public class WorldPositionTests
{
    [Theory]
    [MemberData(nameof(RelativeSectorPositions))]
    public void WorldPosition_GetSectorPosition(WorldSectorId baseSector, WorldSectorId relativeSector, AgentPosition expectedPosition)
    {
        var position = WorldPositionTools.GetSectorPosition(baseSector, relativeSector);

        Assert.Equal(expectedPosition, position);
    }

    public static IEnumerable<object[]> RelativeSectorPositions = new object[][] {
        new object[] { new WorldSectorId(), new WorldSectorId(), new AgentPosition()},
        new object[] { new WorldSectorId(), new WorldSectorId(0,1,0,0), new AgentPosition(new WorldSectorId(), 100000000,0,0)},
        new object[] { new WorldSectorId(), new WorldSectorId(0,0,1,0), new AgentPosition(new WorldSectorId(),50000000,86602540,0)},
        new object[] { new WorldSectorId(), new WorldSectorId(0,0,0,1), new AgentPosition(new WorldSectorId(),28867514,28867514,86602540)},
        new object[] { new WorldSectorId(), new WorldSectorId(0,-1,0,0), new AgentPosition(new WorldSectorId(), -100000000,0,0)},
        new object[] { new WorldSectorId(), new WorldSectorId(0,0,-1,0), new AgentPosition(new WorldSectorId(),-50000000,-86602540,0)},
        new object[] { new WorldSectorId(), new WorldSectorId(0,0,0,-1), new AgentPosition(new WorldSectorId(),-28867514,-28867514,-86602540)},
    };
}