using Godot;
using Xunit.Sdk;

namespace G1.Tests;

public class EclipticPositionTests
{

    [Theory]
    [MemberData(nameof(EclipticPosition_FromShipState_Cases))]
    public void EclipticPosition_FromShipState(Vector3I referencePoint, Vector3 position,
        ulong expectedRadius, double expectedLatitude, double expectedLongitude)
    {
        var result = EclipticPosition.Get(referencePoint, position);

        Assert.Equal(expectedRadius, result.Radius);
        Assert.Equal(expectedLatitude, result.Latitude, 0.5);
        Assert.Equal(expectedLongitude, result.Longitude, 0.5);
    }

    public static IEnumerable<object[]> EclipticPosition_FromShipState_Cases = [
        [new Vector3I(0,0,0), new Vector3(0,0,0), 0, 0, 0],
        // reference
        [new Vector3I(0,0,-1), new Vector3(0,0,0), 1, 0, 0],
        [new Vector3I(0,-1,0), new Vector3(0,0,0), 1, -90, 0],
        [new Vector3I(-1,0,0), new Vector3(0,0,0), 1, 0, -90],
        [new Vector3I(0,0,1), new Vector3(0,0,0), 1, 0, 180],
        [new Vector3I(0,1,0), new Vector3(0,0,0), 1, 90, 0],
        [new Vector3I(1,0,0), new Vector3(0,0,0), 1, 0, 90],
        //relative
        [ new Vector3I(0,0,0),new Vector3(0,0,-1), 1, 0, 0],
        [ new Vector3I(0,0,0),new Vector3(0,-1,0), 1, -90, 0],
        [ new Vector3I(0,0,0),new Vector3(-1,0,0), 1, 0, -90],
        [new Vector3I(0,0,0), new Vector3(0,0,1), 1, 0, 180],
        [new Vector3I(0,0,0), new Vector3(0,1,0), 1, 90, 0],
        [new Vector3I(0,0,0), new Vector3(1,0,0), 1, 0, 90],
        // both
        [new Vector3I(0,0,-10), new Vector3(10,10,0), 17, 35, 45],
        [new Vector3I(0,0,10), new Vector3(10,10,0), 17, 35, 135],
        [new Vector3I(0,0,-10), new Vector3(-10,10,0), 17, 35, -45],
        [new Vector3I(0,0,10), new Vector3(-10,10,0), 17, 35, 225],

        [new Vector3I(0,0,-10), new Vector3(10,-10,0), 17, -35, 45],
        [new Vector3I(0,0,10), new Vector3(10,-10,0), 17, -35, 135],
        [new Vector3I(0,0,-10), new Vector3(-10,-10,0), 17, -35, -45],
        [new Vector3I(0,0,10), new Vector3(-10,-10,0), 17, -35, 225],
        // big numbers
        [new Vector3I(int.MaxValue,0,0), new Vector3(0,0,0), int.MaxValue, 0, 90],
        [new Vector3I(int.MaxValue,0,0), new Vector3(WorldParameters.Physics.SectorSize,0,0), (long)int.MaxValue + WorldParameters.Physics.SectorSize, 0, 90],
    ];
}