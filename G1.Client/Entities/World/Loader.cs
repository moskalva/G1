using System;
using G1.Model;
using Godot;

public static class Loader
{
    public static Mark1 LoadShip(ShipState remoteState)
    {
        var shipScene = remoteState.Type == WorldEntityType.Ship
                      ? GD.Load<PackedScene>("res://Entities/Ship/Mark1/Mark1.tscn")
                      : throw new NotSupportedException($"Unsupported entity type '{remoteState.Type}'");
        var ship = shipScene.Instantiate<Mark1>();
        ship.Id = remoteState.Id;
        return ship;
    }

    public static ExternalEnity LoadExternalEntity() =>
        GD.Load<PackedScene>("res://Entities/Ship/ExternalEnity.tscn").Instantiate<ExternalEnity>();
        
    public static Exterier LoadExterior(WorldEntityType entityType) => entityType switch
    {
        WorldEntityType.Ship =>
            GD.Load<PackedScene>("res://Entities/Ship/Mark1/Exterier/Exterier.tscn").Instantiate<Exterier>(),
        _ => throw new NotSupportedException($"Unsupported entity type '{entityType}'")
    };

    public static DirectionMarker LoadMarkerScene()=>
        GD.Load<PackedScene>("res://Entities/Ship/Common/NavigationMap/DirectionMarker.tscn").Instantiate<DirectionMarker>();
}