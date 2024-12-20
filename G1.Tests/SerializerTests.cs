using G1.Model.Serializers;

namespace G1.Tests;

public class SerializerTests
{
    [Fact]
    public void CanSerialize_ClientState()
    {
        var initial = new ClientStateChange()
        {
            Id = WorldEntityId.Create(),
            PositionAndSpeed = new WorldEntityLocationAndSpeed
            {
                Position = new World3dVector() { X = 1, Y = 2, Z = 3 },
                Velocity = new World3dVector() { X = 4, Y = 5, Z = 6 },
                Rotation = new World3dVector() { X = 1, Y = 2, Z = 3 },
                AngularVelocity = new World3dVector() { X = 4, Y = 5, Z = 6 },
            },
        };
        Execute(initial);
    }
    [Fact]
    public void CanSerialize_ServerState()
    {
        var initial = new ServerStateChange()
        {
            Id = WorldEntityId.Create(),
            PositionAndSpeed = new WorldEntityLocationAndSpeed
            {
                Position = new World3dVector() { X = 1, Y = 2, Z = 3 },
                Velocity = new World3dVector() { X = 4, Y = 5, Z = 6 },
                Rotation = new World3dVector() { X = 1, Y = 2, Z = 3 },
                AngularVelocity = new World3dVector() { X = 4, Y = 5, Z = 6 },
            },
            Type = WorldEntityType.Ship,
            SystemId = 22,
            ReferencePoint = new WorldReferencePoint { X = 1, Y = 2, Z = 3 },
        };
        Execute(initial);
    }

    [Fact]
    public void CanSerialize_WorldEntityId()
    {
        var initial = WorldEntityId.Create();
        Execute(initial);
    }

    [Fact]
    public void CanSerialize_ClientHearBeat()
    {
        var initial = new ClientHeartBeat { Id = WorldEntityId.Create() };
        Execute(initial);
    }

    [Fact]
    public void CanSerialize_ServerHeartBeat()
    {
        var initial = new ServerHeartBeat { Id = WorldEntityId.Create() };
        Execute(initial);
    }

    [Fact]
    public void CanSerialize_ShipLeft()
    {
        var initial = new NeighborLeft { Id = WorldEntityId.Create() };
        Execute(initial);
    }

    private static void Execute<T>(T initial)
    {
        var dataInitial = SerializerHelpers.Serialize<T>(initial);
        var result = SerializerHelpers.Deserialize<T>(dataInitial);
        Assert.Equal(initial, result);
        var dataResult = SerializerHelpers.Serialize<T>(result);
        Assert.Equal(dataInitial, dataResult);
    }
}