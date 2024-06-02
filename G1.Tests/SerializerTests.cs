using G1.Model;

namespace G1.Tests;

public class SerializerTests
{
    [Fact]
    public void CanSerialize_WorldEntityState()
    {
        var initial = new WorldEntityState()
        {
            Id = WorldEntityId.Create(),
            Type = WorldEntityType.Ship,
        };
        Execute(initial);
    }
    [Fact]
    public void CanSerialize_WorldEntityState_Full()
    {
        var initial = new WorldEntityState()
        {
            Id = WorldEntityId.Create(),
            Type = WorldEntityType.Ship,
            Position = new World3dVector() { X = 1, Y = 2, Z = 3 },
            Velocity = new World3dVector() { X = 4, Y = 5, Z = 6 },
        };
        Execute(initial);
    }

    [Fact]
    public void CanSerialize_WorldEntityId()
    {
        var initial = WorldEntityId.Create();
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