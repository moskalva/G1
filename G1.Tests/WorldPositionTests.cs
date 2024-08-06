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
        Console.WriteLine($"Sector: '{relativeSector}'");
        Assert.Equal(expectedPosition, position);
    }

    public static IEnumerable<object[]> WorldPosition_GetSectorPosition_Cases = new object[][]
    {
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
    };

    [Theory]
    [MemberData(nameof(WorldPosition_GetNearSectors_Cases))]
    public void WorldPosition_GetNearSectors(AgentPosition agentPosition, int expectedNumber)
    {
        var nearSectors = WorldPositionTools.GetNearSectors(agentPosition);
        Console.WriteLine($"Agent Position: '{agentPosition}'");
        foreach (var nearSector in nearSectors)
        {
            Console.WriteLine($"Near sector found: '{nearSector}'");
        }
        Assert.Equal(expectedNumber, nearSectors.Length);
    }

    public static IEnumerable<object[]> WorldPosition_GetNearSectors_Cases = new object[][]
    {
        [new AgentPosition(new WorldSectorId(), 0, 0, 0), 0],
        [new AgentPosition(new WorldSectorId(), 50000000,0,0), 1],
        [new AgentPosition(new WorldSectorId(), -50000000,0,0), 1],
        [new AgentPosition(new WorldSectorId(), 0,50000000,0), 1],
        [new AgentPosition(new WorldSectorId(), 0,-50000000,0), 1],

        [new AgentPosition(new WorldSectorId(), 25000000,25000000,35355340), 1],
        [new AgentPosition(new WorldSectorId(), 25000000,-25000000,35355340), 1],
        [new AgentPosition(new WorldSectorId(), -25000000,25000000,35355340), 1],
        [new AgentPosition(new WorldSectorId(), -25000000,-25000000,35355340), 1],

        [new AgentPosition(new WorldSectorId(), 25000000,25000000,-35355340), 1],
        [new AgentPosition(new WorldSectorId(), 25000000,-25000000,-35355340), 1],
        [new AgentPosition(new WorldSectorId(), -25000000,25000000,-35355340), 1],
        [new AgentPosition(new WorldSectorId(), -25000000,-25000000,-35355340), 1],
        
        [new AgentPosition(new WorldSectorId(), 0, -40824829, -40824829), 2],
        [new AgentPosition(new WorldSectorId(), 0, 40824829, -40824829), 2],
        [new AgentPosition(new WorldSectorId(), 40824829, 0, -40824829), 2],
        [new AgentPosition(new WorldSectorId(), -40824829, 0, -40824829), 2],

        [new AgentPosition(new WorldSectorId(), 0, -40824829, 40824829), 2],
        [new AgentPosition(new WorldSectorId(), 0, 40824829, 40824829), 2],
        [new AgentPosition(new WorldSectorId(), 40824829, 0, 40824829), 2],
        [new AgentPosition(new WorldSectorId(), -40824829, 0, 40824829), 2],

        [new AgentPosition(new WorldSectorId(), 60000000, 0, 40824829), 3],
        [new AgentPosition(new WorldSectorId(), -60000000, 0, 40824829), 3],
        [new AgentPosition(new WorldSectorId(), 0, 60000000, 40824829), 3],
        [new AgentPosition(new WorldSectorId(), 0, -60000000, 40824829), 3],

        [new AgentPosition(new WorldSectorId(), 60000000, 0, -40824829), 3],
        [new AgentPosition(new WorldSectorId(), -60000000, 0, -40824829), 3],
        [new AgentPosition(new WorldSectorId(), 0, 60000000, -40824829), 3],
        [new AgentPosition(new WorldSectorId(), 0, -60000000, -40824829), 3],

        [new AgentPosition(new WorldSectorId(), 0, 0, 70710680), 5],
        [new AgentPosition(new WorldSectorId(), 0, 0, -70710680), 5],

    };
}