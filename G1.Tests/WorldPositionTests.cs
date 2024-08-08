using G1.Server;
using G1.Server.Agents;

namespace G1.Tests;

public class WorldPositionTests
{
    [Theory]
    [MemberData(nameof(WorldPosition_GetSectorPosition_Cases))]
    public void WorldPosition_GetSectorPosition(WorldSectorId relativeSector, AgentPosition expectedPosition)
    {
        var position = WorldPositionTools.GetSectorPosition(new WorldSectorId(), relativeSector);
        Assert.Equal(expectedPosition, position);
    }

    public static IEnumerable<object[]> WorldPosition_GetSectorPosition_Cases = [
        new object[] { new WorldSectorId(0, 0,0,0), new AgentPosition()},
        new object[] { new WorldSectorId(0, 0,0,1), new AgentPosition(new WorldSectorId(),50000000,50000000,70710680)},
        new object[] { new WorldSectorId(0, 0,0,-1), new AgentPosition(new WorldSectorId(),-50000000,-50000000,-70710680)},
        new object[] { new WorldSectorId(0, 0,1,0), new AgentPosition(new WorldSectorId(),0,100000000,0)},
        new object[] { new WorldSectorId(0, 0,1,1), new AgentPosition(new WorldSectorId(),50000000,150000000,70710680)},
        new object[] { new WorldSectorId(0, 0,1,-1), new AgentPosition(new WorldSectorId(),-50000000,50000000,-70710680)},
        new object[] { new WorldSectorId(0, 0,-1,0), new AgentPosition(new WorldSectorId(),0,-100000000,0)},
        new object[] { new WorldSectorId(0, 0,-1,1), new AgentPosition(new WorldSectorId(),50000000,-50000000,70710680)},
        new object[] { new WorldSectorId(0, 0,-1,-1), new AgentPosition(new WorldSectorId(),-50000000,-150000000,-70710680)},

        new object[] { new WorldSectorId(0, 1,0,0), new AgentPosition(new WorldSectorId(), 100000000,0,0)},
        new object[] { new WorldSectorId(0, 1,0,1), new AgentPosition(new WorldSectorId(),150000000,50000000,70710680)},
        new object[] { new WorldSectorId(0, 1,0,-1), new AgentPosition(new WorldSectorId(),50000000,-50000000,-70710680)},
        new object[] { new WorldSectorId(0, 1,1,0), new AgentPosition(new WorldSectorId(),100000000,100000000,0)},
        new object[] { new WorldSectorId(0, 1,1,1), new AgentPosition(new WorldSectorId(),150000000,150000000,70710680)},
        new object[] { new WorldSectorId(0, 1,1,-1), new AgentPosition(new WorldSectorId(),50000000,50000000,-70710680)},
        new object[] { new WorldSectorId(0, 1,-1,0), new AgentPosition(new WorldSectorId(),100000000,-100000000,0)},
        new object[] { new WorldSectorId(0, 1,-1,1), new AgentPosition(new WorldSectorId(),150000000,-50000000,70710680)},
        new object[] { new WorldSectorId(0, 1,-1,-1), new AgentPosition(new WorldSectorId(),50000000,-150000000,-70710680)},

        new object[] { new WorldSectorId(0, -1,0,0), new AgentPosition(new WorldSectorId(), -100000000,0,0)},
        new object[] { new WorldSectorId(0, -1,0,1), new AgentPosition(new WorldSectorId(),-50000000,50000000,70710680)},
        new object[] { new WorldSectorId(0, -1,0,-1), new AgentPosition(new WorldSectorId(),-150000000,-50000000,-70710680)},
        new object[] { new WorldSectorId(0, -1,1,0), new AgentPosition(new WorldSectorId(),-100000000,100000000,0)},
        new object[] { new WorldSectorId(0, -1,1,1), new AgentPosition(new WorldSectorId(), -50000000,150000000,70710680)},
        new object[] { new WorldSectorId(0, -1,1,-1), new AgentPosition(new WorldSectorId(),-150000000,50000000,-70710680)},
        new object[] { new WorldSectorId(0, -1,-1,0), new AgentPosition(new WorldSectorId(),-100000000,-100000000,0)},
        new object[] { new WorldSectorId(0, -1,-1,1), new AgentPosition(new WorldSectorId(),-50000000,-50000000,70710680)},
        new object[] { new WorldSectorId(0, -1,-1,-1), new AgentPosition(new WorldSectorId(),-150000000,-150000000,-70710680)},
    ];

    [Theory]
    [MemberData(nameof(WorldPosition_GetNearSectors_Cases))]
    public void WorldPosition_GetNearSectors(AgentPosition agentPosition, int expectedNumber)
    {
        var nearSectors = WorldPositionTools.GetNearSectors(agentPosition);
        Assert.Equal(expectedNumber, nearSectors.Length);
    }

    public static IEnumerable<object[]> WorldPosition_GetNearSectors_Cases = [
        [new AgentPosition(new WorldSectorId(), 0, 0, 0), 1],
        [new AgentPosition(new WorldSectorId(), 50000000,0,0), 2],
        [new AgentPosition(new WorldSectorId(), -50000000,0,0), 2],
        [new AgentPosition(new WorldSectorId(), 0,50000000,0), 2],
        [new AgentPosition(new WorldSectorId(), 0,-50000000,0), 2],

        [new AgentPosition(new WorldSectorId(), 25000000,25000000,35355340), 2],
        [new AgentPosition(new WorldSectorId(), 25000000,-25000000,35355340), 2],
        [new AgentPosition(new WorldSectorId(), -25000000,25000000,35355340), 2],
        [new AgentPosition(new WorldSectorId(), -25000000,-25000000,35355340), 2],

        [new AgentPosition(new WorldSectorId(), 25000000,25000000,-35355340), 2],
        [new AgentPosition(new WorldSectorId(), 25000000,-25000000,-35355340), 2],
        [new AgentPosition(new WorldSectorId(), -25000000,25000000,-35355340), 2],
        [new AgentPosition(new WorldSectorId(), -25000000,-25000000,-35355340), 2],

        [new AgentPosition(new WorldSectorId(), 0, -40824829, -40824829), 3],
        [new AgentPosition(new WorldSectorId(), 0, 40824829, -40824829), 3],
        [new AgentPosition(new WorldSectorId(), 40824829, 0, -40824829), 3],
        [new AgentPosition(new WorldSectorId(), -40824829, 0, -40824829), 3],

        [new AgentPosition(new WorldSectorId(), 0, -40824829, 40824829), 3],
        [new AgentPosition(new WorldSectorId(), 0, 40824829, 40824829), 3],
        [new AgentPosition(new WorldSectorId(), 40824829, 0, 40824829), 3],
        [new AgentPosition(new WorldSectorId(), -40824829, 0, 40824829), 3],

        [new AgentPosition(new WorldSectorId(), 60000000, 0, 40824829), 4],
        [new AgentPosition(new WorldSectorId(), -60000000, 0, 40824829), 4],
        [new AgentPosition(new WorldSectorId(), 0, 60000000, 40824829), 4],
        [new AgentPosition(new WorldSectorId(), 0, -60000000, 40824829), 4],

        [new AgentPosition(new WorldSectorId(), 60000000, 0, -40824829), 4],
        [new AgentPosition(new WorldSectorId(), -60000000, 0, -40824829), 4],
        [new AgentPosition(new WorldSectorId(), 0, 60000000, -40824829), 4],
        [new AgentPosition(new WorldSectorId(), 0, -60000000, -40824829), 4],

        [new AgentPosition(new WorldSectorId(), 0, 0, 70710680), 6],
        [new AgentPosition(new WorldSectorId(), 0, 0, -70710680), 6],
    ];

    [Theory]
    [MemberData(nameof(WorldPosition_TryNormalizePosition_Cases))]
    public void WorldPosition_TryNormalizePosition(AgentPosition agentPosition, AgentPosition expectedNewPosition)
    {
        var shouldSwitch = WorldPositionTools.TryNormalizePosition(agentPosition, out var newPosition);

        var expectSwitch = !agentPosition.Equals(expectedNewPosition);
        Assert.Equal(expectSwitch, shouldSwitch);
        if (expectSwitch)
        {
            Assert.Equal(expectedNewPosition, newPosition);
        }
    }
    public static IEnumerable<object[]> WorldPosition_TryNormalizePosition_Cases = [
        [new AgentPosition(new WorldSectorId(), 49_999_999, 0, 0), new AgentPosition(new WorldSectorId(), 49_999_999, 0, 0)],
        [new AgentPosition(new WorldSectorId(), 50_000_001, 0, 0), new AgentPosition(new WorldSectorId(), 50_000_001, 0, 0)],
        [new AgentPosition(new WorldSectorId(), 60_000_000, 0, 0), new AgentPosition(new WorldSectorId(), 60_000_000, 0, 0)],
        [new AgentPosition(new WorldSectorId(), 61_000_000, 0, 0), new AgentPosition(new WorldSectorId(0, 1,0,0), -39_000_000, 0, 0)],
        [new AgentPosition(new WorldSectorId(), 0, 0, 70710680), new AgentPosition(new WorldSectorId(0, 0,0,1), -50_000_000, -50_000_000, 0)],
    ];
}