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
        new object[] { new WorldSectorId(0, 0,0,1), new AgentPosition(new WorldSectorId(),50000000,28867514,81649656)},
        new object[] { new WorldSectorId(0, 0,0,-1), new AgentPosition(new WorldSectorId(),-50000000,-28867514,-81649656)},
        new object[] { new WorldSectorId(0, 0,1,0), new AgentPosition(new WorldSectorId(),50000000,86602540,0)},
        new object[] { new WorldSectorId(0, 0,1,1), new AgentPosition(new WorldSectorId(),100000000,115470056,81649656)},
        new object[] { new WorldSectorId(0, 0,1,-1), new AgentPosition(new WorldSectorId(),0,57735028,-81649656)},
        new object[] { new WorldSectorId(0, 0,-1,0), new AgentPosition(new WorldSectorId(),-50000000,-86602540,0)},
        new object[] { new WorldSectorId(0, 0,-1,1), new AgentPosition(new WorldSectorId(),0,-57735028,81649656)},
        new object[] { new WorldSectorId(0, 0,-1,-1), new AgentPosition(new WorldSectorId(),-100000000,-115470056,-81649656)},

        new object[] { new WorldSectorId(0, 1,0,0), new AgentPosition(new WorldSectorId(), 100000000,0,0)},
        new object[] { new WorldSectorId(0, 1,0,1), new AgentPosition(new WorldSectorId(),150000000,28867514,81649656)},
        new object[] { new WorldSectorId(0, 1,0,-1), new AgentPosition(new WorldSectorId(),50000000,-28867514,-81649656)},
        new object[] { new WorldSectorId(0, 1,1,0), new AgentPosition(new WorldSectorId(),150000000,86602540,0)},
        new object[] { new WorldSectorId(0, 1,1,1), new AgentPosition(new WorldSectorId(),200000000,115470056,81649656)},
        new object[] { new WorldSectorId(0, 1,1,-1), new AgentPosition(new WorldSectorId(),100000000,57735028,-81649656)},
        new object[] { new WorldSectorId(0, 1,-1,0), new AgentPosition(new WorldSectorId(),50000000,-86602540,0)},
        new object[] { new WorldSectorId(0, 1,-1,1), new AgentPosition(new WorldSectorId(),100000000,-57735028,81649656)},
        new object[] { new WorldSectorId(0, 1,-1,-1), new AgentPosition(new WorldSectorId(),0,-115470056,-81649656)},

        new object[] { new WorldSectorId(0, -1,0,0), new AgentPosition(new WorldSectorId(), -100000000,0,0)},
        new object[] { new WorldSectorId(0, -1,0,1), new AgentPosition(new WorldSectorId(),-50000000,28867514,81649656)},
        new object[] { new WorldSectorId(0, -1,0,-1), new AgentPosition(new WorldSectorId(),-150000000,-28867514,-81649656)},
        new object[] { new WorldSectorId(0, -1,1,0), new AgentPosition(new WorldSectorId(),-50000000,86602540,0)},
        new object[] { new WorldSectorId(0, -1,1,1), new AgentPosition(new WorldSectorId(),0,115470056,81649656)},
        new object[] { new WorldSectorId(0, -1,1,-1), new AgentPosition(new WorldSectorId(),-100000000,57735028,-81649656)},
        new object[] { new WorldSectorId(0, -1,-1,0), new AgentPosition(new WorldSectorId(),-150000000,-86602540,0)},
        new object[] { new WorldSectorId(0, -1,-1,1), new AgentPosition(new WorldSectorId(),-100000000,-57735028,81649656)},
        new object[] { new WorldSectorId(0, -1,-1,-1), new AgentPosition(new WorldSectorId(),-200000000,-115470056,-81649656)},
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
        // [new AgentPosition(new WorldSectorId(), 0, 0, 0), 0],
        // [new AgentPosition(new WorldSectorId(), 50000000,0,0), 1],
        // [new AgentPosition(new WorldSectorId(), -50000000,0,0), 1],
        // [new AgentPosition(new WorldSectorId(), 25000000,43301270,0), 1],
        // [new AgentPosition(new WorldSectorId(), 25000000,-43301270,0), 1],
        // [new AgentPosition(new WorldSectorId(), -25000000,43301270,0), 1],
        // [new AgentPosition(new WorldSectorId(), -25000000,-43301270,0), 1],

        // [new AgentPosition(new WorldSectorId(), 0,-28867514,40824828), 1],
        // [new AgentPosition(new WorldSectorId(), 25000000,14433757,40824828), 1],
        // [new AgentPosition(new WorldSectorId(), -25000000,14433757,40824828), 1],

        // [new AgentPosition(new WorldSectorId(), 0,28867514,-40824828), 1],
        // [new AgentPosition(new WorldSectorId(), 25000000,-14433757,-40824828), 1],
        // [new AgentPosition(new WorldSectorId(), -25000000,-14433757,-40824828), 1],
        
        // [new AgentPosition(new WorldSectorId(), 0, 57735028, 0), 2],
        // [new AgentPosition(new WorldSectorId(), 0, -57735028, 0), 2],
        // [new AgentPosition(new WorldSectorId(), 50000000, 28867514, 0), 2],
        // [new AgentPosition(new WorldSectorId(), -50000000, 28867514, 0), 2],
        // [new AgentPosition(new WorldSectorId(), 50000000, -28867514, 0), 2],
        // [new AgentPosition(new WorldSectorId(), -50000000, -28867514, 0), 2],

        // [new AgentPosition(new WorldSectorId(), 0, Settings.SectorSize*0.2f, Settings.SectorSize*0.6f), 2],
        // [new AgentPosition(new WorldSectorId(), Settings.SectorSize*0.1f, -Settings.SectorSize*0.2f, Settings.SectorSize*0.6f), 2],
        // [new AgentPosition(new WorldSectorId(), -Settings.SectorSize*0.1f, -Settings.SectorSize*0.2f, Settings.SectorSize*0.6f), 2],


        [new AgentPosition(new WorldSectorId(), 0, 0, Settings.SectorSize*0.6f), 3],
        [new AgentPosition(new WorldSectorId(), 0, 0, -Settings.SectorSize*0.6f), 3],
    };
}